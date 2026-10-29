using MediatR;
using TodoApp.ApiService.Services;

namespace TodoApp.ApiService.Endpoints.Todo.Commands.CreateTodo;

public class CreateTodoHandler(TodoContext db, CurrentUser user) : IRequestHandler<CreateTodoCommand>
{
    public async Task Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        var userId = user.Id ?? throw new UnauthorizedAccessException();

        db.Todos.Add(new Domain.Entity.Todo { Name = request.Name, OwnerId = userId });
        await db.SaveChangesAsync(cancellationToken);
    }
}
