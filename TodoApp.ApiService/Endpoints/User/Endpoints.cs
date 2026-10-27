using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoApp.ApiService.Endpoints.User.Commands.CreateUser;
using TodoApp.ApiService.Endpoints.User.Commands.DeleteUser;
using TodoApp.ApiService.Endpoints.User.Commands.LoginUser;
using TodoApp.ApiService.Endpoints.User.Commands.LogoutUser;
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
            var group = app.MapGroup("/api/users").WithTags("Users");

            group.MapGet("/{id}", GetUser);
            group.MapGet("/", GetUsers);
            group.MapPost("/", AddUser);
            group.MapPost("/register", RegisterUser);
            group.MapPost("/login", LoginUser);
            group.MapPost("/logout", LogoutUser);
            group.MapPatch("/{id}", UpdateUser);
            group.MapDelete("/{id}", DeleteUser);
        }
    }

    private static async Task<IResult> GetUser(ISender sender, [FromRoute] int id)
    {
        var query = new GetUserQuery(id);
        return TypedResults.Ok(await sender.Send(query));
    }

    private static async Task<IResult> GetUsers(ISender sender, [FromQuery] int page)
    {
        var query = new GetUsersQuery(page);
        return TypedResults.Ok(await sender.Send(query));
    }

    private static async Task<IResult> AddUser(ISender sender, [FromForm] CreateUserCommand command)
    {
        await sender.Send(command);
        return TypedResults.Created();
    }

    private static async Task<IResult> RegisterUser(ISender sender, [FromBody] RegisterUserCommand command)
    {
        await sender.Send(command);
        return TypedResults.Created();
    }

    private static async Task<IResult> LogoutUser(ISender sender) {
        await sender.Send(new LogoutUserCommand());
        return TypedResults.Ok();
    }
    private static async Task<IResult> LoginUser(ISender sender, [FromBody] LoginUserCommand command)
    {
        await sender.Send(command);
        return TypedResults.Created();
    }

    private static async Task<IResult> UpdateUser(ISender sender, [FromRoute] string id, [FromForm] UpdateUserData data)
    {
        var command = new UpdateUserCommand(id, data);

        await sender.Send(command);
        return TypedResults.Ok();
    }
    private static async Task<IResult> DeleteUser(ISender sender, [FromRoute] string id)
    {
        var command = new DeleteUserCommand(id);
        await sender.Send(command);
        return TypedResults.Ok();
    }
}
