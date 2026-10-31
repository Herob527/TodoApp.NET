using TodoApp.Tests.Endpoints;

namespace TodoApp.Tests.Fixtures;

public sealed record LoginFixture(UserClient UserClient, string Name) : IAsyncDisposable
{
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
        await UserClient.LogoutUserAsync();
    }
}
