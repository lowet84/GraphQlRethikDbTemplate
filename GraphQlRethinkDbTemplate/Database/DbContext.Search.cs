using GraphQlRethinkDbTemplate.Database.Search;

namespace GraphQlRethinkDbTemplate.Database
{
    public partial class DbContext
    {
        public T[] Search<T>(SearchObject<T> searchObject)
        {
            return null;
        }
    }
}
