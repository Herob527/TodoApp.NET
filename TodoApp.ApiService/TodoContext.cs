using Microsoft.EntityFrameworkCore;
using TodoApp.Domain;

namespace TodoApp.ApiService;

public class TodoContext(DbContextOptions<TodoContext> options)
    : DbContext(options)
{
  public DbSet<Todo> Todos {get; set;}
}
