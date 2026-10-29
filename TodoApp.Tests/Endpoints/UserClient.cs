using System.Net.Http.Json;

namespace TodoApp.Tests.Endpoints;
public record UserClient(HttpClient Client)
{

  private readonly string Base = "/api/users";
    public Task<HttpResponseMessage> RegisterUserAsync(string Name, string Password)
    {
        return Client.PostAsJsonAsync( $"{Base}/register", new { Name, Password });
    }
}


