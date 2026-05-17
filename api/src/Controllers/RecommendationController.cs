using api.src.Models.DTO;
using api.src.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.src.Controllers
{
    [ApiController]
    [Route("api/recommendation")]
    [Authorize(Roles = "Customer")]
    public class RecommendationController : ControllerBase
    {
        private readonly IRecommendationService _service;

        public RecommendationController(IRecommendationService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<RecommendationResponseDTO>>> GetRecommendation(RecommendationRequestDTO request)
        {
            try
            {
                var recommendations = await _service.GetRecommendationAsync(request);
                return Ok(ApiResponse<object>.Ok("Recommendations fetched successfully", recommendations));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error(500, "Failed to fetch recommendations", ex.Message));
            }
        }
    }
}
