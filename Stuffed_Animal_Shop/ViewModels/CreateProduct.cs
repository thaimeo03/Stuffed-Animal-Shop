namespace Stuffed_Animal_Shop.ViewModels
{
    public class CreateProduct
    {
        public string Name { get; set; }

        public int Price { get; set; }

        public string Size { get; set; }

        public string Color { get; set; } = "";

        public int Quantity { get; set; }

        public string Description { get; set; }

        public IFormFile MainImage { get; set; }

        public List<IFormFile> Images { get; set; }
    }
}
