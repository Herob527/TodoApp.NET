using System.Net.Http.Json;
using System.Text.Json;
using Aspire.Hosting;
using TodoApp.Contracts.Dto;

namespace TodoApp.Tests.Tests;

public class TodoIntegrationTest : IAsyncLifetime
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    private DistributedApplication _app;

    public async Task InitializeAsync()
    {
        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.TodoApp_AppHost>(["UseVolumes=false"]);


        _app = await builder.BuildAsync();
    }

    public async Task DisposeAsync() => await _app.DisposeAsync();


    [Fact]
    public async Task RegisterUserAndCreateIsSuccessful()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;


        await _app.StartAsync(cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        // Act
        using var httpClient = _app.CreateHttpClient("apiservice");

        using var registerResponse = await httpClient.PostAsJsonAsync("/api/users/register", new { Name = "testingasd", Password = "zaq1@WSX" }, cancellationToken);
        using var createTodoResponse = await httpClient.PostAsJsonAsync("/api/todos", new { Name = "testing2" }, cancellationToken);
        using var getTodosResponse = await httpClient.GetAsync("/api/todos?page=0", cancellationToken);

        string json = await getTodosResponse.Content.ReadAsStringAsync();
        List<TodoDto>? converted = JsonSerializer.Deserialize<List<TodoDto>>(json);


        // Assert
        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Created, createTodoResponse.StatusCode);
        Assert.NotNull(converted);
        Assert.Single(converted);
    }
}
