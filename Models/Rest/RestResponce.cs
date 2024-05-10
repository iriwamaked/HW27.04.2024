namespace ASP1.Models.Rest
{
    public class RestResponce
    {
        public RestStatus Status { get; set; } = new() 
                { Code = 200, Message = "Ok", IsOk = true };
        public RestMeta    Meta { get; set; } = new();
        public Object      Data { get; set; } = new();

    }

    public class RestStatus
    {
        public Int32    Code    { get; set; }
        public String   Message { get; set; } = "";
        public Boolean  IsOk    { get; set; }
    }
    public class RestMeta
    {
        public String Service { get; set; } = "";
        public int Total { get; set; }
        public Int64 ServerTime { get; set; } = DateTime.Now.Ticks;
        public Dictionary<String, String> RequestParameters { get; set; } = [];
    }

}
