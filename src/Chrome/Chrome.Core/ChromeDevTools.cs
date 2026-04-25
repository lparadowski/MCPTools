using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Chrome.Core;

public static class ChromeDevTools
{
    private const string DefaultEndpoint = "http://localhost:9222";

    public static async Task<List<ChromeTab>> ListTabsAsync(string endpoint = DefaultEndpoint)
    {
        using var http = new HttpClient();
        var tabs = await http.GetFromJsonAsync<List<ChromeTab>>($"{endpoint}/json");
        return tabs ?? [];
    }

    public static async Task<byte[]> CaptureScreenshotAsync(
        string webSocketDebuggerUrl,
        CancellationToken cancellationToken = default)
    {
        using var ws = new ClientWebSocket();
        await ws.ConnectAsync(new Uri(webSocketDebuggerUrl), cancellationToken);

        // Bring the tab to the foreground so Chrome renders WebGL/canvas content
        var bringToFrontCmd = JsonSerializer.Serialize(new
        {
            id = 1,
            method = "Page.bringToFront"
        });
        var bringToFrontBuffer = Encoding.UTF8.GetBytes(bringToFrontCmd);
        await ws.SendAsync(bringToFrontBuffer, WebSocketMessageType.Text, true, cancellationToken);
        await ReceiveFullMessageAsync(ws, cancellationToken);

        // Force a reflow to flush the compositor (ensures WebGL canvas is up-to-date)
        var reflowCmd = JsonSerializer.Serialize(new
        {
            id = 2,
            method = "Runtime.evaluate",
            @params = new { expression = "void(document.body.offsetHeight)", returnByValue = true }
        });
        var reflowBuffer = Encoding.UTF8.GetBytes(reflowCmd);
        await ws.SendAsync(reflowBuffer, WebSocketMessageType.Text, true, cancellationToken);
        await ReceiveFullMessageAsync(ws, cancellationToken);

        // Longer delay to let GPU compositor settle after bringing tab to foreground
        await Task.Delay(500, cancellationToken);

        var command = JsonSerializer.Serialize(new
        {
            id = 3,
            method = "Page.captureScreenshot",
            @params = new { format = "png", quality = 100, fromSurface = true }
        });

        var sendBuffer = Encoding.UTF8.GetBytes(command);
        await ws.SendAsync(sendBuffer, WebSocketMessageType.Text, true, cancellationToken);

        var response = await ReceiveFullMessageAsync(ws, cancellationToken);
        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "done", cancellationToken);

        var json = JsonSerializer.Deserialize<JsonElement>(response);
        var base64Data = json.GetProperty("result").GetProperty("data").GetString()!;

        return Convert.FromBase64String(base64Data);
    }

    public static async Task<string> EvaluateJavaScriptAsync(
        string webSocketDebuggerUrl,
        string expression,
        CancellationToken cancellationToken = default)
    {
        using var ws = new ClientWebSocket();
        await ws.ConnectAsync(new Uri(webSocketDebuggerUrl), cancellationToken);

        var command = JsonSerializer.Serialize(new
        {
            id = 1,
            method = "Runtime.evaluate",
            @params = new { expression, returnByValue = true }
        });

        var sendBuffer = Encoding.UTF8.GetBytes(command);
        await ws.SendAsync(sendBuffer, WebSocketMessageType.Text, true, cancellationToken);

        var response = await ReceiveFullMessageAsync(ws, cancellationToken);
        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "done", cancellationToken);

        var json = JsonSerializer.Deserialize<JsonElement>(response);
        var result = json.GetProperty("result").GetProperty("result");

        if (result.TryGetProperty("value", out var value))
        {
            return value.ToString();
        }

        if (result.TryGetProperty("description", out var desc))
        {
            return $"ERROR: {desc.GetString()}";
        }

        return result.GetRawText();
    }

    public static async Task<string> NavigateAsync(
        string webSocketDebuggerUrl,
        string url,
        int timeoutMs = 15000,
        CancellationToken cancellationToken = default)
    {
        using var ws = new ClientWebSocket();
        await ws.ConnectAsync(new Uri(webSocketDebuggerUrl), cancellationToken);

        var enableCmd = JsonSerializer.Serialize(new
        {
            id = 1,
            method = "Page.enable"
        });
        await ws.SendAsync(Encoding.UTF8.GetBytes(enableCmd), WebSocketMessageType.Text, true, cancellationToken);
        await ReceiveFullMessageAsync(ws, cancellationToken);

        var navigateCmd = JsonSerializer.Serialize(new
        {
            id = 2,
            method = "Page.navigate",
            @params = new { url }
        });
        await ws.SendAsync(Encoding.UTF8.GetBytes(navigateCmd), WebSocketMessageType.Text, true, cancellationToken);

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(timeoutMs);

        bool loadFired = false;
        string? errorText = null;

        try
        {
            while (!loadFired)
            {
                var response = await ReceiveFullMessageAsync(ws, timeoutCts.Token);
                var json = JsonSerializer.Deserialize<JsonElement>(response);

                if (json.TryGetProperty("id", out var idProp) && idProp.GetInt32() == 2
                    && json.TryGetProperty("result", out var result)
                    && result.TryGetProperty("errorText", out var err))
                {
                    errorText = err.GetString();
                    break;
                }

                if (json.TryGetProperty("method", out var method)
                    && method.GetString() == "Page.loadEventFired")
                {
                    loadFired = true;
                }
            }
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            // Timeout is OK for SPAs like Google Maps that load progressively
        }

        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "done", cancellationToken);

        if (errorText != null)
        {
            return $"ERROR: {errorText}";
        }

        return "OK";
    }

    public static async Task<ChromeTab?> GetPageTabAsync(int tabIndex, string endpoint = DefaultEndpoint)
    {
        var tabs = await ListTabsAsync(endpoint);
        var pages = tabs.Where(t => t.Type == "page").ToList();
        if (pages.Count == 0 || tabIndex < 0 || tabIndex >= pages.Count)
        {
            return null;
        }

        return pages[tabIndex];
    }

    private static async Task<string> ReceiveFullMessageAsync(
        ClientWebSocket ws,
        CancellationToken cancellationToken)
    {
        var buffer = new byte[64 * 1024];
        var sb = new StringBuilder();

        WebSocketReceiveResult result;
        do
        {
            result = await ws.ReceiveAsync(buffer, cancellationToken);
            sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
        } while (!result.EndOfMessage);

        return sb.ToString();
    }
}
