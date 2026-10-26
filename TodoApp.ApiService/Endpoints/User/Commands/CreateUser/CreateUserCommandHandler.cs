using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Exceptions;

namespace TodoApp.ApiService.Endpoints.User.Commands.CreateUser;

public class CreateUserCommandHandler(TodoContext db, IPasswordHelper passwordHelper) : IRequestHandler<CreateUserCommand>
{
    public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {

        var existingUser = await db.Users.SingleOrDefaultAsync(x => x.Name == request.Name, cancellationToken);
        if (existingUser is not null)
        {
            throw new ConflictException();
        }

        var hashedPassword = passwordHelper.HashPassword(request.Name, request.Password);

        var user = new Domain.Entity.User
        {
            Name = request.Name,
            Password = hashedPassword,
            Role = request.Role
        };
        db.Add(user);

        await db.SaveChangesAsync(cancellationToken);
    }
}
