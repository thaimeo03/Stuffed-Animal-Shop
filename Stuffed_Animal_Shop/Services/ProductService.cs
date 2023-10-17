using Microsoft.CodeAnalysis;
using Stuffed_Animal_Shop.Data;
using Stuffed_Animal_Shop.Models;
using Stuffed_Animal_Shop.ViewModels.Products;

namespace Stuffed_Animal_Shop.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly PhotoService _photoService;

        public ProductService(ApplicationDbContext context, PhotoService photoService)
        {
            this._context = context;
            _photoService = photoService;
        }

        public List<Image> GetImages(Guid productId)
        {
            Console.WriteLine(productId);
            return this._context.Images.Where(i => i.Product.ProductId == productId).ToList();
        }

        public bool ProductExists(Guid id)
        {
            return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }

        public Product GetProductById(Guid id)
        {
            return _context.Products.FirstOrDefault(p => p.ProductId == id);
        }

        public async void UpdateProduct(Product product, EditProduct editProduct)
        {
            var mainImageUrl = product.MainImage;

            if(editProduct.MainImage != null)
            {
                var mainImage = _photoService.AddPhotoAsync(editProduct.MainImage);
                mainImageUrl = mainImage.Url.ToString();
                Console.WriteLine(mainImageUrl);
                Console.WriteLine(mainImageUrl);
            }

            if(editProduct.Images != null)
            {
                var images = _photoService.AddPhotosAsync(editProduct.Images);
                var listImagesProduct = new List<Image>();
                for (int i = 0; i < images.Count(); i++)
                {
                    var newImage = new Image()
                    {
                        ImageUrl = images[i].Url.ToString(),
                        Product = product
                    };
                    listImagesProduct.Add(newImage);
                }
                _context.AddRange(listImagesProduct);
            }

            var sizeList = new List<Size>();
            var colorList = new List<Color>();

            foreach (var item in editProduct.Sizes)
            {
                sizeList.Add(new Size()
                {
                    Name = item,
                    Product = product
                });
            }

            foreach (var item in editProduct.Colors)
            {
                colorList.Add(new Color()
                {
                    Name = item,
                    Product = product
                });
            }

            product.Name = editProduct.Name;
            product.Description = editProduct.Description;
            product.Price = editProduct.Price;
            product.Quantity = editProduct.Quantity;
            product.MainImage = mainImageUrl;

            _context.Sizes.UpdateRange(sizeList);
            _context.Colors.UpdateRange(colorList);
            _context.Products.Update(product);
        }
    }
}
