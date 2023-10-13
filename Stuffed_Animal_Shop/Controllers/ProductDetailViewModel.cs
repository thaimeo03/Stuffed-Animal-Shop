using Stuffed_Animal_Shop.Models;

namespace Stuffed_Animal_Shop.Controllers
{
    internal class ProductDetailViewModel
    {
        public Product Product { get; set; }
        public List<Review> Reviews { get; set; }
    }
}