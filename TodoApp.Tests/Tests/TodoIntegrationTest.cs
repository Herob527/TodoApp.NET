using TodoApp.Contracts.Dto;
using TodoApp.Contracts.Validation;
using TodoApp.Tests.Data;
using TodoApp.Tests.Endpoints;
using TodoApp.Tests.Extensions;
using TodoApp.Tests.Fixtures;
using TodoApp.Tests.Support;

namespace TodoApp.Tests.Tests;

public class TodoIntegrationTest : IClassFixture<AppHostFixture>, IDisposable
{
    private readonly HttpClient _httpClient;

    private readonly TodoClient _todoClient;

    private readonly UserClient _userClient;

    public TodoIntegrationTest(AppHostFixture fixture)
    {
        _httpClient = fixture.App.CreateHttpClient("apiservice");
        _todoClient = new TodoClient(_httpClient);
        _userClient = new UserClient(_httpClient);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    [Fact(DisplayName = "Create valid todo item, it should response with created with one item in list")]
    public async Task CreateValidTodo_Successful()
    {

        await using (await LoginSession.CreateAsync(_userClient, TestUser.Random(), register: true))
        {
            var todoName = string.RandomText(10);
            var createTodoResponse = await _todoClient.CreateAsync(todoName);
            var getTodosResponse = await _todoClient.GetPaginatedAsync();

            Assert.Equal(HttpStatusCode.Created, createTodoResponse.StatusCode);
            Assert.NotNull(getTodosResponse);
            Assert.Single(getTodosResponse);
            Assert.Equal(todoName, getTodosResponse[0].Name);
        }
    }

    [Fact(DisplayName = "Update own todo item, it should succeed and todo item be updated ")]
    public async Task UpdateOwnTodo_Successful()
    {

        await using (await LoginSession.CreateAsync(_userClient, TestUser.Random(), register: true))
        {
            var todoName = string.RandomText(10);
            var newTodoName = string.RandomText(10);
            await _todoClient.CreateAsync(todoName);
            var initialGetTodosResponse = (await _todoClient.GetPaginatedAsync()).First(x => x.Name == todoName);
            var updateTodoResponse = await _todoClient.UpdateAsync(initialGetTodosResponse.Id, newTodoName);
            var afterUpdateGetTodosResponse = (await _todoClient.GetPaginatedAsync()).First(x => x.Id == initialGetTodosResponse.Id);


            Assert.Equal(newTodoName, afterUpdateGetTodosResponse.Name);
            Assert.Equal(HttpStatusCode.OK, updateTodoResponse.StatusCode);
        }
    }

    [Fact(DisplayName = "Update someone's todo item as User, it should fail and todo item be intact.")]
    public async Task UpdateSomeonesTodoAsUser_ShouldFail()
    {

        var todoName = string.RandomText(10);
        var newTodoName = string.RandomText(10);
        var creator = TestUser.Random();
        var unapprovedEditor = TestUser.Random();
        int todoId;
        await using (await LoginSession.CreateAsync(_userClient, creator, register: true))
        {
            await _todoClient.CreateAsync(todoName);
            var initialGetTodosResponse = (await _todoClient.GetPaginatedAsync()).First(x => x.Name == todoName);
            todoId = initialGetTodosResponse.Id;
        }
        HttpResponseMessage updateTodoResponse;
        await using (await LoginSession.CreateAsync(_userClient, unapprovedEditor, register: true))
        {
            updateTodoResponse = await _todoClient.UpdateAsync(todoId, Name: newTodoName);
        }

        var wasModified = false;
        await using (await LoginSession.CreateAsync(_userClient, creator))
        {
            var todoList = await _todoClient.GetPaginatedAsync();
            var hasOriginal = todoList.Any(x => x.Name == todoName);
            var hasModified = todoList.Any(x => x.Name == newTodoName);
            wasModified = !hasOriginal && hasModified;
        }

        Assert.Equal(HttpStatusCode.Unauthorized, updateTodoResponse.StatusCode);
        Assert.False(wasModified);

    }

    [Fact(DisplayName = "Update someone's todo item as Admin, it should succeed and todo item be modified.")]
    public async Task UpdateSomeonesTodoAsAdmin_ShouldSucceed()
    {

        var todoName = string.RandomText(10);
        var newTodoName = string.RandomText(10);
        var creator = TestUser.Random();
        var unapprovedEditor = TestUser.Random();
        int todoId;
        await using (await LoginSession.CreateAsync(_userClient, creator, register: true))
        {
            await _todoClient.CreateAsync(todoName);
            var initialGetTodosResponse = (await _todoClient.GetPaginatedAsync()).First(x => x.Name == todoName);
            todoId = initialGetTodosResponse.Id;
        }
        HttpResponseMessage updateTodoResponse;
        await using (await LoginSession.CreateAsync(_userClient, AdminUser.User))
        {
            updateTodoResponse = await _todoClient.UpdateAsync(todoId, Name: newTodoName);
        }

        var wasModified = false;
        await using (await LoginSession.CreateAsync(_userClient, creator))
        {
            var todoList = await _todoClient.GetPaginatedAsync();
            var hasOriginal = todoList.Any(x => x.Name == todoName);
            var hasModified = todoList.Any(x => x.Name == newTodoName);
            wasModified = !hasOriginal && hasModified;
        }

        Assert.Equal(HttpStatusCode.OK, updateTodoResponse.StatusCode);
        Assert.True(wasModified);

    }

    [Fact(DisplayName = "Delete own todo item, it should succeed and todo item be deleted.")]
    public async Task DeleteOwnTodo_Successful()
    {

        await using (await LoginSession.CreateAsync(_userClient, TestUser.Random(), register: true))
        {
            var todoName = string.RandomText(10);
            await _todoClient.CreateAsync(todoName);
            var initialGetTodosResponse = (await _todoClient.GetPaginatedAsync()).First(x => x.Name == todoName);
            var deleteTodoResponse = await _todoClient.DeleteAsync(initialGetTodosResponse.Id);
            var afterDeleteGetTodosResponse = (await _todoClient.GetPaginatedAsync()).ConvertAll(x => x.Name);


            Assert.DoesNotContain(todoName, afterDeleteGetTodosResponse);
            Assert.Equal(HttpStatusCode.OK, deleteTodoResponse.StatusCode);
        }
    }

    [Fact(DisplayName = "Delete someones todo item as Admin, it should succeed and todo item be removed.")]
    public async Task DeleteSomeonesTodoAsAdmin_ShouldSucceed()
    {

        int todoIdToRemove;
        var ownerOfTodo = TestUser.Random();
        var todoName = string.RandomText(10);
        await using (await LoginSession.CreateAsync(_userClient, ownerOfTodo, register: true))
        {
            await _todoClient.CreateAsync(todoName);
            var initialGetTodosResponse = (await _todoClient.GetPaginatedAsync()).First(x => x.Name == todoName);
            todoIdToRemove = initialGetTodosResponse.Id;
        }

        HttpResponseMessage deleteTodoResponse;
        await using (await LoginSession.CreateAsync(_userClient, AdminUser.User))
        {
            deleteTodoResponse = await _todoClient.DeleteAsync(todoIdToRemove);
        }

        bool isTodoStillThere = false;
        await using (await LoginSession.CreateAsync(_userClient, ownerOfTodo))
        {
            var afterTodoResponse = (await _todoClient.GetPaginatedAsync()).FirstOrDefault(x => x.Name == todoName);
            isTodoStillThere = afterTodoResponse is not null;
        }

        Assert.Equal(HttpStatusCode.OK, deleteTodoResponse.StatusCode);
        Assert.False(isTodoStillThere);
    }

    [Fact(DisplayName = "Delete someones todo item as User, it should fail and todo item be intact.")]
    public async Task DeleteSomeonesTodoAsUser_ShouldFail()
    {

        int todoIdToRemove;
        var ownerOfTodo = TestUser.Random();
        var todoName = string.RandomText(10);
        await using (await LoginSession.CreateAsync(_userClient, ownerOfTodo, register: true))
        {
            await _todoClient.CreateAsync(todoName);
            var initialGetTodosResponse = (await _todoClient.GetPaginatedAsync()).First(x => x.Name == todoName);
            todoIdToRemove = initialGetTodosResponse.Id;
        }

        HttpResponseMessage deleteTodoResponse;
        await using (await LoginSession.CreateAsync(_userClient, TestUser.Random(), register: true))
        {
            deleteTodoResponse = await _todoClient.DeleteAsync(todoIdToRemove);
        }

        bool isTodoStillThere = false;
        await using (await LoginSession.CreateAsync(_userClient, ownerOfTodo))
        {
            var afterTodoResponse = (await _todoClient.GetPaginatedAsync()).FirstOrDefault(x => x.Name == todoName);
            isTodoStillThere = afterTodoResponse is not null;
        }

        Assert.Equal(HttpStatusCode.Unauthorized, deleteTodoResponse.StatusCode);
        Assert.True(isTodoStillThere);
    }

    [Fact(DisplayName = "Create invalid todo, it should respond with bad request")]
    public async Task CreateInvalidTodo_ValidationFail()
    {

        string tooLong = string.RandomText(TodoConstraints.NameMaxLength + 1);

        await using (await LoginSession.CreateAsync(_userClient, TestUser.Random(), register: true))
        {

            var tooShortTodo = await _todoClient.CreateAsync("");
            var tooLongTodo = await _todoClient.CreateAsync(tooLong);

            Assert.Equal(HttpStatusCode.BadRequest, tooShortTodo.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, tooLongTodo.StatusCode);
        }
    }

    [Fact(DisplayName = "Create todos as separate users, each user gets own todos and admin sees all")]
    public async Task CreateTodoAsSeparateUser_ShouldSucceed()
    {

        string firstUserTodo = string.RandomText(10);
        string secondUserTodo = string.RandomText(10);

        var firstUser = TestUser.Random();
        var secondUser = TestUser.Random();

        Dictionary<string, List<TodoDto>> users = [];

        await using (await LoginSession.CreateAsync(_userClient, firstUser, register: true))
        {
            var todo = await _todoClient.CreateAsync(firstUserTodo);
            var getTodosResponse = await _todoClient.GetPaginatedAsync();
            users.Add(firstUser.Name, getTodosResponse!);
        }

        await using (await LoginSession.CreateAsync(_userClient, secondUser, register: true))
        {
            var todo = await _todoClient.CreateAsync(secondUserTodo);
            var getTodosResponse = await _todoClient.GetPaginatedAsync();
            users.Add(secondUser.Name, getTodosResponse!);
        }

        await using (await LoginSession.CreateAsync(_userClient, AdminUser.User))
        {
            var getTodosResponse = await _todoClient.GetAllAsync();
            users.Add("admin", getTodosResponse);
        }

        Assert.Single(users[firstUser.Name]);
        Assert.Single(users[secondUser.Name]);

        Assert.Equal(firstUserTodo, users[firstUser.Name][0].Name);
        Assert.Equal(secondUserTodo, users[secondUser.Name][0].Name);

        var adminTodoNames = users["admin"].ConvertAll(x => x.Name);
        Assert.Contains(firstUserTodo, adminTodoNames);
        Assert.Contains(secondUserTodo, adminTodoNames);
    }
}
