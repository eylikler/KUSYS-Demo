using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using KUSYS_Demo.WebApi.Authorization;
using KUSYS_Demo.WebApi.Helpers;
using KUSYS_Demo.WebApi.Models.Users;
using KUSYS_Demo.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace KUSYS_Demo.WebApi.Controllers;


[Authorize]
[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private IUserService _userService;
    private IMapper _mapper;
    private readonly AppSettings _appSettings;

    public UsersController(
        IUserService userService,
        IMapper mapper,
        IOptions<AppSettings> appSettings)
    {
        _userService = userService;
        _mapper = mapper;
        _appSettings = appSettings.Value;
    }

    [HttpGet("auth")]
    public IActionResult GetAuthUser()
    {
        var response = _userService.GetAuthUser();

        return Ok(response);
    }

    [HttpGet("rolecheck")]
    public IActionResult RoleCheck(string roleName)
    {
        var response = _userService.IsUserInRole(roleName);

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public IActionResult Authenticate(AuthenticateRequest model)
    {
        var response = _userService.Authenticate(model);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult Register(RegisterRequest model)
    {
        _userService.Register(model);
        return Ok(new { message = "Registration successful" });
    }

    [Authorize("Admin")]
    [HttpGet]
    public IActionResult GetAll()
    {
        var users = _userService.GetAll();
        return Ok(users);
    }

    [Authorize("Admin")]
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var user = _userService.GetById(id);
        return Ok(user);
    }

    [Authorize("Admin")]
    [HttpPut("{id}")]
    public IActionResult Update(int id, UpdateRequest model)
    {
        _userService.Update(id, model);
        return Ok(new { message = "User updated successfully" });
    }

    [Authorize("Admin")]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _userService.Delete(id);
        return Ok(new { message = "User deleted successfully" });
    }
}