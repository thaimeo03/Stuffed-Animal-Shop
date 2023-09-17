using Microsoft.EntityFrameworkCore;
using Stuffed_Animal_Shop.Models;

namespace Stuffed_Animal_Shop.Data
{
    public class ApplicationDbContext : DbContext   
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {}

        public DbSet<User> Users { get; set; }
        public DbSet<Cart> Carts { get; set; }
    }
}
