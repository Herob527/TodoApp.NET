using MediatR;

namespace TodoApp.ApiService.Endpoints.Todo.Commands.DeleteTodo;
public record struct DeleteTodoCommand(int Id): IRequest;
