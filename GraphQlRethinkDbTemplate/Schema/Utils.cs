using System;
using System.Linq;
using System.Reflection;
using GraphQlRethinkDbTemplate.Attributes;

namespace GraphQlRethinkDbTemplate.Schema
{
    public static class Utils
    {
        public static bool UsesDeafultDbRead(this Type type)
        {
            var attributes = type.GetTypeInfo().GetCustomAttributes();

            return attributes.Any(d => d is UseDeafultDbReadAttribute);
        }
    }
}
