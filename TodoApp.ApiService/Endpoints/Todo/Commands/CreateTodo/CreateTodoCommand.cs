using MediatR;

namespace TodoApp.ApiService.Endpoints.Todo.Commands.CreateTodo;

public record struct CreateTodoCommand(string Name): IRequest;
