using System.Net.Http.Json;
using TodoApp.Contracts.Dto;

namespace TodoApp.Tests.Endpoints;
public record UserClient(HttpClient Client)
{

  private readonly string Base = "/api/users";
    public Task<HttpResponseMessage> RegisterUserAsync(string Name, string Password)
    {
        return Client.PostAsJsonAsync( $"{Base}/register", new { Name, Password });
    }

    public Task<HttpResponseMessage> LoginUserAsync(string Name, string Password)
    {
        return Client.PostAsJsonAsync($"{Base}/login", new { Name, Password });
    }

    public Task<HttpResponseMessage> UpdateUserAsync(string Id, string Name)
    {
        return Client.PatchAsJsonAsync($"{Base}/{Id}", new { Name });
    }

    public Task<HttpResponseMessage> UpdateSelfAsync(string Name)
    {
        return Client.PatchAsJsonAsync($"{Base}/me", new { Name });
    }
    public Task<HttpResponseMessage> LogoutUserAsync()
    {
        return Client.PostAsync($"{Base}/logout", null);
    }

    public Task<UserDto?> GetUserMeAsync()
    {
        return Client.GetFromJsonAsync<UserDto>($"{Base}/me");
    }
}
