using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entity;
using TodoApp.Domain.Enum;
using TodoApp.Domain.Exceptions;

namespace TodoApp.ApiService.Endpoints.User.Commands.RegisterUser;

public class RegisterUserCommandHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) : IRequestHandler<RegisterUserCommand>
{
    public async Task Handle(RegisterUserCommand request, CancellationToken cancellationToken)
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

        Console.WriteLine(result);
        if (!result.Succeeded)
        {
          throw new NotFoundException();
        }


        var createdUser = await userManager.FindByNameAsync(request.Name) ?? throw new NotFoundException();

        await userManager.AddToRoleAsync(createdUser, nameof(Role.User));

        await signInManager.SignInAsync(createdUser, true);
    }
}
