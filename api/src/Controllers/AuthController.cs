using api.src.Models.DTO;
using api.src.Services;
using Microsoft.AspNetCore.Mvc;

namespace api.src.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<UserDTO>>> Register(
           RegisterRequestDTO registerRequestDTO)
        {
            try
            {
                if (registerRequestDTO == null)
                {
                    return BadRequest(
                        ApiResponse<object>.BadRequest(
                            "Registration data is required"));
                }

                var user = await _authService.RegisterAsync(registerRequestDTO);

                if (user == null)
                {
                    return BadRequest(
                        ApiResponse<object>.BadRequest(
                            "Registration failed"));
                }

                var response = ApiResponse<UserDTO>.CreatedAt(
                    "User registered successfully",
                    user);

                return StatusCode(201, response);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(
                    ApiResponse<object>.Conflict(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    ApiResponse<object>.Error(
                        500,
                        "An error occured while registering user",
                        ex.Message));
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponseDTO>>> Login(
    LoginRequestDTO loginrequestDTO)
        {
            try
            {
                if (loginrequestDTO == null)
                {
                    return BadRequest(
                        ApiResponse<object>.BadRequest(
                            "Login data is required"));
                }

                var loginResponse =
                    await _authService.LoginAsync(loginrequestDTO);

                if (loginResponse == null)
                {
                    return Unauthorized(
                        ApiResponse<object>.Error(
                            401,
                            "Invalid email or password"));
                }

                var response =
                    ApiResponse<LoginResponseDTO>.Ok(
                        "Login successful",
                        loginResponse);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    ApiResponse<object>.Error(
                        500,
                        "An error occured during login",
                        ex.Message));
            }
        }

    }
}
