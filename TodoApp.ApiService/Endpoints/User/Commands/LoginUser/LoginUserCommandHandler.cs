using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Exceptions;


namespace TodoApp.ApiService.Endpoints.User.Commands.LoginUser;

public class LoginUserCommandHandler(TodoContext db, IPasswordHelper passwordHelper) : IRequestHandler<LoginUserCommand>
{
    public async Task Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await db.Users.SingleOrDefaultAsync(x => x.Name == request.Name, cancellationToken) ?? throw new NotFoundException();

        var password = passwordHelper.VerifyPassword(request.Name, user.Password, request.Password);

        if (!password)
        {
            throw new UnauthorizedException();
        }

    }
}


