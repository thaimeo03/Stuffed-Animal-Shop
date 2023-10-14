using System.ComponentModel.DataAnnotations;

namespace Stuffed_Animal_Shop.ViewModels.Users
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
