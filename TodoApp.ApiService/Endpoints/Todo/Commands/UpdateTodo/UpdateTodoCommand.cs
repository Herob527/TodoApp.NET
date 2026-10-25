using MediatR;

namespace TodoApp.ApiService.Endpoints.Todo.Commands.UpdateTodo;
record struct UpdateTodoCommand(int Id, string NewName): IRequest;
