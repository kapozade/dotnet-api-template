namespace Supreme.Domain.Settings;

public record RedisSettings
{
    public string Host { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int DbId { get; set; }
    public int Port { get; set; }
}
