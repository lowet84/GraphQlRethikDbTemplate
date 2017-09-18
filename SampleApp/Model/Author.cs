using GraphQlRethinkDbLibrary.Schema.Types;

namespace SampleApp.Model
{
    public class Author : NodeBase<Author>
    {
        public Author(string fistName, string lastName)
        {
            Name = new Name(fistName, lastName);
        }

        public Name Name { get; }
    }

    public class Name
    {
        public Name(string fistName, string lastName)
        {
            FistName = fistName;
            LastName = lastName;
        }

        public string FistName { get; }
        public string LastName { get; }
    }
}
