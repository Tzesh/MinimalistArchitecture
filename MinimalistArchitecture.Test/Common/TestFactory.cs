namespace MinimalistArchitecture.Test.Common;
public static class TestFactory
{
    // create application variable
    public static WebApplicationFactory<Program> Application = new WebApplicationFactory<Program>();

    // create a token variable
    private static String? _token;

    // create a token getter
    private static async Task<String> getToken()
    {
        if (_token == null)
        {
            // get client
            var client = TestFactory.GetClient();

            // create dto
            var dto = new UserDTO
            {
                Name = "Uğur Dindar",
                Email = "test@ugurdindar.com",
                Password = "12345678"
            };

            // register
            var result = await client.PostAsJsonAsync("/user/register", dto);

            // login
            result = await client.PostAsJsonAsync("/user/login", dto);

            // wait for the response
            var data = await result.Content.ReadAsStringAsync();

            // set token
            _token = data;
        }

        return _token;
    }

    public static HttpClient GetClientWithAuthorization()
    {
        // get client
        var _client = TestFactory.GetClient();

        // get token
        var token = getToken().Result;

        if (!_client.DefaultRequestHeaders.Contains("Authorization"))

        // authorize client
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.Trim('"')}");

        // return client
        return _client;
    }

    public static HttpClient GetClient()
    {
        // return client
        return Application.CreateClient();
    }
}