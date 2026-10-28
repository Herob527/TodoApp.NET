using FluentValidation;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoApp.ApiService;
using TodoApp.ApiService.Endpoints.Todo;
using TodoApp.ApiService.Endpoints.User;
using TodoApp.ApiService.Middlewares;
using TodoApp.ApiService.Pipelines;
using TodoApp.ApiService.Repository;
using TodoApp.Domain.Entity;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddMediatR(cfg =>
{

    cfg.RegisterServicesFromAssemblyContaining<Program>();
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
});


builder.Services.AddDbContext<TodoContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("appdata")));

builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
        options => builder.Configuration.Bind("JwtSettings", options))
    .AddCookie("Identity.Application",
        options => builder.Configuration.Bind("CookieSettings", options));

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<TodoContext>();

builder.Services.AddAuthorizationBuilder().SetDefaultPolicy(
    new AuthorizationPolicyBuilder(
      JwtBearerDefaults.AuthenticationScheme, "Identity.Application"
    )
    .RequireAuthenticatedUser()
    .Build()
).AddPolicy("Admin", policy => policy.RequireClaim("role", "Admin"));


// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAntiforgery((options) => options.HeaderName = "X-XSRF-TOKEN");

builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<SignInManager<ApplicationUser>>();

builder.Services.AddScoped<UserManager<ApplicationUser>>();

builder.Services.AddScoped<UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAntiforgery();

app.MapDefaultEndpoints();

app.MapTodoEndpoints();

app.MapUserEndpoints();

app.MapGet("/antiforgery/token", (
    HttpContext context,
    IAntiforgery antiforgery) =>
{
    var tokens = antiforgery.GetAndStoreTokens(context);

    return Results.Ok(new
    {
        requestToken = tokens.RequestToken
    });
});

app.Run();
