using Microsoft.AspNetCore.Mvc;

namespace ASP1.Models.Shop
{
    public class ShopProductFormModel
    {
        [FromForm(Name = "category-id")]
        public Guid CategoryId { get; set; } 
        [FromForm(Name = "product-price")]
        public double? Price { get; set; } 
        [FromForm(Name = "product-name")]
        public string? Name { get; set; } = null!;
        [FromForm(Name = "product-description")]
        public string? Description { get; set; } = null!;
        [FromForm(Name = "product-image")]
        public IFormFile? Image { get; set; } = null!;
        public Dictionary<string, string> ToParams() => new()
        {
            {"product-name", Name ??"null" },
            {"product-description", Description ??"null"},
            {"product-price",Price.ToString() ?? "null"},
            { "product-image", Image?.FileName ??"null"}
        };
    }
}
