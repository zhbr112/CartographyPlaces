using CartographyPlaces.AuthAPI.Data;
using CartographyPlaces.AuthAPI.Models;
using CartographyPlaces.AuthAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.AddNpgsqlDbContext<UserDbContext>("user-db");

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.MapGet("/login", async ([AsParameters] UserDTO userDTO, IUserService userService) =>
{
    try
    {
        return Results.Ok(await userService.LoginUser(userDTO));
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.Run();