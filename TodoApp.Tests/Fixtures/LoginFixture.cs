using TodoApp.Tests.Endpoints;

namespace TodoApp.Tests.Fixtures;

public sealed class LoginFixture : IAsyncDisposable
{
    private readonly UserClient _userClient;
    private readonly string _name;

    private LoginFixture(UserClient userClient, string name)
    {
        _userClient = userClient;
        _name = name;
    }

    public static async Task<LoginFixture> CreateAsync(
        UserClient userClient,
        string name,
        string password,
        bool register = false)
    {
        if (register)
        {
            var registerResponse = await userClient.RegisterUserAsync(name, password);
            registerResponse.EnsureSuccessStatusCode();
        }

        var loginResponse = await userClient.LoginUserAsync(name, password);
        loginResponse.EnsureSuccessStatusCode();

        return new LoginFixture(userClient, name);
    }

    public async ValueTask DisposeAsync()
    {
        Console.WriteLine($"Test: Logging out user {_name}");
        await _userClient.LogoutUserAsync();
    }
}
