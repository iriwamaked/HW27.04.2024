
namespace ASP1.Services.Upload
{
    public class UploadServiceV1 : IUploadService
    {
        public string SaveFormFile(IFormFile formFile)
        {
            //делегируем и указываем дефолтный путь
            return SaveFormFile(formFile, "wwwroot/upload/");
        }

        public string SaveFormFile(IFormFile formFile, string path)
        {
            //делегируем дальше с пустым массивом передачи разрешенных расширений
            return SaveFormFile(formFile, path, []);
        }

        public string SaveFormFile(IFormFile formFile, string path, IEnumerable<string> extensionsAllowed)
        {
            //сюда не могут прийти нулевые пути и файл, кидаемся исключениями
            ArgumentNullException.ThrowIfNull(formFile, nameof(formFile));
            ArgumentNullException.ThrowIfNull(path, nameof(path));

            //Отделяем расширение файла
            String ext = Path.GetExtension(formFile.FileName);

            //Проверяем расширение на разрешение (исключаем .exe)
            //проверяем, если не пустое исключение и хоть что-то есть, но есть то, что мы прописали в исключениях
            if (extensionsAllowed.Any() && !extensionsAllowed.Any(e=> e==ext))
            {
                throw new Exception("Extension not allowed");
            };

            //Генерируем новое имя для файла (старые нельзя сохранять, возможны конфликты, если
            //пользователи будут загружать файлы с одинаковыми именами
            String savedName = Guid.NewGuid().ToString() + ext; //берем на базе GUID, но сохраняем расширение

            //Определяем место для сохранения
            String location = Path.Combine(Directory.GetCurrentDirectory(), path, savedName);

            //Сохраняем
            using var stream = System.IO.File.OpenWrite(location);
            formFile.CopyTo(stream);
            //передаем сохраненное имя как результат
            return savedName;
        }
    }
}
