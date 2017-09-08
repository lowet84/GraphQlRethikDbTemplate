using System;
using GraphQlRethinkDbTemplate.Schema.Types.Converters;
using GraphQL.Conventions;
using GraphQL.Conventions.Relay;
using Newtonsoft.Json;

namespace GraphQlRethinkDbTemplate.Schema.Types
{
    public abstract class NodeBase<T> : NodeBase
    {
        protected NodeBase() : base(Id.New<T>(Guid.NewGuid().ToString()))
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