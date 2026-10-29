using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Dto;
using TodoApp.Domain.Entity;

namespace TodoApp.ApiService.Endpoints.User.Queries.GetUsers;

public class GetUsersQueryHandler(
    TodoContext dbContext)
    : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    private const int PageSize = 10;

    public async Task<List<UserDto>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken) => await dbContext.Users
        .OrderBy(x => x.Id)
        .Skip(request.Page * PageSize)
        .Take(PageSize)
        .Select(user => new UserDto
        {
            Id = user.Id,
            UserName = user.UserName!,
            Roles = (
                from userRole in dbContext.UserRoles
                join role in dbContext.Roles
                    on userRole.RoleId equals role.Id
                where userRole.UserId == user.Id
                select role.Name!
            ).ToList()
        })
        .AsNoTracking()
        .ToListAsync(cancellationToken);
}
