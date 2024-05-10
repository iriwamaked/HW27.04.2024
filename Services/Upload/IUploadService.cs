namespace ASP1.Services.Upload
{
    public interface IUploadService
    {
        String SaveFormFile(IFormFile formFile); //просто сохраняет в дефолтном месте файл
        String SaveFormFile(IFormFile formFile, String path); //перегрузка с дополнительным путем, куда мы будем сохранять
        String SaveFormFile(IFormFile formFile, String path, 
            IEnumerable<String> extensionsAllowed); //добавляем перечень допустимых расширений для доп. контроля
    }
}
