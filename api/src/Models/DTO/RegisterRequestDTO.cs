using System.ComponentModel.DataAnnotations;

namespace api.src.Models.DTO
{
    public class RegisterRequestDTO
    {
        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
