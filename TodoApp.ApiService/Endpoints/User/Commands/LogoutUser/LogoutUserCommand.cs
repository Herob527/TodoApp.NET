using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace TodoApp.ApiService.Endpoints.User.Commands.LogoutUser;

[Authorize]
public record LogoutUserCommand() : IRequest;


