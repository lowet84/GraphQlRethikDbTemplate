using System.Collections.Generic;

namespace GraphQlRethinkDbLibrary.Database.Search
{
    public class SearchObject<T>
    {
        private readonly List<SearchOperation> _operations = new List<SearchOperation>();

        public SearchObject<T> Add(SearchOperationType operation, string propertyName, params string[] values)
        {
            _operations.Add(new SearchOperation(operation, propertyName, values));
            return this;
        }

        public IEnumerable<SearchOperation> Operations => _operations;
    }

    public class SearchOperation
    {
        public SearchOperationType OperationType { get; }
        public string PropertyName { get; }
        public string[] Values { get; }

        public SearchOperation(SearchOperationType operationType, string propertyName, string[] values)
        {
            OperationType = operationType;
            PropertyName = propertyName;
            Values = values;
        }
    }

    public enum SearchOperationType
    {
        Equals,
        Match,
        AnyEquals,
        MatchMultiple
    }
}
