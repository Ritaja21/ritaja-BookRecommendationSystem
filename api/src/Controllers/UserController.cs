using api.src.Models;
using api.src.Models.DTO;
using api.src.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace api.src.Controllers
{
    [ApiController]
    [Route("/api/user")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IMapper _mapper;

        public UserController(IUserService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponse<UserDTO>>> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(
                    ApiResponse<object>.Error(
                        401,
                        "Invalid Token"));
            }

            var user = await _service.GetUserAsync(int.Parse(userIdClaim));

            if (user == null)
            {
                return NotFound(
                    ApiResponse<object>.NotFound(
                        "User not found"));
            }

            var userDTO = _mapper.Map<UserDTO>(user);

            return Ok(
                ApiResponse<UserDTO>.Ok(
                    "Profile fetched successfully",
                    userDTO));
        }

        [HttpPatch("profile")]
        public async Task<ActionResult<ApiResponse<UserDTO>>> UpdateProfile(
    UserUpdateDTO userUpdateDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(
                    ApiResponse<object>.Error(401, "Invalid token"));
            }

            var updatedUser = await _service.UpdateProfileAsync(
                int.Parse(userIdClaim),
                userUpdateDTO);

            if (updatedUser == null)
            {
                return NotFound(
                    ApiResponse<object>.NotFound("User not found"));
            }

            var userDTO = _mapper.Map<UserDTO>(updatedUser);

            return Ok(
                ApiResponse<UserDTO>.Ok(
                    "Profile updated successfully",
                    userDTO));
        }
    }
}
