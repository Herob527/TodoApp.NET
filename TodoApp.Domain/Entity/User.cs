namespace TodoApp.Domain.Entity;

using TodoApp.Domain.Enum;

public record User
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Password { get; init; }
    public Role Role { get; init; }
}
