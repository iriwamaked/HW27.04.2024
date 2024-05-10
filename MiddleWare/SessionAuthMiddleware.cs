using ASP1.Data.Dal;
using System.Globalization;
using System.Security.Claims;

namespace ASP1.MiddleWare
{
    //Класс с функциональностью
    public class SessionAuthMiddleware
    {
        //При запуске проекта выстраивается последовательность запуска Middleware и 
        //каждый класс получает ссылку на следующий, внедряется через конструктор 

        //конструктор вызываеся для связывания цепочки
        private readonly RequestDelegate _next;

        public SessionAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        //посколько конструктор занят под настройки цепочки вызовов
        //инжекция осуществляется в метод InvokeAsync - его аргументы (параметры), 
        //это те сервисы, которые обычно и в конструкторе других классов
        public async Task InvokeAsync(HttpContext context, DataAccessor dataAccessor)
        {
            var userId = context.Session.GetString("auth-user-id");

            if (userId != null) //у сессии есть данные для аутентификации
            { 
                //проверяем эти данные
                var user=dataAccessor.UserDao.GetUserById(userId);
                if (user != null)
                {
                    //Используем встроенную к ASP схему работы с данными аутентификации (утверждения Claims)
                    //представляют собой пары ключ-значение для обозначения типовых данных
                    Claim[] claims = new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Sid, user.Id.ToString()),
                        new Claim(ClaimTypes.UserData, user.AvatarUrl??""),
                        new Claim(ClaimTypes.DateOfBirth, user.Birthdate?.ToString()??"")
                    };
                    //тут context.User - не наш, это стандартный HTTP, ASP-поле, которое
                    //собирает данные Claims и схемы аутентификации
                    //по сути из нашего Usera мы собираем ASP-User, который собирается из стандартизированные Claims
                    context.User = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            claims,
                            nameof(SessionAuthMiddleware) //название схемы аутентификации
                        )
                    );
                        
                }
            }
            //в процессе работы Middleware должно принять решение, продолжать ли дальнейшую обработку запроса
            //если это так, то должен быть вызван следующий обработчик
            //Чаще всего Middleware не останавливает работу, а что-то добавляет, фильтрует

            // Call the next delegate/middleware in the pipeline.
            await _next(context); //контекс передается дальше
        }
    }

    //Класс-расширение для создания короткого метода app.UseSessionAuth()
    public static class SessionAuthMiddlewareExtension
    {
        public static IApplicationBuilder UseSessionAuth(
        this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SessionAuthMiddleware>();
        }

    }
}
