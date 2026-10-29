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

        ApplicationUser user = new()
        {
            UserName = request.Name,
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded) throw new HttpRequestException();


        await userManager.AddToRoleAsync(user, request.Role.ToString());

    }
}
