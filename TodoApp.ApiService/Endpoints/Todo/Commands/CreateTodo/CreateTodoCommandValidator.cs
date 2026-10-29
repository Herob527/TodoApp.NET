using FluentValidation;
using TodoApp.Contracts.Validation;

namespace TodoApp.ApiService.Endpoints.Todo.Commands.CreateTodo;

public class CreateTodoValidator: AbstractValidator<CreateTodoCommand>
{
  public CreateTodoValidator()
  {
    RuleFor(x => x.Name).NotEmpty().MaximumLength(TodoConstraints.NameMaxLength);
  }
}
