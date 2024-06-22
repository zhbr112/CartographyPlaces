namespace CartographyPlaces.AuthAPI.Models;

public class User
{
    public required long Id { get; set; }
    public required string FirstName { get; set; }
    public string SecondName { get; set; } = "";
    public required string Username { get; set; }
    public required string PhotoUrl { get; set; }
}