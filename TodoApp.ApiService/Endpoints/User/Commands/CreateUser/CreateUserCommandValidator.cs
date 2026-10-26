using FluentValidation;

namespace TodoApp.ApiService.Endpoints.User.Commands.CreateUser;

public class CreateUserCommandValidator: AbstractValidator<CreateUserCommand>
{
  public CreateUserCommandValidator()
  {
    RuleFor(x => x.Name).NotEmpty().MinimumLength(3).MaximumLength(40);
    RuleFor(x => x.Password).NotEmpty().MaximumLength(8).MaximumLength(40);
    RuleFor(x => x.Role).NotEmpty().IsInEnum();
  }
}


