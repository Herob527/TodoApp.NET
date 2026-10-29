using Microsoft.AspNetCore.Identity;

namespace TodoApp.ApiService.Dto;

public record UserDto
{
  public string Id { get; init; }
  public string UserName { get; init; }
  public List<string> Roles { get; init; }
}
