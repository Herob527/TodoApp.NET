using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace TodoApp.ApiService.Endpoints.User.Commands.LoginUser;

[AllowAnonymous]
public record LoginUserCommand(string Name, string Password): IRequest;


