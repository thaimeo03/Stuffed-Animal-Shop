using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Stuffed_Animal_Shop.Models
{
    public class Review
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ReviewId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        [EmailAddress]
        public string EmailUser { get; set; }

        [Column(TypeName = "int")]
        public int ?Rating { get; set; } = null;

        [Column(TypeName = "nvarchar(100)")]
        public string ?Comment { get; set; } = null;

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Product Product { get; set; }
    }
}
