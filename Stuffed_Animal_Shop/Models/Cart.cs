using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stuffed_Animal_Shop.Models
{
    public class Cart
    {
        [Key]
        [Column(TypeName = "varchar(36)")]
        public string CartId { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
