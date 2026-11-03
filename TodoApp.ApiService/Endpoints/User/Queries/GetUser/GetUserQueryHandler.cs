using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApp.Contracts.Dto;
using TodoApp.Domain.Exceptions;

namespace TodoApp.ApiService.Endpoints.User.Queries.GetUser;

public class GetUserQueryHandler(TodoContext dbContext) : IRequestHandler<GetUserQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        return await dbContext
            .Users
            .Select(user => new UserDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Roles = (from userRole in dbContext.UserRoles join role in dbContext.Roles on userRole.RoleId equals role.Id where userRole.UserId == user.Id select role.Name!).ToList()
            })
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken) ?? throw new NotFoundException();
    }
}


