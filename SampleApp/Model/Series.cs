using System.Linq;
using GraphQlRethinkDbLibrary.Schema.Types;
using GraphQL.Conventions;
using Newtonsoft.Json;

namespace SampleApp.Model
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
        public Author[] Authors => Books?.SelectMany(d => d.BookAuthors.Select(e=>e.Author)).ToArray();
    }
}
