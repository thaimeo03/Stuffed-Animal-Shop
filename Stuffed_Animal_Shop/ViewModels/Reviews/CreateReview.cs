using Stuffed_Animal_Shop.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Stuffed_Animal_Shop.ViewModels.Reviews
{
    public class CreateReview
    {
        public Guid ReviewId { get; set; }

        public string EmailUser { get; set; }

        public int? Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Product Product { get; set; } = null;
    }
}
