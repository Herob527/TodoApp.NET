using MediatR;
using TodoApp.Domain.Enum;

namespace TodoApp.ApiService.Endpoints.User.Commands.UpdateUser;

public record UpdateUserData(string? Name, Role? Role): IRequest;
public record UpdateUserCommand(string Id, UpdateUserData Data): IRequest;



