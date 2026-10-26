using MediatR;
using Microsoft.AspNetCore.Identity;
using TodoApp.Domain.Entity;
using TodoApp.Domain.Exceptions;

namespace TodoApp.ApiService.Endpoints.User.Commands.DeleteUser;

public class DeleteUsercommandHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
      var user = await userManager.FindByIdAsync(request.Id) ?? throw new NotFoundException();
      await userManager.DeleteAsync(user);
    }
}


