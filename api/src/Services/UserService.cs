using api.src.Models;
using api.src.Models.DTO;
using api.src.Repositories;

namespace api.src.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly ILogger<UserService> _logger;

        public UserService (IUserRepository repo, ILogger<UserService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<User?> GetUserAsync(int id)
        {
            return await _repo.GetUserAsync(id);
        }

        public async Task<User?> UpdateProfileAsync(int id, UserUpdateDTO userUpdateDTO)
        {
            _logger.LogInformation("Profile update attempt for user ID: {UserId}",id);

            var user = await _repo.GetUserAsync(id);

            if (user == null)
            {
                _logger.LogWarning("Profile update failed. User not found: {UserId}",id);
                return null;
            }

            user.Name = userUpdateDTO.Name;
            user.UpdatedAt = DateTime.UtcNow;

            var updatedUser = await _repo.UpdateUserAsync(user);

            _logger.LogInformation(
                "Profile updated successfully for user ID: {UserId}",
                id);

            return updatedUser;
        }
    }
}
