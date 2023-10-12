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
        private readonly EmailService _emailService;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
            _userService = new UserService(context);
            _emailService = new EmailService();
        }

        private async void Auth(string Email, string Role)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, Email),
                new Claim(ClaimTypes.Role, Role)
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity), properties);
        }

        // GET: Users/Register
        public IActionResult Register()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new UserRegister());
        }

        // POST: Users/Register
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegister userRegister)
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

            this.Auth(user.Email, user.Role);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        // Get: Users/login
        public IActionResult Login()
        {
            return View();
        }

        // Get: Users/forgotpassword
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLogin userLogin)
        {
            Console.WriteLine(userLogin.Email + " " + userLogin.Password);

            if (!ModelState.IsValid)
            {
                return View(userLogin);
            }

            var user = _userService.AccountExsited(userLogin.Email, userLogin.Password);
            if (user == null)
            {
                Console.WriteLine("NOooo");
                TempData["Error"] = "Email or password is incorrect";
                return View(userLogin);
            }

            this.Auth(user.Email, user.Role);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPassword forgotPassword)
        {
            if (!ModelState.IsValid)
            {
                return View(forgotPassword);
            }

            var user = _userService.GetUserByEmail(forgotPassword.Email);

            if (user == null)
            {
                TempData["Error"] = "Email is incorrect";
                return View(forgotPassword);
            }

            string newPassword = _userService.ResetPassword(user.Email);

            Console.WriteLine(newPassword);

            _emailService.SendEmail(user.Email, "New password", newPassword);
            return RedirectToAction("Login", "Users");
        }
    }
}
