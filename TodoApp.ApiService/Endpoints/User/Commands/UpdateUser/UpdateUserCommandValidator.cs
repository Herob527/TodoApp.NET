using FluentValidation;

namespace TodoApp.ApiService.Endpoints.User.Commands.UpdateUser;
public class UpdateUserCommandValidator: AbstractValidator<UpdateUserCommand>
{
  public UpdateUserCommandValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
    When(x => x.Data.Name is not null, () => RuleFor(x => x.Data.Name).NotEmpty().MinimumLength(3).MaximumLength(40)) ;
    When(x => x.Data.Role is not null, () => RuleFor(x => x.Data.Role).IsInEnum());
  }
}


