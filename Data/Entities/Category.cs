namespace ASP1.Data.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        //Slug - делаем для красоты. Это ресурсный идентификатор.
        //Часть url-адреса, который идентицирует ресурс, используется, чтобы делать url на категорию
        //не url/id, а url/изделия из камня
        // то есть позволяют однозначно идентифицировать ресурс и являются человекопонятными 
        //и используются для красивых адресо
        public string   Slug { get; set; } = null!;
        public string   Name { get; set; } = null!;
        public string   Description { get; set; } = null!;
        public string   ImageUrl { get; set; } = null!;
        //активная или не активная категория, по дефолту - активная, если не указано иное
        public Boolean  IsActive { get; set; } = true;

    }
}
