namespace Rabbit.Infrastructure.Settings;

public class InfrastructureSettings
{
    public required string RabbitMqUri { get; set; }
    public required string RabbitMqManagementUri { get; set; }
    public string RabbitMqManagementUsername { get; set; } = "guest";
    public string RabbitMqManagementPassword { get; set; } = "guest";
    public string Vhost { get; set; } = "/";
}
