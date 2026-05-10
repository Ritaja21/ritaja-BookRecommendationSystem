using api.src.Models;
using api.src.Models.DTO;

namespace api.src.Services
{
    public interface IUserService
    {
        Task<User?> GetUserAsync(int id);
        Task<User?> UpdateProfileAsync(int id, UserUpdateDTO userUpdateDTO);
    }
}
