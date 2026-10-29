using Aspire.Hosting;

namespace TodoApp.Tests.Fixtures;

public class AppHostFixture : IAsyncDisposable
{
    public DistributedApplication App { get; }
    public AppHostFixture() {

        var builder = DistributedApplicationTestingBuilder
            .CreateAsync<Projects.TodoApp_AppHost>(["UseVolumes=false"]).Result;
        App = builder.Build();
  }

    public async ValueTask DisposeAsync() => await App.DisposeAsync();
}
