using MediatR;

namespace TodoApp.ApiService.Endpoints.Todo.Commands.CreateTodo;

public class CreateTodoHandler(TodoContext db) : IRequestHandler<CreateTodoCommand>
{
    public async Task Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        db.Todos.Add(new Domain.Todo { Name = request.Name });
        await db.SaveChangesAsync(cancellationToken);
    }
}
