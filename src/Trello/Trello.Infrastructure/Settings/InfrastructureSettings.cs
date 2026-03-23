namespace Trello.Infrastructure.Settings;

public class InfrastructureSettings
{
    public required string TrelloApiKey { get; set; }
    public required string TrelloApiToken { get; set; }
    public bool DisableSslValidation { get; set; }
}
