using MediatR;

namespace TodoApp.ApiService.Endpoints.Todo.Commands.CreateTodo;

public record CreateTodoCommand(string Name): IRequest;
