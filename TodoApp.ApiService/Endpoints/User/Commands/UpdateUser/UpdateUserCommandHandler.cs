using MediatR;
using TodoApp.Domain.Exceptions;

namespace TodoApp.ApiService.Endpoints.User.Commands.UpdateUser;

public class UpdateUserCommandHandler(TodoContext db) : IRequestHandler<UpdateUserCommand>

{
    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await db.Users.FindAsync([request.Id], cancellationToken) ?? throw new NotFoundException();
        user.Name = request.Data.Name ?? user.Name;
        user.Role = request.Data.Role ?? user.Role;
        await db.SaveChangesAsync(cancellationToken);
    }
}


