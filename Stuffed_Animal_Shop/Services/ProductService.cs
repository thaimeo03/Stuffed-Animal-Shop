using Microsoft.CodeAnalysis;
using Stuffed_Animal_Shop.Data;
using Stuffed_Animal_Shop.Models;

namespace Stuffed_Animal_Shop.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;
        public ProductService(ApplicationDbContext context)
        {
            this._context = context;
        }

        public List<Image> GetImages(string productId)
        {
            return this._context.Images.Where(i => i.Product.ProductId.ToString() == productId).ToList();
        }
    }
}
