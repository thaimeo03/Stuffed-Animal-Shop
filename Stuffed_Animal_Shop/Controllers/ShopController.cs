using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stuffed_Animal_Shop.Data;
using Stuffed_Animal_Shop.Models;
using Stuffed_Animal_Shop.Services;
using Stuffed_Animal_Shop.ViewModels.Shops;
using System.Diagnostics;
using System.Security.Claims;

namespace Stuffed_Animal_Shop.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;
        private readonly ShopService _shopService;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
            _userService = new UserService(context);
            _shopService = new ShopService(context);
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 21)
        {
            var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            var user = this._userService.GetUserByEmail(userEmail);

            if (user != null)
            {
                ViewBag.User = user;
            }


            ProductResult productResult = this._shopService.GetProducts(page, pageSize);

            var products = productResult.Products;
            ViewBag.TotalPages = productResult.TotalPages;
            ViewBag.Page = productResult.CurrentPage;

            return products != null ?
                        View(products) :
                        Problem("Entity set 'ApplicationDbContext.Products'  is null.");
        }

        [Route("/shop/detail/{productId}")]
        public IActionResult Detail([FromRoute] string productId)
        {
            var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            var user = this._userService.GetUserByEmail(userEmail);

            var product = _context.Products.FirstOrDefault(p => p.ProductId == Guid.Parse(productId));

            //var colors = _context.Colors.Where(c => c.Product == product).ToList();

            //var sizes = _context.Sizes.Where(s => s.Product == product).ToList();

            var images = _context.Images.Where(i => i.Product == product).ToList();

            if (user != null)
            {
                ViewBag.User = user;
            }

            //ViewBag.Colors = colors;
            //ViewBag.Sizes = sizes;
            ViewBag.Images = images;

            return View("Detail", product);
        }

            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
