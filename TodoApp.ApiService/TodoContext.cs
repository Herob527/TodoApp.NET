using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entity;

namespace TodoApp.ApiService;

public class TodoContext(DbContextOptions<TodoContext> options)
    : DbContext(options)
{
  public DbSet<Todo> Todos {get; set;}
  public DbSet<User> Users {get; set;}
}
