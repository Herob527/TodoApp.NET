using MediatR;

namespace TodoApp.ApiService.Endpoints.Todo.Commands.DeleteTodo;
public record DeleteTodoCommand(int Id): IRequest;
