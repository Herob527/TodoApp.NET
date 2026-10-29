using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApp.ApiService.Services;
using TodoApp.Contracts.Dto;
using TodoApp.Domain.Enum;

namespace TodoApp.ApiService.Endpoints.Todo.Queries.GetTodos;

public class GetTodosQueryHandler(TodoContext db, CurrentUser userRepository): IRequestHandler<GetTodosQuery, List<TodoDto>>
{
    const int pageSize = 10;

    public async Task<List<TodoDto>> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {

        var userId = userRepository.Id ?? throw new UnauthorizedAccessException();
        var isAdmin = userRepository.Role == Role.Admin;

        var query = db
          .Todos
          .Skip(request.Page * pageSize)
          .Take(pageSize)
          .OrderBy(t => t.LastUpdateDate)
          .AsNoTracking();

        if (!isAdmin) query = query.Where(t => t.OwnerId == userId);

        return await query
          .ProjectToType<TodoDto>()
          .AsNoTracking()
          .ToListAsync(cancellationToken: cancellationToken);
    }
}
