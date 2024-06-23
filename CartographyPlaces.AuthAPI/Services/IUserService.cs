using CartographyPlaces.AuthAPI.Models;
using System.IdentityModel.Tokens.Jwt;

namespace CartographyPlaces.AuthAPI.Services;

public interface IUserService
{
    public Task<JwtSecurityToken> LoginUser(User user, string Hash, int Auth_date);
}
