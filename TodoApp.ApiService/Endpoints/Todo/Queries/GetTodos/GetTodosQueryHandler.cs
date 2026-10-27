using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApp.ApiService.Dto;
using TodoApp.ApiService.Repository;

namespace TodoApp.ApiService.Endpoints.Todo.Queries.GetTodos;

public class GetTodosQueryHandler(TodoContext db, UserRepository userRepository): IRequestHandler<GetTodosQuery, List<TodoDto>>
{
    const int pageSize = 10;

    public async Task<List<TodoDto>> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {

        var userId = userRepository.Id ?? throw new UnauthorizedAccessException();

        return await db
          .Todos
          .Skip(request.Page * pageSize)
          .Take(pageSize)
          .OrderBy(t => t.LastUpdateDate)
          .Where(t => t.OwnerId == userId)
          .ProjectToType<TodoDto>()
          .ToListAsync(cancellationToken: cancellationToken);
    }
}
