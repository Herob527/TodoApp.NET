using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApp.ApiService.Dto;

namespace TodoApp.ApiService.Endpoints.Todo.Queries.GetTodos;

public class GetTodosQueryHandler(TodoContext db): IRequestHandler<GetTodosQuery, List<TodoDto>>
{
    const int pageSize = 10;

    public async Task<List<TodoDto>> Handle(GetTodosQuery request, CancellationToken cancellationToken) => await db
        .Todos
        .Skip(request.Page * pageSize)
        .Take(pageSize)
        .Select(t => new TodoDto { Id = t.Id, Name = t.Name })
        .ToListAsync(cancellationToken: cancellationToken);
}
