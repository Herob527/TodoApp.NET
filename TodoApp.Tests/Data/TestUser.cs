using TodoApp.Tests.Extensions;

namespace TodoApp.Tests.Data;

public sealed record TestUser(string Name, string Password = TestUser.DefaultPassword)
{
    public const string DefaultPassword = "zaq1@WSX";

    public static TestUser Random() => new(string.RandomText(10));
}
