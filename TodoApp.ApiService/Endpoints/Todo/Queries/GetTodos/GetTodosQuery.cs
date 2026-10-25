using MediatR;
using TodoApp.ApiService.Dto;

namespace TodoApp.ApiService.Endpoints.Todo.Queries.GetTodos;
public record GetTodosQuery: IRequest<List<TodoDto>>;
