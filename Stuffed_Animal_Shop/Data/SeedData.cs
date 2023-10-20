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

        public async Task GenerateData(int userCount, int productCount, int reviewCount, int orderCount)
        {
            var users = GenerateFakeUserWithCart(orderCount);
            var carts = GenerateFakeCart(orderCount);
            var products = GenerateFakeProduct(productCount);
            var categories = GenerateFakeCategory(productCount);
            var reviews = GenerateFakeReview(reviewCount);
            var orders = GenerateFakeOrder(orderCount, users);
            var images = GenerateFakeImage(productCount);
            var sizes = GenerateFakeSize(4);
            var colors = GenerateFakeColor(3);
            var cartItems = GenerateFakeCartItem(5);

            for (int i = 0; i < userCount; i++)
            {
                carts[i].User = users[i];
            }

            for(int i = 0;  i < userCount; i++)
            {
                for(int j = 0; j < 5; j++)
                {
                    cartItems[j].Cart = carts[j];
                    cartItems[j].Product = products[j];
                }
            }

            for (int i = 0; i < productCount; i++)
            {
                products[i].Categories = RandomListItem(categories);
                images[i].Product = products[i];

                for(int j =  0; j < 4; j++)
                {
                    sizes[j].Product = products[i];
                }
                for (int j = 0; j < 3; j++)
                {
                    colors[j].Product = products[i];
                }
            }

            for (int i = 0; i < reviewCount; i++)
            {
                reviews[i].EmailUser = RandomListItem(users)[0].Email;
                reviews[i].Product = RandomListItem(products)[0];
            }

            for (int i = 0; i < orderCount; i++)
            {
                orders[i].Cart = carts[i];
            }

            _context.Users.AddRange(users);
            _context.Products.AddRange(products);
            _context.Reviews.AddRange(reviews);
            _context.Orders.AddRange(orders);
            _context.Categories.AddRange(categories);
            _context.Carts.AddRange(carts);
            _context.Sizes.AddRange(sizes);
            _context.Colors.AddRange(colors);
            _context.Images.AddRange(images);
            _context.CartItems.AddRange(cartItems);

            await _context.SaveChangesAsync();
        }

        public List<User> GenerateFakeUserWithCart(int count)
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
                users.Add(user);
            }

            return users;
        }

        public List<Category> GenerateFakeCategory(int count)
        {
            var categories = new List<Category>();
            var categoryFaker = new Faker<Category>()
                .RuleFor(c => c.CategoryId, f => f.Random.Guid())
                .RuleFor(c => c.Name, f => f.Commerce.Department());

            for (int i = 0; i < count; i++)
            {
                var category = categoryFaker.Generate();
                categories.Add(category);
            }

            return categories;
        }

        public List<Cart> GenerateFakeCart(int count)
        {
            var carts = new List<Cart>();
            var cartFaker = new Faker<Cart>()
                .RuleFor(c => c.CartId, f => f.Random.Guid());

            for (int i = 0; i < count; i++)
            {
                var cart = cartFaker.Generate();
                carts.Add(cart);
            }

            return carts;
        }

            public List<Product> GenerateFakeProduct(int count)
        {
            var products = new List<Product>();
            var productFaker = new Faker<Product>()
                .RuleFor(p => p.ProductId, f => f.Random.Guid())
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Price, f => f.Random.Number(1, 120))
                .RuleFor(p => p.Quantity, f => f.Random.Number(20, 350))
                .RuleFor(p => p.Sold, f => f.Random.Number(0, 200))
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.MainImage, f => f.Image.PicsumUrl());

            for (int i = 0; i < count; i++)
            {
                var product = productFaker.Generate();
                products.Add(product);
            }

            return products;
        }

        public List<Review> GenerateFakeReview(int count)
        {
            var reviews = new List<Review>();
            var reviewFaker = new Faker<Review>()
                .RuleFor(r => r.ReviewId, f => f.Random.Guid())
                .RuleFor(r => r.Rating, f => f.Random.Number(1, 5))
                .RuleFor(r => r.Comment, f => f.Lorem.Sentence());

            for (int i = 0; i < count; i++)
            {
                var product = reviewFaker.Generate();
                reviews.Add(product);
            }

            _context.Reviews.AddRange(reviews);

            return reviews;
        }

        public List<Order> GenerateFakeOrder(int count, List<User> users)
        {
            var orders = new List<Order>();
            var orderFaker = new Faker<Order>()
                .RuleFor(o => o.OrderId, f => f.Random.Guid())
                .RuleFor(o => o.Status, f => f.PickRandom("Pending", "Delivered"));

            for (int i = 0; i < count; i++)
            {
                var order = orderFaker.Generate();
                order.EmailUser = RandomListItem(users)[0].Email;
                orders.Add(order);
            }

            return orders;
        }

        public List<Image> GenerateFakeImage(int count)
        {
            var images = new List<Image>();
            var imageFaker = new Faker<Image>()
                .RuleFor(i => i.ImageId, f => f.Random.Guid())
                .RuleFor(i => i.ImageUrl, f => f.Image.PicsumUrl());

            for (int i = 0; i < count; i++)
            {
                var image = imageFaker.Generate();
                images.Add(image);
            }

            return images;
        }

        public List<Size> GenerateFakeSize(int count)
        {
            var sizes = new List<Size>();
            var sizeFaker = new Faker<Size>()
                .RuleFor(s => s.SizeId, f => f.Random.Guid())
                .RuleFor(s => s.Name, f => f.PickRandom("L", "XL", "M", "SM", "XXL"));

            for (int i = 0; i < count; i++)
            {
                var image = sizeFaker.Generate();
                sizes.Add(image);
            }

            return sizes;
        }
        public List<Color> GenerateFakeColor(int count)
        {
            var colors = new List<Color>();
            var colorFaker = new Faker<Color>()
                .RuleFor(s => s.ColorId, f => f.Random.Guid())
                .RuleFor(s => s.Name, f => f.PickRandom("Red", "Green", "Blue", "White", "Black"));

            for (int i = 0; i < count; i++)
            {
                var image = colorFaker.Generate();
                colors.Add(image);
            }

            return colors;
        }

        public List<CartItem> GenerateFakeCartItem(int count = 5)
        {
            var cartItems = new List<CartItem>();
            var cartItemFaker = new Faker<CartItem>()
                .RuleFor(p => p.CartItemId, f => f.Random.Guid())
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.ItemPrice, f => f.Random.Number(2, 200))
                .RuleFor(p => p.Count, f => f.Random.Number(1, 5))
                .RuleFor(p => p.Size, f => f.PickRandom("L", "XL", "M", "SM", "XXL"))
                .RuleFor(p => p.Color, f => f.PickRandom("Red", "Green", "Blue", "White", "Black"));

            for (int i = 0; i < count; i++)
            {
                var cartItem = cartItemFaker.Generate();
                cartItems.Add(cartItem);
            }

            return cartItems;
        }


        public List<T> RandomListItem<T>(List<T> list)
        {
            Random random = new Random();
            int n = list.Count;
            List<T> shuffledList = new List<T>(n);

            while (n > 0)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
                shuffledList.Add(value);
            }

            return shuffledList;
        }
    }
}
