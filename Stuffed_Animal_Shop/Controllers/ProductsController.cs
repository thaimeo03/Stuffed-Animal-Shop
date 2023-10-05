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
using Stuffed_Animal_Shop.ViewModels;

namespace Stuffed_Animal_Shop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PhotoService _photoService;
        private readonly ProductService _productService;

        public ProductsController(ApplicationDbContext context, IOptions<CloudinarySetting> config)
        {
            _context = context;
            _photoService = new PhotoService(config);
        }

        [Authorize(Roles = "User")] // Change role to admin
        [Route("admin/products")]
        // GET: Products
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
                    Size = createProduct.Size,
                    Color = createProduct.Color,
                    Quantity = createProduct.Quantity,
                    Description = createProduct.Description,
                    MainImage = image.Url.ToString(),
                };

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

                _context.Add(product);
                _context.AddRange(listImagesProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(createProduct);
        }

        [Route("admin/products/edit")]
        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ProductId,Name,Price,Size,Color,Quantity,Sold,Description,MainImage,CreatedAt,UpdatedAt")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        [Route("admin/products/delete")]
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
        [HttpPost("admin/products/delete"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            Console.WriteLine(id);
            if (_context.Products == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(Guid.Parse(id));

            // Delete images in cloudinary
            var images = this._productService.GetImages(id);
            var productImageIds = new List<string>();
            foreach(var image in images)
            {
                productImageIds.Add(image.ImageId.ToString());
            }

            if (product != null)
            {
                _context.Products.Remove(product);
            }


            await this._photoService.DeletePhotoAsync(product.MainImage);
            await this._photoService.DeletePhotosAsync(productImageIds);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(Guid id)
        {
          return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }
    }
}
