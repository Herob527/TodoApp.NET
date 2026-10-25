namespace TodoApp.ApiService.Endpoints.User;
public static class Endpoints
{

    extension(IEndpointRouteBuilder app)
    {
        public void MapUserEndpoints()
        {
            var group = app.MapGroup("/api/users").WithTags("Users").DisableAntiforgery();

            group.MapGet("/{id}", GetUser);
            group.MapGet("/", GetUsers);
            group.MapPost("/", AddUser);
            group.MapPost("/register", RegisterUser);
            group.MapPatch("/{id}", UpdateUser);
            group.MapDelete("/{id}", DeleteUser);
        }
    }
}
