using MediatR;

namespace TodoApp.ApiService.Endpoints.Todo.Commands.DeleteTodo;

public class DeleteTodoHandler(TodoContext db): IRequestHandler<DeleteTodoCommand>
{
  public async Task Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var item = await db.Todos.FindAsync([request.Id], cancellationToken: cancellationToken);
        if (item is null) return;
        db.Remove(item);
        await db.SaveChangesAsync(cancellationToken);
    }
}
