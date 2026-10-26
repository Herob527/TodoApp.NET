using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace TodoApp.ApiService.Endpoints.Todo.Commands.DeleteTodo;

[Authorize]
public record DeleteTodoCommand(int Id): IRequest;
