using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Stuffed_Animal_Shop.ViewModels.Users
{
	public class UserEdit
	{
		[Required(ErrorMessage = "Wait wut UserId where")]
		public Guid UserId { get; set; }

		[DisplayName("Name")]
		[Required(ErrorMessage = "Please insert your Name")]
		public string? Name { get; set; }

		//[DisplayName("Mật khẩu cũ")]
		//[Required(ErrorMessage = "Mật khẩu chưa điền!")]
		//public string? OldPassword { get; set; }

		//[DisplayName("Mật khẩu mới")]
		//[Required(ErrorMessage = "Mật khẩu chưa điền!")]
		//public string? Password { get; set; }

		//[DisplayName("Nhập lại mật khẩu mới")]
		//[Required(ErrorMessage = "Mật khẩu chưa điền!")]
		//public string? ConfirmPassword { get; set; }

		[DisplayName("Email")]
		[Required(ErrorMessage = "Please insert your Email")]
		public string? Email { get; set; }

		[DisplayName("Address")]
		[Required(ErrorMessage = "Please insert your Address")]
		public string? Address { get; set; }

		[DisplayName("Phone Numbers")]
		[Required(ErrorMessage = "Please insert your Phone Numbers")]
		[MinLength(
			length: 10,
			ErrorMessage = "Phone numbers must be at least 10 characters"
			)]
		[MaxLength(
			length: 12,
			ErrorMessage = "Phone numbers must be less than 10 characters"
			)]
		//1234567890
		//123-456-7890
		//(123) 456-7890
		//123 456 7890
		//123.456.7890
		//+91 (123) 456-7890
		[RegularExpression(
			pattern: @"^(\+\d{1,2}\s?)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$",
			ErrorMessage = "Incorrect phone numbers format"
			)]
		public string? PhoneNumber { get; set; }

		public IFormFile? Avatar { get; set; }

		public UserEdit(Guid userId, string name, string email, string address, string phoneNumber, IFormFile? avatar)
		{
			this.UserId = userId;
			this.Name = name;
			this.Email = email;
			this.Address = address;
			this.PhoneNumber = phoneNumber;
			this.Avatar = avatar;
		}

		public UserEdit() { }
	}
}
