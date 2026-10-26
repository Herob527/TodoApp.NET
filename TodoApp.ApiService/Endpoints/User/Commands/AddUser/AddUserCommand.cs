using MediatR;
using TodoApp.Domain.Enum;

namespace TodoApp.ApiService.Endpoints.User.Commands.AddUser;

public record AddUserCommand(string Name, string Email, string Password, Role Role): IRequest;


