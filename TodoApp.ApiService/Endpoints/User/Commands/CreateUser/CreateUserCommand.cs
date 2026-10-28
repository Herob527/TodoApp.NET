using MediatR;
using Microsoft.AspNetCore.Authorization;
using TodoApp.Domain.Enum;

namespace TodoApp.ApiService.Endpoints.User.Commands.CreateUser;

[Authorize(Policy = "Admin")]
public record CreateUserCommand(string Name, string Password, Role Role): IRequest;


