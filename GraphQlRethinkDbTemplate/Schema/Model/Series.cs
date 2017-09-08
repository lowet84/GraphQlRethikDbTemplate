using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQlRethinkDbTemplate.Attributes;
using GraphQlRethinkDbTemplate.Schema.Types;
using GraphQL.Conventions;
using Newtonsoft.Json;

namespace GraphQlRethinkDbTemplate.Schema.Model
{
    [Description("A series of books")]
    public class Series : NodeBase<Series>
    {
        public Series(string name, Book[] books)
        {
            Name = name;
            Books = books;
        }

        [Description("Name of the book series")]
        public string Name { get; }

        [Description("The books in the series")]
        public Book[] Books { get; }

        [Description("All of the authors of the books"), JsonIgnore]
        public Author[] Authors => Books?.Select(d => d.BookAuthor.Author).ToArray();
    }
}
