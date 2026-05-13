using System.ComponentModel.DataAnnotations;

namespace api.src.Models.DTO
{
    public class UserReadDTO
    {
        // marking read by user
        [Required]
        public int BookId { get; set; }
    }
}
