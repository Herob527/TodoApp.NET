using MediatR;

namespace TodoApp.ApiService.Endpoints.Todo.Commands.UpdateTodo;
public record UpdateTodoCommand(int Id, string NewName): IRequest;
