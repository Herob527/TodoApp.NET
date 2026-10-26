using MediatR;

namespace TodoApp.ApiService.Endpoints.User.Commands.CreateUser;

public class CreateUserCommandHandler(TodoContext db, IPasswordHelper passwordHelper) : IRequestHandler<CreateUserCommand>
{
    public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
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
