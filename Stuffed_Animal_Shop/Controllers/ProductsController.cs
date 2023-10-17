using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus.DataSets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stuffed_Animal_Shop.Data;
using Stuffed_Animal_Shop.Models;
using Stuffed_Animal_Shop.Services;
using Stuffed_Animal_Shop.Utilities;
using Stuffed_Animal_Shop.ViewModels.Products;

namespace Stuffed_Animal_Shop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PhotoService _photoService;
        private readonly ProductService _productService;

        public ProductsController(ApplicationDbContext context, IOptions<CloudinarySetting> config)
        {
            _context = context;
            _photoService = new PhotoService(config);
            _productService = new ProductService(context, _photoService);
        }
        
        // GET: Products
        [Route("admin/products")]
        public async Task<IActionResult> Index()
        {
              return _context.Products != null ? 
                          View(await _context.Products.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Products'  is null.");
        }

        [Route("admin/products/create")]
        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost("admin/products/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProduct createProduct)
        {
            if (ModelState.IsValid)
            {
                var image = _photoService.AddPhotoAsync(createProduct.MainImage);
                var product = new Product()
                {
                    Name = createProduct.Name,
                    Price = createProduct.Price,
                    Quantity = createProduct.Quantity,
                    Description = createProduct.Description,
                    MainImage = image.Url.ToString(),
                };

                var sizeList = new List<Size>();
                var colorList = new List<Color>();

                foreach (var item in createProduct.Sizes)
                {
                    sizeList.Add(new Size()
                    {
                        Name = item,
                        Product = product
                    });
                }

                foreach (var item in createProduct.Colors)
                {
                    colorList.Add(new Color()
                    {
                        Name = item,
                        Product = product
                    });
                }

                var images = _photoService.AddPhotosAsync(createProduct.Images);
                var listImagesProduct = new List<Image>();
                for(int i = 0; i < images.Count(); i++)
                {
                    var newImage = new Image()
                    {
                        ImageUrl = images[i].Url.ToString(),
                        Product = product
                    };
                    listImagesProduct.Add(newImage);
                }

                _context.AddRange(sizeList);
                _context.AddRange(colorList);
                _context.AddRange(listImagesProduct);
                _context.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(createProduct);
        }

        [Route("admin/products/edit/{id}")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            var editProduct = new EditProduct()
            {
                Name = product.Name,
                Price = product.Price,
                //Size = product.Size,
                //Color = product.Color,
                Quantity = product.Quantity,
                Description = product.Description,
            };

            return View(editProduct);
        }

        //// POST: Products/Edit/5
        [HttpPost("admin/products/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditProduct editProduct)
        {
            var productCurrent = this._productService.GetProductById(id);
            Console.WriteLine(editProduct.Name + " " + editProduct.MainImage);
            if (productCurrent == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Delete main image and subimages of old product in cloudinary, database
                if (editProduct.MainImage != null)
                {
                    var mainImagePublicId = this._photoService.GetPublicId(imageUrl: productCurrent.MainImage);
                    await this._photoService.DeletePhotoAsync(mainImagePublicId);
                }

                if (editProduct.Images != null)
                {
                    var images = this._productService.GetImages(productId: id);
                    Console.WriteLine(images[0].ImageUrl);
                    var imageListPublicIds = new List<string>();
                    foreach (var image in images)
                    {
                        var publicId = this._photoService.GetPublicId(image.ImageUrl);
                        imageListPublicIds.Add(publicId);
                    }
                    await this._photoService.DeletePhotosAsync(imageListPublicIds);
                    _context.RemoveRange(images);
                }

                // Update new prouct
                _productService.UpdateProduct(product: productCurrent, editProduct: editProduct);


                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(editProduct);
        }


        // GET: Products/Delete/5
        [Route("admin/products/delete/{id}")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost("admin/products/delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {


            if (_context.Products == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Products'  is null.");
            }
            var product = _productService.GetProductById(id);

            //Delete images in cloudinary
            var mainImagePublicId = this._photoService.GetPublicId(product.MainImage);
            Console.WriteLine(mainImagePublicId);
            var images = this._productService.GetImages(id);
            var imageListPublicIds = new List<string>();
            foreach (var image in images)
            {
                var publicId = this._photoService.GetPublicId(image.ImageUrl);
                imageListPublicIds.Add(publicId);
            }

            if (product != null)
            {
                _context.Products.Remove(product);
            }


            await this._photoService.DeletePhotoAsync(mainImagePublicId);
            await this._photoService.DeletePhotosAsync(imageListPublicIds);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        
    }
}
