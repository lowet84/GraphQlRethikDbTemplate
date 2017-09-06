using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Conventions;
using Newtonsoft.Json;

namespace GraphQlRethinkDbTemplate.Schema.Types.Converters
{
    public class OtherTableConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch (value)
            {
                case TypeBase single:
                    writer.WriteValue(single.Id.ToString());
                    break;
                case IEnumerable<TypeBase> array:
                    writer.WriteStartArray();
                    foreach (var item in array)
                    {
                        writer.WriteValue(item.Id.ToString());
                    }
                    writer.WriteEndArray();
                    break;
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}
