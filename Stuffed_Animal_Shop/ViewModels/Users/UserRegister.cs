using System.ComponentModel.DataAnnotations;

namespace Stuffed_Animal_Shop.ViewModels.Users
{
    public class UserRegister
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password do not match")]
        public string ConfirmPassword { get; set; }

        public bool KeepLogedIn { get; set; }
    }
}
