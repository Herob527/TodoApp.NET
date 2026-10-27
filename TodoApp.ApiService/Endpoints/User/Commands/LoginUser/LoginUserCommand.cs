using MediatR;

namespace TodoApp.ApiService.Endpoints.User.Commands.LoginUser;

public record LoginUserCommand(string Name, string Password): IRequest;


