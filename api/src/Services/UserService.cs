using api.src.Models;
using api.src.Models.DTO;
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

        public async Task<User?> UpdateProfileAsync(int id, UserUpdateDTO userUpdateDTO)
        {
            var user = await _repo.GetUserAsync(id);

            if (user == null)
            {
                return null;
            }

            user.Name = userUpdateDTO.Name;
            user.UpdatedAt = DateTime.UtcNow;

            return await _repo.UpdateUserAsync(user);
        }
    }
}
