using Newtonsoft.Json;

namespace Core.Models
{
    public record Book
    {
        [JsonRequired]
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonRequired]
        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;
    }
}