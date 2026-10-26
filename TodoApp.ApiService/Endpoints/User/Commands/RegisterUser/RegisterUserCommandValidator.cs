using FluentValidation;

namespace TodoApp.ApiService.Endpoints.User.Commands.RegisterUser;

public class RegisterUserCommandValidator: AbstractValidator<RegisterUserCommand>
{
  public RegisterUserCommandValidator()
  {
    RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(40);
    RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(40);
  }
}


