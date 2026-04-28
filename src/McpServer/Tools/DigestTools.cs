using System.ComponentModel;
using System.Reflection;
using ModelContextProtocol.Server;

namespace McpServer.Tools;

[McpServerToolType]
public static class DigestTools
{
    [McpServerTool(Name = "get_digest_config")]
    [Description("Get the teams configuration containing team definitions, user ID mappings across services (GitHub, Jira, Confluence), and team-specific settings like estimation parameters. Use this to look up user identifiers before fetching activity or running analysis.")]
    public static async Task<string> GetDigestConfig()
    {
        var path = FindTeamsConfig();

        if (path is null)
        {
            return "teams.json not found. Copy teams.example.json to teams.json and fill in user IDs.";
        }

        return await File.ReadAllTextAsync(path);
    }

    private static string? FindTeamsConfig()
    {
        var candidate = Path.Combine(Directory.GetCurrentDirectory(), "teams.json");
        if (File.Exists(candidate))
        {
            return candidate;
        }

        var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        while (dir is not null)
        {
            candidate = Path.Combine(dir, "teams.json");
            if (File.Exists(candidate))
            {
                return candidate;
            }

            dir = Path.GetDirectoryName(dir);
        }

        return null;
    }
}
