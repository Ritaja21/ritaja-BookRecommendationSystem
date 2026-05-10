using System.ComponentModel.DataAnnotations;

namespace api.src.Models.DTO
{
    public class UserUpdateDTO
    {
        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }

    }
}
