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
            const string ALL = "all";
            List<string> sizesFiltered = filter.Sizes != null ? filter.Sizes : new List<string>();
            List<string> colorsFiltered = filter.Colors != null ? filter.Colors : new List<string>();
            List<string> pricesFiltered = filter.Prices != null ? filter.Prices.Select(price => price).ToList() : new List<string>();
            int page = filter.Page != null ? filter.Page.Value : 1;
            int pageSize = filter.PageSize != null ? filter.PageSize.Value : 21;
            string sort = filter.Sort != null ? filter.Sort : "";

            List<Product> productsBySize = new List<Product>();
            List<Product> productsByColor = new List<Product>();
            List<Product> productsByPrice = new List<Product>();
            List<Product> productSort = new List<Product>();
            List<Product> productsFiltered = _context.Products.ToList();

            if (sizesFiltered.Count > 0)
            {
                productsBySize = sizesFiltered.First().Equals(ALL) ?
                    _context.Products.Where(p => p != null).Distinct().ToList() :
                    _context.Sizes.Where(s => sizesFiltered.Contains(s.Name)).Select(s => s.Product).Where(p => p != null).Distinct().ToList();
            }
            if (colorsFiltered.Count > 0)
            {
                productsByColor = colorsFiltered.First().Equals(ALL) ?
                    _context.Products.Where(p => p != null).Distinct().ToList() :
                    _context.Colors.Where(s => colorsFiltered.Contains(s.Name)).Select(s => s.Product).Where(p => p != null).Distinct().ToList();
            }
            if (pricesFiltered.Count > 0)
            {
                if (pricesFiltered.First().Equals(ALL))
                {
                    productsByPrice = _context.Products.Where(p => p != null).Distinct().ToList();
                }
                else
                {
                    foreach (string price in pricesFiltered) {
                        productsByPrice.AddRange(_context.Products.Where(p => p.Price >= int.Parse(price) - 100 && p.Price < int.Parse(price)).ToList());
                    }
                }
            }

            

            // Get all duplicate products
            if(!(filter.Prices == null && filter.Sizes == null && filter.Colors == null))
            {
                productsFiltered = productsBySize.Intersect(productsByColor).Intersect(productsByPrice).ToList();
            }

            // Sort product
            if (!sort.Equals(string.Empty))
            {
                if (sort.Equals("latest"))
                {
                    productsFiltered = productsFiltered.OrderBy(s => s.CreatedAt).ToList();
                }
                else if (sort.Equals("best_rating"))
                {
                    List<Product> productsWithBestRating = _context.Reviews
                        .OrderByDescending(r => r.Rating)
                        .Select(r => r.Product)
                        .Distinct()
                        .ToList();

                    productsFiltered = productsFiltered
                        .Where(p => productsWithBestRating.Contains(p))
                        .ToList();
                    // productSort = _context.Reviews.OrderBy(r => r.Rating).Select(r => r.Product).ToList();
                }
                else if (sort.Equals("cheap"))
                {
                    productsFiltered = productsFiltered.OrderBy(s => s.Price).ToList();
                }
                else if (sort.Equals("expensive"))
                {
                    productsFiltered = productsFiltered.OrderByDescending(s => s.Price).ToList();
                }
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
