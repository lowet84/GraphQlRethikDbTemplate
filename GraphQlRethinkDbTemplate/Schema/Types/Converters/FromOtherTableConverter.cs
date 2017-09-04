using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GraphQlRethinkDbTemplate.Schema.Types.Converters
{
    public class FromOtherTableConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch (value)
            {
                case TypeBase single:
                    writer.WriteValue(single.Id.ToString());
                    break;
                case IEnumerable<TypeBase> array:
                    var ret = array.Select(d => d.Id.ToString()).ToArray();
                    var json = JsonConvert.SerializeObject(ret);
                    writer.WriteValue(json);
                    break;
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}
