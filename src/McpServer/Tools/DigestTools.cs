using System.ComponentModel;
using System.Reflection;
using ModelContextProtocol.Server;

namespace McpServer.Tools;

[McpServerToolType]
public static class DigestTools
{
    [McpServerTool(Name = "get_digest_config")]
    [Description("Get the digest configuration containing team definitions and user ID mappings across services (GitHub, Jira, Confluence). Use this to look up user identifiers before fetching activity.")]
    public static async Task<string> GetDigestConfig()
    {
        var path = FindDigestConfig();

        if (path is null)
        {
            return "digest.json not found. Copy digest.example.json to digest.json and fill in user IDs.";
        }

        return await File.ReadAllTextAsync(path);
    }

    private static string? FindDigestConfig()
    {
        var candidate = Path.Combine(Directory.GetCurrentDirectory(), "digest.json");
        if (File.Exists(candidate))
        {
            return candidate;
        }

        var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        while (dir is not null)
        {
            candidate = Path.Combine(dir, "digest.json");
            if (File.Exists(candidate))
            {
                return candidate;
            }

            dir = Path.GetDirectoryName(dir);
        }

        return null;
    }
}
