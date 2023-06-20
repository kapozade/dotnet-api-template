namespace Supreme.Domain.Constants;

public static class MessageKeys
{
    public const string _notFound = "{0}.not.found";
    
    public static string GenerateMessage(string template, params object[] param)
    {
        return string.Format(template, param);
    }
}
