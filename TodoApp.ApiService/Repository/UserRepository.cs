using System.Security.Claims;
using TodoApp.Domain.Enum;

namespace TodoApp.ApiService.Repository;

public class UserRepository(IHttpContextAccessor accessor)
{

  public string? Id => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
  public string? Name => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);

  public Role Role => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role) switch
  {
    "User" => Role.User,
    "Admin" => Role.Admin,
    _ => Role.User
  };
}
