using KUSYS_Demo.WebApplication.DTOs.Students;
using KUSYS_Demo.WebApplication.DTOs.Users;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace KUSYS_Demo.WebApplication.Controllers
{
    public class DashboardController : BaseController
    {
        public DashboardController(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {

        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Ana Sayfa";

            return View();
        }
    }
}
