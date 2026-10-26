using MediatR;

namespace TodoApp.ApiService.Endpoints.User.Commands.AddUser;

public class AddUserCommandHandler(TodoContext db, IPasswordHelper passwordHelper) : IRequestHandler<AddUserCommand>
{
    public async Task Handle(AddUserCommand request, CancellationToken cancellationToken)
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
