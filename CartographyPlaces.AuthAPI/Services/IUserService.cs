using CartographyPlaces.AuthAPI.Models;

namespace CartographyPlaces.AuthAPI.Services;

public interface IUserService
{
    public Task<string> LoginUser(UserDTO userDTO);
}
