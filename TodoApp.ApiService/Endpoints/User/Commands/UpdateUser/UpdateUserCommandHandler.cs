using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using TodoApp.ApiService.Repository;
using TodoApp.Domain.Entity;
using TodoApp.Domain.Enum;
using TodoApp.Domain.Exceptions;

namespace TodoApp.ApiService.Endpoints.User.Commands.UpdateUser;

public class UpdateUserCommandHandler(UserManager<ApplicationUser> userManager, UserRepository userRepository) : IRequestHandler<UpdateUserCommand>

{
    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userRepository.Id ?? throw new UnauthorizedAccessException();
        var isAdmin = userRepository.Role == Role.Admin;
        var isModifyingSelf = currentUser == request.Id;

        if (!(isAdmin || isModifyingSelf)) throw new UnauthorizedAccessException();

        var user = await userManager.FindByIdAsync(request.Id) ?? throw new NotFoundException();

        if (request.Data.Name is not null) {
          // For some reason, it doesn't request name, but normalized name which is uppercase
          var isNameTaken = await userManager.FindByNameAsync(request.Data.Name);
          if (isNameTaken is not null) throw new ConflictException();
          user.UserName = request.Data.Name;
        }

        if (request.Data.Role is not null && isAdmin && !isModifyingSelf) {
          var currentRole = await userManager.GetRolesAsync(user);
          await userManager.RemoveFromRoleAsync(user, string.Join(", ", currentRole));
          await userManager.AddToRoleAsync(user, string.Join(", ", request.Data.Role));
        }
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new BadHttpRequestException("Invalid data");
        }
    }
}


