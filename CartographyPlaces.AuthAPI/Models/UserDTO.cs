namespace CartographyPlaces.AuthAPI.Models
{
    public class UserDTO
    {
        public required long Id { get; set; }
        public required string First_name { get; set; }
        public string? Second_name { get; set; }
        public required string Username { get; set; }
        public required string Photo_url { get; set; }
        public required int Auth_date { get; set; }
        public DateTime DTAuth_date => DateTimeOffset.FromUnixTimeSeconds(Auth_date).DateTime;
        public required string Hash { get; set; }
    }
}
