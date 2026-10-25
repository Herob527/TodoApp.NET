using MediatR;

namespace TodoApp.ApiService.Endpoints.User.Commands.AddUser;
public record AddUserCommand(string FirstName, string LastName, string Email, string Password): IRequest;


