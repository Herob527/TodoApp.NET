using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entity;
using TodoApp.Domain.Exceptions;

namespace TodoApp.ApiService.Endpoints.User.Commands.CreateUser;

public class CreateUserCommandHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<CreateUserCommand>
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
        }, request.Password);

        if (!result.Succeeded) throw new HttpRequestException();

        var createdUser = await userManager.FindByNameAsync(request.Name) ?? throw new NotFoundException();

        await userManager.AddToRoleAsync(createdUser, request.Role.ToString());

    }
}
