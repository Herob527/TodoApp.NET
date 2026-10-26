using FluentValidation;

namespace TodoApp.ApiService.Endpoints.User.Commands.LoginUser;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(40);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(40);
    }
}


