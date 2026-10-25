using Microsoft.EntityFrameworkCore;

namespace TodoApp.ApiService;

public class TodoContext(DbContextOptions<TodoContext> options)
    : DbContext(options)
{
  public DbSet<Todo> Todos {get; set;}
}
