using MediatR;

namespace TodoApp.ApiService.Endpoints.Todo.Commands.UpdateTodo;

public class UpdateTodoHandler(TodoContext db): IRequestHandler<UpdateTodoCommand>
{
  public async Task Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
  {
      var item = await db.Todos.FindAsync([request.Id], cancellationToken: cancellationToken);
      if (item is null) return;
      item.Name = request.NewName;
      await db.SaveChangesAsync(cancellationToken);
  }
}
