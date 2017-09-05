using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using GraphQlRethinkDbTemplate.Attributes;
using GraphQL.Conventions;

namespace GraphQlRethinkDbTemplate.Schema
{
    public static class Utils
    {
        public static bool UsesDeafultDbRead(this Type type)
        {
            var attributes = type.GetTypeInfo().GetCustomAttributes();

            return attributes.Any(d => d is UseDefaultDbReadAttribute);
        }

        public static object CreateDummyObject(Type type, Id id)
        {
            var flags = BindingFlags.Instance
                        | BindingFlags.GetProperty
                        | BindingFlags.SetProperty
                        | BindingFlags.GetField
                        | BindingFlags.SetField
                        | BindingFlags.NonPublic;

            var item = FormatterServices.GetUninitializedObject(type);
            var fields = item.GetType().BaseType.BaseType.GetFields(flags);
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

        public static T[] AddOrInitializeArray<T>(this T[] array, params T[] items)
        {
            var list = new List<T>(array ?? new T[0]);
            list.AddRange(items);
            return list.ToArray();
        }
    }
}
