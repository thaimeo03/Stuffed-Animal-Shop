using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stuffed_Animal_Shop.Models
{
    public class Cart
    {

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid CartId { get; set; }

        public User User { get; set; }
        public virtual Order Order { get; set; }
    }
}