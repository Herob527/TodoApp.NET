using FluentValidation;
using TodoApp.Contracts.Validation;

namespace TodoApp.ApiService.Endpoints.User.Commands.UpdateUserMe;
public class UpdateUserMeCommandValidator: AbstractValidator<UpdateUserMeCommand>
{
  public UpdateUserMeCommandValidator()
  {
    When(x => x.Name is not null, () => RuleFor(x => x.Name).NotEmpty().MinimumLength(UserConstraints.NameMinLength).MaximumLength(UserConstraints.NameMaxLength)) ;
  }
}


