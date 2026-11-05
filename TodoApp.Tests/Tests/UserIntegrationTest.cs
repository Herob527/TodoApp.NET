using TodoApp.Contracts.Dto;
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
        var user = TestUser.Random();
        var firstResponse = await _userClient.RegisterUserAsync(user);
        var secondResponse = await _userClient.RegisterUserAsync(user);

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
    }

    [Fact(DisplayName = "Register as User and logout, should succeed and user should not be logged in")]
    public async Task RegisterUserAndLogout()
    {

        var user = TestUser.Random();
        const string cookie =  ".AspNetCore.Identity.Application=";
        const string authCookie = "Set-Cookie";

        var registerResponse = await _userClient.RegisterUserAsync(user);
        var loginResponse = await _userClient.LoginUserAsync(user);
        var logoutResponse = await _userClient.LogoutUserAsync();

        var loginAuthCookie = loginResponse.Headers.GetValues(authCookie)?.SingleOrDefault(x => x.StartsWith(cookie));
        var logoutAuthCookie = logoutResponse.Headers.GetValues(authCookie)?.SingleOrDefault(x => x.StartsWith(cookie));

        // Mind the semicolon. If coockie has =;, that means it's empty, which means logout
        Assert.False(loginAuthCookie.StartsWith( $"{cookie};"));
        Assert.StartsWith($"{cookie};", logoutAuthCookie);
        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, logoutResponse.StatusCode);
    }

    [Fact(DisplayName = "Register user as User and update own name, should succeed and user should have name changed")]
    public async Task RegisterUserAndUpdate_ShouldSucceed()
    {

        var user = TestUser.Random();
        var newName = string.RandomText(10);
        var registerResponse = await _userClient.RegisterUserAsync(user);


        await using (await LoginFixture.CreateAsync(_userClient, user)) {
          var updateResponse = await _userClient.UpdateSelfAsync(newName);
          var getMeResponse = await _userClient.GetUserMeAsync();

          Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);
          Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
          Assert.NotNull(getMeResponse);
          Assert.Equal(newName, getMeResponse.UserName);
        }
    }

    [Fact(DisplayName = "Login invalid username of password, should throw not found")]
    public async Task LoginOrPasswordInvalid_ThrowsNotFound() {
        var realUser = new TestUser(string.RandomText(10), string.RandomText(10));
        var invalidUserName = string.RandomText(10);
        var invalidPassword = string.RandomText(10);

        await _userClient.RegisterUserAsync(realUser);

        var invalidUserResponse = await _userClient.LoginUserAsync(new TestUser(invalidUserName, realUser.Password));
        var invalidPassResponse = await _userClient.LoginUserAsync(new TestUser(realUser.Name, invalidPassword));

        Assert.Equal(HttpStatusCode.NotFound, invalidUserResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, invalidPassResponse.StatusCode);
    }

    [Fact(DisplayName = "Login as Admin and update someones name, should succeed")]
    public async Task LoginAsAdminAndUpdateUser_ShouldSucceed()
    {

        var user = TestUser.Random();
        var newName = string.RandomText(10);
        var registerResponse = await _userClient.RegisterUserAsync(user);

        await using (await LoginFixture.CreateAsync(_userClient, AdminUser.User))
        {
          var usersList = (await _userClient.GetAllUsersAsync()).First(x => x.UserName == user.Name);
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

        await using (await LoginFixture.CreateAsync(_userClient, AdminUser.User))
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
        var user = TestUser.Random();
        await _userClient.RegisterUserAsync(user);

        await using (await LoginFixture.CreateAsync(_userClient, AdminUser.User))
        {
          var usersList = (await _userClient.GetAllUsersAsync()).First(x => x.UserName == user.Name);
          var updateResponse = await _userClient.UpdateUserAsync(usersList.Id, Role: Role.Admin);
          var getUpdatedUserResponse = await _userClient.GetUserAsync(usersList.Id);

          Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
          Assert.NotNull(getUpdatedUserResponse);
          Assert.Contains(nameof(Role.Admin), getUpdatedUserResponse.Roles);
        }
    }

    [Fact(DisplayName = "Login as Admin and delete other user, should succeed and user should be deleted")]
    public async Task LoginAsAdminAndRemoveOtherUser_ShouldSucceed()
    {
        var user = TestUser.Random();
        await _userClient.RegisterUserAsync(user);

        await using (await LoginFixture.CreateAsync(_userClient, AdminUser.User))
        {
          var usersList = (await _userClient.GetAllUsersAsync()).First(x => x.UserName == user.Name);
          var deleteResponse = await _userClient.DeleteUserAsync(usersList.Id);
          var getUserException = await Assert.ThrowsAsync<HttpRequestException>(async () => await _userClient.GetUserAsync(usersList.Id));

          Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
          Assert.Equal(HttpStatusCode.NotFound, getUserException.StatusCode);
        }
    }

    [Fact(DisplayName = "Login as Admin and delete yourself, should fail and user should not be deleted")]
    public async Task LoginAsAdminAndRemoveYourself_ShouldFail()
    {
        var userName = string.RandomText(10);

        await using (await LoginFixture.CreateAsync(_userClient, AdminUser.User))
        {
          var usersList = (await _userClient.GetAllUsersAsync()).First(x => x.UserName == AdminUser.Name);
          var deleteResponse = await _userClient.DeleteUserAsync(usersList.Id);
          var getUser = await _userClient.GetUserAsync(usersList.Id);

          Assert.Equal(HttpStatusCode.Conflict, deleteResponse.StatusCode);
          Assert.NotNull(getUser);
        }
    }

    [Fact(DisplayName = "Login as User and delete other user, should throw insufficient permissions and user should not be deleted")]
    public async Task LoginAsUserAndRemoveOtherUser_ShouldFail()
    {
        var userToRemove = TestUser.Random();
        var nonAdminUser = TestUser.Random();
        await _userClient.RegisterUserAsync(userToRemove);
        await _userClient.RegisterUserAsync(nonAdminUser);

        UserDto? removedUser;

        await using (await LoginFixture.CreateAsync(_userClient, AdminUser.User))
        {
          var usersList = (await _userClient.GetAllUsersAsync()).First(x => x.UserName == userToRemove.Name);
          removedUser = await _userClient.GetUserAsync(usersList.Id);
        }

        await using (await LoginFixture.CreateAsync(_userClient, nonAdminUser))
        {
          var deleteResponse = await _userClient.DeleteUserAsync(removedUser!.Id);
          var getUser = await _userClient.GetUserAsync(removedUser!.Id);

          Assert.Equal(HttpStatusCode.Unauthorized, deleteResponse.StatusCode);
          Assert.NotNull(getUser);
        }
    }

    [Fact(DisplayName = "Login as User and update other user, should throw insufficient permissions and user should not be updated")]
    public async Task LoginAsUserAndUpdateOtherUser_ShouldFail()
    {
        var userToUpdate = TestUser.Random();
        var nonAdminUser = TestUser.Random();
        await _userClient.RegisterUserAsync(userToUpdate);
        await _userClient.RegisterUserAsync(nonAdminUser);

        UserDto? updatedUser;

        await using (await LoginFixture.CreateAsync(_userClient, AdminUser.User))
        {
          var usersList = (await _userClient.GetAllUsersAsync()).First(x => x.UserName == userToUpdate.Name);
          updatedUser = await _userClient.GetUserAsync(usersList.Id);
        }

        await using (await LoginFixture.CreateAsync(_userClient, nonAdminUser))
        {
          var updateResponse = await _userClient.UpdateUserAsync(updatedUser!.Id, Name: string.RandomText(10));
          var getUser = await _userClient.GetUserAsync(updatedUser!.Id);

          Assert.Equal(HttpStatusCode.Unauthorized, updateResponse.StatusCode);
          Assert.NotNull(getUser);
          Assert.Equal(updatedUser.UserName, getUser.UserName);
        }
    }
}
