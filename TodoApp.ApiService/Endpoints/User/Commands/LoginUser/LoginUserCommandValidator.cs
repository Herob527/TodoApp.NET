using FluentValidation;
using TodoApp.Contracts.Validation;

namespace TodoApp.ApiService.Endpoints.User.Commands.LoginUser;

public class LoginUserCommandValidator: AbstractValidator<LoginUserCommand>
{
  public LoginUserCommandValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty()
      .MinimumLength(UserConstraints.NameMinLength)
      .MaximumLength(UserConstraints.NameMaxLength)
      .Matches(UserConstraints.NamePattern);
    RuleFor(x => x.Password)
      .NotEmpty()
      .MinimumLength(UserConstraints.PasswordMinLength)
      .MaximumLength(UserConstraints.PasswordMaxLength);
  }
}


