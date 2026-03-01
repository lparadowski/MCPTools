using System.ComponentModel;
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
            var tab = await GetPageTabAsync(tabIndex);
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

    [McpServerTool(Name = "place_guess_pin")]
    [Description("""
        Place a pin on the GeoGuessr guess map at specific latitude/longitude coordinates.
        Uses React fiber traversal to trigger the Google Maps click handler.
        Chrome must be running with --remote-debugging-port=9222.
        """)]
    public static async Task<string> PlaceGuessPin(
        [Description("Latitude of the guess location")] double lat,
        [Description("Longitude of the guess location")] double lng,
        [Description("Tab index from list_chrome_tabs (default: 0)")] int tabIndex = 0)
    {
        try
        {
            var tab = await GetPageTabAsync(tabIndex);
            if (tab == null)
                return "No suitable tab found.";
            if (string.IsNullOrEmpty(tab.WebSocketDebuggerUrl))
                return $"Tab [{tabIndex}] does not expose a WebSocket debugger URL.";

            var js = $$"""
                (function() {
                    var lat = {{lat}};
                    var lng = {{lng}};
                    var element = document.querySelectorAll('[class^="guess-map_canvas__"]')[0];
                    if (!element) return 'ERROR: guess map canvas not found';

                    var reactKey = Object.keys(element).find(function(k) { return k.startsWith('__reactFiber$'); });
                    if (!reactKey) return 'ERROR: React fiber not found';

                    var elementProps = element[reactKey];
                    var mapClickHandlers = elementProps.return.return.memoizedProps.map.__e3_.click;
                    var handlerKeys = Object.keys(mapClickHandlers);
                    var lastKey = handlerKeys[handlerKeys.length - 1];
                    var clickProps = mapClickHandlers[lastKey];

                    var latLngFns = { latLng: { lat: function() { return lat; }, lng: function() { return lng; } } };

                    var called = 0;
                    var keys = Object.keys(clickProps);
                    for (var i = 0; i < keys.length; i++) {
                        if (typeof clickProps[keys[i]] === 'function') {
                            clickProps[keys[i]](latLngFns);
                            called++;
                        }
                    }
                    return 'OK: pin placed at ' + lat + ',' + lng + ' (' + called + ' handlers called)';
                })()
                """;

            return await ChromeDevTools.EvaluateJavaScriptAsync(tab.WebSocketDebuggerUrl, js);
        }
        catch (Exception ex)
        {
            return $"Error placing pin: {ex.Message}";
        }
    }

    [McpServerTool(Name = "click_guess_button")]
    [Description("""
        Click the 'Guess' button on the GeoGuessr page to submit the current guess.
        A pin must be placed first using place_guess_pin.
        Chrome must be running with --remote-debugging-port=9222.
        """)]
    public static async Task<string> ClickGuessButton(
        [Description("Tab index from list_chrome_tabs (default: 0)")] int tabIndex = 0)
    {
        try
        {
            var tab = await GetPageTabAsync(tabIndex);
            if (tab == null)
                return "No suitable tab found.";
            if (string.IsNullOrEmpty(tab.WebSocketDebuggerUrl))
                return $"Tab [{tabIndex}] does not expose a WebSocket debugger URL.";

            var js = """
                (function() {
                    var btn = document.querySelector('[data-qa="perform-guess"]');
                    if (!btn) return 'ERROR: guess button not found';
                    if (btn.disabled) return 'ERROR: guess button is disabled (place a pin first)';
                    btn.click();
                    return 'OK: guess submitted';
                })()
                """;

            return await ChromeDevTools.EvaluateJavaScriptAsync(tab.WebSocketDebuggerUrl, js);
        }
        catch (Exception ex)
        {
            return $"Error clicking guess: {ex.Message}";
        }
    }

    [McpServerTool(Name = "click_next_round")]
    [Description("""
        Click the 'Next' button on the GeoGuessr results screen to proceed to the next round.
        Also reads the round score and distance before clicking.
        Chrome must be running with --remote-debugging-port=9222.
        """)]
    public static async Task<string> ClickNextRound(
        [Description("Tab index from list_chrome_tabs (default: 0)")] int tabIndex = 0)
    {
        try
        {
            var tab = await GetPageTabAsync(tabIndex);
            if (tab == null)
                return "No suitable tab found.";
            if (string.IsNullOrEmpty(tab.WebSocketDebuggerUrl))
                return $"Tab [{tabIndex}] does not expose a WebSocket debugger URL.";

            var js = """
                (function() {
                    // Try multiple known selectors for the next/continue button
                    var btn = document.querySelector('[data-qa="close-round-result"]')
                           || document.querySelector('[data-qa="play-again-button"]')
                           || document.querySelector('a[data-qa="play-again-button"]');

                    // Fallback: find button by text content
                    if (!btn) {
                        var buttons = document.querySelectorAll('button, a');
                        for (var i = 0; i < buttons.length; i++) {
                            var text = buttons[i].textContent.trim().toLowerCase();
                            if (text === 'next' || text === 'play again'
                                || text === 'continue' || text === 'next round'
                                || text === 'view results' || text === 'see results') {
                                btn = buttons[i];
                                break;
                            }
                        }
                    }

                    if (!btn) return 'ERROR: next round button not found';

                    // Try to read score info before clicking
                    var scoreEl = document.querySelector('[data-qa="round-score"]')
                               || document.querySelector('[class*="round-result"]');
                    var score = scoreEl ? scoreEl.textContent.trim() : 'unknown';

                    btn.click();
                    return 'OK: clicked next (' + btn.textContent.trim().substring(0, 30) + '). Score info: ' + score;
                })()
                """;

            return await ChromeDevTools.EvaluateJavaScriptAsync(tab.WebSocketDebuggerUrl, js);
        }
        catch (Exception ex)
        {
            return $"Error clicking next: {ex.Message}";
        }
    }

    private static async Task<ChromeTab?> GetPageTabAsync(int tabIndex)
    {
        var tabs = await ChromeDevTools.ListTabsAsync();
        var pages = tabs.Where(t => t.Type == "page").ToList();
        if (pages.Count == 0 || tabIndex < 0 || tabIndex >= pages.Count)
            return null;
        return pages[tabIndex];
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
