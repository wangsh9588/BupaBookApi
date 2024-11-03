using System.Text.Json.Serialization;

namespace Core.Models
{
    public class BookOwner
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("age")]
        public int Age { get; set; }

        [JsonPropertyName("books")]
        public IEnumerable<Book> Books { get; set; } = [];
    }
}
