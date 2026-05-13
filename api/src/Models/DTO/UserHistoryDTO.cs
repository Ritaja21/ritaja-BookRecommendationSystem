namespace api.src.Models.DTO
{
    public class UserHistoryDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int? Rating {  get; set; }
        public bool IsRead { get; set; }
    }
}
