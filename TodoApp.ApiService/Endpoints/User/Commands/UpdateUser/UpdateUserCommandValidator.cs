using FluentValidation;

namespace TodoApp.ApiService.Endpoints.User.Commands.UpdateUser;
public class UpdateUserCommandValidator: AbstractValidator<UpdateUserCommand>
{
  public UpdateUserCommandValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
    RuleFor(x => x.Data.Name).NotEmpty().MinimumLength(3).MaximumLength(40);
    RuleFor(x => x.Data.Role).NotEmpty().IsInEnum();
  }
}


