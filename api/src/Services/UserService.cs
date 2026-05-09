using api.src.Models;
using api.src.Repositories;

namespace api.src.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService (IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<User?> GetUserAsync(int id)
        {
            return await _repo.GetUserAsync(id);
        }
    }
}
