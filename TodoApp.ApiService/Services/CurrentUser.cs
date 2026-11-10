using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Enum;

namespace TodoApp.ApiService.Services;

public record CurrentUser(IHttpContextAccessor Accessor, TodoContext Db)
{
  public string? Id => Context?.FindFirstValue(ClaimTypes.NameIdentifier);
  public Task<bool> IsInRoleAsync(Role role, CancellationToken cancellationToken = default) =>
      IsInAnyRoleAsync([role.ToString()], cancellationToken);

  public Task<bool> IsInAnyRoleAsync(IEnumerable<string> roles, CancellationToken cancellationToken = default)
  {
      var userId = Id;
      if (userId is null) return Task.FromResult(false);

        return Db.UserRoles
          .Where(userRole => userRole.UserId == userId)
          .Join(Db.Roles, userRole => userRole.RoleId, role => role.Id, (_, role) => role.Name)
          .AnyAsync(roleName => roles.Contains(roleName), cancellationToken);
  }

  private ClaimsPrincipal? Context => Accessor.HttpContext?.User;
}
