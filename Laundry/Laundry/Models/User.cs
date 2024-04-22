using System.ComponentModel.DataAnnotations;

namespace Laundry.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required, StringLength(130)]
        public string Name { get; set; }
        [Required, StringLength(130)]
        public string Email { get; set; }
        [DataType(DataType.Password), Required, StringLength(30)]
        public string Password { get; set; }
        [StringLength(150)]
        public string? Role { get; set; }
    }
}
