using Supreme.Domain.Settings;

namespace Supreme.Domain.Extensions;

public static class RedisSettingsExtensions 
{
    public static string GenerateConnectionString(this RedisSettings value)
    {
        return $"{value.Host}:{value.Port},defaultDatabase={value.DbId},password={value.Password},user={value.User}";
    }
}