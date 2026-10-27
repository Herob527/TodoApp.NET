using System.Security.Claims;
using MediatR;

namespace TodoApp.ApiService.Endpoints.Todo.Commands.CreateTodo;

public class CreateTodoHandler(TodoContext db, IHttpContextAccessor accessor) : IRequestHandler<CreateTodoCommand>
{
    public async Task Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        var userId = accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return;
        // TODO: Replace owner id with param
        db.Todos.Add(new Domain.Entity.Todo { Name = request.Name, OwnerId = userId });
        await db.SaveChangesAsync(cancellationToken);
    }
}
