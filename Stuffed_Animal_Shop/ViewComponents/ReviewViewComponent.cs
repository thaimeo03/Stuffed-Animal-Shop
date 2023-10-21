namespace Stuffed_Animal_Shop.ViewComponents
{
    using Microsoft.AspNetCore.Mvc;
    using Stuffed_Animal_Shop.Data;
    using Stuffed_Animal_Shop.Models;
    using Stuffed_Animal_Shop.Services;

    public class ReviewViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public ReviewViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke(Product product, User user)
        {
            List<User> users = new List<User>();

            var reviews = _context.Reviews.Where(r => r.Product == product).ToList();
            var usersFromReviews = reviews.Select(r => r.EmailUser).ToList();

            foreach (var userEmail in usersFromReviews)
            {
                users.Add(_context.Users.SingleOrDefault(u => u.Email == userEmail));
            }

            ViewBag.Product = product;
            ViewBag.ListUser = users;
            ViewBag.MyEmail = user.Email;

            return View("Review", reviews);
        }

    }
}
