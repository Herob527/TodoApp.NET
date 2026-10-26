using MediatR;
using Microsoft.AspNetCore.Identity;
using TodoApp.Domain.Entity;
using TodoApp.Domain.Exceptions;

namespace TodoApp.ApiService.Endpoints.User.Commands.UpdateUser;

public class UpdateUserCommandHandler(TodoContext db, UserManager<ApplicationUser> userManager) : IRequestHandler<UpdateUserCommand>

{
    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id) ?? throw new NotFoundException();
        user.UserName = request.Data.Name ?? user.UserName;
        user.Role = request.Data.Role ?? user.Role;
        await db.SaveChangesAsync(cancellationToken);
    }
}


