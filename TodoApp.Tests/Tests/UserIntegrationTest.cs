using TodoApp.Tests.Endpoints;
using TodoApp.Tests.Fixtures;

namespace TodoApp.Tests.Tests;

public class UserIntegrationTest : IAsyncLifetime
{
    private AppHostFixture _fixture = null!;

    private HttpClient _httpClient = null!;

    private UserClient _userClient = null!;

    private static readonly TimeSpan DefaultTimeout =
        TimeSpan.FromSeconds(30);

    public async Task InitializeAsync()
    {
        _fixture = await AppHostFixture.CreateAsync("user");

        await _fixture
            .App
            .StartAsync()
            .WaitAsync(DefaultTimeout);

        _httpClient = _fixture.App.CreateHttpClient("apiservice");
        _userClient = new UserClient(_httpClient);
    }

    public async Task DisposeAsync()
    {
        await _fixture.DisposeAsync();
    }

    [Fact(DisplayName = "Register same user twice, first should be created, second should be conflict")]
    public async Task RegisterSameUserTwiceIsConflict()
    {
        const string name = "alamakota";
        var firstResponse = await _userClient.RegisterUserAsync(name, "zaq1@WSX");
        var secondResponse = await _userClient.RegisterUserAsync(name, "zaq1@WSX");

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
    }

    [Fact(DisplayName = "Register user, then logout, should succeed and user should not be logged in")]
    public async Task RegisterUserAndLogout()
    {

        const string name = "testing";
        const string cookie =  ".AspNetCore.Identity.Application=";
        var registerResponse = await _userClient.RegisterUserAsync(name, "zaq1@WSX");
        var logoutResponse = await _userClient.LogoutUserAsync();

        var registerAuthCookie = registerResponse.Headers.GetValues("Set-Cookie")?.SingleOrDefault(x => x.StartsWith(cookie));
        var logoutAuthCookie = logoutResponse.Headers.GetValues("Set-Cookie")?.SingleOrDefault(x => x.StartsWith(cookie));

        // Mind the semicolon. If coockie has =;, that means it's empty, which means logout
        Assert.False(registerAuthCookie.StartsWith( $"{cookie};"));
        Assert.StartsWith($"{cookie};", logoutAuthCookie);
        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, logoutResponse.StatusCode);
    }

    [Fact(DisplayName = "Register user, then update name, should succeed and user should have name changed")]
    public async Task RegisterUserAndUpdate()
    {

        const string name = "testing3";
        const string newName = "testing4";
        var registerResponse = await _userClient.RegisterUserAsync(name, "zaq1@WSX");
        var updateResponse = await _userClient.UpdateSelfAsync(newName);
        var getMeResponse = await _userClient.GetUserMeAsync();

        // Mind the semicolon. If coockie has =;, that means it's empty, which means logout
        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.NotNull(getMeResponse);
        Assert.Equal(newName, getMeResponse.UserName);
    }
}
