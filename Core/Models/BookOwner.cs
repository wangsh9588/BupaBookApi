using Newtonsoft.Json;

namespace Core.Models
{
    public class BookOwner
    {
        [JsonRequired]
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonRequired]
        [JsonProperty("age")]
        public int Age { get; set; }

        [JsonRequired]
        [JsonProperty("books")]
        public IEnumerable<Book> Books { get; set; } = [];
    }
}
