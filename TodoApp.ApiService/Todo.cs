namespace TodoApp.ApiService;

public record Todo
{
  public int Id { get; init; }
  public string? Name { get; init; }
  public bool IsComplete { get; init; }
  public DateTime CreationDate {get;init;}
  public DateTime LastUpdateDate {get;init;}
}
