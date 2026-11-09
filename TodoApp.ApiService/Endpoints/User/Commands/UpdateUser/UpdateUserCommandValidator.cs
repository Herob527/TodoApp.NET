using FluentValidation;
using TodoApp.Contracts.Validation;

namespace TodoApp.ApiService.Endpoints.User.Commands.UpdateUser;
public class UpdateUserCommandValidator: AbstractValidator<UpdateUserCommand>
{
  public UpdateUserCommandValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
    When(
      x => x.Data.Name is not null,
      () => RuleFor(x => x.Data.Name)
        .NotEmpty()
        .MinimumLength(UserConstraints.NameMinLength)
        .MaximumLength(UserConstraints.NameMaxLength)
        .Matches(UserConstraints.NamePattern)
    );
    When(
      x => x.Data.Role is not null,
      () => RuleFor(x => x.Data.Role).IsInEnum()
    );
  }
}
