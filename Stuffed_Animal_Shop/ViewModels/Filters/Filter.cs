using Microsoft.AspNetCore.Mvc;
using System.Web;

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

        public string ToQueryString()
        {
            var queryString = new List<string>();

            if (Prices != null)
                queryString.AddRange(Prices.Select(price => $"Prices={HttpUtility.UrlEncode(price)}"));

            if (Colors != null)
                queryString.AddRange(Colors.Select(color => $"Colors={HttpUtility.UrlEncode(color)}"));

            if (Sizes != null)
                queryString.AddRange(Sizes.Select(size => $"Sizes={HttpUtility.UrlEncode(size)}"));

            if (Page.HasValue)
                queryString.Add($"Page={Page}");

            if (PageSize.HasValue)
                queryString.Add($"PageSize={PageSize}");

            if (Name.HasValue)
                queryString.Add($"Name={Name}");

            return string.Join("&", queryString);
        }
    }
}
