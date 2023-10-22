using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stuffed_Animal_Shop.Data;
using Stuffed_Animal_Shop.Models;
using Stuffed_Animal_Shop.Services;

namespace Stuffed_Animal_Shop.Controllers
{
	[Authorize(Roles = "Admin")]

	public class OrdersController : Controller
	{
		private readonly ApplicationDbContext _context;

		public OrdersController(ApplicationDbContext context)
		{
			this._context = context;
		}

		[Route("admin/orders")]
		public async Task<IActionResult> Index()
		{
			List<Order> orders = _context.Orders.ToList();
			List<User> test = _context.Users.ToList();
			List<User> users = new List<User>();

			foreach (Order order in orders)
			{
				User user = _context.Users.FirstOrDefault(u => u == order.User);
				if (user != null)
				{
					users.Add(user);
				}
			}

			ViewBag.Users = users;

			return _context.Orders != null ? 
						this.View(orders) :
						this.Problem("Entity set 'ApplicationDbContext.Orders' is null.");
		}

		public IActionResult GetTheStuff(Guid yeaTheStuff)
		{
			var thatsTheStuff_OrderItems = this._context.OrderItems.Where(o => o.Order.OrderId == yeaTheStuff).ToList();

			//foreach (var stuff in thatsTheStuff_OrderItems)
			//{
			//	Debug.WriteLine("\n" + stuff.Name + "\n");
			//}

			return this.PartialView("OrderDetailed", thatsTheStuff_OrderItems);
		}
	}
}
