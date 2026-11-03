using TodoApp.Domain.Enum;
using TodoApp.Tests.Data;
using TodoApp.Tests.Endpoints;
using TodoApp.Tests.Extensions;
using TodoApp.Tests.Fixtures;

namespace TodoApp.Tests.Tests;

public class UserIntegrationTest : IClassFixture<AppHostFixture>, IDisposable
{
    private readonly HttpClient _httpClient;

    private readonly UserClient _userClient;

    public UserIntegrationTest(AppHostFixture fixture)
    {
        _httpClient = fixture.App.CreateHttpClient("apiservice");
        _userClient = new UserClient(_httpClient);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    [Fact(DisplayName = "Register same user twice, first should be created and second should be conflict")]
    public async Task RegisterSameUserTwiceIsConflict()
    {
        var name = string.RandomText(10);
        const string password = "zaq1@WSX";
        var firstResponse = await _userClient.RegisterUserAsync(name, password);
        var secondResponse = await _userClient.RegisterUserAsync(name, password);

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
    }

    [Fact(DisplayName = "Register user and logout, should succeed and user should not be logged in")]
    public async Task RegisterUserAndLogout()
    {

        var name = string.RandomText(10);
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

        var name = string.RandomText(10);
        var newName = string.RandomText(10);
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

    [Fact(DisplayName = "Login as Admin and update someones name, should succeed")]
    public async Task LoginAsAdminAndUpdateUser_ShouldSucceed()
    {

        var name = string.RandomText(10);
        var newName = string.RandomText(10);
        const string password = "zaq1@WSX";
        var registerResponse = await _userClient.RegisterUserAsync(name, password);

        await using (await LoginFixture.CreateAsync(_userClient, AdminUser.Name, AdminUser.Password))
        {
          var usersList = (await _userClient.GetAllUsersAsync()).First(x => x.UserName == name);
          var updateResponse = await _userClient.UpdateUserAsync(usersList.Id, newName);
          var getUpdatedUserResponse = await _userClient.GetUserAsync(usersList.Id);

          Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
          Assert.NotNull(getUpdatedUserResponse);
          Assert.Equal(newName, getUpdatedUserResponse.UserName);
        }
    }

    [Fact(DisplayName = "Login as Admin and update own role, should fail with bad request")]
    public async Task LoginAsAdminAndUpdateOwn_ShouldFail()
    {

        await using (await LoginFixture.CreateAsync(_userClient, AdminUser.Name, AdminUser.Password))
        {
          var usersList = (await _userClient.GetAllUsersAsync()).First(x => x.UserName == AdminUser.Name);
          var updateResponse = await _userClient.UpdateUserAsync(usersList.Id, Role: Role.User);
          var getUpdatedUserResponse = await _userClient.GetUserAsync(usersList.Id);

          Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
          Assert.NotNull(getUpdatedUserResponse);
          Assert.Contains(nameof(Role.Admin), getUpdatedUserResponse.Roles);
        }
    }

    [Fact(DisplayName = "Login as Admin and promote someones role to admin, should succeed")]
    public async Task LoginAsAdminAndPromoteOtherUser_ShouldSucceed()
    {
        var userName = string.RandomText(10);
        await _userClient.RegisterUserAsync(userName, "zaq1@WSX");

        await using (await LoginFixture.CreateAsync(_userClient, AdminUser.Name, AdminUser.Password))
        {
          var usersList = (await _userClient.GetAllUsersAsync()).First(x => x.UserName == userName);
          var updateResponse = await _userClient.UpdateUserAsync(usersList.Id, Role: Role.Admin);
          var getUpdatedUserResponse = await _userClient.GetUserAsync(usersList.Id);

          Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
          Assert.NotNull(getUpdatedUserResponse);
          Assert.Contains(nameof(Role.Admin), getUpdatedUserResponse.Roles);
        }
    }

}
