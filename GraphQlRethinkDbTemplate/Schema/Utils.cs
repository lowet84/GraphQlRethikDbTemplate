using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using GraphQlRethinkDbTemplate.Attributes;
using GraphQlRethinkDbTemplate.Schema.Types;
using GraphQL;
using GraphQL.Conventions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RethinkDb.Driver.Ast;

namespace GraphQlRethinkDbTemplate.Schema
{
    public static class Utils
    {
        const BindingFlags Flags = BindingFlags.Instance
                                   | BindingFlags.GetProperty
                                   | BindingFlags.SetProperty
                                   | BindingFlags.GetField
                                   | BindingFlags.SetField
                                   | BindingFlags.NonPublic;

        public static bool UsesDeafultDbRead(this Type type)
        {
            var attributes = type.GetTypeInfo().GetCustomAttributes();

            return attributes.Any(d => d is UseDefaultDbReadAttribute);
        }

        private static object CreateEmptyObject(Type type)
        {
            return FormatterServices.GetUninitializedObject(type);
        }

        public static object CreateDummyObject(Type type, Id id)
        {
            if (!id.IsIdentifierForType(type))
            {
                throw new ArgumentException($"Id is not identifyer for type {type}");
            }
            var item = CreateEmptyObject(type);
            ForceSetValue(item, "Id", id);
            return item;
        }

        private static void ForceSetValue(object item, string propertyName, object value)
        {
            var fields = GetFields(item.GetType());
            var field = fields.First(d => d.Name.StartsWith($"<{propertyName}>"));
            field.SetValue(item, value);
        }

        public static T CreateDummyObject<T>(Id id) where T : class
        {
            var item = CreateDummyObject(typeof(T), id);
            return item as T;
        }

        public static T[] AddOrInitializeArray<T>(T[] array, params T[] items)
        {
            var list = new List<T>(array ?? new T[0]);
            list.AddRange(items);
            return list.ToArray();
        }

        public static object DeserializeObject(Type type, JToken jToken)
        {
            switch (jToken.Type)
            {
                case JTokenType.Object:
                    var ret = CreateEmptyObject(type);
                    var properties = type.GetProperties();
                    foreach (var property in properties)
                    {
                        var jPropertyName = property.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? property.Name;
                        var value = jToken[jPropertyName];
                        if (value == null) continue;

                        var deserializedValue = DeserializeObject(property.PropertyType, value);
                        ForceSetValue(ret, property.Name, deserializedValue);
                    }
                    return ret;
                case JTokenType.Array:
                    var arrayType = type.GetElementType();
                    var arrayValues = jToken.ToList();
                    var arrayRetTemp = arrayValues.Select(d => DeserializeObject(arrayType, d)).ToArray();
                    var arrayRet = Array.CreateInstance(arrayType, arrayRetTemp.Length);
                    for (int index = 0; index < arrayRetTemp.Length; index++)
                    {
                        arrayRet.SetValue(arrayRetTemp[index],index);
                    }
                    return arrayRet;
                case JTokenType.String:
                    var strVal = jToken.GetValue().ToString();
                    if(type == typeof(Id))
                        return new Id(strVal);
                    if (!type.IsTypeBase()) return strVal;

                    var dummyRet = CreateDummyObject(type, new Id(strVal));
                    return dummyRet;
            }
            throw new NotImplementedException();
        }



        public static bool IsTypeBase(this Type type)
        {
            return typeof(TypeBase).IsAssignableFrom(type)
                || typeof(TypeBase).IsAssignableFrom(type.GetElementType());
        }

        private static FieldInfo[] GetFields(Type type)
        {
            var ret = new List<FieldInfo>(type.GetFields(Flags));
            if (type.BaseType != null)
            {
                ret.AddRange(GetFields(type.BaseType));
            }
            return ret.ToArray();
        }









        //private static object GetValue(PropertyInfo propertyInfo, JToken jToken, string jPropertyName)
        //{
        //    var jProperties = jToken.ToDictionary(d => d.Path, d => d);
        //    if (propertyInfo.PropertyType == typeof(string))
        //    {
        //        var value = jProperties[jPropertyName].GetValue();
        //    }
        //    //var property = jToken

        //    return null;
        //}

        //private static JObject Normalize<T>(string json)
        //{
        //    var jObject = JObject.Parse(json);
        //    var properties = typeof(T).GetProperties().Where(d => d.IsTypeBase()).ToList();
        //    foreach (var property in properties)
        //    {
        //        var jPropertyName = property.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? property.Name;
        //        if (property.PropertyType.IsArray)
        //        {
        //            var type = property.PropertyType.GetElementType();
        //            if (jObject.GetValue(jPropertyName).All(d => d.Type == JTokenType.String))
        //            {
        //                var ids = jObject.GetValue(jPropertyName).Select(d => d.ToString()).ToList();
        //                var dummys = ids.Select(d => CreateDummyObject(type, new Id(d)));
        //                jObject[jPropertyName] = new JArray(dummys.Select(JObject.FromObject));
        //            }
        //        }
        //        else
        //        {
        //            if (jObject.GetValue(jPropertyName).Type != JTokenType.String) continue;

        //            var type = property.PropertyType;
        //            var dummy = CreateDummyObject(type, new Id(jObject.GetValue(jPropertyName).ToString()));
        //            var newJObject = JObject.FromObject(dummy);
        //            jObject[jPropertyName] = newJObject;
        //        }
        //    }

        //    return jObject;
        //}

        //private static void FixIds(object item, string json)
        //{
        //    var jObject = JObject.Parse(json);
        //    var properties = item.GetType().GetProperties().Where(d => d.IsTypeBase()).ToList();
        //}
    }
}
