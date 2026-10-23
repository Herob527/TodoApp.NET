var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("db").AddDatabase("appdata");

var apiService = builder.AddProject<Projects.TodoApp_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.TodoApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
