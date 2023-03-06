using MinimalistArchitecture.Todo;
namespace MinimalistArchitecture;

public class RouteManager
{
    public static void RegisterRoutes(WebApplication app)
    {
        TodoRoutes.RegisterRoutes(app);
    }
}