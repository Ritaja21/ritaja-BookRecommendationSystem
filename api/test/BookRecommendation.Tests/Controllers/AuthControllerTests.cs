using api.src.Controllers;
using api.src.Models;
using api.src.Models.DTO;
using api.src.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookRecommendation.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;

        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();

            _authController = new AuthController(
                _mockAuthService.Object
            );
        }
        [Fact]
        public async Task Register_ShouldReturn201_WhenRegistrationSucceeds()
        {
            // Arrange
            var registerDto = new RegisterRequestDTO
            {
                Name = "Test",
                Email = "test@example.com",
                Password = "Password123"
            };

            var userDto = new UserDTO
            {
                Id = 1,
                Name = "Test",
                Email = "test@example.com"
            };

            _mockAuthService
                .Setup(service => service.RegisterAsync(registerDto))
                .ReturnsAsync(userDto);

            // Act
            var result = await _authController.Register(registerDto);

            // Assert
            var createdResult = result.Result as ObjectResult;

            createdResult.Should().NotBeNull();

            createdResult!.StatusCode.Should().Be(201);
        }
        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = await _authController.Register(null!);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;

            badRequestResult.Should().NotBeNull();

            badRequestResult!.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenLoginSucceeds()
        {
            // Arrange
            var loginDto = new LoginRequestDTO
            {
                Email = "test@example.com",
                Password = "Password123"
            };

            var loginResponse = new LoginResponseDTO
            {
                Token = "fake-jwt-token",
                UserDTO = new UserDTO
                {
                    Id = 1,
                    Name = "Test",
                    Email = "test@example.com"
                }
            };

            _mockAuthService
                .Setup(service => service.LoginAsync(loginDto))
                .ReturnsAsync(loginResponse);

            // Act
            var result = await _authController.Login(loginDto);

            // Assert
            var okResult = result.Result as OkObjectResult;

            okResult.Should().NotBeNull();

            okResult!.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginDto = new LoginRequestDTO
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            _mockAuthService
                .Setup(service => service.LoginAsync(loginDto))
                .ReturnsAsync((LoginResponseDTO?)null);

            // Act
            var result = await _authController.Login(loginDto);

            // Assert
            var unauthorizedResult =
                result.Result as UnauthorizedObjectResult;

            unauthorizedResult.Should().NotBeNull();

            unauthorizedResult!.StatusCode.Should().Be(401);
        }
    }
}