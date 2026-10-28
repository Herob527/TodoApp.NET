using FluentValidation;

namespace TodoApp.ApiService.Endpoints.Todo.Commands.UpdateTodo;

public class UpdateTodoValidator: AbstractValidator<UpdateTodoCommand>
{
  public UpdateTodoValidator()
    {
        RuleFor(x => x.Data.Name).NotEmpty().MaximumLength(50);
    }
}
