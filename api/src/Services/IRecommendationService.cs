using api.src.Models.DTO;

namespace api.src.Services
{
    public interface IRecommendationService
    {
        Task<RecommendationResponseDTO> GetRecommendationAsync(RecommendationRequestDTO recommendationRequestDTO);
    }
}
