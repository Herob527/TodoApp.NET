namespace TodoApp.Domain.Entity;

using Microsoft.AspNetCore.Identity;


public class ApplicationUser: IdentityUser
{
    public ICollection<Todo> Todos { get; set; } = [];
}
