using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using GraphQlRethinkDbTemplate.Attributes;
using GraphQlRethinkDbTemplate.Schema.Types;
using GraphQL;
using GraphQL.Conventions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GraphQlRethinkDbTemplate.Schema
{
    public static class Utils
    {
        private const BindingFlags Flags =
            BindingFlags.Instance
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
                    return HandleObject(type, jToken);
                case JTokenType.Array:
                    return HandleArray(type, jToken);
                case JTokenType.String:
                    return HandleString(type, jToken);
            }
            throw new NotImplementedException($"Type: {jToken.Type.ToString()} is not handled yet");
        }

        private static object HandleString(Type type, JToken jToken)
        {
            var strVal = jToken.GetValue().ToString();
            if (type == typeof(Id))
                return new Id(strVal);
            if (!type.IsTypeBase()) return strVal;

            var dummyRet = CreateDummyObject(type, new Id(strVal));
            return dummyRet;
        }

        private static object HandleArray(Type type, JToken jToken)
        {
            var arrayType = type.GetElementType();
            var arrayValues = jToken.ToList();
            var arrayRetTemp = arrayValues.Select(d => DeserializeObject(arrayType, d)).ToArray();
            var arrayRet = Array.CreateInstance(arrayType, arrayRetTemp.Length);
            for (var index = 0; index < arrayRetTemp.Length; index++)
            {
                arrayRet.SetValue(arrayRetTemp[index], index);
            }
            return arrayRet;
        }

        private static object HandleObject(Type type, JToken jToken)
        {
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
        }

        public static bool IsTypeBase(this Type type)
        {
            return typeof(TypeBase).IsAssignableFrom(type)
                || typeof(TypeBase).IsAssignableFrom(type.GetElementType());
        }

        private static IEnumerable<FieldInfo> GetFields(Type type)
        {
            var ret = new List<FieldInfo>(type.GetFields(Flags));
            if (type.BaseType != null)
            {
                ret.AddRange(GetFields(type.BaseType));
            }
            return ret.ToArray();
        }
    }
}
