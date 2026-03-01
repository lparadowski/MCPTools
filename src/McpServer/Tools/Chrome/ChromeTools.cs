using System.ComponentModel;
using Chrome.Core;
using ModelContextProtocol.Server;

namespace McpServer.Tools.Chrome;

[McpServerToolType]
public static class ChromeTools
{
    private static readonly string ScreenshotDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "MCPTools", "screenshots");

    [McpServerTool(Name = "list_chrome_tabs")]
    [Description("List all open Chrome tabs. Chrome must be running with --remote-debugging-port=9222.")]
    public static async Task<string> ListChromeTabs()
    {
        try
        {
            var tabs = await ChromeDevTools.ListTabsAsync();
            var pages = tabs.Where(t => t.Type == "page").ToList();

            if (pages.Count == 0)
                return "No tabs found. Is Chrome running with --remote-debugging-port=9222?";

            var lines = pages.Select((t, i) => $"[{i}] {t.Title}\n    {t.Url}");
            return $"Found {pages.Count} tabs:\n\n{string.Join("\n\n", lines)}";
        }
        catch (HttpRequestException)
        {
            return "Could not connect to Chrome. Make sure Chrome is running with --remote-debugging-port=9222";
        }
    }

    [McpServerTool(Name = "navigate_to_url")]
    [Description("""
        Navigate a Chrome tab to a given URL and wait for the page to load.
        Handles redirects (e.g. maps.app.goo.gl short links).
        Chrome must be running with --remote-debugging-port=9222.
        """)]
    public static async Task<string> NavigateToUrl(
        [Description("The URL to navigate to")] string url,
        [Description("Tab index from list_chrome_tabs (default: 0)")] int tabIndex = 0)
    {
        try
        {
            var tab = await ChromeDevTools.GetPageTabAsync(tabIndex);
            if (tab == null)
                return "No suitable tab found.";
            if (string.IsNullOrEmpty(tab.WebSocketDebuggerUrl))
                return $"Tab [{tabIndex}] does not expose a WebSocket debugger URL.";

            var result = await ChromeDevTools.NavigateAsync(tab.WebSocketDebuggerUrl, url);

            if (result.StartsWith("ERROR:"))
                return result;

            return $"OK: navigated to {url}";
        }
        catch (Exception ex)
        {
            return $"Error navigating: {ex.Message}";
        }
    }

    [McpServerTool(Name = "take_tab_screenshot")]
    [Description("""
        Capture a screenshot of a Chrome tab and save it to disk.
        Returns the file path so you can view the image.
        Chrome must be running with --remote-debugging-port=9222.
        If no tab index is given, captures the first tab.
        """)]
    public static async Task<string> TakeTabScreenshot(
        [Description("Tab index from list_chrome_tabs (default: 0)")] int tabIndex = 0)
    {
        try
        {
            var tabs = await ChromeDevTools.ListTabsAsync();
            var pages = tabs.Where(t => t.Type == "page").ToList();

            if (pages.Count == 0)
                return "No tabs found. Is Chrome running with --remote-debugging-port=9222?";

            if (tabIndex < 0 || tabIndex >= pages.Count)
                return $"Tab index {tabIndex} out of range. Found {pages.Count} tabs (0-{pages.Count - 1}).";

            var tab = pages[tabIndex];

            if (string.IsNullOrEmpty(tab.WebSocketDebuggerUrl))
                return $"Tab [{tabIndex}] '{tab.Title}' does not expose a WebSocket debugger URL.";

            var screenshotBytes = await ChromeDevTools.CaptureScreenshotAsync(tab.WebSocketDebuggerUrl);

            Directory.CreateDirectory(ScreenshotDir);
            var filename = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            var filePath = Path.Combine(ScreenshotDir, filename);
            await File.WriteAllBytesAsync(filePath, screenshotBytes);

            return $"Screenshot saved: {filePath}\nTab: {tab.Title}\nURL: {tab.Url}\nSize: {screenshotBytes.Length} bytes";
        }
        catch (HttpRequestException)
        {
            return "Could not connect to Chrome. Make sure Chrome is running with --remote-debugging-port=9222";
        }
    }

}
