using System.Net.Http.Json;
using TodoApp.Contracts.Dto;
using TodoApp.Domain.Enum;
using TodoApp.Tests.Data;

namespace TodoApp.Tests.Endpoints;
public record UserClient(HttpClient Client)
{

  private readonly string Base = "/api/users";
    public Task<HttpResponseMessage> RegisterUserAsync(TestUser user)
    {
        return Client.PostAsJsonAsync( $"{Base}/register", new { Name = user.Name, Password = user.Password });
    }

    public Task<HttpResponseMessage> LoginUserAsync(TestUser user)
    {
        return Client.PostAsJsonAsync($"{Base}/login", new { Name = user.Name, Password = user.Password });
    }

    public Task<HttpResponseMessage> DeleteUserAsync(string Id) {
        return Client.DeleteAsync($"{Base}/{Id}");
    }

    public Task<HttpResponseMessage> UpdateUserAsync(string Id, string? Name = null, Role? Role = null)
    {
        return Client.PatchAsJsonAsync($"{Base}/{Id}", new { Name, Role });
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

    public Task<UserDto?> GetUserAsync(string Id)
    {
        return Client.GetFromJsonAsync<UserDto>($"{Base}/{Id}");
    }

    public Task<List<UserDto>?> GetUsersAsync(int page = 0)
    {
        return Client.GetFromJsonAsync<List<UserDto>>($"{Base}?page={page}");
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var result = new List<UserDto>();

        for (var page = 0; ; page++)
        {
            var items = await GetUsersAsync(page);

            if (items is null || items.Count == 0)
            {
                return result;
            }

            result.AddRange(items);
        }
    }


}
