using System.ComponentModel.DataAnnotations;

namespace Stuffed_Animal_Shop.ViewModels.Shops
{
    public class Search
    {
        [Required]
        public string ProductName { get; set; }
    }
}
