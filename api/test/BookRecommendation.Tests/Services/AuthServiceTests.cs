using api.src.Models.DTO;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BookRecommendation.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepository> _mockAuthRepo;

        private readonly Mock<IMapper> _mockMapper;

        private readonly Mock<IConfiguration> _mockConfiguration;

        private readonly Mock<ILogger<AuthService>> _mockLogger;

        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockAuthRepo = new Mock<IAuthRepository>();

            _mockMapper = new Mock<IMapper>();

            _mockConfiguration = new Mock<IConfiguration>();

            _mockLogger = new Mock<ILogger<AuthService>>();

            _mockConfiguration
                .Setup(config => config["JwtSettings:Secret"])
                .Returns("ThisIsMyVerySecretKey12345678901");

            _authService = new AuthService(
                _mockAuthRepo.Object,
                _mockMapper.Object,
                _mockConfiguration.Object,
                _mockLogger.Object
            );
        }
        [Fact]
        public async Task RegisterAsync_ShouldRegisterUser_WhenEmailDoesNotExist()
        {
            // Arrange
            var registerDto = new RegisterRequestDTO
            {
                Name = "Test",
                Email = "test@example.com",
                Password = "Password123"
            };

            var createdUser = new User
            {
                Id = 1,
                Name = "Test",
                Email = "test@example.com",
                Password = "hashedpassword",
                Role = "Customer"
            };

            var userDto = new UserDTO
            {
                Id = 1,
                Name = "Test",
                Email = "test@example.com"
            };

            _mockAuthRepo
                .Setup(repo => repo.IsEmailExistsAsync(registerDto.Email))
                .ReturnsAsync(false);

            _mockAuthRepo
                .Setup(repo => repo.RegisterAsync(It.IsAny<User>()))
                .ReturnsAsync(createdUser);

            _mockMapper
                .Setup(mapper => mapper.Map<UserDTO>(createdUser))
                .Returns(userDto);

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            result.Should().NotBeNull();

            result.Email.Should().Be("test@example.com");

            result.Name.Should().Be("Test");
        }
        [Fact]
        public async Task RegisterAsync_ShouldThrowException_WhenEmailAlreadyExists()
        {
            // Arrange
            var registerDto = new RegisterRequestDTO
            {
                Name = "Test",
                Email = "test@example.com",
                Password = "Password123"
            };

            _mockAuthRepo
                .Setup(repo => repo.IsEmailExistsAsync(registerDto.Email))
                .ReturnsAsync(true);

            // Act
            Func<Task> act = async () =>
                await _authService.RegisterAsync(registerDto);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage($"User with email '{registerDto.Email}' already exists");
        }
        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreCorrect()
        {
            // Arrange
            var loginDto = new LoginRequestDTO
            {
                Email = "test@example.com",
                Password = "Password123"
            };

            var user = new User
            {
                Id = 1,
                Name = "Test",
                Email = "test@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Password123"),
                Role = "Customer"
            };

            var userDto = new UserDTO
            {
                Id = 1,
                Name = "Test",
                Email = "test@example.com"
            };

            _mockAuthRepo
                .Setup(repo => repo.GetUserByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            _mockMapper
                .Setup(mapper => mapper.Map<UserDTO>(user))
                .Returns(userDto);

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            result.Should().NotBeNull();

            result.Token.Should().NotBeNullOrEmpty();

            result.UserDTO.Email.Should().Be(loginDto.Email);
        }
        [Fact]
        public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsIncorrect()
        {
            // Arrange
            var loginDto = new LoginRequestDTO
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            var user = new User
            {
                Id = 1,
                Name = "Test",
                Email = "test@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("CorrectPassword"),
                Role = "Customer"
            };

            _mockAuthRepo
                .Setup(repo => repo.GetUserByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            result.Should().BeNull();
        }
    }
}