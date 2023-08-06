namespace KUSYS_Demo.WebApi.Helpers;

using Microsoft.EntityFrameworkCore;
using KUSYS_Demo.WebApi.Entities;

public class DataContext : DbContext
{
    protected readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<StudentCourse> StudentCourses { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // connect to sql server database
        options.UseSqlServer(Configuration.GetConnectionString("WebApiDatabase"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasMany(s => s.StudentCourses)
            .WithOne(sc => sc.Student)
            .HasForeignKey(sc => sc.StudentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(s => s.User)
            .WithOne(u => u.Student)
            .OnDelete(DeleteBehavior.Cascade);

            entity.Property(s => s.FirstName)
            .IsRequired();

            entity.Property(s => s.LastName)
            .IsRequired();

            entity.Property(s => s.IdentityNumber)
            .IsRequired();

            entity.Property(s => s.BirthDate)
            .IsRequired();
        });


        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(c => c.CourseId);

            entity.HasMany(s => s.StudentCourses)
            .WithOne(sc => sc.Course)
            .HasForeignKey(sc => sc.CourseId);

            entity.Property(s => s.CourseName)
            .IsRequired();
        });


        // Unique constraint for StudentCourse table (a student can select a course once)
        modelBuilder.Entity<StudentCourse>(entity =>
        {
            entity.HasKey(sc => sc.StudentCourseId);

            entity.HasOne(sc => sc.Student)
            .WithMany(s => s.StudentCourses)
            .HasForeignKey(sc => sc.StudentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(sc => new { sc.StudentId, sc.CourseId })
            .IsUnique();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasMany(s => s.UserRoles)
            .WithOne(sc => sc.User)
            .HasForeignKey(sc => sc.UserId);

            entity.Property(u => u.IsActive)
            .HasDefaultValue(true)
            .IsRequired();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasMany(s => s.UserRoles)
            .WithOne(sc => sc.Role)
            .HasForeignKey(sc => sc.RoleId);

            entity.Property(s => s.Name)
            .IsRequired();
        });


        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });

            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
        });
    }
}