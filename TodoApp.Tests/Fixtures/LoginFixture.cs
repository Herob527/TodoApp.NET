using TodoApp.Tests.Data;
using TodoApp.Tests.Endpoints;

namespace TodoApp.Tests.Fixtures;

public sealed record LoginFixture(UserClient UserClient, string Name) : IAsyncDisposable
{
    public static async Task<LoginFixture> CreateAsync(
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

        return new LoginFixture(userClient, user.Name);
    }

    public async ValueTask DisposeAsync()
    {
        await UserClient.LogoutUserAsync();
    }
}
