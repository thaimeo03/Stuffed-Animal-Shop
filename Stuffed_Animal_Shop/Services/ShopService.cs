using Stuffed_Animal_Shop.Data;
using Stuffed_Animal_Shop.Models;

namespace Stuffed_Animal_Shop.Services
{
    public class ProductResult
    {
        public List<Product> Products { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }

    public class ShopService
    {
        private readonly ApplicationDbContext _context;

        public ShopService(ApplicationDbContext context)
        {
            this._context = context;
        }

        public int GetTotalProducts()
        {
            return _context.Products.Count();
        }

        public ProductResult GetProducts(int page, int pageSize)
        {
            int totalProducts = this.GetTotalProducts();
            int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            var products = _context.Products.Skip((page - 1) * pageSize).Take(pageSize).ToList();


            return new ProductResult
            {
                Products = products,
                TotalPages = totalPages,
                CurrentPage = page
            };
        }

    }
}
