using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoApp.ApiService.Endpoints.Todo.Commands.CreateTodo;
using TodoApp.ApiService.Endpoints.Todo.Commands.DeleteTodo;
using TodoApp.ApiService.Endpoints.Todo.Commands.UpdateTodo;
using TodoApp.ApiService.Endpoints.Todo.Queries.GetTodos;

namespace TodoApp.ApiService.Endpoints.Todo;

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
    private static async Task<IResult> GetTodos(ISender sender, [FromQuery] int page)
    {
        var query = new GetTodosQuery(page);
        var res = await sender.Send(query);
        return TypedResults.Ok(res);
    }

    private record AddTodoRequest(string Name);

    private static async Task<IResult> AddTodo(ISender sender, [FromForm] AddTodoRequest todo)
    {
        var command = new CreateTodoCommand(todo.Name);
        await sender.Send(command);

        return TypedResults.Created();
    }

    private static async Task<IResult> DeleteTodo(ISender sender, [FromRoute] int id)
    {
        var command = new DeleteTodoCommand(id);
        await sender.Send(command);
        return TypedResults.Ok();
    }

    private static async Task<IResult> UpdateTodo(ISender sender, [FromRoute] int id, [FromForm] AddTodoRequest todo)
    {
        var command = new UpdateTodoCommand(id, todo.Name);
        await sender.Send(command);
        return TypedResults.Ok();
    }
}
