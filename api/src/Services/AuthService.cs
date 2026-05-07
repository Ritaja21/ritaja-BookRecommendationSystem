using api.src.Models;
using api.src.Models.DTO;
using api.src.Repositories;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api.src.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(IAuthRepository repo, IMapper mapper, IConfiguration configuration)
        {
            _repo = repo;
            _mapper = mapper;
            _configuration = configuration;
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

        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginrequestDTO)
        {
            try
            {
                var user = await _repo.GetUserByEmailAsync(loginrequestDTO.Email);

                if (user == null)
                {
                    return null;
                }

                bool isPasswordValid =
                    BCrypt.Net.BCrypt.Verify(loginrequestDTO.Password, user.Password);

                if (!isPasswordValid)
                {
                    return null;
                }

                var token = GenerateJwtToken(user);

                return new LoginResponseDTO
                {
                    Token = token,
                    UserDTO = _mapper.Map<UserDTO>(user)
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "An unexpected error occured during login", ex);
            }
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(
                _configuration["JwtSettings:Secret"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

            new Claim(ClaimTypes.Name, user.Name),

            new Claim(ClaimTypes.Email, user.Email),

            new Claim(ClaimTypes.Role, user.Role)
        }),

                Expires = DateTime.UtcNow.AddDays(1),

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }
}
