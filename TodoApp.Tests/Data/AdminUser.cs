namespace TodoApp.Tests.Data;
public static class AdminUser
{
    public const string Name = "Admin";
    public const string Password = "zaq1@WSX";

    public static TestUser User { get; } = new(Name, Password);
}
