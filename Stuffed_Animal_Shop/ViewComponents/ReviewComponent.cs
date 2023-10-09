namespace Stuffed_Animal_Shop.ViewComponents
{
    using Microsoft.AspNetCore.Mvc;
    using Stuffed_Animal_Shop.Data;
    using Stuffed_Animal_Shop.Services;

    public class ReviewViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public ReviewViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke(Guid productId)
        {
            Console.WriteLine(productId);
            var reviews = _context.Reviews.Where(r => r.Product.ProductId == productId).ToList();
            return View("Review", reviews);
        }
    }
}
