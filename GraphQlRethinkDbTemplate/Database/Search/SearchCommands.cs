namespace GraphQlRethinkDbTemplate.Database.Search
{
    public static class SearchCommands
    {
        public static SearchObject<T> Where<T>(this SearchObject<T> searchObject)
        {
            return searchObject;
        }
    }
}
