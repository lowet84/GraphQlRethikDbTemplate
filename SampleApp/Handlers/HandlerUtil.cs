using System;
using GraphQlRethinkDbCore;
using GraphQlRethinkDbLibrary;
using GraphQL.Conventions;

namespace SampleApp.Handlers
{
    public static class HandlerUtil
    {
        public static T Get<T>(Id id) where T : class
        {
            var type = typeof(T);
            if (!id.IsIdentifierForType<T>())
                throw new Exception($"Id is not valid for type {type.Name}");
            var item = UserContext.GetShallow<T>(id);
            return item;
        }
    }
}
