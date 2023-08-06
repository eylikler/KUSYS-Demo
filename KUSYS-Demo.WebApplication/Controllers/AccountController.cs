using KUSYS_Demo.WebApplication.DTOs.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.Json;

namespace KUSYS_Demo.WebApplication.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {

        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserDto model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.username) || string.IsNullOrEmpty(model.password))
                {
                    ViewData["Message"] = "Kullanıcı adı veya Parola boş geçilemez.";

                    return View(model);
                }

                var errMessage = await GetTokenAsync(model.username, model.password);

                if (!string.IsNullOrEmpty(errMessage))
                {
                    ViewData["Message"] = errMessage;

                    return View(model);
                }                
            }
            catch (Exception)
            {
                ViewData["Message"] = "Bir hata oluştu.";
            }

            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult Logoff()
        {
            HttpContext.Session.Clear();    // clear token
            return RedirectToAction("Login", "Account");
        }

        public async Task<string> GetTokenAsync(string username, string password)
        {
            var apiAuthUrl = "http://localhost:4000/users/authenticate";

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var content = new StringContent(JsonSerializer.Serialize(new { username, password }), Encoding.UTF8, "application/json");

                var authResponse = await httpClient.PostAsync(apiAuthUrl, content);
                var authResponseContent = await authResponse.Content.ReadAsStringAsync();

                if (authResponse.StatusCode == HttpStatusCode.OK)
                {
                    var userResponse = JsonSerializer.Deserialize<AuthResponseDto>(authResponseContent);

                    HttpContext.Session.SetString("_JWToken", userResponse.token);

                    SetUserRolesFromToken(userResponse.token);

                    return "";
                }
                else
                {
                    var responseMsg = JsonSerializer.Deserialize<MessageDto>(authResponseContent);

                    return responseMsg.message;
                }
            }
            catch (Exception)
            {
                return "Bir hata oluştu.";
            }
        }

        public void SetUserRolesFromToken(string jwtToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadToken(jwtToken) as JwtSecurityToken;

            if (token != null)
            {
                // Rolleri al
                var roles = token.Claims.Where(c => c.Type == "roles").Select(c => c.Value).ToList();
                //var roles = token.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

                // Geçici olarak rolleri sakla
                var identity = new ClaimsIdentity(roles.Select(r => new Claim("roles", r)));
                //var identity = new ClaimsIdentity(roles.Select(r => new Claim(ClaimTypes.Role, r)));
                var principal = new ClaimsPrincipal(identity);
                HttpContext.User = principal;
            }
        }
    }
}
