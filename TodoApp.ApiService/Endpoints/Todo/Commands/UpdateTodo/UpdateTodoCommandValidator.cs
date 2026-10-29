using FluentValidation;
using TodoApp.Contracts.Validation;

namespace TodoApp.ApiService.Endpoints.Todo.Commands.UpdateTodo;

public class UpdateTodoValidator: AbstractValidator<UpdateTodoCommand>
{
  public UpdateTodoValidator()
    {
        RuleFor(x => x.Data.Name).NotEmpty().MaximumLength(TodoConstraints.NameMaxLength);
    }
}
