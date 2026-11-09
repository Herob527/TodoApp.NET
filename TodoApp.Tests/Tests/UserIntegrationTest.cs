using TodoApp.Contracts.Dto;
using TodoApp.Contracts.Validation;
using TodoApp.Domain.Enum;
using TodoApp.Tests.Data;
using TodoApp.Tests.Endpoints;
using TodoApp.Tests.Extensions;
using TodoApp.Tests.Fixtures;
using TodoApp.Tests.Support;

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

    [Theory(DisplayName = "Register user with invalid data lengths and attempt login, login and register should fail with bad request and user should not be created")]
    [InlineData(UserConstraints.NameMinLength - 1, UserConstraints.PasswordMinLength)]
    [InlineData(UserConstraints.NameMinLength, UserConstraints.PasswordMinLength - 1)]
    [InlineData(UserConstraints.NameMaxLength + 1, UserConstraints.PasswordMinLength)]
    [InlineData(UserConstraints.NameMaxLength, UserConstraints.PasswordMaxLength + 1)]
    public async Task RegisterUserWithInvalidDataLength_ShouldFail(int usernameLength, int passwordLength)
    {
        var user = new TestUser(string.RandomText(usernameLength), string.RandomPassword(passwordLength));
        var firstResponse = await _userClient.RegisterUserAsync(user);
        var userLoginResponse = await _userClient.LoginUserAsync(user);

        var wasCreated = await WithLoginSession(async () =>
        {
            var getUsers = (await _userClient.GetAllUsersAsync()).FirstOrDefault(x => x.UserName == user.Name);
            return getUsers is not null;
        }, AdminUser.User);

        Assert.Equal(HttpStatusCode.BadRequest, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, userLoginResponse.StatusCode);
        Assert.False(wasCreated);
    }

    [Theory(DisplayName = "Update user with invalid new name lengths, update should fail with bad request and user should not be updated")]
    [InlineData(UserConstraints.NameMinLength - 1)]
    [InlineData(UserConstraints.NameMaxLength + 1)]
    public async Task UpdateUserWithInvalidDataLength_ShouldFail(int usernameLength)
    {
        var user = new TestUser(string.RandomText(10), string.RandomPassword(10));
        var newUserName = string.RandomText(usernameLength);
        await _userClient.RegisterUserAsync(user);

        var result = await WithLoginSession(async () =>
        {
            var initialGetUser = (await _userClient.GetAllUsersAsync()).FirstOrDefault(x => x.UserName == user.Name);
            var updateUserRequest = await _userClient.UpdateUserAsync(initialGetUser!.Id, newUserName);
            var afterUpdateUser = await _userClient.GetUserAsync(initialGetUser.Id);
            return new { wasUpdated = initialGetUser.UserName != afterUpdateUser!.UserName, wasBadRequest = updateUserRequest.StatusCode == HttpStatusCode.BadRequest };
        }, AdminUser.User);

        Assert.False(result.wasUpdated);
        Assert.True(result.wasBadRequest);
    }

    [Fact(DisplayName = "Update user with name containing invalid characters, update should fail with bad request and user should not be updated")]
    public async Task UpdateUserWithInvalidCharacters_ShouldFail()
    {
        var user = new TestUser(string.RandomText(10), string.RandomPassword(10));
        var newUserName = string.RandomPassword(10);
        await _userClient.RegisterUserAsync(user);

        var result = await WithLoginSession(async () =>
        {
            var initialGetUser = (await _userClient.GetAllUsersAsync()).FirstOrDefault(x => x.UserName == user.Name);
            var updateUserRequest = await _userClient.UpdateUserAsync(initialGetUser!.Id, newUserName);
            var afterUpdateUser = await _userClient.GetUserAsync(initialGetUser.Id);
            return new { wasUpdated = initialGetUser.UserName != afterUpdateUser!.UserName, wasBadRequest = updateUserRequest.StatusCode == HttpStatusCode.BadRequest };
        }, AdminUser.User);

        Assert.False(result.wasUpdated);
        Assert.True(result.wasBadRequest);
    }

    [Fact(DisplayName = "Register user with login name not matching pattern and attempt login, login and register should fail with bad request and user should not be created")]
    public async Task RegisterUserWithInvalidLogin_ShouldFail()
    {
        var user = new TestUser(string.RandomPassword(10), string.RandomPassword(10));
        var firstResponse = await _userClient.RegisterUserAsync(user);
        var userLoginResponse = await _userClient.LoginUserAsync(user);

        var wasCreated = await WithLoginSession(async () =>
        {
            var getUsers = (await _userClient.GetAllUsersAsync()).FirstOrDefault(x => x.UserName == user.Name);
            return getUsers is not null;
        }, AdminUser.User);


        Assert.Equal(HttpStatusCode.BadRequest, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, userLoginResponse.StatusCode);
        Assert.False(wasCreated);
    }

    [Theory(DisplayName = "Register user with valid data, it should succeed and user should be created")]
    [InlineData(UserConstraints.NameMinLength, UserConstraints.PasswordMinLength)]
    [InlineData(UserConstraints.NameMaxLength, UserConstraints.PasswordMaxLength)]
    public async Task RegisterUserWithValidData_ShouldSucceed(int usernameLength, int passwordLength)
    {
        string password = string.RandomPassword(passwordLength);

        var user = new TestUser(string.RandomText(usernameLength), password);
        var firstResponse = await _userClient.RegisterUserAsync(user);
        var userLoginResponse = await _userClient.LoginUserAsync(user);

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, userLoginResponse.StatusCode);
    }

    [Fact(DisplayName = "Register as User and logout, should succeed and user should not be logged in")]
    public async Task RegisterUserAndLogout()
    {

        var user = TestUser.Random();
        const string cookie = ".AspNetCore.Identity.Application=";
        const string authCookie = "Set-Cookie";

        var registerResponse = await _userClient.RegisterUserAsync(user);
        var loginResponse = await _userClient.LoginUserAsync(user);
        var logoutResponse = await _userClient.LogoutUserAsync();

        var loginAuthCookie = loginResponse.Headers.GetValues(authCookie)?.SingleOrDefault(x => x.StartsWith(cookie));
        var logoutAuthCookie = logoutResponse.Headers.GetValues(authCookie)?.SingleOrDefault(x => x.StartsWith(cookie));

        // Mind the semicolon. If coockie has =;, that means it's empty, which means logout
        Assert.False(loginAuthCookie.StartsWith($"{cookie};"));
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

        var (updateResponse, getMeResponse) = await WithLoginSession(async () =>
        {
            var updateResponse = await _userClient.UpdateSelfAsync(newName);
            var getMeResponse = await _userClient.GetUserMeAsync();
            return (updateResponse, getMeResponse);
        }, user);

        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.NotNull(getMeResponse);
        Assert.Equal(newName, getMeResponse.UserName);
    }

    [Fact(DisplayName = "Login invalid username of password, should throw not found")]
    public async Task LoginOrPasswordInvalid_ThrowsNotFound()
    {
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

        await using (await LoginSession.CreateAsync(_userClient, AdminUser.User))
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

        await using (await LoginSession.CreateAsync(_userClient, AdminUser.User))
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

        await using (await LoginSession.CreateAsync(_userClient, AdminUser.User))
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

        await using (await LoginSession.CreateAsync(_userClient, AdminUser.User))
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
        await using (await LoginSession.CreateAsync(_userClient, AdminUser.User))
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

        await using (await LoginSession.CreateAsync(_userClient, AdminUser.User))
        {
            var usersList = (await _userClient.GetAllUsersAsync()).First(x => x.UserName == userToRemove.Name);
            removedUser = await _userClient.GetUserAsync(usersList.Id);
        }

        await using (await LoginSession.CreateAsync(_userClient, nonAdminUser))
        {
            var deleteResponse = await _userClient.DeleteUserAsync(removedUser!.Id);
            var getUser = await _userClient.GetUserAsync(removedUser!.Id);

            Assert.Equal(HttpStatusCode.Forbidden, deleteResponse.StatusCode);
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

        await using (await LoginSession.CreateAsync(_userClient, AdminUser.User))
        {
            var usersList = (await _userClient.GetAllUsersAsync()).First(x => x.UserName == userToUpdate.Name);
            updatedUser = await _userClient.GetUserAsync(usersList.Id);
        }

        await using (await LoginSession.CreateAsync(_userClient, nonAdminUser))
        {
            var updateResponse = await _userClient.UpdateUserAsync(updatedUser!.Id, Name: string.RandomText(10));
            var getUser = await _userClient.GetUserAsync(updatedUser!.Id);

            Assert.Equal(HttpStatusCode.Forbidden, updateResponse.StatusCode);
            Assert.NotNull(getUser);
            Assert.Equal(updatedUser.UserName, getUser.UserName);
        }
    }

    [Fact(DisplayName = "Login as User and promote self to admin, should fail and role should stay User")]
    public async Task LoginAsUserAndPromoteSelf_ShouldFail()
    {
        var user = TestUser.Random();
        await _userClient.RegisterUserAsync(user);

        await using (await LoginSession.CreateAsync(_userClient, user))
        {
            var me = await _userClient.GetUserMeAsync();
            var updateResponse = await _userClient.UpdateUserAsync(me!.Id, Role: Role.Admin);
            var after = await _userClient.GetUserMeAsync();

            Assert.Equal(HttpStatusCode.Forbidden, updateResponse.StatusCode);
            Assert.DoesNotContain(nameof(Role.Admin), after!.Roles);
        }
    }

    [Fact(DisplayName = "Login as User and create another user, should fail and new user should not be created")]
    public async Task LoginAsUserAndCreateUser_ShouldFail()
    {
        var user = TestUser.Random();
        var userToCreate = TestUser.Random();
        await _userClient.RegisterUserAsync(user);

        var status = await WithLoginSession(async () => {
            var createStatus = await _userClient.CreateUserAsync(userToCreate, Role.User);
            return createStatus.StatusCode;
        }, user);

        var newUser = await WithLoginSession(async () => (await _userClient.GetAllUsersAsync()).FirstOrDefault(x => x.UserName == userToCreate.Name), AdminUser.User);

        Assert.Equal(HttpStatusCode.Forbidden, status);
        Assert.Null(newUser);
    }

    [Fact(DisplayName = "Login as admin and create another valid user, should succeed and new user should be created")]
    public async Task LoginAsAdminAndCreateUser_ShouldSucceed()
    {
        var user = TestUser.Random();
        var userToCreate = TestUser.Random();
        await _userClient.RegisterUserAsync(user);

        var status = await WithLoginSession(async () => {
            var createStatus = await _userClient.CreateUserAsync(userToCreate, Role.User);
            return createStatus.StatusCode;
        }, AdminUser.User);

        var wasCreated = await WithLoginSession(async () => (await _userClient.GetAllUsersAsync()).FirstOrDefault(x => x.UserName == userToCreate.Name), AdminUser.User);

        Assert.Equal(HttpStatusCode.Created, status);
        Assert.NotNull(wasCreated);
    }


    private async Task<T> WithLoginSession<T>(Func<Task<T>> callback, TestUser user)
    {

        await using (await LoginSession.CreateAsync(_userClient, user))
        {
            return await callback();
        }
    }
}
