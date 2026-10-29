using System.Net.Http.Json;

namespace TodoApp.Tests.Endpoints;
public record UserClient(HttpClient Client)
{

  private readonly string Base = "/api/users";
    public Task<HttpResponseMessage> RegisterUser(string Name, string Password)
    {
        return Client.PostAsJsonAsync( $"{Base}/register", new { Name, Password });
    }

    public string Login => $"{Base}/login";

  public string Logout => $"{Base}/logout";

  public string Create => $"{Base}";
}


