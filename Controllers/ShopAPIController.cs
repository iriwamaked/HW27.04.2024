using ASP1.Data.Dal;
using ASP1.Models.Rest;
using ASP1.Models.Shop;
using ASP1.Services.Upload;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP1.Controllers
{
    [Route("api/shop")]
    [ApiController]
    public class ShopAPIController(DataAccessor dataAccessor, IUploadService uploadService, ILogger<ShopAPIController> logger) : ControllerBase
    {
        private readonly DataAccessor _dataAccessor = dataAccessor;
        private readonly IUploadService _uploadService = uploadService;
        private readonly ILogger<ShopAPIController> _logger = logger;

        [HttpGet("category")]
        public RestResponce GetCategory()
        {
            var res = _dataAccessor.ShopDao.GetCategories();
            return new()
            {
                Status = new()
                {
                    Code = StatusCodes.Status200OK,
                    Message = "Ok",
                    IsOk = true
                },
                Data = _dataAccessor.ShopDao.GetCategories(),
                Meta = new()
                {
                    Service = "/api/shop/category",
                    Total=res.Count,
                }
            };
        }

        [HttpPost("category")]

        public RestResponce DoPost(ShopCategoryFormModel? model)
        {
            RestResponce restResponce = new();
            restResponce.Meta = new()
            {
                Service = "/api/shop/category",
                ServerTime = DateTime.Now.Ticks,
                RequestParameters = model?.ToParams() ?? []
            };

            _logger.LogInformation(String.Join(',', restResponce.Meta.RequestParameters.Values));
            //model==null - не может, быть нулевой, если придет нуль, средствами ASp будет отказано в операции
            if (String.IsNullOrEmpty(model?.Slug) ||
                String.IsNullOrEmpty(model?.Name) ||
                String.IsNullOrEmpty(model?.Description))
            {
                restResponce.Status = new()
                {
                    Code = StatusCodes.Status422UnprocessableEntity,
                    Message = "Missing required data",
                    IsOk = false
                };
            }
            else
            {
                try
                {
                    var category = _dataAccessor.ShopDao.AddCategory(
                        name: model.Name,
                        slug: model.Slug,
                        description: model.Description,
                        imageUrl: _uploadService.SaveFormFile(model.Image, "wwwroot/img/shop"));
                    restResponce.Status = new()
                    {
                        Code = StatusCodes.Status201Created,
                        Message = "Ok",
                        IsOk = true
                    };
                    restResponce.Data = category;

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    restResponce.Status = new()
                    {
                        Code = StatusCodes.Status500InternalServerError,
                        Message = "Server error. Details in logs",
                        IsOk = false
                    };

                }
            }
            return restResponce;

        }

        [HttpPost("product")]

        public RestResponce AddProduct(ShopProductFormModel? model)
        {
            RestResponce restResponce = new();
            restResponce.Meta = new()
            {
                Service = "/api/shop/product",
                ServerTime = DateTime.Now.Ticks,
                RequestParameters = model?.ToParams() ?? []
            };

            _logger.LogInformation(String.Join(',', restResponce.Meta.RequestParameters.Values));
            //model==null - не может, быть нулевой, если придет нуль, средствами ASp будет отказано в операции
            if (model?.Price==null ||
                String.IsNullOrEmpty(model?.Name) ||
                String.IsNullOrEmpty(model?.Description))
            {
                restResponce.Status = new()
                {
                    Code = StatusCodes.Status422UnprocessableEntity,
                    Message = "Missing required data",
                    IsOk = false
                };
            }
            else
            {
                try
                {
                    var product = _dataAccessor.ShopDao.AddProduct(
                        categoryId:model.CategoryId,
                        name: model.Name,
                        price: model.Price.HasValue ? model.Price.Value : 0.0,
                        description: model.Description,
                        imageUrl: _uploadService.SaveFormFile(model.Image, "wwwroot/img/shop"));
                    restResponce.Status = new()
                    {
                        Code = StatusCodes.Status201Created,
                        Message = "Ok",
                        IsOk = true
                    };
                    restResponce.Data = product;

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    restResponce.Status = new()
                    {
                        Code = StatusCodes.Status500InternalServerError,
                        Message = "Server error. Details in logs",
                        IsOk = false
                    };

                }
            }
            return restResponce;

        }
    }
}
