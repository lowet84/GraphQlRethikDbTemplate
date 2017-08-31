using System;
using GraphQL.Conventions;
using GraphQL.Conventions.Relay;
using Newtonsoft.Json;

namespace GraphQlRethikDbTemplate.Schema.Types
{
    public abstract class TypeBase<T> : INode
    {
        protected TypeBase()
        {
            Id = Id.New<T>(Guid.NewGuid().ToString());
        }

        [JsonProperty(PropertyName = "id")]
        [JsonConverter(typeof(IdConverter))]
        public Id Id { get; }
    }

    internal class IdConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var id = objectType.GetConstructor(new[] {typeof(String)}).Invoke(new[] {reader.Value.ToString()});
            return id;
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}