using TodoApp.Contracts.Dto;
using TodoApp.Contracts.Validation;
using TodoApp.Tests.Endpoints;
using TodoApp.Tests.Extensions;
using TodoApp.Tests.Fixtures;

namespace TodoApp.Tests.Tests;

public class TodoIntegrationTest : IAsyncLifetime
{
    private AppHostFixture _fixture = null!;

    private HttpClient _httpClient = null!;

    private TodoClient _todoClient = null!;

    private UserClient _userClient = null!;

    private static readonly TimeSpan DefaultTimeout =
        TimeSpan.FromSeconds(60);

    private const string password = "zaq1@WSX";


    public async Task InitializeAsync()
    {
        _fixture = await AppHostFixture.CreateAsync("todo");

        await _fixture
            .App
            .StartAsync()
            .WaitAsync(DefaultTimeout);

        _httpClient = _fixture.App.CreateHttpClient("apiservice");
        _todoClient = new TodoClient(_httpClient);
        _userClient = new UserClient(_httpClient);
    }

    public async Task DisposeAsync()
    {
        await _fixture.DisposeAsync();
    }

    [Fact(DisplayName = "Create valid todo item, it should response with created with one item in list")]
    public async Task CreateTodoSuccessful()
    {

        await using (await LoginFixture.CreateAsync(_userClient, string.RandomText(10), password, register: true))
        {
            var createTodoResponse = await _todoClient.CreateAsync("testing2");
            var getTodosResponse = await _todoClient.GetPaginatedAsync();

            Assert.Equal(HttpStatusCode.Created, createTodoResponse.StatusCode);
            Assert.NotNull(getTodosResponse);
            Assert.Single(getTodosResponse);
        }
    }

    [Fact(DisplayName = "Create invalid todo, it should respond with bad request")]
    public async Task CreateTodoValidationFail()
    {

        string tooLong = string.RandomText(TodoConstraints.NameMaxLength + 1);

        await using (await LoginFixture.CreateAsync(_userClient, string.RandomText(10), password, register: true))
        {

            var tooShortTodo = await _todoClient.CreateAsync("");
            var tooLongTodo = await _todoClient.CreateAsync(tooLong);

            Assert.Equal(HttpStatusCode.BadRequest, tooShortTodo.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, tooLongTodo.StatusCode);
        }
    }

    [Fact(DisplayName = "Create todos as separate users, each user gets own todos")]
    public async Task CreateTodoAsSeparateUser()
    {

        string firstUserTodo = string.RandomText(10);
        string secondUserTodo = string.RandomText(10);

        string firstUserName = string.RandomText(10);
        string secondUserName = string.RandomText(10);

        Dictionary<string, List<TodoDto>> users = [];

        users.Add(firstUserName, []);
        users.Add(secondUserName, []);

        await using (await LoginFixture.CreateAsync(_userClient, firstUserName, password, register: true))
        {
            var todo = await _todoClient.CreateAsync(firstUserTodo);
            var getTodosResponse = await _todoClient.GetPaginatedAsync();
            users[firstUserName].AddRange(getTodosResponse!);
        }

        await using (await LoginFixture.CreateAsync(_userClient, secondUserName, password, register: true))
        {
            var todo = await _todoClient.CreateAsync(secondUserTodo);
            var getTodosResponse = await _todoClient.GetPaginatedAsync();
            users[secondUserName].AddRange(getTodosResponse!);
        }

        Assert.Single(users[firstUserName]);
        Assert.Single(users[secondUserName]);

        Assert.Equal(firstUserTodo, users[firstUserName][0].Name);
        Assert.Equal(secondUserTodo, users[secondUserName][0].Name);
    }
}
