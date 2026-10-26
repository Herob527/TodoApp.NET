using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace TodoApp.ApiService.Endpoints.Todo.Commands.UpdateTodo;

[Authorize]
public record UpdateTodoCommand(int Id, string NewName): IRequest;
