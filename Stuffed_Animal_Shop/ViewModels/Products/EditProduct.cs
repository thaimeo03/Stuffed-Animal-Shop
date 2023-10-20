namespace Stuffed_Animal_Shop.ViewModels.Products
{
    public class EditProduct
    {
        public string Name { get; set; }

        public int Price { get; set; }

        public List<string> Sizes { get; set; }

        public List<string> Colors { get; set; }

        public int Quantity { get; set; }

        public string Description { get; set; }

        public IFormFile? MainImage { get; set; }

        public List<IFormFile>? Images { get; set; }
    }
}
