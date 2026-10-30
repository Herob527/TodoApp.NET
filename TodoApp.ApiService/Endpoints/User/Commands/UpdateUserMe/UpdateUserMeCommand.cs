using MediatR;
using Microsoft.AspNetCore.Authorization;
using TodoApp.Domain.Enum;

namespace TodoApp.ApiService.Endpoints.User.Commands.UpdateUserMe;


[Authorize]
public record UpdateUserMeCommand(string? Name): IRequest;
