using Aspire.Hosting;
using TodoApp.Tests.Data;

namespace TodoApp.Tests.Fixtures;

public sealed class AppHostFixture : IAsyncLifetime
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);

    private readonly string _containerName = $"todo-tests-{Guid.NewGuid():N}";

    public DistributedApplication App { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var builder =
            await DistributedApplicationTestingBuilder
                .CreateAsync<Projects.TodoApp_AppHost>(
                    ["UseVolumes=false", $"DBContainerName={_containerName}", $"AdminUser={AdminUser.Name}", $"AdminPassword={AdminUser.Password}"]);

        App = builder.Build();

        await App
            .StartAsync()
            .WaitAsync(DefaultTimeout);
    }

    public async Task DisposeAsync()
    {
        await App.DisposeAsync();
    }
}
