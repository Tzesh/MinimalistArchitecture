namespace MinimalistArchitecture.Test;

public class TodoTest : TestBase
{
    [Fact]
    public async Task TestCreate()
    {
        // get the client
        var client = _clientWithAuthorization;

        // create a new todo item
        var result = await client.PostAsJsonAsync("/todo/", new TodoDTO
        {
            Name = "Wake up",
            IsComplete = false
        });
        
        // wait for the response
        var data = await result.Content.ReadAsStringAsync();

        // control the response status code
        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        Assert.Equal("{\"name\":\"Wake up\",\"isComplete\":false,\"id\":\"00000000-0000-0000-0000-000000000000\"}", data);
    }

    [Fact]
    public async Task TestGet()
    {
        // get the client
        var client = _clientWithAuthorization;

        // create a new todo item
        var result = await client.GetAsync("/todo/{03fbd193-3b22-4ecd-ade7-fa5a336e474e}");
        
        // wait for the response
        var data = await result.Content.ReadAsStringAsync();
        
        Console.WriteLine(data);

        // control the response status code
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task TestGetAll()
    {
        // get the client
        var client = _clientWithAuthorization;

        // get all todo items
        var result = await client.GetAsync("/todo/");
        
        // wait for the response
        var data = await result.Content.ReadAsStringAsync();

        // control the response status code
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }
}