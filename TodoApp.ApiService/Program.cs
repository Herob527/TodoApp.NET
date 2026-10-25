using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TodoApp.ApiService;
using TodoApp.ApiService.Endpoints.Todo;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

builder.Services.AddMediatR(options => options.RegisterServicesFromAssemblyContaining<Program>());

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddDbContext<TodoContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("appdata")));

builder.Services.AddAuthentication().AddBearerToken();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapDefaultEndpoints();

app.MapTodoEndpoints();

app.Run();
