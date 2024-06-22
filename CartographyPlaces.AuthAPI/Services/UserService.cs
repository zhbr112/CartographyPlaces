using CartographyPlaces.AuthAPI.Data;
using CartographyPlaces.AuthAPI.Models;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CartographyPlaces.AuthAPI.Services
{
    public class UserService (UserDbContext db, IHttpContextAccessor httpContextAccessor, IConfiguration config) : IUserService
    {
        private User ValidateUser(UserDTO userDTO)
        {
            if ((DateTime.UtcNow - userDTO.DTAuth_date).TotalHours>24) 
                throw new Exception();

            var dataCheckString = string.Join('\n', 
                httpContextAccessor.HttpContext!.Request.Query
                .Where(x => !x.Key.Equals("hash"))
                .OrderBy(x => x.Key)
                .Select(x => $"{x.Key}={x.Value}"));

            var secretKey = SHA256.HashData(Encoding.UTF8.GetBytes(config["botToken"]?? 
                throw new Exception("Нет botToken")));

            if (!Convert.ToHexString(HMACSHA256.HashData(secretKey, 
                Encoding.UTF8.GetBytes(dataCheckString)))
                .Equals(userDTO.Hash, StringComparison.OrdinalIgnoreCase))
                throw new Exception("Данные не прошли проверку");

            return new User { 
                Id = userDTO.Id, 
                FirstName=userDTO.First_name, 
                SecondName=userDTO.Second_name??"", 
                Username=userDTO.Username, 
                PhotoUrl=userDTO.Photo_url 
            };
        }

        public async Task<string> LoginUser(UserDTO userDTO)
        {
            var user = ValidateUser(userDTO);
            if (await db.Users.FirstOrDefaultAsync(x => x.Id == user.Id) is null) 
                await db.Users.AddAsync(user);

            var claims = new List<Claim> { 
                new ("Id", user.Id.ToString()) , 
                new ("FirstName", user.FirstName),
                new ("SecondName", user.SecondName),
                new ("Username", user.Username),
                new ("PhotoUrl", user.PhotoUrl)
            };

            var jwt = new JwtSecurityToken(
                    issuer: config["Properties:issuer"] ?? throw new Exception("Нет issuer"),
                    audience: config["Properties:audience"] ?? throw new Exception("Нет audience"),
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromDays(1)),
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Properties:jwtKey"] 
                        ?? throw new Exception("Нет jwtKey"))), 
                        SecurityAlgorithms.HmacSha256));
            
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
