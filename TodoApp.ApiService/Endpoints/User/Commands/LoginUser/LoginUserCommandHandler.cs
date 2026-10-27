using MediatR;
using Microsoft.AspNetCore.Identity;
using TodoApp.Domain.Entity;
using TodoApp.Domain.Exceptions;

namespace TodoApp.ApiService.Endpoints.User.Commands.LoginUser;

public class LoginUserCommandHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    : IRequestHandler<LoginUserCommand>
{
    public async Task Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(request.Name) ?? throw new NotFoundException();

        var result = await signInManager.PasswordSignInAsync(user, request.Password, true, false);
        if (!result.Succeeded)
        {
            throw new NotFoundException();
        }
    }
}
