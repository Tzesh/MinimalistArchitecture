using minimalistArchitecture.Todo;
namespace minimalistArchitecture;

public class RouteManager
{
    public static void RegisterRoutes(WebApplication app)
    {
        TodoRoutes.RegisterRoutes(app);
    }
}