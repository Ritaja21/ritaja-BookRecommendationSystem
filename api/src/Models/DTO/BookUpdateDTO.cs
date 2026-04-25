using System.ComponentModel.DataAnnotations;

namespace api.src.Models.DTO
{
    public class BookUpdateDTO
    {
        [Required]
        public required int Id { get; set; }
        [Required]

        public required string Title { get; set; }
        [Required]
        public required string Author { get; set; }

        public string Genre { get; set; }

        public string Description { get; set; }
    }
}
