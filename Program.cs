using Microsoft.EntityFrameworkCore;
using MinimalistArchitecture.Todo;
using Microsoft.OpenApi.Models;
using MinimalistArchitecture;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// database configurations
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// validation setup
builder.Services.AddValidatorsFromAssemblyContaining<TodoValidator>(ServiceLifetime.Singleton);

// swagger setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Minimalist Architecture",
        Description = "An ASP.NET Core Minimal Web API Structure",
        Contact = new OpenApiContact
        {
            Name = "GitHub",
            Url = new Uri("https://github.com/tzesh/MinimalistArchitecture")
        }
    });
});

// build the application
var app = builder.Build();

// register routes
ServiceManager.RegisterRoutes(app);

// if application is not in development then use an exception route
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}
// else use swagger
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// use routing
app.UseHttpsRedirection();

// run the application
app.Run();