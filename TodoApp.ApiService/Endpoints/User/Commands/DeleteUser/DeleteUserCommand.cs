using MediatR;
using Microsoft.AspNetCore.Authorization;
using TodoApp.Domain.Enum;

namespace TodoApp.ApiService.Endpoints.User.Commands.DeleteUser;

[Authorize(Roles = nameof(Role.Admin))]
public record DeleteUserCommand(string Id): IRequest;


