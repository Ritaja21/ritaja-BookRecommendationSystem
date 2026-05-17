using Microsoft.Identity.Client;

namespace api.src.Models.DTO
{
    public class RecommendationResponseDTO
    {
        public List<BookDTO> InternalRecommendations { get; set; } = new();
        public List<RecommendationBookDTO> ExternalRecommendations { get; set; } = new();
    }
}
