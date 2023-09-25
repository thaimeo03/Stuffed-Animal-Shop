using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stuffed_Animal_Shop.Models
{
    public class Product
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ProductId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        [MinLength(1)]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int Price { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(10)")]
        public string Size { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        public string Color { get; set; } = "";

        [Required]
        [Column(TypeName = "int")]
        public int Quantity { get; set; }

        [Column(TypeName = "int")]
        public int Sold { get; set; } = 0;

        [Required]
        [Column(TypeName = "nvarchar(300)")]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string MainImage { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
