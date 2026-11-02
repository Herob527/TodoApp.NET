using TodoApp.Tests.Endpoints;
using TodoApp.Tests.Extensions;
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

    [Fact(DisplayName = "Register same user twice, first should be created and second should be conflict")]
    public async Task RegisterSameUserTwiceIsConflict()
    {
        const string name = "alamakota";
        const string password = "zaq1@WSX";
        var firstResponse = await _userClient.RegisterUserAsync(name, password);
        var secondResponse = await _userClient.RegisterUserAsync(name, password);

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
    }

    [Fact(DisplayName = "Register user and logout, should succeed and user should not be logged in")]
    public async Task RegisterUserAndLogout()
    {

        const string name = "testing";
        const string cookie =  ".AspNetCore.Identity.Application=";
        const string password = "zaq1@WSX";
        var registerResponse = await _userClient.RegisterUserAsync(name, password);
        var loginResponse = await _userClient.LoginUserAsync(name, password);
        var logoutResponse = await _userClient.LogoutUserAsync();

        var loginAuthCookie = loginResponse.Headers.GetValues("Set-Cookie")?.SingleOrDefault(x => x.StartsWith(cookie));
        var logoutAuthCookie = logoutResponse.Headers.GetValues("Set-Cookie")?.SingleOrDefault(x => x.StartsWith(cookie));

        // Mind the semicolon. If coockie has =;, that means it's empty, which means logout
        Assert.False(loginAuthCookie.StartsWith( $"{cookie};"));
        Assert.StartsWith($"{cookie};", logoutAuthCookie);
        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, logoutResponse.StatusCode);
    }

    [Fact(DisplayName = "Register user as User and update name, should succeed and user should have name changed")]
    public async Task RegisterUserAndUpdate()
    {

        const string name = "testing3";
        const string newName = "testing4";
        const string password = "zaq1@WSX";
        var registerResponse = await _userClient.RegisterUserAsync(name, password);

        // Using login fixture causes 401 on users/me
        _ = await _userClient.LoginUserAsync(name, password);
        var updateResponse = await _userClient.UpdateSelfAsync(newName);
        var getMeResponse = await _userClient.GetUserMeAsync();
        _ = await _userClient.LogoutUserAsync();

        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.NotNull(getMeResponse);
        Assert.Equal(newName, getMeResponse.UserName);
    }

    [Fact(DisplayName = "Login invalid username of password, should throw not found")]
    public async Task LoginOrPasswordInvalid_ThrowsNotFound() {
        string realUserName = string.RandomText(10);
        string password = string.RandomText(10);
        string invalidUserName = string.RandomText(10);
        string invalidPassword = string.RandomText(10);

        await _userClient.RegisterUserAsync(realUserName, password);

        var invalidUserResponse = await _userClient.LoginUserAsync(invalidUserName, password);
        var invalidPassResponse = await _userClient.LoginUserAsync(realUserName, invalidPassword);

        Assert.Equal(HttpStatusCode.NotFound, invalidUserResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, invalidPassResponse.StatusCode);

    }

}
