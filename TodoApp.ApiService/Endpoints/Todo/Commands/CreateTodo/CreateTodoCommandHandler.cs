using MediatR;

namespace TodoApp.ApiService.Endpoints.Todo.Commands.CreateTodo;

public class CreateTodoHandler(TodoContext db) : IRequestHandler<CreateTodoCommand>
{
    public async Task Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        // TODO: Replace owner id with param
        db.Todos.Add(new Domain.Entity.Todo { Name = request.Name, OwnerId = 1 });
        await db.SaveChangesAsync(cancellationToken);
    }
}
