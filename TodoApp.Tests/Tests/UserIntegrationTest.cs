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

    [Fact]
    public async Task RegisterSameUserTwiceIsConflict()
    {
        var firstResponse = await _userClient.RegisterUserAsync("testingasd", "zaq1@WSX");
        var secondResponse = await _userClient.RegisterUserAsync("testingasd", "zaq1@WSX");

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
    }
}
