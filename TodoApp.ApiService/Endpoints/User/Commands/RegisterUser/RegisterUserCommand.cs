using MediatR;

namespace TodoApp.ApiService.Endpoints.User.Commands.RegisterUser;
public record RegisterUserCommand(string FirstName, string LastName, string Email, string Password): IRequest;



