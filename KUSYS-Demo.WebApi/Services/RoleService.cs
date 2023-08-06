namespace KUSYS_Demo.WebApi.Services;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using KUSYS_Demo.WebApi.Authorization;
using KUSYS_Demo.WebApi.Entities;
using KUSYS_Demo.WebApi.Helpers;
using KUSYS_Demo.WebApi.Models.Roles;

public interface IRoleService
{
    IEnumerable<Role> GetAll();
    Role GetById(int id);
    void Insert(InsertRoleRequest model);
    void Update(int id, UpdateRoleRequest model);
    void Delete(int id);
    Task<bool> RoleExists(string roleName);
    Task<Role> GetRoleByNameAsync(string roleName);
    Task<int> CreateRoleAsync(string roleName);
}

public class RoleService : IRoleService
{
    private DataContext _context;
    private readonly IMapper _mapper;

    public RoleService(
        DataContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public IEnumerable<Role> GetAll()
    {
        return _context.Roles;
    }

    public Role GetById(int id)
    {
        return getRole(id);
    }

    public void Insert(InsertRoleRequest model)
    {
        // validate
        if (_context.Roles.Any(x => x.Name == model.Name))
            throw new AppException("Role Name '" + model.Name + "' is already exist");

        // map model to new role object
        var role = _mapper.Map<Role>(model);

        // save role
        _context.Roles.Add(role);
        _context.SaveChanges();
    }

    public void Update(int id, UpdateRoleRequest model)
    {
        var role = getRole(id);

        // validate
        if (model.Name != role.Name && _context.Roles.Any(x => x.Name == model.Name))
            throw new AppException("Role Name '" + model.Name + "' is already exist");

        // copy model to role and save
        _mapper.Map(model, role);
        _context.Roles.Update(role);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var role = getRole(id);
        _context.Roles.Remove(role);
        _context.SaveChanges();
    }

    public async Task<bool> RoleExists(string roleName)
    {
        return await _context.Roles.AnyAsync(x => x.Name == roleName);
    }

    public async Task<Role> GetRoleByNameAsync(string roleName)
    {
        return await _context.Roles.FirstOrDefaultAsync(x => x.Name == roleName);
    }

    public async Task<int> CreateRoleAsync(string roleName)
    {
        // validate
        if (await RoleExists(roleName))
            return 0;

        var role = new Role
        {
            Name = roleName
        };

        // save role
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        return role.Id;
    }

    // helper methods

    private Role getRole(int id)
    {
        var role = _context.Roles.Find(id);
        if (role == null) throw new KeyNotFoundException("Role not found");

        return role;
    }
}