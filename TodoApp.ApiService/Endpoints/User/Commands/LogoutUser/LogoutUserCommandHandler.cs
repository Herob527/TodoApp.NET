using MediatR;
using Microsoft.AspNetCore.Identity;
using TodoApp.Domain.Entity;

namespace TodoApp.ApiService.Endpoints.User.Commands.LogoutUser;

public class LogoutUserCommandHandler(SignInManager<ApplicationUser> signInManager) : IRequestHandler<LogoutUserCommand>
{
    public async Task Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
      await signInManager.SignOutAsync();
    }
}
