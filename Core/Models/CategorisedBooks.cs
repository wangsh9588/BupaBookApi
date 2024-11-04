using Newtonsoft.Json;

namespace Core.Models
{
    public class CategorisedBooks
    {
        [JsonProperty("adultBooks")]
        public HashSet<string> AdultBooks { get; private set; } = [];

        [JsonProperty("childrenBooks")]
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
