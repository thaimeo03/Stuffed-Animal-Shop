using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stuffed_Animal_Shop.Data;
using Stuffed_Animal_Shop.Services;
using System.Security.Claims;

namespace Stuffed_Animal_Shop.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserService _userService;
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
            _userService = new UserService(context);
        }

        [Authorize(Roles = "Admin, User")]
        [Route("/profile/{UserId}")]
        public IActionResult Index()
        {
            var userEmail = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            var user = this._userService.GetUserByEmail(userEmail);

            if (user != null)
            {
                ViewBag.User = user;
            }

            return View();
        }
    }
}
