using System.ComponentModel.DataAnnotations;

namespace api.src.Models.DTO
{
    public class BookDTO
    {
       
        public int BookId { get; set; }
       
        public required string Title { get; set; }
      
        public required string Author { get; set; }

        public string? Genre { get; set; }
        public string? Description { get; set; }

    }
}
