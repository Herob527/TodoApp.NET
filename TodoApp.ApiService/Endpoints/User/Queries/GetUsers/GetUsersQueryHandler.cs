using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApp.ApiService.Dto;

namespace TodoApp.ApiService.Endpoints.User.Queries.GetUsers;

public class GetUsersQueryHandler(TodoContext db) : IRequestHandler<GetUsersQuery, List<UserDto>>
{

    const int pageSize = 10;
    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await db.Users
          .Skip(request.Page * pageSize)
          .Take(pageSize)
          .ProjectToType<UserDto>()
          .ToListAsync(cancellationToken);
    }
}


