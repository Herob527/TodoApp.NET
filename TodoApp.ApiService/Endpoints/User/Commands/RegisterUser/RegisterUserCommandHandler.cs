using MediatR;
using TodoApp.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace TodoApp.ApiService.Endpoints.User.Commands.RegisterUser;

public class RegisterUserCommandHandler(TodoContext db, IPasswordHelper passwordHelper) : IRequestHandler<RegisterUserCommand>
{
    public async Task Handle(RegisterUserCommand request, CancellationToken cancellationToken)
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
        };
        db.Add(user);

        await db.SaveChangesAsync(cancellationToken);
    }
}
