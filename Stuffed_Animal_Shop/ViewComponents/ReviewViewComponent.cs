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


            List<string> avatars = new List<string>();
            List<string> users = new List<string>();

            var reviews = _context.Reviews.Where(r => r.Product == product).ToList();
            var usersFromReviews = reviews.Select(r => r.EmailUser).ToList();


            foreach (var userEmail in usersFromReviews)
            {
                Console.WriteLine(userEmail);
                avatars.Add(_context.Users.SingleOrDefault(u => u.Email == userEmail)?.Avatar);
                users.Add(_context.Users.SingleOrDefault(u => u.Email == userEmail)?.Name);

            }
            Console.WriteLine(users.Count);

            string[] arrAvatar = avatars.ToArray();
            string[] arrUsers = users.ToArray();
            for (int i = 0; i < arrAvatar.Length; i++)
            {
                if (arrAvatar[i] == null)
                {
                    arrAvatar[i] = "~/img/user.jpg";

                }
            }
            ViewBag.Avatars = arrAvatar;
            ViewBag.Product = product;
            ViewBag.Users = arrUsers;
            ViewBag.MyEmail = user.Email;

            return View("Review", reviews);
        }

    }
}
