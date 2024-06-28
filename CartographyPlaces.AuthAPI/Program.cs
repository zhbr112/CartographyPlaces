using CartographyPlaces.AuthAPI.Data;
using CartographyPlaces.AuthAPI.Models;
using CartographyPlaces.AuthAPI.Services;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();

builder.AddNpgsqlDbContext<UserDbContext>("user-db");

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddSingleton<JwtSecurityTokenHandler>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/login", async ([AsParameters] UserDTO userDTO, IUserService userService, JwtSecurityTokenHandler jwtSecurityTokenHandler) =>
{
    var user = new User(userDTO.Id, userDTO.Username, userDTO.Photo_url,
        userDTO.First_name, userDTO.Second_name);

    string resultJwt;

    try
    {
        var jwt = await userService.LoginUserAsync(user, userDTO.Hash, userDTO.Auth_date);
        resultJwt = jwtSecurityTokenHandler.WriteToken(jwt);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }

    return Results.Ok(resultJwt);
});

app.MapGet("/{id}", async (Guid id, IUserService userService) =>
{
    try
    {
        var user = await userService.GetUserAsync(id);

        return Results.Ok(user);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.Run();

public record class UserDTO(long Id, string First_name, string? Second_name,
    string Username, string Photo_url, int Auth_date, string Hash);