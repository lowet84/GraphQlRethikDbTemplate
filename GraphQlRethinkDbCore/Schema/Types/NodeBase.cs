using GraphQlRethinkDbCore.Schema.Converters;
using GraphQL.Conventions;
using GraphQL.Conventions.Relay;
using Newtonsoft.Json;

namespace GraphQlRethinkDbCore.Schema.Types
{
    public abstract class NodeBase<T> : NodeBase
    {
        protected NodeBase() : base(Utils.CreateNewId<T>())
        {
        }
    }

    public abstract class NodeBase : INode
    {
        protected NodeBase(Id id)
        {
            Id = id;
        }

        [JsonConverter(typeof(IdConverter))]
        [JsonProperty(PropertyName = "id")]
        public Id Id { get; }
    }
}