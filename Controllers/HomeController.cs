using ASP1.Data.Dal;
using ASP1.Models;
using ASP1.Models.Home;
using ASP1.Models.Home.SignUp;
using ASP1.Services.Hash;
using ASP1.Services.Kdf;
using ASP1.Services.Upload;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Diagnostics;

namespace ASP1.Controllers
{
    public class HomeController : Controller
    {
        /*Традиційно інжекція сервісів здійснюється через конструктор. Це унеможливлює
         створення обєкту без передачі йому залежностей (сервісів). 
        У початковому проєкті інжекція демонструється наприкладі _logger*/

        private readonly ILogger<HomeController> _logger;

        /*Оголошуємо власну залежність (інжекцію) - поле з модифікаторами 
         private readonly - це не дозволить змінювати це поле випадковими командами.
        DIP - тип залежності - абстрація (інтефейс). */

        private readonly IHashService _hashService;
        private readonly IKdfService _kdfService;

        private readonly DataAccessor _dataAccessor;
        private readonly IUploadService _uploadService;

        //додаємо параметр до конструктора
        public HomeController(ILogger<HomeController> logger, IHashService hashService, 
            IKdfService kdfService, DataAccessor dataAccessor, IUploadService uploadService)
        {
            _logger = logger;
            _hashService = hashService;
            this._kdfService = kdfService;
            _dataAccessor = dataAccessor;
            _uploadService = uploadService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Razor()
        {
            return View();
        }

        /*Параметр відповідає за прийом даних (форми)
         * Nullable (?) говорить про те, що даних може не бути
         * якщо сторінка приймає дані форми, то виникає необхідність включити FormModel в PageModels
         */
        public IActionResult Models(HomeModelsFormModel? formModel)
        {
            HomeModelsPageModel model = new()
            {
                PageTitle = "Моделі в ASP",
                FormModel = formModel
            };
            return View(model);
        }

        public ViewResult IoS()
        {
            HomeIoCPageModel model = new()
            {
                HashExample = _hashService.Digest("123"),
                ServiceCode = _hashService.GetHashCode().ToString(), //обєкти, що мають однаковий геш-код - рівні 
                Title = "IoC",
                KdfExample = _kdfService.GetDerivedKey("123", "123")
            };
            return View(model);
        }

        public ViewResult Data()
        {
            return View();
        }

        public ViewResult SignIn()
        {
            return View();
        }


        public ViewResult SignUp(SignUpFormModel? formModel)
        {
            SignUpPageModel pageModel = new()
            {
                signUpFormModel = formModel,
                ValidationErrors = _ValidateSingUpModel(formModel)
            };
            //проверяем, есть ли какие-то ошибки и есть переданные данные

            if (formModel?.UserEmail != null) //есть переданные данные
            {
                if (pageModel.ValidationErrors.Count > 0) //и ошибки
                {
                    pageModel.Message = "Реєстрацію відхилено";
                    pageModel.IsSuccess = false;
                }
                else
                {
                    //регистрируем пользователя
                    _dataAccessor.UserDao.SignUpUser(mapUser(formModel));
                    pageModel.Message = "Реєстрація успішна";
                    pageModel.IsSuccess = true;
                }

            }
            //else
            //{
            //    pageModel.Message = "Реєстрацію відхилено";
            //    pageModel.IsSuccess = false;
            //}
            return View(pageModel);
        }

        //на входе модель, на выходе - сущность Entity, алгоритм регистрации
        private Data.Entities.User mapUser(SignUpFormModel formModel)
        {
            String salt = Guid.NewGuid().ToString();//у нас есть сервис для соли, используем его!
            Data.Entities.User user = new()
            {
                Id = Guid.NewGuid(),
                Name = formModel.UserName,
                Email = formModel.UserEmail,
                Register = DateTime.Now,
                Birthdate = formModel.Birthdate,
                AvatarUrl = formModel.SavedFileName,
                Salt = salt,
                DerivedKey = _kdfService.GetDerivedKey(formModel.Password, salt)
            };
            return user;
        }

        //
        private Dictionary<String, String> _ValidateSingUpModel(SignUpFormModel? formModel)
        {
            Dictionary<String, String> res = new(); //перелік помилок по кожному з полів моделі
            if (formModel == null)
            {
                res[nameof(formModel)] = "Model is null";
            }
            else
            {
                if (String.IsNullOrEmpty(formModel.UserName))
                {
                    res[nameof(formModel.UserName)] = "Name is empty";
                }
                if (String.IsNullOrEmpty(formModel.UserEmail))
                {
                    res[nameof(formModel.UserEmail)] = "Email is empty";
                }
                if (String.IsNullOrEmpty(formModel.Password))
                {
                    res[nameof(formModel.Password)] = "Password is empty";
                }
                if (formModel.Password != formModel.PasswordRepeat)
                {
                    res[nameof(formModel.PasswordRepeat)] = "Password mismatch";
                }
                //Проверить, присутствует ли почта уже в БД
                if (!_dataAccessor.UserDao.IsEmailFree(formModel.UserEmail))
                {
                    res[nameof(formModel.UserEmail)] = "Email in use";
                }
                //Соглашение с условиями
                if (!formModel.Confirm)
                {
                    res[nameof(formModel.Confirm)] = "Confirm expeced";
                }
                //Если нет ошибок, то обрабатываем файл-аватарку

                if (res.Count == 0)
                {
                    if (formModel.AvatarFile != null)
                    {
                        try
                        {
                            formModel.SavedFileName = _uploadService.SaveFormFile(
                                formModel.AvatarFile, 
                                "wwwroot/img/avatars");
                        }
                        catch (Exception ex) 
                        {
                            res[nameof(formModel.AvatarFile)] = ex.Message;
                        }
                        ////Отделяем расширение файла
                        //String ext = Path.GetExtension(formModel.AvatarFile.FileName);
                        ////Определяем место для сохранения
                        //String path = Directory.GetCurrentDirectory() + "/wwwroot/img/avatars/";
                        ////Генерируем новое имя для файла (старые нельзя сохранять, возможны конфликты, если
                        ////пользователи будут загружать файлы с одинаковыми именами
                        //String savedName = Guid.NewGuid().ToString() + ext; //берем на базе GUID, но сохраняем расширение
                        //                                                    //Сохраняем
                        //using var stream = System.IO.File.OpenWrite(path + savedName);
                        //formModel.AvatarFile.CopyTo(stream);
                        ////Передаем сохраненное имя в модель
                        ////В модели нужно создать дополнительное поле, чтобы это имя файла туда заложить
                        //formModel.SavedFileName = savedName;
                    }
                }

            }
            return res;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
