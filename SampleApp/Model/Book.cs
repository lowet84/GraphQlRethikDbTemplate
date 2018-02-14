using GraphQlRethinkDbCore.Attributes;
using GraphQlRethinkDbCore.Schema.Types;
using GraphQL.Conventions;

namespace SampleApp.Model
{
    [Description("A book")]
    public class Book : NodeBase<Book>
    {
        public Book(string title, Author author)
        {
            Title = title;
            BookAuthors = new[] {new BookAuthor(author)};
        }

        [Description("The title of the book")]
        [SecondaryIndex]
        public string Title { get; }

        [Description("The author of the book")]
        public BookAuthor[] BookAuthors { get; }
    }

    public class BookAuthor
    {
        public BookAuthor(Author author)
        {
            Author = author;
        }

        public Author Author { get; }
    }
}
