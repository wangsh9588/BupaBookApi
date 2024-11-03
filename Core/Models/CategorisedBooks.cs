using System.Text.Json.Serialization;

namespace Core.Models
{
    public class CategorisedBooks
    {
        [JsonPropertyName("adultBooks")]
        public HashSet<string> AdultBooks { get; private set; } = [];

        [JsonPropertyName("childrenBooks")]
        public HashSet<string> ChildrenBooks { get; private set; } = [];

        public void SetAdultBooks(HashSet<string> adultBooks)
        {
            AdultBooks = adultBooks;
        }

        public void SetChildrenBooks(HashSet<string> childrenBooks)
        {
            ChildrenBooks = childrenBooks;
        }
    }
}
