using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stuffed_Animal_Shop.Data;
using Stuffed_Animal_Shop.Models;
using Stuffed_Animal_Shop.Services;
using System.Diagnostics;
using System.Security.Claims;

namespace Stuffed_Animal_Shop.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
            _userService = new UserService(context);
        }

        public async Task<IActionResult> Index()
        {
            var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            var user = this._userService.GetUserByEmail(userEmail);

            if (user != null)
            {
                ViewBag.User = user;
            }

            return _context.Products != null ?
                        View(await _context.Products.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Products'  is null.");
        }

        [Route("/shop/detail/{productId}")]
        public IActionResult Detail([FromRoute] string productId)
        {
            var product = _context.Products.Where(p => p.ProductId == Guid.Parse(productId)).FirstOrDefault();
            var reviews = _context.Reviews.Where(r => r.Product.ProductId == Guid.Parse(productId)).ToList();

            return View("Detail", product);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
