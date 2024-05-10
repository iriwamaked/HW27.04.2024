using Microsoft.AspNetCore.Mvc;

namespace ASP1.Models.Shop
{
    public class ShopCategoryFormModel
    {
        [FromForm(Name = "category-slug")]
        public string? Slug { get; set; } = null!;
        [FromForm(Name = "category-name")]
        public string? Name { get; set; } = null!;
        [FromForm(Name = "category-description")]
        public string? Description { get; set; } = null!;
        [FromForm(Name = "category-image")]
        public IFormFile? Image { get; set; } = null!;
        public Dictionary<string, string> ToParams() => new()
        {
            {"category-name", Name ??"null" },
            {"category-description", Description ??"null"},
            {"category-slug",Slug ??"null"},
            { "category-image", Image?.FileName ??"null"}
        };

    }
}
