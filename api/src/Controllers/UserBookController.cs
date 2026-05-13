using api.src.Models.DTO;
using api.src.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace api.src.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize(Roles = "Customer")]

    public class UserBookController : ControllerBase
    {
        private readonly IUserBookService _service;
        private readonly IMapper _mapper;

        public UserBookController (IUserBookService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpPost("read")]
        public async Task<ActionResult<ApiResponse<object>>> MarkBookAsRead(UserReadDTO userReadDTO)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (UserIdClaim == null)
            {
                return Unauthorized(ApiResponse<object>.Error(401, "Unauthorized Access"));
            }

            int userId = int.Parse(UserIdClaim);

            var result = await _service.MarkBookAsReadAsync(userId, userReadDTO);

            return Ok(ApiResponse<object>.Ok("Booked marked as read", result));
        }

    }
}
