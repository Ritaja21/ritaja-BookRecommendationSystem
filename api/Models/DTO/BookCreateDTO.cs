
using System.ComponentModel.DataAnnotations;

namespace api.Models.DTO
{
    public class BookCreateDTO
    {
        [Required]

        public required string Title { get; set; }
        [Required]
        public required string Author { get; set; }

        public string? Genre { get; set; }

        public string? Description { get; set; }

    }
}
