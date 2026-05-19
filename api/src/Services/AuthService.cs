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
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAuthRepository repo, IMapper mapper, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _repo.IsEmailExistsAsync(email);
        }

        public async Task<UserDTO?> RegisterAsync(RegisterRequestDTO registerRequestDTO)
        {
            try
            {
                _logger.LogInformation("Register attempt for email: {Email}",registerRequestDTO.Email);

                if (await _repo.IsEmailExistsAsync(registerRequestDTO.Email))
                {
                    _logger.LogWarning("Registration failed. Email already exists: {Email}",registerRequestDTO.Email);
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

                _logger.LogInformation("User registered successfully: {Email}",createdUser.Email);

                return _mapper.Map<UserDTO>(createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError( ex,"Error occurred during user registration");

                throw new InvalidOperationException(
                    "An unexpected error occured during user registration", ex);
            }
        }

        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginrequestDTO)
        {
            try
            {
                _logger.LogInformation("Login attempt for email: {Email}",loginrequestDTO.Email);

                var user = await _repo.GetUserByEmailAsync(loginrequestDTO.Email);

                if (user == null)
                {
                    _logger.LogWarning("Login failed. User not found: {Email}",loginrequestDTO.Email);
                    return null;
                }

                bool isPasswordValid =
                    BCrypt.Net.BCrypt.Verify(loginrequestDTO.Password, user.Password);

                if (!isPasswordValid)
                {
                    _logger.LogWarning("Login failed. Invalid password for email: {Email}",loginrequestDTO.Email);
                    return null;
                }

                var token = GenerateJwtToken(user);
                _logger.LogInformation("User logged in successfully: {Email}",user.Email);

                return new LoginResponseDTO
                {
                    Token = token,
                    UserDTO = _mapper.Map<UserDTO>(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error occurred during login");
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
