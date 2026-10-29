using System.Security.Claims;
using TodoApp.Domain.Enum;

namespace TodoApp.ApiService.Services;

public record CurrentUser(IHttpContextAccessor Accessor)
{
  public string? Id => Context?.FindFirstValue(ClaimTypes.NameIdentifier);
  public string? Name => Context?.FindFirstValue(ClaimTypes.Name);

  public Role Role => Context?.FindFirstValue(ClaimTypes.Role) switch
  {
      "User" => Role.User,
      "Admin" => Role.Admin,
      _ => Role.User
  };

  private ClaimsPrincipal? Context => Accessor.HttpContext?.User;
}
