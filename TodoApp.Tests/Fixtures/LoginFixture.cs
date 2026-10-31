using TodoApp.Tests.Endpoints;

namespace TodoApp.Tests.Fixtures;

public sealed class LoginFixture(UserClient UserClient, string Name, string Password, bool Register = false) : IAsyncLifetime
{

    public async Task InitializeAsync()
    {
        if (Register) {
          await UserClient.RegisterUserAsync(Name, Password);

        }
        await UserClient.LoginUserAsync(Name, Password);
    }

    public async Task DisposeAsync()
    {
        Console.WriteLine($"Test: Logging out user {Name}");
        await UserClient.LogoutUserAsync();
    }


}
