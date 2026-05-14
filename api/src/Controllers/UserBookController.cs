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
            try
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
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    ApiResponse<object>.Error(
                        500,
                        "Error marking book as read",
                        ex.Message));
            }

        }

        [HttpPost("rate")]
        public async Task<ActionResult<ApiResponse<object>>> RateBookAsync(RateBookDTO rateBookDTO)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    return Unauthorized(
                       ApiResponse<object>.Error(
                           401,
                           "Unauthorized"));
                }
                int userId = int.Parse(userIdClaim);

                var result = await _service.RateBookAsync(userId, rateBookDTO);

                return Ok(
                   ApiResponse<object>.Ok(
                       "Book rated successfully",
                       result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, "Failed to rate book", ex.Message));
            }
        }

        [HttpGet("history")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserHistoryDTO>>>> GetHistory()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userIdClaim == null)
                {
                    return Unauthorized(
                        ApiResponse<object>.Error(
                            401,
                            "Unauthorized"));
                }

                int userId = int.Parse(userIdClaim);

                var history = await _service.GetUserBookHistoryAsync(userId);

                var historyDTOs = _mapper.Map<List<UserHistoryDTO>>(history);

                return Ok(
                    ApiResponse<IEnumerable<UserHistoryDTO>>.Ok(
                        "User history fetched successfully",
                        historyDTOs));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    ApiResponse<object>.Error(
                        500,
                        "Error fetching user history",
                        ex.Message));
            }
        }
    
}
}
