using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace TodoApp.ApiService.Endpoints.Todo.Commands.CreateTodo;

[Authorize]
public record CreateTodoCommand(string Name): IRequest;
