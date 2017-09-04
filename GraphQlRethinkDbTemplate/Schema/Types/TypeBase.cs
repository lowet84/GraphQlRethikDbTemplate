using System;
using System.Collections.Specialized;
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

        [JsonProperty(PropertyName = "id")]
        [JsonConverter(typeof(IdConverter))]
        public Id Id { get; }
    }
}