using MediatR;
using Microsoft.AspNetCore.Authorization;
using TodoApp.Contracts.Dto;

namespace TodoApp.ApiService.Endpoints.Todo.Queries.GetTodos;

[Authorize]
public record GetTodosQuery(int Page): IRequest<List<TodoDto>>;
