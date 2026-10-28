
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder
      .AddPostgres("db")
      .WithContainerName("todo-db")
      .AddDatabase("appdata");

var migrations = builder
      .AddProject<Projects.TodoApp_MigrationService>("migrations")
      .WithReference(postgres)
      .WaitFor(postgres);

var apiService = builder
    .AddProject<Projects.TodoApp_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(postgres)
    .WithReference(migrations)
    .WaitForCompletion(migrations)
    .WaitFor(postgres);

builder.AddProject<Projects.TodoApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
