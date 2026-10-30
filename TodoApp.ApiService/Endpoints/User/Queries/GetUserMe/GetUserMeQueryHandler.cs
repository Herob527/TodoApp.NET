using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoApp.ApiService.Services;
using TodoApp.Contracts.Dto;
using TodoApp.Domain.Entity;
using TodoApp.Domain.Exceptions;

namespace TodoApp.ApiService.Endpoints.User.Queries.GetUserMe;

public class GetUserMeQueryHandler(UserManager<ApplicationUser> userManager, CurrentUser userRepository) : IRequestHandler<GetUserMeQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserMeQuery request, CancellationToken cancellationToken)
    {
        return await userManager
            .Users
            .ProjectToType<UserDto>()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == userRepository.Id, cancellationToken: cancellationToken) ?? throw new NotFoundException();
    }
}


