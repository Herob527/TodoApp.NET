using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoApp.ApiService.Endpoints.User.Commands.AddUser;
using TodoApp.ApiService.Endpoints.User.Commands.DeleteUser;
using TodoApp.ApiService.Endpoints.User.Commands.RegisterUser;
using TodoApp.ApiService.Endpoints.User.Commands.UpdateUser;
using TodoApp.ApiService.Endpoints.User.Queries.GetUser;
using TodoApp.ApiService.Endpoints.User.Queries.GetUsers;

namespace TodoApp.ApiService.Endpoints.User;
public static class Endpoints
{

    extension(IEndpointRouteBuilder app)
    {
        public void MapUserEndpoints()
        {
            var group = app.MapGroup("/api/users").WithTags("Users").DisableAntiforgery();

            group.MapGet("/{id}", GetUser);
            group.MapGet("/", GetUsers);
            group.MapPost("/", AddUser);
            group.MapPost("/register", RegisterUser);
            group.MapPatch("/{id}", UpdateUser);
            group.MapDelete("/{id}", DeleteUser);
        }
    }

    private static async Task<IResult> GetUser(ISender sender, [FromRoute] int id)
    {
        var query = new GetUserQuery(id);
        var result = await sender.Send(query);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> GetUsers(ISender sender, [FromQuery] int page)
    {
        var query = new GetUsersQuery(page);
        var result = await sender.Send(query);
        return TypedResults.Ok(result);
    }

    private static async Task<IResult> AddUser(ISender sender, [FromForm] AddUserCommand command)
    {
        await sender.Send(command);
        return TypedResults.Created();
    }

    private static async Task<IResult> RegisterUser(ISender sender, [FromForm] RegisterUserCommand command)
    {
        await sender.Send(command);
        return TypedResults.Created();
    }

    private static async Task<IResult> UpdateUser(ISender sender, [FromRoute] int id, [FromForm] UpdateUserData data)
    {
        var command = new UpdateUserCommand(id, data);

        await sender.Send(command);
        return TypedResults.Ok();
    }
    private static async Task<IResult> DeleteUser(ISender sender, [FromRoute] int id)
    {
        var command = new DeleteUserCommand(id);
        await sender.Send(command);
        return TypedResults.Ok();
    }
}
