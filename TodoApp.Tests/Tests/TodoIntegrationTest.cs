using TodoApp.Contracts.Validation;
using TodoApp.Tests.Endpoints;
using TodoApp.Tests.Extensions;
using TodoApp.Tests.Fixtures;

namespace TodoApp.Tests.Tests;

public class TodoIntegrationTest : IAsyncLifetime
{
    private AppHostFixture _fixture = null!;

    private HttpClient _httpClient = null!;

    private TodoClient _todoClient = null!;

    private UserClient _userClient = null!;

    private static readonly TimeSpan DefaultTimeout =
        TimeSpan.FromSeconds(60);

    private (string, string) user = ("testingasd", "zaq1@WSX");


    public async Task InitializeAsync()
    {
        _fixture = await AppHostFixture.CreateAsync("todo");

        await _fixture
            .App
            .StartAsync()
            .WaitAsync(DefaultTimeout);

        _httpClient = _fixture.App.CreateHttpClient("apiservice");
        _todoClient = new TodoClient(_httpClient);
        _userClient = new UserClient(_httpClient);
        await _userClient.RegisterUserAsync(user.Item1, user.Item2);
    }

    public async Task DisposeAsync()
    {
        await _fixture.DisposeAsync();
    }

    [Fact]
    public async Task CreateTodoSuccessful()
    {
        await _userClient.LoginUserAsync(user.Item1, user.Item2);
        var createTodoResponse = await _todoClient.CreateAsync("testing2");
        var getTodosResponse = await _todoClient.GetPaginatedAsync();
        await _userClient.LogoutUserAsync();

        Assert.Equal(HttpStatusCode.Created, createTodoResponse.StatusCode);
        Assert.NotNull(getTodosResponse);
        Assert.Single(getTodosResponse);
    }

    [Fact]
    public async Task CreateTodoValidationFail()
    {

        await _userClient.LoginUserAsync(user.Item1, user.Item2);

        string exactLength = string.RandomText(TodoConstraints.NameMaxLength);
        string tooLong = string.RandomText(TodoConstraints.NameMaxLength + 1);
        var tooShortTodo = await _todoClient.CreateAsync("");
        var exactLengthTodo = await _todoClient.CreateAsync(exactLength);
        var tooLongTodo = await _todoClient.CreateAsync(tooLong);
        var getTodosResponse = await _todoClient.GetPaginatedAsync();
        await _userClient.LogoutUserAsync();

        Assert.Equal(HttpStatusCode.Created, exactLengthTodo.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, tooShortTodo.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, tooLongTodo.StatusCode);
    }
}
