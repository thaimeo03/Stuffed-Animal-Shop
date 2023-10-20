using Bogus.DataSets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stuffed_Animal_Shop.Data;
using Stuffed_Animal_Shop.Models;
using Stuffed_Animal_Shop.Services;
using Stuffed_Animal_Shop.Utilities;
using Stuffed_Animal_Shop.ViewModels.Users;
using System.Data;
using System.Security.Claims;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Stuffed_Animal_Shop.Controllers
{
	public class ProfileController : Controller
	{
		private readonly UserService _userService;
		private readonly PhotoService _photoService;
		private readonly ApplicationDbContext _context;

		public ProfileController(ApplicationDbContext context, IOptions<CloudinarySetting> config)
		{
			_context = context;
			_photoService = new PhotoService(config);
			_userService = new UserService(context);
		}

		[Authorize(Roles = "Admin, User")]
		[Route("/profile/{UserId}")]
		public IActionResult Index()
		{
			var userEmail = this.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

			var user = this._userService.GetUserByEmail(userEmail);

			var userEdit = new UserEdit
			(
				userId: user.UserId,
				name: user.Name,
				email: user.Email,
				address: user.Address,
				phoneNumber: user.PhoneNumber,
				avatar: null
			);

			//this.ViewBag.Message = "Temp!";

			return this.View(userEdit);
		}

		[Authorize(Roles = "Admin, User")]
		[HttpPost("/profile/{UserId}")]
		public async Task<IActionResult> Index([FromRoute] string UserId, UserEdit userEdit)
		{
			//			Finish this yeah

			for (int i = 0; i < 5; i++)
			{
				Console.WriteLine("\n");
			}
			Console.WriteLine(userEdit.Email);

			string imageUrl = "";

			if (userEdit.Avatar != null)
			{
				var image = _photoService.AddPhotoAsync(userEdit.Avatar);
				imageUrl = image.Url.ToString();
			}

			if (!this.ModelState.IsValid)
			{
				Console.WriteLine("INVALID!");

				return this.View(userEdit);
			}

			if (_userService.UserExsisted(userEdit.Email) &&
				!_context.Users.Any(u => 
					u.UserId == userEdit.UserId && 
					u.Email == userEdit.Email))
			{

				Console.WriteLine("UserExsisted True!");

				this.ModelState.AddModelError("Email", "Email existed!");
				
				return this.View(userEdit);
			}

			try
			{
				var user = _context.Users.Find(userEdit.UserId);

				user.Email = userEdit.Email;
				user.Name = userEdit.Name;
				user.PhoneNumber = userEdit.PhoneNumber;
				user.Address = userEdit.Address;

				if (imageUrl != "")
				{
					Console.WriteLine("imageUrl EXISTS");

					if (user.Avatar != "") 
					{
						var imageID = this._photoService.GetPublicId(imageUrl: user.Avatar);
						await this._photoService.DeletePhotoAsync(imageID);
					}

					user.Avatar = imageUrl;
				}

				_context.Users.Update(user);
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				this.ViewBag.Message = ex.Message;

				return this.View();
			}

			this.ViewBag.Message = "Profile updated successfully!";

			return this.View();
		}
	}
}
