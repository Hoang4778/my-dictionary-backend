namespace backend.Models
{
    public class ServerResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public object? data { get; set; }
    }
}
