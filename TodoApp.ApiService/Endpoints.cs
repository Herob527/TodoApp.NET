using Microsoft.AspNetCore.Mvc;
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
    private static async Task<IResult> GetTodos([FromServices] TodoContext db, [FromQuery] int page = 0)
    {
        const int pageSize = 10;
        var todos = await db.Todos.Skip(page * pageSize).Take(pageSize).ToListAsync();
        return Results.Json(todos);
    }

    private static async Task<IResult> AddTodo([FromServices] TodoContext db) => Results.Json(new { message = "Hello from .NET 10" });
    private static async Task<IResult> DeleteTodo([FromServices] TodoContext db) => Results.Json(new { message = "Hello from .NET 10" });
    private static async Task<IResult> UpdateTodo([FromServices] TodoContext db) => Results.Json(new { message = "Hello from .NET 10" });
}
