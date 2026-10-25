namespace TodoApp.Domain;

public record Todo
{
  public int Id { get; init; }
  public string? Name { get; set; }
  public bool IsComplete { get; init; }
  public DateTime CreationDate {get;init;} = DateTime.UtcNow;
  public DateTime LastUpdateDate {get;init;} = DateTime.UtcNow;
}
