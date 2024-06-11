namespace Onyx_POS.Models
{
    public class SpModel
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public DateTime Modify_Date { get; set; }
    }
    public class CommonResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string RedirectUrl { get; set; }
        public dynamic Data { get; set; }
    }
}
