using System.Security.Claims;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApp.ApiService.Dto;

namespace TodoApp.ApiService.Endpoints.Todo.Queries.GetTodos;

public class GetTodosQueryHandler(TodoContext db, IHttpContextAccessor accessor): IRequestHandler<GetTodosQuery, List<TodoDto>>
{
    const int pageSize = 10;

    public async Task<List<TodoDto>> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {

        var userId = (accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)) ?? throw new UnauthorizedAccessException();

        return await db
        .Todos
        .Skip(request.Page * pageSize)
        .Take(pageSize)
        .OrderBy(t => t.LastUpdateDate)
        .ProjectToType<TodoDto>()
        .ToListAsync(cancellationToken: cancellationToken);
    }
}
