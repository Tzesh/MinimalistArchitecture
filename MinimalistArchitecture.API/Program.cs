using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MinimalistArchitecture.Common;
using MinimalistArchitecture.Routes.Todo;
using MinimalistArchitecture.Routes.User;
using MinimalistArchitecture.Common.Abstract;

var builder = WebApplication.CreateBuilder(args);

// add JWT configuration
builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.SaveToken = true;
    o.RequireHttpsMetadata = false;
    o.TokenValidationParameters = new TokenValidationParameters
    {
       ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = false,
        ClockSkew = TimeSpan.Zero
    };
});

// add authorization
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();

// database configurations
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDbContext<UserDb>(opt => opt.UseInMemoryDatabase("Users"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// validation setup
builder.Services.AddValidatorsFromAssemblyContaining(typeof(Validator<DTO>), ServiceLifetime.Singleton);

// swagger setup
builder.Services.AddSwaggerGen(options =>
{
    // information about the project
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

    // options of security requirements
    options.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] {}
    }
    });

    // options of security definition
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JSON Web Token based security",
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
    app.UseDeveloperExceptionPage();
}

// use routing
app.UseHttpsRedirection();

// use authentication
app.UseAuthentication();

// use authorization
app.UseAuthorization();

// run the application
app.Run();

// add partial class to access program.cs from tests of the endpoints
namespace MinimalistArchitecture
{
public partial class Program { }
}