using ASP1.Data.Dal;
using ASP1.Models.Shop;
using Microsoft.AspNetCore.Mvc;

namespace ASP1.Controllers
{
    public class ShopController (DataAccessor dataAccessor): Controller
    {
        private readonly DataAccessor _dataAccessor=dataAccessor;

        public IActionResult Index()
        {
            ViewData["Categories"]=_dataAccessor.ShopDao.GetCategories();
            return View();
        }

        public IActionResult Category([FromRoute] String id)
        {
            ShopCategoryPageModel model = new()
            {
                Slug=id,
                Category=_dataAccessor.ShopDao.GetCategoryBySlug(id)

            };
            ViewData["Categories"] = _dataAccessor.ShopDao.GetCategories();
            return View(model);
        }
    }
}
