namespace TodoApp.Domain.Entity;

public record Todo
{
  public int Id { get; init; }
  public string? Name { get; set; }
  public bool IsComplete { get; init; }
  public DateTime CreationDate {get;init;} = DateTime.UtcNow;
  public DateTime LastUpdateDate {get;init;} = DateTime.UtcNow;
  public required string OwnerId { get; set; }
  public ApplicationUser Owner { get; set; }
}
