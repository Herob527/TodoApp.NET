using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TodoApp.ApiService;
public static class Endpoints
{

  extension(IEndpointRouteBuilder app) {
    public void MapTodoEndpoints() {

      var group = app.MapGroup("/api/todos").WithTags("Todos").DisableAntiforgery();
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

    private record AddTodoRequest(string Name);

    private static async Task<IResult> AddTodo([FromServices] TodoContext db, [FromForm] AddTodoRequest todo)
    {
        db.Todos.Add(new Todo { Name = todo.Name });
        await db.SaveChangesAsync();
        return Results.Created();
    }

    private static async Task<IResult> DeleteTodo([FromServices] TodoContext db, [FromQuery] int id)
    {
      var item = await db.Todos.SingleAsync(t => t.Id == id);
      if (item is null) return Results.NotFound();
      db.Remove(item);
      await db.SaveChangesAsync();
      return Results.StatusCode(200);
    }

    private static async Task<IResult> UpdateTodo([FromServices] TodoContext db, [FromQuery] int id, [FromBody] string todo)
    {
      var item = await db.Todos.SingleAsync(t => t.Id == id);
      if (item is null) return Results.NotFound();
      item.Name = todo;
      await db.SaveChangesAsync();
      return Results.StatusCode(200);
    }
}
