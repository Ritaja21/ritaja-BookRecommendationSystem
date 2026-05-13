using System.ComponentModel.DataAnnotations;

namespace api.src.Models.DTO
{
    public class RateBookDTO
    {
        //rating book
        [Required]
        public int BookId { get; set; }
        [Range(1,5)]
        public int Rating { get; set; }
    }
}
