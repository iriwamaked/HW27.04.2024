using ASP1.Data.Context;
using ASP1.Services.Kdf;
using System.Runtime.CompilerServices;

namespace ASP1.Data.Dal
{
    //сервіс - реєструємо у Program.cs
    public class DataAccessor
    {
        public DataContext _context;
        private readonly IKdfService _kdfService;
        private readonly Object _dbLocker = new(); //это не инжекция, это внутренний механизм, он извне не нужен
        //DataAcessor через контейнер получает все зависимости и всереди уже перераспределяет между теми
        //Dao, которые в них нуждаются
        public UserDao UserDao { get; private set; }
        public ShopDao ShopDao { get; private set; }
        public DataAccessor(DataContext context, IKdfService kdfService)
        {
            _context = context;
            _kdfService = kdfService;
            UserDao = new(_context, _kdfService, _dbLocker);
            ShopDao = new(_context, _dbLocker);
        }
    }
}
