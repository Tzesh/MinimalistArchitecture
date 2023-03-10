namespace MinimalistArchitecture.Test;

public class UserTest : TestBase
{
    [Fact]
    public async Task TestRegister()
    {
        // get client
        var client = _client;

        // register
        var result = await client.PostAsJsonAsync("/user/register", new UserDTO
        {
            Name = "Uğur Dindar",
            Email = "mail@ugurdindar.com",
            Password = "12345678"
        });
        
        // wait for the response
        var data = await result.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.Created, result.StatusCode);
        Assert.Equal("{\"name\":\"Uğur Dindar\",\"email\":\"mail@ugurdindar.com\",\"password\":\"12345678\",\"id\":\"00000000-0000-0000-0000-000000000000\"}", data);
    }

    [Fact]
    public async Task<String> TestLogin()
    {
        // get client
        var client = _client;

        var dto = new UserDTO
        {
            Name = "Uğur Dindar",
            Email = "mail1@ugurdindar.com",
            Password = "12345678"
        };

        // register
        var result = await client.PostAsJsonAsync("/user/register", dto);

        // login
        result = await client.PostAsJsonAsync("/user/login", dto);

        // wait for the response
        var data = await result.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(data);

        return data;
    }
}