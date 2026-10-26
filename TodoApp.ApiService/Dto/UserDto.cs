using TodoApp.Domain.Enum;

namespace TodoApp.ApiService.Dto;

public record UserDto
{
  public int Id { get; init; }
  public string Name { get; init; }
  public Role Role { get; init; }
}
