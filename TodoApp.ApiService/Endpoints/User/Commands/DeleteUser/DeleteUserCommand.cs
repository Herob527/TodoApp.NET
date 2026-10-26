using MediatR;

namespace TodoApp.ApiService.Endpoints.User.Commands.DeleteUser;
public record DeleteUserCommand(string Id): IRequest;


