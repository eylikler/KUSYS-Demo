namespace KUSYS_Demo.WebApi.Services;

using AutoMapper;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using KUSYS_Demo.WebApi.Authorization;
using KUSYS_Demo.WebApi.Entities;
using KUSYS_Demo.WebApi.Helpers;
using KUSYS_Demo.WebApi.Models.Users;
using Microsoft.Extensions.Options;

public interface IUserService
{
    User GetAuthUser();
    bool IsUserInRole(string roleName);
    AuthenticateResponse Authenticate(AuthenticateRequest model);
    IEnumerable<User> GetAll();
    User GetById(int id);
    void Register(RegisterRequest model);
    void Update(int id, UpdateRequest model);
    void Delete(int id);
    Task<User> GetUserByUserNameAsync(string userName);
    Task<int> GenerateUserAsync(string userName, string password);
}

public class UserService : IUserService
{
    private DataContext _context;
    private IJwtUtils _jwtUtils;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppSettings _appSettings;

    public UserService(
        DataContext context,
        IJwtUtils jwtUtils,
        IMapper mapper,
        IOptions<AppSettings> appSettings,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _jwtUtils = jwtUtils;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _appSettings = appSettings.Value;
    }

    public User GetAuthUser()
    {
        return _httpContextAccessor.HttpContext.Items["User"] as User;
    }

    public bool IsUserInRole(string roleName)
    {
        var user = _httpContextAccessor.HttpContext.Items["User"] as User;

        if (user.UserRoles != null)
        {
            return user.UserRoles.Select(ur => ur.Role.Name).Any(role => role.Equals(roleName, StringComparison.OrdinalIgnoreCase));
            //return user.UserRoles.Any(role => role.Role.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
        }

        return false;
    }

    public AuthenticateResponse Authenticate(AuthenticateRequest model)
    {
        //var user = _context.Users.SingleOrDefault(x => x.Username == model.Username);
        var user = _context.Users
            .Where(x=>x.Username == model.Username)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefault();

        // validate
        if (user == null || !BCrypt.Verify(model.Password, user.PasswordHash))
            throw new AppException("Username or password is incorrect");    //TODO: Döngüye giriyor düzeltilecek!

        // authentication successful
        var response = _mapper.Map<AuthenticateResponse>(user);
        response.Token = _jwtUtils.GenerateToken(user);
        response.ExpirationInMinutes = (int)_appSettings.ValidFor.TotalMinutes;

        return response;
    }

    public IEnumerable<User> GetAll()
    {
        var result = _context.Users
            .Include(s => s.Student)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role);

        return result;
    }

    public User GetById(int id)
    {
        return getUser(id);
    }

    public void Register(RegisterRequest model)
    {
        // validate
        if (_context.Users.Any(x => x.Username == model.Username))
            throw new AppException("Username '" + model.Username + "' is already taken");

        // map model to new user object
        var user = _mapper.Map<User>(model);

        // hash password
        user.PasswordHash = BCrypt.HashPassword(model.Password);

        // save user
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void Update(int id, UpdateRequest model)
    {
        var user = getUser(id);

        // validate
        if (model.Username != user.Username && _context.Users.Any(x => x.Username == model.Username))
            throw new AppException("Username '" + model.Username + "' is already taken");

        // hash password if it was entered
        if (!string.IsNullOrEmpty(model.Password))
            user.PasswordHash = BCrypt.HashPassword(model.Password);

        // copy model to user and save
        _mapper.Map(model, user);
        _context.Users.Update(user);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var user = getUser(id);
        _context.Users.Remove(user);
        _context.SaveChanges();
    }

    public async Task<User> GetUserByUserNameAsync(string userName)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Username == userName);
    }

    public async Task<int> GenerateUserAsync(string userName, string password)
    {
        // validate
        if (await _context.Users.AnyAsync(x => x.Username == userName))
            throw new AppException("Username '" + userName + "' is already taken");

        var user = new User
        {
            Username = userName,
            PasswordHash = BCrypt.HashPassword(password),
            IsActive = true
        };

        // save user
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user.Id;
    }

    // helper methods

    private User getUser(int id)
    {
        var user = _context.Users
            .Include(s => s.Student)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefault(u => u.Id == id);

        if (user == null)
            throw new KeyNotFoundException("User not found");

        return user;
    }
}