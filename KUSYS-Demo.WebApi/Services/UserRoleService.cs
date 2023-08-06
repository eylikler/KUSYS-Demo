namespace KUSYS_Demo.WebApi.Services;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using KUSYS_Demo.WebApi.Authorization;
using KUSYS_Demo.WebApi.Entities;
using KUSYS_Demo.WebApi.Helpers;

public interface IUserRoleService
{
    UserRole GetById(int id);
    Task CreateUserRoleAsync(UserRole userRole);
    Task GenerateUserRoleAsync(UserRole userRole);
}

public class UserRoleService : IUserRoleService
{
    private DataContext _context;

    public UserRoleService(DataContext context)
    {
        _context = context;
    }

    public UserRole GetById(int id)
    {
        return getUserRole(id);
    }

    public async Task CreateUserRoleAsync(UserRole userRole)
    {
        // validate
        if (await _context.UserRoles.AnyAsync(x => x.UserId == userRole.UserId && x.RoleId == userRole.RoleId))
            throw new AppException("User '" + userRole.User.Username + "' already has Role " + userRole.Role.Name);

        // save user role
        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync();
    }

    public async Task GenerateUserRoleAsync(UserRole userRole)
    {
        // validate
        if (!(await _context.UserRoles.AnyAsync(x => x.UserId == userRole.UserId && x.RoleId == userRole.RoleId)))
        {
            // save user role
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
        }
    }

    // helper methods

    private UserRole getUserRole(int id)
    {
        var userRole = _context.UserRoles.Find(id);
        if (userRole == null) throw new KeyNotFoundException("User Role not found");

        //user.UserRoles = 

        return userRole;
    }
}