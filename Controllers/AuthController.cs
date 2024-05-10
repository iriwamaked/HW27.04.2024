using ASP1.Data.Dal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP1.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataAccessor _dataAccessor;

        public AuthController(DataAccessor dataAccessor)
        {
            _dataAccessor = dataAccessor;
        }

        [HttpGet]
        //fetch отправляет данные
        public object Get(String email, String password) //название метода роли не играет, главное - атрибут
        {
            var user=_dataAccessor.UserDao.Authenticate(email, password);
            String status;
            if (user == null)
            {
                status = "error";
            }
            else
            {
                status = "success";
                //return new { status = $"Auth works: {email}, {password}" }; //тут создается объект и возвращается в формате json и передается на ответ
                //сохраняем в сессию данные автентификации (как набор байт, объекты нужно сериализовать), с
                HttpContext.Session.SetString("auth-user-id", user.Id.ToString()); //"auth-user-id" - ключ и сам идентификатор пользователя
            }
            return new { status };
        }
    }
}
