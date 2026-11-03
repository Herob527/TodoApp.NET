namespace TodoApp.Contracts.Dto;

public record TodoDto
{
  public string Id { get; init; }
  public string Name { get; init; }
  public bool IsComplete { get; init; }
  public DateTime CreationDate { get; init; }
  public DateTime LastUpdateDate { get; init; }
}
