namespace TodoApp.Domain.Entity;

using TodoApp.Domain.Enum;

using Microsoft.AspNetCore.Identity;


public class ApplicationUser: IdentityUser
{
    public Role Role { get; set; }

    public ICollection<Todo> Todos { get; set; } = [];
}
