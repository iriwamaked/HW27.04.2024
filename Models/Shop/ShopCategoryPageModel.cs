using ASP1.Data.Entities;

namespace ASP1.Models.Shop
{
    public class ShopCategoryPageModel
    {
        public String Slug { get; set; } = null!;
        public Category Category { get; set; } = null!;
    }
}
