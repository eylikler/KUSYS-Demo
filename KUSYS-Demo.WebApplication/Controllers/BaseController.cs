using KUSYS_Demo.WebApplication.DTOs.Users;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KUSYS_Demo.WebApplication.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IHttpClientFactory _httpClientFactory;

        public BaseController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async override void OnActionExecuting(ActionExecutingContext context)
        {
            //TODO: Daha sonra kontrol edilecek.
            //var isAdmin = await RoleCheck("Admin");
            //ViewData["IsAdmin"] = isAdmin;

            //var getAuthUser = await GetAuthUser();

            //ViewData["AuthUser"] = getAuthUser;
        }

        public async Task<UserDto> GetAuthUser()
        {
            var apiUrl = "http://localhost:4000/Users/auth";

            try
            {
                var httpClient = _httpClientFactory.CreateClient();

                var accessToken = HttpContext.Session.GetString("_JWToken");

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var response = await httpClient.GetAsync(apiUrl);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var userDto = JsonSerializer.Deserialize<UserDto>(responseContent);

                    return userDto;
                }
            }
            catch (Exception)
            {

            }

            return null;
        }

        public async Task<bool> RoleCheck(string roleName)
        {
            var apiUrl = "http://localhost:4000/Users/rolecheck?roleName=" + roleName;
            bool isAdmin = false;

            try
            {
                var httpClient = _httpClientFactory.CreateClient();

                var accessToken = HttpContext.Session.GetString("_JWToken");

                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var response = await httpClient.GetAsync(apiUrl);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    isAdmin = JsonSerializer.Deserialize<bool>(responseContent);
                }
            }
            catch (Exception)
            {

            }

            return isAdmin;
        }
    }
}
