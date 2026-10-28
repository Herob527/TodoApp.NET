using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace TodoApp.ApiService.Endpoints.User.Commands.RegisterUser;

[AllowAnonymous]
public record RegisterUserCommand(string Name, string Password): IRequest;
