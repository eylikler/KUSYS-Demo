namespace KUSYS_Demo.WebApi.Helpers;

public class AppSettings
{
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public TimeSpan ValidFor { get; set; }
}