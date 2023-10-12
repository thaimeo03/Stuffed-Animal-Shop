using System.ComponentModel.DataAnnotations;

namespace Stuffed_Animal_Shop.ViewModels
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
