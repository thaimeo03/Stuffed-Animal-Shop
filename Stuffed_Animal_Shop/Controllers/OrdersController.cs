﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
			return _context.Orders != null ? 
						this.View(await _context.Orders.ToListAsync()) :
						this.Problem("Entity set 'ApplicationDbContext.Orders' is null.");
		}
	}
}
