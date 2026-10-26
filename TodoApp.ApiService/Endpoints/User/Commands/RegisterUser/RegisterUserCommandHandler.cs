using MediatR;

namespace TodoApp.ApiService.Endpoints.User.Commands.RegisterUser;
public class RegisterUserCommandHandler(TodoContext db, IPasswordHelper passwordHelper) : IRequestHandler<RegisterUserCommand>
{
    public async Task Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
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
