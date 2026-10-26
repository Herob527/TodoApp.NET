using MediatR;
using Microsoft.AspNetCore.Authorization;
using TodoApp.Domain.Attributes;
using TodoApp.Domain.Enum;

namespace TodoApp.ApiService.Endpoints.User.Commands.CreateUser;

[UserRole(Role.Admin, Role.User)]
public record CreateUserCommand(string Name, string Email, string Password, Role Role): IRequest;


