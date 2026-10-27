using MediatR;

namespace TodoApp.ApiService.Endpoints.User.Commands.RegisterUser;

public record RegisterUserCommand(string Name, string Password): IRequest;
