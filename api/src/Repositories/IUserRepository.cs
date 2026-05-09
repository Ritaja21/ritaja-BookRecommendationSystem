using api.src.Models;

namespace api.src.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserAsync(int id);
    }
}
