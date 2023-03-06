using System.Reflection;
using MinimalistArchitecture.Abstract;

namespace MinimalistArchitecture;

/// <summary>
/// A class that registers all routes in the application
/// </summary>
public class ServiceManager
{
    /// <summary>
    /// Register all routes in the application
    /// </summary>
    /// <param name="app"></param>
    public static void RegisterRoutes(WebApplication app)
    {
        // get all classes that inherit from Service
        var serviceList = Assembly.GetExecutingAssembly().GetTypes()
                      .Where(t => t.IsSubclassOf(typeof(Service)) && !t.IsAbstract)
                      .ToList();

        // create an instance of each class
        foreach (var service in serviceList)
        {
            var serviceInstance = Activator.CreateInstance(service, app);
        }
    }
}