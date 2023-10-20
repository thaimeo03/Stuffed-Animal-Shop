using Bogus.DataSets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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


        [Route("/shop/sort")]
        [HttpPost("/shop/sort")]
        public async Task<IActionResult> Index(string sort)
        {
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
                        View(products) :
                        Problem("Entity set 'ApplicationDbContext.Products'  is null.");
        }

        [HttpPost]
        public async Task<IActionResult> Filter(IFormCollection form)
        {
            List<Product> products = await _context.Products.ToListAsync();
            List<Color> cls = await _context.Colors.ToListAsync();

            foreach (Color color in cls)
            {
                Console.WriteLine(color.Name.ToString());
            }
            // Khởi tạo mảng để lưu trữ các giá trị được chọn

            List<Product> productFilter = new List<Product>();
            List<Product> productFilterPrices = new List<Product>();
            List<Product> productFilterColors = new List<Product>();
            List<Product> productFilterSizes = new List<Product>();

            string selectedPrices = "";
            string selectedColors = "";
            string selectedSizes = "";

            // Lấy giá trị của các checkbox từ form và thêm vào mảng tương ứng
            foreach (var key in form.Keys)
            {
                if (key.StartsWith("prices"))
                {
                    selectedPrices = form[key].ToString();
                }
                else if (key.StartsWith("colos"))
                {
                    selectedColors = form[key];
                }
                else if (key.StartsWith("sizes"))
                {
                    selectedSizes = form[key];
                }
            }

            List<string> prices = selectedPrices.Split(",").ToList();
            foreach (string price in prices)
            {
                if (price.Equals("all"))
                {
                    productFilterPrices = products;
                    break;
                }
                else
                {
                    string[] pr = price.Split('-');
                    int first_price = int.Parse(pr[0]);
                    int last_price = int.Parse(pr[pr.Length - 1]);
                    List<Product> listTMP = await _context.Products.Where(p => p.Price >= first_price && p.Price <= last_price).ToListAsync();
                    productFilterPrices.AddRange(listTMP);
                }
            }

            List<string> colors = selectedColors.Split(",").ToList();
            Console.WriteLine(colors.First());

            if (colors.First().Equals("all") && colors != null)
            {
                productFilterColors = products;
            }
            else
            {
                productFilterColors = await _context.Colors
                    .Where(c => colors.Contains(c.Name))
                    .Select(c => c.Product)
                    .Where(p => p != null)
                    .Distinct()
                    .ToListAsync();
            }
            Console.WriteLine(productFilterColors.Count);


            List<string> sizes = selectedSizes.Split(",").ToList();

            if (sizes[0].Equals("all") && sizes != null)
            {
                productFilterSizes = products;

            }
            else
            {
                productFilterSizes = await _context.Sizes
                    .Where(s => sizes.Contains(s.Name))
                    .Select(s => s.Product)
                    .Where(p => p != null)
                    .Distinct()
                    .ToListAsync();
            }

            productFilter = productFilterPrices.Intersect(productFilterSizes).Intersect(productFilterColors).ToList();

            return _context.Products != null ?
                        View("Index", productFilter) :
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

            if (user != null)
            {
                ViewBag.User = user;
            }

            ViewBag.reviewCnt = reviews.Count;
            ViewBag.user = user;

            return View("Detail", product);
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
