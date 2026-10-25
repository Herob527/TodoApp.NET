using FluentValidation;

namespace TodoApp.ApiService.Endpoints.Todo.Queries.GetTodos;

public class GetTodosQueryValidator: AbstractValidator<GetTodosQuery>
{
  public GetTodosQueryValidator()
  {
      RuleFor(x => x.Page).GreaterThanOrEqualTo(0);
  }
}
