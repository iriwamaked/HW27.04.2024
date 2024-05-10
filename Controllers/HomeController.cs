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
        /*���������� �������� ������ ����������� ����� �����������. �� ������������
         ��������� ����� ��� �������� ���� ����������� (������). 
        � ����������� ����� �������� ������������� ��������� _logger*/

        private readonly ILogger<HomeController> _logger;

        /*��������� ������ ��������� (��������) - ���� � �������������� 
         private readonly - �� �� ��������� �������� �� ���� ����������� ���������.
        DIP - ��� ��������� - ��������� (��������). */

        private readonly IHashService _hashService;
        private readonly IKdfService _kdfService;

        private readonly DataAccessor _dataAccessor;
        private readonly IUploadService _uploadService;

        //������ �������� �� ������������
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

        /*�������� ������� �� ������ ����� (�����)
         * Nullable (?) �������� ��� ��, �� ����� ���� �� ����
         * ���� ������� ������ ��� �����, �� ������ ����������� �������� FormModel � PageModels
         */
        public IActionResult Models(HomeModelsFormModel? formModel)
        {
            HomeModelsPageModel model = new()
            {
                PageTitle = "����� � ASP",
                FormModel = formModel
            };
            return View(model);
        }

        public ViewResult IoS()
        {
            HomeIoCPageModel model = new()
            {
                HashExample = _hashService.Digest("123"),
                ServiceCode = _hashService.GetHashCode().ToString(), //�����, �� ����� ��������� ���-��� - ��� 
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
            //���������, ���� �� �����-�� ������ � ���� ���������� ������

            if (formModel?.UserEmail != null) //���� ���������� ������
            {
                if (pageModel.ValidationErrors.Count > 0) //� ������
                {
                    pageModel.Message = "��������� ��������";
                    pageModel.IsSuccess = false;
                }
                else
                {
                    //������������ ������������
                    _dataAccessor.UserDao.SignUpUser(mapUser(formModel));
                    pageModel.Message = "��������� ������";
                    pageModel.IsSuccess = true;
                }

            }
            //else
            //{
            //    pageModel.Message = "��������� ��������";
            //    pageModel.IsSuccess = false;
            //}
            return View(pageModel);
        }

        //�� ����� ������, �� ������ - �������� Entity, �������� �����������
        private Data.Entities.User mapUser(SignUpFormModel formModel)
        {
            String salt = Guid.NewGuid().ToString();//� ��� ���� ������ ��� ����, ���������� ���!
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
            Dictionary<String, String> res = new(); //������ ������� �� ������� � ���� �����
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
                //���������, ������������ �� ����� ��� � ��
                if (!_dataAccessor.UserDao.IsEmailFree(formModel.UserEmail))
                {
                    res[nameof(formModel.UserEmail)] = "Email in use";
                }
                //���������� � ���������
                if (!formModel.Confirm)
                {
                    res[nameof(formModel.Confirm)] = "Confirm expeced";
                }
                //���� ��� ������, �� ������������ ����-��������

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
                        ////�������� ���������� �����
                        //String ext = Path.GetExtension(formModel.AvatarFile.FileName);
                        ////���������� ����� ��� ����������
                        //String path = Directory.GetCurrentDirectory() + "/wwwroot/img/avatars/";
                        ////���������� ����� ��� ��� ����� (������ ������ ���������, �������� ���������, ����
                        ////������������ ����� ��������� ����� � ����������� �������
                        //String savedName = Guid.NewGuid().ToString() + ext; //����� �� ���� GUID, �� ��������� ����������
                        //                                                    //���������
                        //using var stream = System.IO.File.OpenWrite(path + savedName);
                        //formModel.AvatarFile.CopyTo(stream);
                        ////�������� ����������� ��� � ������
                        ////� ������ ����� ������� �������������� ����, ����� ��� ��� ����� ���� ��������
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
