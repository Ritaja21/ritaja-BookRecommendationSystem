using api.src.Models;

namespace api.src.Repositories
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailAsync(string email);

        Task<User> RegisterAsync(User user);

        Task<bool> IsEmailExistsAsync(string email);
    }
}
