using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using GraphQlRethinkDbTemplate.Attributes;
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

        public static object CreateDummyObject(Type type, Id id)
        {


            var item = FormatterServices.GetUninitializedObject(type);
            var fields = item.GetType().BaseType.BaseType.GetFields(Flags);
            var idField = fields.First(d => d.Name.StartsWith("<Id>"));
            if (id.IsIdentifierForType(type))
            {
                idField.SetValue(item, id);
            }
            else
            {
                throw new ArgumentException($"Id is not identifyer for type {type}");
            }
            return item;
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

        public static T DeserializeJObject<T>(JObject jObject)
        {
            var type = typeof(T);
            var item = FormatterServices.GetUninitializedObject(type);
            var fields = GetFields(type);
            var properties = type.GetProperties().Select(d =>
                new
                {
                    Info = d,
                    Name = d.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? d.Name
                });
            foreach (var property in jObject.Properties())
            {
                var typeProperty = properties.First(d => d.Name == property.Name);
                var field = fields.First(d => d.Name.StartsWith($"<{typeProperty.Info.Name}>"));
            }
            return default(T);
        }

        private static FieldInfo[] GetFields(Type type)
        {
            var ret = new List<FieldInfo>();

            ret.AddRange(type.GetFields(Flags));
            if (type.BaseType != null)
            {
                ret.AddRange(GetFields(type.BaseType));
            }

            return ret.ToArray();
        }
    }
}
