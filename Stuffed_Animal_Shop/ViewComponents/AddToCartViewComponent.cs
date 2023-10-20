using Microsoft.AspNetCore.Mvc;
using Stuffed_Animal_Shop.Data;
using Stuffed_Animal_Shop.Models;

namespace Stuffed_Animal_Shop.ViewComponents
{
    public class AddToCartViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public AddToCartViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke(Guid productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);

            var colors = _context.Colors.Where(c => c.Product == product).ToList();

            var sizes = _context.Sizes.Where(s => s.Product == product).ToList();

            var images = _context.Images.Where(i => i.Product == product).ToList();

            ViewBag.Product = product;
            ViewBag.Colors = colors;
            ViewBag.Sizes = sizes;

            return View("AddToCart");
        }
    }
}
