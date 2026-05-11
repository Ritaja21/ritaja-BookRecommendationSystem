using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.src.Models
{
    public class UserBook
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int BookId { get; set; }
        [Range(1,5)]
        public int? Rating { get; set; }
        public bool IsRead { get; set; } = false;

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [ForeignKey("BookId")]
        public Book Book { get; set; } = null!;
    }
}
