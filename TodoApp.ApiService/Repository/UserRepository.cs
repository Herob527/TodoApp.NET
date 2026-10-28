using System.Security.Claims;
using TodoApp.Domain.Enum;

namespace TodoApp.ApiService.Repository;

public class UserRepository(IHttpContextAccessor accessor)
{
  public string? Id => Context?.FindFirstValue(ClaimTypes.NameIdentifier);
  public string? Name => Context?.FindFirstValue(ClaimTypes.Name);

  public Role Role => Context?.FindFirstValue(ClaimTypes.Role) switch
  {
      "User" => Role.User,
      "Admin" => Role.Admin,
      _ => Role.User
  };

  private ClaimsPrincipal? Context => accessor.HttpContext?.User;
}
