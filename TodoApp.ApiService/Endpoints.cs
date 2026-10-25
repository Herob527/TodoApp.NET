namespace TodoApp.ApiService;
public static class Endpoints
{

  extension(IEndpointRouteBuilder app) {
    public void MapTodoEndpoints() {

      var group = app.MapGroup("/api/todos").WithTags("Todos");
      group.MapGet("/", GetTodos);

    }
  }
  private static async Task<IResult> GetTodos() => Results.Json(new { message = "Hello from .NET 10" });
}
