using MediatR;

namespace TodoApp.ApiService.Endpoints.Todo.Commands.DeleteTodo;
record struct DeleteTodoCommand(int Id): IRequest;
