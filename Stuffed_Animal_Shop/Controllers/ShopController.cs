using Bogus.DataSets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Stuffed_Animal_Shop.Data;
using Stuffed_Animal_Shop.Models;
using Stuffed_Animal_Shop.Services;
using Stuffed_Animal_Shop.ViewModels.Filters;
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

        [Route("/shop")]
        public async Task<IActionResult> Index(Filter filter)
        {
            var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            var user = this._userService.GetUserByEmail(userEmail);

            if (user != null)
            {
                ViewBag.User = user;
            }

            ProductResult productResult = this._shopService.GetProductsByFilter(filter);

            List<Product> productsFiltered = productResult.Products;
            ViewBag.TotalPages = productResult.TotalPages;
            ViewBag.Page = productResult.CurrentPage;

            return productsFiltered != null ?
                        View(productsFiltered) :
                        Problem("Entity set 'ApplicationDbContext.Products'  is null.");
        }


        [Route("/shop/sort")]
        [HttpPost("/shop/sort")]
        public async Task<IActionResult> Sort(string sort)
        {
            string queryString = HttpContext.Request.QueryString.ToString();
            Console.WriteLine(queryString);
            List<Product> products = new List<Product>();

            if(sort.Equals("latest"))
            {
                products = await _context.Products.OrderBy(s => s.CreatedAt).ToListAsync();
            }else if (sort.Equals("best_rating"))
            {
                products = await _context.Reviews.OrderBy(r => r.Rating).Select(r => r.Product).ToListAsync();
            }else if (sort.Equals("cheap"))
            {
                products = await _context.Products.OrderBy(s => s.Price).ToListAsync();
            }else if (sort.Equals("expensive"))
            {
                products = await _context.Products.OrderByDescending(s => s.Price).ToListAsync();
            }

            return products != null ?
                        View("Index", products) :
                        Problem("Entity set 'ApplicationDbContext.Products'  is null.");
        }

        [HttpPost]
        public IActionResult CreateReview(IFormCollection form)
        {

            string selectedContext = "";
            string selectedEmail = "";
            string selectedRatting = "";
            string selectedProductID = "";

            foreach (var key in form.Keys)
            {
                if (key.StartsWith("context"))
                {
                    selectedContext = form[key].ToString();
                }
                else if (key.StartsWith("email"))
                {
                    selectedEmail = form[key];
                }
                else if (key.StartsWith("ratting"))
                {
                    selectedRatting = form[key];
                }
                else if (key.StartsWith("productID"))
                {
                    selectedProductID = form[key];
                }
            }
            DateTime d = DateTime.Now;
            Product product = _context.Products.Where(p => p.ProductId == Guid.Parse(selectedProductID)).FirstOrDefault();
            List<Review> reviews = this._context.Reviews.Where(r => r.Product == product).ToList();

            try
            {
                Review review = new Review()
                {
                    EmailUser = selectedEmail,
                    Comment = selectedContext,
                    Rating = int.Parse(selectedRatting),
                    CreatedAt = d,
                    Product = product,
                };

                this._context.Reviews.Add(review);
                this._context.SaveChanges();
                ViewBag.reviewCnt = reviews.Count + 1;
            }catch (Exception ex)
            {
                ViewBag.reviewCnt = reviews.Count;
            }

            return RedirectToAction("Detail", product);
        }

        [Route("/shop/detail/{productId}")]
        public IActionResult Detail([FromRoute] string productId)
        {

            var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            var user = this._userService.GetUserByEmail(userEmail);

            var product = _context.Products.FirstOrDefault(p => p.ProductId == Guid.Parse(productId));
            List<Review> reviews = this._context.Reviews.Where(r => r.Product == product).ToList();

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
