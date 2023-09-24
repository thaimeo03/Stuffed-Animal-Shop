using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stuffed_Animal_Shop.Data;
using Stuffed_Animal_Shop.ViewModels;
using Stuffed_Animal_Shop.Models;
using Stuffed_Animal_Shop.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Stuffed_Animal_Shop.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
            _userService = new UserService(context);
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
              return _context.Users != null ? 
                          View(await _context.Users.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Users'  is null.");
        }

        // GET: Users/Create
        public IActionResult Create()
        {   
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new UserRegister());
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserRegister userRegister)
        {
            if(!ModelState.IsValid)
            {
                return View(userRegister);
            }

            if(_userService.UserExsisted(userRegister.Email))
            {
                TempData["Error"] = "This email address is already in use";
                return View(userRegister);
            }

            var user = new User()
            {
                Name = userRegister.Name,
                Email = userRegister.Email,
                Password = userRegister.Password
            };
            var cart = new Cart();
            user.Cart = cart;
            _context.Add(user);

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, 
                CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = userRegister.KeepLogedIn
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity), properties);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
