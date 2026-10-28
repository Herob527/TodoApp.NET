using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoApp.ApiService.Dto;
using TodoApp.Domain.Entity;

namespace TodoApp.ApiService.Endpoints.User.Queries.GetUsers;

public class GetUsersQueryHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<GetUsersQuery, List<UserDto>>
{

    const int pageSize = 10;
    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await userManager.Users
          .OrderBy(x => x.Id)
          .Skip(request.Page * pageSize)
          .Take(pageSize)
          .ProjectToType<UserDto>()
          .AsNoTracking()
          .ToListAsync(cancellationToken);
    }
}


