using Microsoft.EntityFrameworkCore;
using Stuffed_Animal_Shop.Data;
using Stuffed_Animal_Shop.Models;
using Stuffed_Animal_Shop.Utilities;
using System;

namespace Stuffed_Animal_Shop.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly RandomCustom random = new RandomCustom();
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

        public User GetUserByEmail (string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public string ResetPassword(string email)
        {
            string newPassword = random.RandomString(6);

            var user = GetUserByEmail(email);
            user.Password = newPassword;

            _context.Users.Update(user);

            _context.SaveChanges();

            return newPassword;
        }
    }
}
