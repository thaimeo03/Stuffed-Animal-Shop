using Microsoft.AspNetCore.Mvc;

namespace Stuffed_Animal_Shop.ViewModels.Filters
{
    public class Filter
    {
        public List<string>? Prices { get; set; } = null;

        public List<string>? Colors { get; set; } = null;

        public List<string>? Sizes { get; set; } = null;

        public int? Page { get; set; } = null;

        public int? PageSize { get; set; } = null;

        public int? Name { get; set; } = null;
    }
}
