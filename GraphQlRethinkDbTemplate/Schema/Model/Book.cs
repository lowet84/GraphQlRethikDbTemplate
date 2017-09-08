using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using GraphQlRethinkDbTemplate.Schema.Types;

namespace GraphQlRethinkDbTemplate.Schema.Model
{
    [Description("A book")]
    public class Book : NodeBase<Book>
    {
        public Book(string title, Author author)
        {
            Title = title;
            BookAuthor = new BookAuthor(author);
        }

        [Description("The title of the book")]
        public string Title { get; }

        [Description("The author of the book")]
        public BookAuthor BookAuthor { get; }
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
