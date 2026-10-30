using FluentValidation;
using TodoApp.Contracts.Validation;

namespace TodoApp.ApiService.Endpoints.User.Commands.UpdateUserMe;
public class UpdateUserMeCommandValidator: AbstractValidator<UpdateUserMeCommand>
{
  public UpdateUserMeCommandValidator()
  {
    When(x => x.Data.Name is not null, () => RuleFor(x => x.Data.Name).NotEmpty().MinimumLength(UserConstraints.NameMinLength).MaximumLength(UserConstraints.NameMaxLength)) ;
    When(x => x.Data.Role is not null, () => RuleFor(x => x.Data.Role).IsInEnum());
  }
}


