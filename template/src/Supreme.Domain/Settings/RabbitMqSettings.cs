namespace Supreme.Domain.Settings;

public record RabbitMqSettings
{
    public string Host { get; set; } = string.Empty;
    public string VirtualHost { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Port { get; set; }
}
