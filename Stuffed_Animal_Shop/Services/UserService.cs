using Microsoft.EntityFrameworkCore;
using Stuffed_Animal_Shop.Data;
using Stuffed_Animal_Shop.Models;

namespace Stuffed_Animal_Shop.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        public UserService(ApplicationDbContext context) {
            this._context = context;
        }

        public bool UserExsisted (string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        public User AccountExsited (string email, string password)
        {
            return _context.Users.Where(u => u.Email == email && u.Password == password).FirstOrDefault();
        }
    }
}
