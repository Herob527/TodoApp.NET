using System.Security.Claims;

namespace TodoApp.ApiService.Repository;

public class UserRepository(IHttpContextAccessor accessor)
{

  public string? Id => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
  public string? Name => accessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
}
