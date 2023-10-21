using Microsoft.EntityFrameworkCore;
using Stuffed_Animal_Shop.Data;
using Stuffed_Animal_Shop.Models;
using Stuffed_Animal_Shop.ViewModels.Filters;

namespace Stuffed_Animal_Shop.Services
{
    public class ProductResult
    {
        public List<Product> Products { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public string QueryString { get; set; }
    }

    public class ShopService
    {
        private readonly ApplicationDbContext _context;

        public ShopService(ApplicationDbContext context)
        {
            this._context = context;
        }


        public ProductResult GetProductsByFilter(Filter filter)
        {
            List<string> sizesFiltered = filter.Sizes != null ? filter.Sizes : new List<string>();
            List<string> colorsFiltered = filter.Colors != null ? filter.Colors : new List<string>();
            List<int> pricesFiltered = filter.Prices != null ? filter.Prices.Select(price => int.Parse(price)).ToList() : new List<int>();
            int page = filter.Page != null ? filter.Page.Value : 1;
            int pageSize = filter.PageSize != null ? filter.PageSize.Value : 21;

            List<Product> productsBySize = new List<Product>();
            List<Product> productsByColor = new List<Product>();
            List<Product> productsByPrice = new List<Product>();

            if (sizesFiltered.Count > 0)
            { 
                productsBySize = _context.Sizes.Where(s => sizesFiltered.Contains(s.Name)).Select(s => s.Product).Where(p => p != null).Distinct().ToList();
            }
            if (colorsFiltered.Count > 0)
            {
                productsByColor = _context.Colors.Where(c => colorsFiltered.Contains(c.Name)).Select(c => c.Product).Where(p => p != null).Distinct().ToList();
            }
            //if (pricesFiltered.Count > 0)
            //{
            //    productsByPrice = _context.Products.Where(product => pricesFiltered.Any(price => price - 100 < product.Price && product.Price < price)).ToList();
            //}

            // Get all duplicate products
            List<Product> productsFiltered = _context.Products.ToList();
            if(!(filter.Prices == null && filter.Sizes == null && filter.Colors == null))
            {
                productsFiltered = productsBySize.Intersect(productsByColor).ToList();
            }

            // Pagination
            int totalProducts = productsFiltered.Count();
            int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            var products = productsFiltered.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            
            string queryString = filter.ToQueryString();

            return new ProductResult
            {
                Products = products,
                TotalPages = totalPages,
                CurrentPage = page,
                QueryString = queryString
            };
        }

    }
}
