using MediatR;
using Microsoft.AspNetCore.Authorization;
using TodoApp.Domain.Enum;

namespace TodoApp.ApiService.Endpoints.User.Commands.UpdateUser;


[Authorize]
public record UpdateUserCommand(string Id, UpdateUserData Data): IRequest;

public record UpdateUserData(string? Name, Role? Role): IRequest;
