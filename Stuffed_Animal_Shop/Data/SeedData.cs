using Stuffed_Animal_Shop.Models;
using Bogus;



namespace Stuffed_Animal_Shop.Data
{
    public class SeedData
    {
        private readonly ApplicationDbContext _context;

        public SeedData(ApplicationDbContext context)
        {
            _context = context;
        }

        public async void GenerateFakeUserWithCart(int count)
        {
            var users = new List<User>();
            var faker = new Faker<User>()
                .RuleFor(u => u.UserId, f => f.Random.Guid())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Password, f => f.Internet.Password())
                .RuleFor(u => u.Name, f => f.Person.FullName)
                .RuleFor(u => u.Role, f => f.PickRandom("User", "Admin"));


            for (int i = 0; i < count; i++)
            {
                var user = faker.Generate();
                user.Cart = new Cart();
                users.Add(user);
            }

            users.AddRange(users);
            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();
        }
    }
}
