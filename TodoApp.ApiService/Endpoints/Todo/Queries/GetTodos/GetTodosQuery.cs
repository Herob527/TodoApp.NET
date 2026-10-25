using MediatR;
using TodoApp.ApiService.Dto;

namespace TodoApp.ApiService.Endpoints.Todo.Queries.GetTodos;
public record GetTodosQuery(int Page): IRequest<List<TodoDto>>;
