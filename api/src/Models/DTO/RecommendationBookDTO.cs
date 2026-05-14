namespace api.src.Models.DTO
{
    public class RecommendationBookDTO
    {
        public string Title { get; set; } = string.Empty;
        public string? Author {  get; set; } = string.Empty;
        public string? Genre { get; set; }
        public bool IsInternal { get; set; }
        public string? SearchUrl { get; set; }

    }
}
