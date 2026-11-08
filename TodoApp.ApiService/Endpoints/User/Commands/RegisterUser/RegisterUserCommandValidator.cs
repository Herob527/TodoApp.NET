using FluentValidation;
using TodoApp.Contracts.Validation;

namespace TodoApp.ApiService.Endpoints.User.Commands.RegisterUser;

// TODO: Add validation for user name to not contain special characters, only numbers and digits
public class RegisterUserCommandValidator: AbstractValidator<RegisterUserCommand>
{
  public RegisterUserCommandValidator()
  {
    RuleFor(x => x.Name).NotEmpty().MinimumLength(UserConstraints.NameMinLength).MaximumLength(UserConstraints.NameMaxLength);
    RuleFor(x => x.Password).NotEmpty().MinimumLength(UserConstraints.PasswordMinLength).MaximumLength(UserConstraints.PasswordMaxLength);
  }
}


