using System;
using GraphQlRethinkDbTemplate.Schema.Types.Converters;
using GraphQL.Conventions;
using GraphQL.Conventions.Relay;
using Newtonsoft.Json;

namespace GraphQlRethinkDbTemplate.Schema.Types
{
    public abstract class TypeBase<T> : TypeBase
    {
        protected TypeBase() : base(Id.New<T>(Guid.NewGuid().ToString()))
        {
        }
    }

    public abstract class TypeBase : INode
    {
        protected TypeBase(Id id)
        {
            Id = id;
        }

        [JsonConverter(typeof(IdConverter))]
        [JsonProperty(PropertyName = "id")]
        public Id Id { get; }
    }
}