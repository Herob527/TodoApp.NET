namespace TodoApp.ApiService.Dto
{
    public record TodoDto
    {
      int Id { get; init; }
      string Name { get; init; }
      bool IsComplete { get; init; }
    }
}
