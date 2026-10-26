using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApp.ApiService.Dto;
using TodoApp.Domain.Exceptions;

namespace TodoApp.ApiService.Endpoints.User.Queries.GetUser;

public class GetUserQueryHandler(TodoContext db) : IRequestHandler<GetUserQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        return await db.Users.ProjectToType<UserDto>().SingleOrDefaultAsync(cancellationToken: cancellationToken) ?? throw new NotFoundException();
    }
}


