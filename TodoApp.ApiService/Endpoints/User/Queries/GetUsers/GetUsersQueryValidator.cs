using FluentValidation;

namespace TodoApp.ApiService.Endpoints.User.Queries.GetUsers;
public class GetUsersQueryValidator: AbstractValidator<GetUsersQuery>
{
  public GetUsersQueryValidator()
  {
    RuleFor(x => x.Page).GreaterThanOrEqualTo(0).LessThan(int.MaxValue);
  }
}


