using MediatR;

namespace TodoApp.ApiService.Endpoints.Todo.Commands.Create;

record struct CreateTodoCommand(string Name): IRequest;
