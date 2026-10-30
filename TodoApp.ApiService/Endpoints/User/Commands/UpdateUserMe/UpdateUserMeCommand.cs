using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace TodoApp.ApiService.Endpoints.User.Commands.UpdateUserMe;


[Authorize]
public record UpdateUserMeCommand(string? Name): IRequest;
