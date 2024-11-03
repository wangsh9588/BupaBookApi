using Core.Models;

namespace BupaBookApi.Tests
{
    public class TestDataHelper
    {
        public List<BookOwner> GetBookOwners()
        {
            return new List<BookOwner>
            {
                new BookOwner
                {
                    Name = "Jane",
                    Age = 23,
                    Books = new List<Book>
                    {
                        new Book
                        {
                            Name = "Hamlet",
                            Type = "Hardcover"
                        },
                        new Book
                        {
                            Name = "Wuthering Heights",
                            Type = "Paperback"
                        }
                    }
                },
                new BookOwner
                {
                    Name = "Charlotte",
                    Age = 14,
                    Books = new List<Book>
                    {
                        new Book
                        {
                            Name = "Hamlet",
                            Type = "Paperback"
                        }
                    }
                },
                new BookOwner
                {
                    Name = "Max",
                    Age = 25,
                    Books = new List<Book>
                    {
                        new Book
                        {
                            Name = "React: The Ultimate Guide",
                            Type = "Hardcover"
                        },
                        new Book
                        {
                            Name = "Gulliver's Travels",
                            Type = "Hardcover"
                        },
                        new Book
                        {
                            Name = "Jane Eyre",
                            Type = "Paperback"
                        },
                        new Book
                        {
                            Name = "Great Expectations",
                            Type = "Hardcover"
                        }
                    }
                },
                new BookOwner
                {
                    Name = "William",
                    Age = 15,
                    Books = new List<Book>
                    {
                        new Book
                        {
                            Name = "Great Expectations",
                            Type = "Hardcover"
                        }
                    }
                },
                new BookOwner
                {
                    Name = "Charles",
                    Age = 17,
                    Books = new List<Book>
                    {
                        new Book
                        {
                            Name = "Little Red Riding Hood",
                            Type = "Hardcover"
                        },
                        new Book
                        {
                            Name = "The Hobbit",
                            Type = "EBook"
                        }
                    }
                }
            };
        }
    }
}
