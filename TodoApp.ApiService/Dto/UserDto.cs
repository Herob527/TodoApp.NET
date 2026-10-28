using TodoApp.Domain.Enum;

namespace TodoApp.ApiService.Dto;

public record UserDto
{
  public string Id { get; init; }
  public string UserName { get; init; }
  public Role Role { get; init; }
}
