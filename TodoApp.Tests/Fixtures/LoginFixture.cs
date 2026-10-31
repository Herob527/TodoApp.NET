using TodoApp.Tests.Endpoints;

namespace TodoApp.Tests.Fixtures;

public sealed class LoginFixture : IAsyncDisposable
{
    private readonly UserClient _userClient;

    private LoginFixture(UserClient userClient)
    {
        _userClient = userClient;
    }

    public static async Task<LoginFixture> StartSessionAsync(UserClient userClient, string name, string password)
    {
        var fixture = new LoginFixture(userClient);
        await userClient.LoginUserAsync(name, password);
        return fixture;
    }

    public async ValueTask DisposeAsync()
    {
        await _userClient.LogoutUserAsync();
    }
}
