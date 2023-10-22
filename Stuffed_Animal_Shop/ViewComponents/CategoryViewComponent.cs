using Microsoft.AspNetCore.Mvc;
using Stuffed_Animal_Shop.Data;
using Stuffed_Animal_Shop.Models;
using Stuffed_Animal_Shop.ViewModels.Filters;

namespace Stuffed_Animal_Shop.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CategoryViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            Filter filter = new Filter();
            List<Category> categories = _context.Categories.ToList();

            ViewBag.Filter = filter;

            return View("Category", categories);
        }
    }
}
