using MediatR;

namespace TodoApp.ApiService.Endpoints.User.Commands.UpdateUser;

public record UpdateUserData(int Id, string FirstName, string LastName, string Email, string Password): IRequest;
public record UpdateUserCommand(int Id, UpdateUserData Data): IRequest;



