using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Stuffed_Animal_Shop.Models
{
    public class CartItem
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        [ForeignKey("Product")]
        public Guid CartItemId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        [MinLength(1)]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int Count { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(10)")]
        public string Size { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(10)")]
        public string Color { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string Image { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public decimal ItemPrice { get; set; }

        public Cart Cart { get; set; }

        public virtual Product Product { get; set; }
    }
}
