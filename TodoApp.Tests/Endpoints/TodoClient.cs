using System.Net.Http.Json;
using TodoApp.Contracts.Dto;

namespace TodoApp.Tests.Endpoints;

public record TodoClient(HttpClient Client)
{

  private readonly string Base = "/api/todos";

    public async Task<List<TodoDto>?> GetPaginatedAsync(int Page = 0) => await Client.GetFromJsonAsync<List<TodoDto>>($"{Base}?page={Page}");

    public async Task<List<TodoDto>> GetAllAsync()
    {
        var result = new List<TodoDto>();

        for (var page = 0; ; page++)
        {
            var items = await GetPaginatedAsync(page);

            if (items is null || items.Count == 0)
            {
                return result;
            }

            result.AddRange(items);
        }
    }

    public async Task<HttpResponseMessage> CreateAsync(string Name) => await Client.PostAsJsonAsync($"{Base}", new { Name });
}
