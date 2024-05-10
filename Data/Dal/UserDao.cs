using ASP1.Data.Context;
using ASP1.Data.Entities;
using ASP1.Services.Kdf;

namespace ASP1.Data.Dal
{
    public class UserDao
    {
        private readonly DataContext _context;
        private readonly IKdfService _kdfService;
        private readonly Object _dbLocker;
        public UserDao(DataContext context, IKdfService kdfService, object dbLocker)
        {
            _context = context;
            _kdfService = kdfService;
            _dbLocker = dbLocker;
        }

        public User? GetUserById(String id)
        {
            User? user;
            lock(_dbLocker )
            {   
                try { user= _context.Users.Find(Guid.Parse(id)); }
                 catch {user= null; }
            }

            return user;
        }
        public bool IsEmailFree(String email)
        {
            return !_context
                .Users
                .Where(u => u.Email == email)
                .Any(); //значит что что-то есть

        }

        public void SignUpUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public User? Authenticate(String email, String password) //будет возвращен либо найдено, либо null
        {
            User? user=_context.Users.FirstOrDefault(u => u.Email == email);

            //сначала нужно найти пользователя по логину, соль и потом проверить соответствие пароля
            //поэтому задачу разделяем, сначала ищем пользователя (логины уникальны)

            //пароль, который нам пришел, смешиваем с солью, которая лежит у пользователя, повторяем генерацию ключа
            //и проверяем, соответствуют ли пароли
            //наоборот сделать нельзя, то есть нельзя, зная ключ, сделать из него назад пароль, в этом и состоит суть защиты механизма
            //проверить можем, а подделать или подобрать - нет 
            if (user != null && _kdfService.GetDerivedKey(password, user.Salt) == user.DerivedKey)
            {
                return user;
            }     
            return null;
        }
    }
}
