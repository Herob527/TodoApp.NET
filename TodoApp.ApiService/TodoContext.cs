using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entity;

namespace TodoApp.ApiService;

public class TodoContext(DbContextOptions<TodoContext> options)
        : IdentityDbContext<ApplicationUser>(options)
{

    // Your existing todo entities:
    public DbSet<Todo> Todos => Set<Todo>();
}
