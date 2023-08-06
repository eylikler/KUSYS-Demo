namespace KUSYS_Demo.WebApi.Entities;

using System.Text.Json.Serialization;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }

    [JsonIgnore]
    public virtual ICollection<UserRole> UserRoles { get; set; } = null;
}
