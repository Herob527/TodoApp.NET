using MediatR;
using Microsoft.AspNetCore.Identity;
using TodoApp.ApiService.Repository;
using TodoApp.Domain.Entity;
using TodoApp.Domain.Enum;
using TodoApp.Domain.Exceptions;

namespace TodoApp.ApiService.Endpoints.User.Commands.UpdateUser;

public class UpdateUserCommandHandler(TodoContext db, UserManager<ApplicationUser> userManager, UserRepository userRepository) : IRequestHandler<UpdateUserCommand>

{
    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userRepository.Id ?? throw new UnauthorizedAccessException();
        var isAdmin = userRepository.Role == Role.Admin;
        var isOwner = currentUser == request.Id;

        if (!(isAdmin || isOwner)) throw new UnauthorizedAccessException();

        var user = await userManager.FindByIdAsync(request.Id) ?? throw new NotFoundException();
        user.UserName = request.Data.Name ?? user.UserName;
        user.Role = request.Data.Role ?? user.Role;
        await db.SaveChangesAsync(cancellationToken);
    }
}


