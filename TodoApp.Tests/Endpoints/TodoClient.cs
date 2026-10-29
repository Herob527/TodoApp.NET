using System.Net.Http.Json;
using TodoApp.Contracts.Dto;

namespace TodoApp.Tests.Endpoints;

public record TodoClient(HttpClient Client)
{

  private readonly string Base = "/api/todos";

    public async Task<List<TodoDto>> GetPaginatedAsync(int Page = 0) => await Client.GetFromJsonAsync<List<TodoDto>>($"{Base}?page={Page}");

    public async Task<HttpResponseMessage> CreateAsync(string Name) => await Client.PostAsJsonAsync($"{Base}", new { Name });
}


