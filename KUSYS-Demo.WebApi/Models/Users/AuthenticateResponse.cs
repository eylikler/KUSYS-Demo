namespace KUSYS_Demo.WebApi.Models.Users;

public class AuthenticateResponse
{
    public string Username { get; set; }
    public string Token { get; set; }
    public int ExpirationInMinutes { get; set; }
}