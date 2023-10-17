using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stuffed_Animal_Shop.Models
{
    public class Color
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ColorId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(10)")]
        public string Name { get; set; }

        public Product Product { get; set; }
    }
}
