namespace MinimalistArchitecture.Test;

/// <summary>
/// Class that tests given routes for unauthorized access
/// </summary>
public class UnauthorizedTest : TestBase {

    /// <summary>
    /// Method that tests given routes for unauthorized access
    /// </summary>
    /// <param name="method">The method to use</param>
    /// <param name="route">The route to test</param>
    [Theory]
    // you can add more routes here as follows:
    // [InlineData("<method>", "<route>")]
    [InlineData("get", "/user/{03fbd193-3b22-4ecd-ade7-fa5a336e474e}")]
    [InlineData("get", "/todo/")]
    [InlineData("post", "/todo/")]
    [InlineData("delete", "/todo/{03fbd193-3b22-4ecd-ade7-fa5a336e474e}")]
    [InlineData("put", "/todo/{03fbd193-3b22-4ecd-ade7-fa5a336e474e}")]
    public async void TestUnauthorized(String method, String route)
    {
        // get client
        var client = _client;

        // create result
        HttpResponseMessage result;

        // switch method
        switch (method)
        {
            case "post":
                result = await client.PostAsync(route, null);
                break;
            case "put":
                result = await client.PutAsync(route, null);
                break;
            case "delete":
                result = await client.DeleteAsync(route);
                break;
            default:
                result = await client.GetAsync(route);
                break;
        }

        // get data
        var data = await result.Content.ReadAsStringAsync();

        // control the response status code
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }
}