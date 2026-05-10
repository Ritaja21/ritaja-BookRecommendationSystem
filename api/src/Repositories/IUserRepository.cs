using api.src.Models;

namespace api.src.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserAsync(int id);
        Task<User?> UpdateUserAsync(User user);
    }
}
