namespace Rabbit.Domain.Entities;

public class Queue
{
    public required string Name { get; set; }
    public required string VirtualHost { get; set; }
    public long Messages { get; set; }
    public long MessagesReady { get; set; }
    public long MessagesUnacknowledged { get; set; }
    public long Consumers { get; set; }
    public string? State { get; set; }
    public bool Durable { get; set; }
    public bool AutoDelete { get; set; }
    public bool Exclusive { get; set; }
}
