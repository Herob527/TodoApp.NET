using MediatR;

namespace TodoApp.ApiService.Endpoints.Todo.Commands.CreateTodo;

record struct CreateTodoCommand(string Name): IRequest;
