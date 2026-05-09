using api.src.Models;

namespace api.src.Services
{
    public interface IUserService
    {
        Task<User?> GetUserAsync(int id); 
    }
}
