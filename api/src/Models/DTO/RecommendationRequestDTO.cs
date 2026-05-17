namespace api.src.Models.DTO
{
    public class RecommendationRequestDTO
    {
        public string? Prompt { get; set; }
        public string? Genre { get; set; }
        public double? MinimumRating { get; set; }
    }
}
