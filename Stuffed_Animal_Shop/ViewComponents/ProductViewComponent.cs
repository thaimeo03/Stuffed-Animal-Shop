namespace Stuffed_Animal_Shop.ViewComponents
{
	using Microsoft.AspNetCore.Mvc;
	using Stuffed_Animal_Shop.Data;
	using Stuffed_Animal_Shop.Models;
	using Stuffed_Animal_Shop.Services;

	public class ProductViewComponent : ViewComponent
	{
		private readonly ApplicationDbContext _context;

		public ProductViewComponent(ApplicationDbContext context)
		{
			_context = context;
		}

		public IViewComponentResult Invoke(bool isRanked)
		{
			if (isRanked)
			{
				List<Product> productsRanked = _context.Products.OrderBy(m => m.Sold).Take(8).ToList();

				return this.View("ProductsByRanking", productsRanked);
			}	
			else
			{
				List<Product> productsArrived = _context.Products.OrderBy(m => m.CreatedAt).Take(8).ToList();

				return this.View("ProductsByTimeArrived", productsArrived);
			}
		}
	}
}
