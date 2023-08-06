using KUSYS_Demo.WebApi.Authorization;
using KUSYS_Demo.WebApi.Helpers;
using KUSYS_Demo.WebApi.Mapper;
using KUSYS_Demo.WebApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// add services to DI container
{
    var services = builder.Services;
    var env = builder.Environment;

    services.AddDbContext<DataContext>();

    services.AddCors();
    services.AddControllers();

    // configure automapper
    services.AddAutoMapper(typeof(Program));

    services.AddHttpContextAccessor();


    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

    // configure DI for application services
    services.AddScoped<IJwtUtils, JwtUtils>();

    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IRoleService, RoleService>();
    services.AddScoped<IUserRoleService, UserRoleService>();
    services.AddScoped<IStudentService, StudentService>();
    services.AddScoped<ICourseService, CourseService>();
    services.AddScoped<IStudentCourseService, StudentCourseService>();

    services.AddScoped<StudentMapper>();
    services.AddScoped<StudentCourseMapper>();

    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "KUSYS-Demo",
            Version = "v1",
            Description = "Koç University Student Information System"
        });
    });
}

var app = builder.Build();

// migrate any database changes on startup (includes initial db creation)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var dataContext = services.GetRequiredService<DataContext>();
    dataContext.Database.Migrate();

    var roleService = services.GetRequiredService<IRoleService>();
    var userService = services.GetRequiredService<IUserService>();
    var userRoleService = services.GetRequiredService<IUserRoleService>();
    var courseService = services.GetRequiredService<ICourseService>();
    SeedData.InitializeAsync(roleService, userService, userRoleService, courseService).Wait();
}

// configure HTTP request pipeline
{
    // global cors policy
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    // global error handler
    app.UseMiddleware<ErrorHandlerMiddleware>();

    // custom jwt auth middleware
    app.UseMiddleware<JwtMiddleware>();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "KUSYS-Demo v1");
});

app.MapGet("/users/authenticate", () => "Hello World!").RequireAuthorization();

app.Run("http://localhost:4000");