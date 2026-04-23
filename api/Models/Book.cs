using Microsoft.EntityFrameworkCore.Storage;
using System.ComponentModel.DataAnnotations;
namespace api.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        [Required]
        public required string Title { get; set; }
        [Required]
        public required string Author { get; set; }
       
        public string? Genre { get; set; }
        public string? Description { get; set; }


    }
}
