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
            //if (objectType.IsArray)
            //{
            //    var array = serializer.Deserialize(reader, objectType);
            //    return array;
            //    //var type = objectType.GetElementType();
            //    //var listType = typeof(List<>);
            //    //var constructedListType = listType.MakeGenericType(type);

            //    //var ret = Activator.CreateInstance(constructedListType);

            //    //var go = true;
            //    //while (go)
            //    //{
            //    //    var item = DeserializeObject(reader, type, serializer);
            //    //    if (item != null)
            //    //    {
            //    //        constructedListType.GetMethod("Add").Invoke(ret, new[] {item});
            //    //    }
            //    //    else
            //    //    {
            //    //        go = false;
            //    //    }
            //    //}
            //    //var array = constructedListType.GetMethod("ToArray").Invoke(ret, new object[0]);
            //    //return array;
            //}
            //else
            //{
            //    var ret = serializer.Deserialize(reader, objectType);
            //    return ret;
            //}
        }

        //private object DeserializeObject(JsonReader reader, Type objectType, JsonSerializer serializer)
        //{
        //    try
        //    {
        //        var json = reader.ReadAsString();
        //        if (!json.StartsWith("{"))
        //        {
        //            var id = new Id(json);
        //            if (id.IsIdentifierForType(objectType))
        //                return Utils.CreateDummyObject(objectType, id);
        //            return null;
        //        }
        //        var ret = serializer.Deserialize(reader, objectType);
        //        return ret;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}
