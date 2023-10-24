using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stuffed_Animal_Shop.Data;
using Stuffed_Animal_Shop.Models;
using Stuffed_Animal_Shop.ViewModels.Categories;

namespace Stuffed_Animal_Shop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        [Route("admin/categories")]
        public async Task<IActionResult> Index()
        {
              return _context.Categories != null ? 
                          View(await _context.Categories.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
        }

        // GET: Categories/Details/5
        [Route("admin/categories/details/{id}")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            List<Product> products = _context.Products.ToList();
            List<Product> productsInCategory = _context.Categories.Where(c => c.CategoryId == id).SelectMany(c => c.Products).ToList();
            List<Product> productsNotInCategory = products.Except(productsInCategory).ToList();

            ViewBag.ProductsInCategory = productsInCategory;
            ViewBag.ProductsNotInCategory = productsNotInCategory;

            return View(category);
        }

        // GET: Categories/Create
        [Route("admin/categories/create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost("admin/categories/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategory createCategory)
        {
            if (ModelState.IsValid)
            {
                Category category = new Category()
                {
                    Name = createCategory.Name
                };

                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(createCategory);
        }

        // GET: Categories/Edit/5
        [Route("admin/categories/edit/{id}")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost("admin/categories/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditCategory editCategory)
        {
            if (ModelState.IsValid)
            {
                Category category = await _context.Categories.FindAsync(id);
                category.Name = editCategory.Name;

                _context.Update(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Categories/Delete/5
        [Route("admin/categories/delete/{id}")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost("admin/categories/delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpPost("admin/categories/change")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeProduct(ChangeProduct changeProduct)
        {
            Category category = await _context.Categories.FindAsync(changeProduct.CategoryId);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            if (changeProduct.ProductIds != null)
            {
                List<Product> productsChange = new List<Product>();
                foreach (var id in changeProduct.ProductIds)
                {
                    Product product = await _context.Products.FindAsync(id);
                    productsChange.Add(product);
                }

                Category newCategory = new Category()
                {
                    CategoryId = changeProduct.CategoryId,
                    Name = category.Name,
                    Products = productsChange
                };

                _context.Add(newCategory);
                await _context.SaveChangesAsync();
            }
            else
            {
                Category newCategory = new Category()
                {
                    CategoryId = changeProduct.CategoryId,
                    Name = category.Name,
                    Products = new List<Product>()
                };
                _context.Categories.Add(newCategory);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(Guid id)
        {
          return (_context.Categories?.Any(e => e.CategoryId == id)).GetValueOrDefault();
        }
    }
}
