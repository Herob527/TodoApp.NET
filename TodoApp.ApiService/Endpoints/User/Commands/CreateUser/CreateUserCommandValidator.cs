using FluentValidation;
using TodoApp.Contracts.Validation;

namespace TodoApp.ApiService.Endpoints.User.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(UserConstraints.NameMinLength).MaximumLength(UserConstraints.NameMaxLength);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(UserConstraints.PasswordMinLength).MaximumLength(UserConstraints.PasswordMaxLength);
        RuleFor(x => x.Role).IsInEnum();
    }
}


