namespace TodoApp.ApiService.Dto;

public record TodoDto
{
  public int Id { get; init; }
  public string Name { get; init; }
  public bool IsComplete { get; init; }
}
