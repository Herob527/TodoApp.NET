using TodoApp.Tests.Endpoints;
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
    }

    public async Task DisposeAsync()
    {
        await _fixture.DisposeAsync();
    }

    [Fact]
    public async Task RegisterUserAndCreateIsSuccessful()
    {
        var registerResponse = await _userClient.RegisterUserAsync("testingasd", "zaq1@WSX");
        var createTodoResponse = await _todoClient.CreateAsync("testing2");
        var getTodosResponse = await _todoClient.GetPaginatedAsync();

        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Created, createTodoResponse.StatusCode);
        Assert.NotNull(getTodosResponse);
        Assert.Single(getTodosResponse);
    }
}
