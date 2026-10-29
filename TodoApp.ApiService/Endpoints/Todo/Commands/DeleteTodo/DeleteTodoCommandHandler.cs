using MediatR;
using Microsoft.EntityFrameworkCore;
using TodoApp.ApiService.Repository;
using TodoApp.Domain.Enum;
using TodoApp.Domain.Exceptions;

namespace TodoApp.ApiService.Endpoints.Todo.Commands.DeleteTodo;

public class DeleteTodoHandler(TodoContext db, CurrentUser userRepository) : IRequestHandler<DeleteTodoCommand>
{
    public async Task Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var userId = userRepository.Id ?? throw new UnauthorizedAccessException();
        var item = await db
            .Todos
            .Include(t => t.Owner)
            .SingleOrDefaultAsync(t => t.Id == request.Id, cancellationToken: cancellationToken) ?? throw new NotFoundException();

        var isOwner = item.OwnerId == userId;

        var isAdmin = userRepository.Role == Role.Admin;
        if (!(isOwner || isAdmin)) throw new UnauthorizedAccessException();
        db.Remove(item);
        await db.SaveChangesAsync(cancellationToken);
    }
}
