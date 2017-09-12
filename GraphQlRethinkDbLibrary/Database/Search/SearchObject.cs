using System.Collections.Generic;

namespace GraphQlRethinkDbLibrary.Database.Search
{
    public class SearchObject<T>
    {
        private readonly List<SearchOperation> _operations = new List<SearchOperation>();

        public SearchObject<T> Add(SearchOperationType operation, string propertyName, string value)
        {
            _operations.Add(new SearchOperation(operation, propertyName, value));
            return this;
        }

        public IEnumerable<SearchOperation> Operations => _operations;
    }

    public class SearchOperation
    {
        public SearchOperationType OperationType { get; }
        public string PropertyName { get; }
        public string Value { get; }

        public SearchOperation(SearchOperationType operationType, string propertyName, string value)
        {
            OperationType = operationType;
            PropertyName = propertyName;
            Value = value;
        }
    }

    public enum SearchOperationType
    {
        Equals,
        Match,
        AnyEquals
    }
}
