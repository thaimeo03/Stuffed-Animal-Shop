using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stuffed_Animal_Shop.Data;
using Stuffed_Animal_Shop.Models;
using Stuffed_Animal_Shop.Services;
using Stuffed_Animal_Shop.ViewModels.Shops;
using System.Diagnostics;
using System.Security.Claims;

namespace Stuffed_Animal_Shop.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
            _userService = new UserService(context);
        }
        public IActionResult Index()
        {
            var userEmail = this.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            var user = this._userService.GetUserByEmail(userEmail);

            if (user != null)
            {
                ViewBag.User = user;

                List<CartItem> cartItems = _context.CartItems.Where(c => c.Cart.User == user).ToList();

                return View(cartItems);
            }

            return View();
        }

        [HttpPost]
        [Route("/cart/add-to-cart/{productId}")]
        public async Task<IActionResult> AddToCart([FromRoute] Guid productId, AddToCart addToCart)
        {
            Console.WriteLine(addToCart.SizeItem + ' ' + addToCart.ColorItem + ' ' + addToCart.Count);
            if (ModelState.IsValid)
            {
                var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
                var user = this._userService.GetUserByEmail(userEmail);
                var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
                var currentCart = _context.Carts.FirstOrDefault(c => c.User == user);

                var cartItem = new CartItem()
                {
                    Cart = currentCart,
                    Product = product,
                    Name = product.Name,
                    Count = int.Parse(addToCart.Count),
                    Size = addToCart.SizeItem,
                    Color = addToCart.ColorItem,
                    ItemPrice = product.Price,
                    Image = product.MainImage
                };

                _context.CartItems.Add(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("/cart/delete-cartItem/{cartItemId}")]
        public async Task<IActionResult> DeleteCartItem([FromRoute] Guid cartItemId)
        {
            var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            var user = this._userService.GetUserByEmail(userEmail);

            var cartItem = _context.CartItems.FirstOrDefault(c => c.Cart.User == user && c.CartItemId == cartItemId);

            if( cartItem != null) {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            var user = this._userService.GetUserByEmail(userEmail);

            var cartItems = _context.CartItems.Where(c => c.Cart.User == user).ToList();

            // Move cartItems to orderItems (get current cart) and delete cartItems in current cart
            if (cartItems.Count() > 0)
            {
                var orderItems = new List<OrderItem>();
                Order currentOrder = new Order()
                {
                    User = user,
                };
                foreach (var cartItem in cartItems)
                {

                    Product product = _context.Products.FirstOrDefault(p => p.CartItem == cartItem);

                    OrderItem orderItem = new OrderItem()
                    {
                        Name = cartItem.Name,
                        ItemPrice = cartItem.ItemPrice,
                        Count = cartItem.Count,
                        Color = cartItem.Color,
                        Size = cartItem.Size,
                        Image = cartItem.Image,
                        Order = currentOrder,
                        ProductId = product.ProductId
                    };
                    orderItems.Add(orderItem);
                }

                _context.CartItems.RemoveRange(cartItems);
                _context.Orders.Add(currentOrder);
                _context.OrderItems.AddRange(orderItems);
                await _context.SaveChangesAsync();
            }


            return RedirectToAction("Index"); ;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
