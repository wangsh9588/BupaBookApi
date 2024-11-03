namespace Core.Models
{
    public class BookApiConfig
    {
        public string BaseUrl { get; set; } = string.Empty;

        public int Retry { get; set; }
    }
}
