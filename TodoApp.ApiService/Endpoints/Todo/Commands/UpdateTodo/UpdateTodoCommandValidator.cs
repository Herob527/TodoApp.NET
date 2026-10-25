using FluentValidation;

namespace TodoApp.ApiService.Endpoints.Todo.Commands.UpdateTodo;

public class UpdateTodoValidator: AbstractValidator<UpdateTodoCommand>
{
  public UpdateTodoValidator()
    {
        RuleFor(x => x.NewName).NotEmpty().MaximumLength(50);
    }
}
