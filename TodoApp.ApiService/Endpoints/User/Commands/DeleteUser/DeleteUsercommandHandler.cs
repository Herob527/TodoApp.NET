using MediatR;
using Microsoft.AspNetCore.Identity;
using TodoApp.ApiService.Services;
using TodoApp.Domain.Entity;
using TodoApp.Domain.Enum;
using TodoApp.Domain.Exceptions;

namespace TodoApp.ApiService.Endpoints.User.Commands.DeleteUser;

public class DeleteUsercommandHandler(UserManager<ApplicationUser> userManager, CurrentUser userRepository) : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
      if (userRepository.Id == request.Id) throw new ConflictException();

      var isCurrentAdmin = await userRepository.IsInRoleAsync(Role.Admin, cancellationToken);

      if (!isCurrentAdmin) throw new UnauthorizedAccessException();


      var user = await userManager.FindByIdAsync(request.Id) ?? throw new NotFoundException();
      await userManager.DeleteAsync(user);
    }
}


