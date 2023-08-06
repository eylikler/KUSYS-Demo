namespace KUSYS_Demo.WebApplication.DTOs.Users
{
    public class AuthResponseDto
    {
        public string username { get; set; }
        public string token { get; set; }
        public int expirationInMinutes { get; set; }
    }
}
