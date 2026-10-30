
using Aspire.Hosting;

namespace TodoApp.Tests.Fixtures;

public sealed class AppHostFixture : IAsyncDisposable
{
    public DistributedApplication App { get; }

    private AppHostFixture(DistributedApplication app)
    {
        App = app;
    }

    public static async Task<AppHostFixture> CreateAsync(string containerName)
    {
        var builder =
            await DistributedApplicationTestingBuilder
                .CreateAsync<Projects.TodoApp_AppHost>(
                    ["UseVolumes=false", $"DBContainerName={containerName}"]);

        var app = builder.Build();

        return new AppHostFixture(app);
    }

    public async ValueTask DisposeAsync()
    {
        await App.DisposeAsync();
    }
}
