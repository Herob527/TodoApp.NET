using MediatR;
using TodoApp.Domain.Enum;

namespace TodoApp.ApiService.Endpoints.User.Commands.CreateUser;

public record CreateUserCommand(string Name, string Email, string Password, Role Role): IRequest;


