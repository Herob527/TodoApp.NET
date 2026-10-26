using MediatR;
using Microsoft.AspNetCore.Authorization;
using TodoApp.Domain.Enum;

namespace TodoApp.ApiService.Endpoints.User.Commands.CreateUser;

[Authorize(Roles = Role.Admin)]
public record CreateUserCommand(string Name, string Email, string Password, Role Role): IRequest;


