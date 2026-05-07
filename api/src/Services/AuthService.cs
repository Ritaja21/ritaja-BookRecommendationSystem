using api.src.Models;
using api.src.Models.DTO;
using api.src.Repositories;
using AutoMapper;

namespace api.src.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repo;
        private readonly IMapper _mapper;

        public AuthService(IAuthRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _repo.IsEmailExistsAsync(email);
        }

        public async Task<UserDTO?> RegisterAsync(RegisterRequestDTO registerRequestDTO)
        {
            try
            {
                if (await _repo.IsEmailExistsAsync(registerRequestDTO.Email))
                {
                    throw new InvalidOperationException(
                        $"User with email '{registerRequestDTO.Email}' already exists");
                }

                User user = new()
                {
                    Name = registerRequestDTO.Name,

                    Email = registerRequestDTO.Email,

                    Password = BCrypt.Net.BCrypt.HashPassword(
                        registerRequestDTO.Password),

                    Role = "Customer",

                    CreatedAt = DateTime.UtcNow
                };

                var createdUser = await _repo.RegisterAsync(user);

                return _mapper.Map<UserDTO>(createdUser);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "An unexpected error occured during user registration", ex);
            }
        }

    }
}
