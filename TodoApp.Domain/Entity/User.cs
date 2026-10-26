namespace TodoApp.Domain.Entity;

using TodoApp.Domain.Enum;

public record User
{
    public int Id { get; init; }
    public string Name { get; set; }
    public string Password { get; init; }
    public Role Role { get; set; }
}
