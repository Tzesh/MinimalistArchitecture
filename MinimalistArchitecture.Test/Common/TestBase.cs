namespace MinimalistArchitecture.Test.Common;

/// <summary>
/// Base class for all tests
/// </summary>
public class TestBase
{
    // getter for the client
    protected HttpClient _client
    {
        get
        {
            var client = TestFactory.GetClient();
            return client;
        }
    }

    // getter for the client with authorization
    protected HttpClient _clientWithAuthorization
    {
        get
        {
            return TestFactory.GetClientWithAuthorization();
        }
    }
}