using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalistArchitecture.Common;
using MinimalistArchitecture.Common.Abstract;

namespace MinimalistArchitecture.Routes.User;
public class UserService : Service
{
    private IConfiguration _configuration;

    // constructor
    public UserService(WebApplication app) : base(app)
    {
        // create a route group
        RouteGroupBuilder UserEndpoints = app.MapGroup("/user");

        // add the validation filter to the group
        UserEndpoints.AddEndpointFilterFactory(ValidationFactory.ValidationFilterFactory);

        // add the routes to the group
        UserEndpoints.MapGet("/{id}", GetUser);
        UserEndpoints.MapPost("/login", Login);
        UserEndpoints.MapPost("/register", Register);

        this._configuration = app.Configuration;
    }

    [Authorize]
    public async Task<IResult> GetUser(Guid id, UserDb db)
    {
        return await db.Users.FindAsync(id)
            is User User
                ? TypedResults.Ok(new UserDTO(User))
                : TypedResults.NotFound();
    }
    
    [AllowAnonymous]
    public async Task<IResult> Login([Validate] UserDTO UserDto, UserDb db)
    {
        // hash the password
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(UserDto.Password);

        // get the user from the database by email
        var User = await db.Users.FirstOrDefaultAsync(x => x.Email == UserDto.Email);
        
        // if email is not found
        if (User is null)
            return TypedResults.BadRequest("Email or password is incorrect"); // return a bad request

        // if the password is incorrect
        if (!BCrypt.Net.BCrypt.Verify(UserDto.Password, User.Password))
            return TypedResults.BadRequest("Email or password is incorrect"); // return a bad request

        // create necessary variables for creating the token
        var jwtTokenHandler = new JwtSecurityTokenHandler();

        // create required token fields
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
        
        // create a new token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new []
            {
                new Claim("Id", User.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, User.Name),
                new Claim(JwtRegisteredClaimNames.Email, User.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),

            Expires = DateTime.UtcNow.AddHours(12),
            Audience = audience,
            Issuer = issuer,
            NotBefore = DateTime.UtcNow,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
        };

        // write the token
        var token = jwtTokenHandler.CreateToken(tokenDescriptor);

        // create a string from the token
        var jwtToken = jwtTokenHandler.WriteToken(token);

        // return the token
        return TypedResults.Ok(jwtToken);
    }

    [AllowAnonymous]
    public async Task<IResult> Register([Validate] UserDTO UserDto, UserDb db)
    {
        // hash the password
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(UserDto.Password);

        // check if the email already exists
        if (await db.Users.AnyAsync(x => x.Email == UserDto.Email))
        {
            // return a bad request
            return TypedResults.BadRequest("Email already exists");
        }

        // create a new user
        var User = new User
        {
            Name = UserDto.Name,
            Email = UserDto.Email,
            Password = hashedPassword
        };

        // add the user to the database
        db.Users.Add(User);

        // save the changes
        await db.SaveChangesAsync();

        // return a created response
        return TypedResults.Created($"/Useritems/{User.Id}", UserDto);
    }
}