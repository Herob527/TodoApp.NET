using Microsoft.EntityFrameworkCore;

namespace TodoApp.ApiService;
public static class Endpoints
{

  extension(IEndpointRouteBuilder app) {
    public void MapTodoEndpoints() {

      var group = app.MapGroup("/api/todos").WithTags("Todos");
      group.MapGet("/", GetTodos);
      group.MapPost("/", AddTodo);
      group.MapDelete("/", DeleteTodo);
      group.MapPatch("/", UpdateTodo);

    }
  }
    private static async Task<IResult> GetTodos(DbContext db)
    {
        return Results.Json(new { message = "Hello from .NET 10" });
    }

    private static async Task<IResult> AddTodo(DbContext db) => Results.Json(new { message = "Hello from .NET 10" });
    private static async Task<IResult> DeleteTodo(DbContext db) => Results.Json(new { message = "Hello from .NET 10" });
    private static async Task<IResult> UpdateTodo(DbContext db) => Results.Json(new { message = "Hello from .NET 10" });
}
