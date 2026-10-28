using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entity;
using TodoApp.Domain.Exceptions;

namespace TodoApp.ApiService.Endpoints.User.Commands.CreateUser;

public class CreateUserCommandHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) : IRequestHandler<CreateUserCommand>
{
    public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {

        var existingUser = await userManager.Users.SingleOrDefaultAsync(x => x.UserName == request.Name, cancellationToken);
        if (existingUser is not null)
        {
            throw new ConflictException();
        }

        var result = await userManager.CreateAsync(new ApplicationUser
        {
            UserName = request.Name,
            Role = request.Role
        }, request.Password);

        if (!result.Succeeded) throw new HttpRequestException();

        var createdUser = await userManager.FindByNameAsync(request.Name) ?? throw new NotFoundException();

        await userManager.AddClaimAsync(createdUser, new Claim("role", request.Role.ToString()));

    }
}
