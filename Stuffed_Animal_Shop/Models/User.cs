using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stuffed_Animal_Shop.Models
{
    public class User
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Password { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        [MinLength(1)]
        public string Name { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string Role { get; set; } = "User";

        [Column(TypeName = "nvarchar(100)")]
        public string Address { get; set; } = "";

        [Column(TypeName = "nvarchar(12)")]
        [MinLength(10)]
        public string PhoneNumber { get; set; } = "";

        [Column(TypeName = "varchar(300)")]
        public string Avatar { get; set; } = "";

        [Column(TypeName = "")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public virtual Cart Cart { get; set; }
    }
}
