using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Conventions;
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
            var temp = new List<string>();
            while (true)
            {
                temp.Add(reader.ReadAsString());
            }
            try
            {
                var json = reader.ReadAsString();
                if (!json.StartsWith("{"))
                {
                    var id = new Id(json);
                    if (id.IsIdentifierForType(objectType))
                        return Utils.CreateDummyObject(objectType, id);
                    return null;
                }
                var ret = serializer.Deserialize(reader, objectType);
                return ret;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}
