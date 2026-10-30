using MediatR;
using Microsoft.AspNetCore.Identity;
using TodoApp.ApiService.Services;
using TodoApp.Domain.Entity;
using TodoApp.Domain.Exceptions;

namespace TodoApp.ApiService.Endpoints.User.Commands.UpdateUserMe;

public class UpdateUserMeCommandHandler(UserManager<ApplicationUser> userManager, CurrentUser userRepository) : IRequestHandler<UpdateUserMeCommand>

{
    public async Task Handle(UpdateUserMeCommand request, CancellationToken cancellationToken)
    {
        var userId = userRepository.Id ?? throw new UnauthorizedAccessException();

        var user = await userManager.FindByIdAsync(userId) ?? throw new NotFoundException();

        if (request.Name is not null) {
          // For some reason, it doesn't request name, but normalized name which is uppercase
          var isNameTaken = await userManager.FindByNameAsync(request.Name);
          if (isNameTaken is not null) throw new ConflictException();
          user.UserName = request.Name;
        }

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new BadHttpRequestException("Invalid data");
        }
    }
}


