namespace Rabbit.Api.Responses;

public class ExchangeResponse
{
    public required string Name { get; set; }
    public required string Type { get; set; }
    public required string VirtualHost { get; set; }
    public bool Durable { get; set; }
    public bool AutoDelete { get; set; }
    public bool Internal { get; set; }
}
