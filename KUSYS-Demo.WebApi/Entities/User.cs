using System.Text.Json.Serialization;

namespace KUSYS_Demo.WebApi.Entities;

public class User
{
    public int Id { get; set; }
    public int? StudentId { get; set; } = null;
    public string Username { get; set; } = null!;

    [JsonIgnore]
    public string? PasswordHash { get; set; } = null;
    public bool? IsActive { get; set; } = true;

    public virtual ICollection<UserRole>? UserRoles { get; set; } = null;
    public virtual Student? Student { get; set; } = null;
}