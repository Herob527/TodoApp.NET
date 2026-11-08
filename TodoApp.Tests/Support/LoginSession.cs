using TodoApp.Tests.Data;
using TodoApp.Tests.Endpoints;

namespace TodoApp.Tests.Support;

public sealed record LoginSession(UserClient UserClient) : IAsyncDisposable
{
    public static async Task<LoginSession> CreateAsync(
        UserClient userClient,
        TestUser user,
        bool register = false)
    {
        if (register)
        {
            var registerResponse = await userClient.RegisterUserAsync(user);
            registerResponse.EnsureSuccessStatusCode();
        }

        var loginResponse = await userClient.LoginUserAsync(user);
        loginResponse.EnsureSuccessStatusCode();

        return new LoginSession(userClient);
    }

    public async ValueTask DisposeAsync()
    {
        await UserClient.LogoutUserAsync();
    }
}
