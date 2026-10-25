using Microsoft.AspNetCore.Mvc;
using TodoApp.ApiService.Endpoints.Todo.Queries.GetTodos;

namespace TodoApp.ApiService.Endpoints.Todo
{
    public static class Endpoints
    {
        extension(IEndpointRouteBuilder app)
        {
            public void MapTodoEndpoints()
            {

                var group = app.MapGroup("/api/todos").WithTags("Todos").DisableAntiforgery();
                group.MapGet("", GetTodos);
                group.MapPost("", AddTodo);
                group.MapDelete("{id}", DeleteTodo);
                group.MapPatch("{id}", UpdateTodo);

            }
        }
        private static async Task<IResult> GetTodos([FromQuery] int page)
        {
            var todos = new GetTodosQuery { Page = page };
            return Results.Json(todos);
        }

        private record AddTodoRequest(string Name);

        private static async Task<IResult> AddTodo([FromServices] TodoContext db, [FromForm] AddTodoRequest todo)
        {
            db.Todos.Add(new Todo { Name = todo.Name });
            await db.SaveChangesAsync();
            return Results.Created();
        }

        private static async Task<IResult> DeleteTodo([FromServices] TodoContext db, [FromRoute] int id)
        {
            var item = await db.Todos.FindAsync(id);
            if (item is null) return Results.NotFound();
            db.Remove(item);
            await db.SaveChangesAsync();
            return Results.StatusCode(200);
        }

        private static async Task<IResult> UpdateTodo([FromServices] TodoContext db, [FromRoute] int id, [FromForm] AddTodoRequest todo)
        {
            var item = await db.Todos.FindAsync(id);
            if (item is null) return Results.NotFound();
            item.Name = todo.Name;
            await db.SaveChangesAsync();
            return Results.StatusCode(200);
        }
    }
}
