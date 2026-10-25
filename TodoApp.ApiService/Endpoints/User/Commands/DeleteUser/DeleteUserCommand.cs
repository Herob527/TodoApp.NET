using MediatR;

namespace TodoApp.ApiService.Endpoints.User.Commands.DeleteUser;
public record DeleteUserCommand(int Id): IRequest;


