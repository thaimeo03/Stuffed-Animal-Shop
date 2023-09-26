using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stuffed_Animal_Shop.Models
{
    public class Cart
    {
        public Cart ()
        {
            this.Products = new HashSet<Product>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        [ForeignKey("User")]
        public Guid CartId { get; set; }

        public virtual User User { get; set; }
        public virtual Order Order { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}