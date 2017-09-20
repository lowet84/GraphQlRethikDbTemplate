using System;
using GraphQlRethinkDbLibrary.Schema;
using GraphQlRethinkDbLibrary.Schema.Types;
using GraphQL.Conventions;
using GraphQLParser.AST;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GraphQlRethinkDbLibrary.Database
{
    public partial class DbContext
    {
        public T AddDefault<T>(T item, Id? replaces = null) where T : NodeBase
        {
            var type = typeof(T);
            var table = GetTable(type);
            Utils.InitalizeArrays(item);
            var jObject = JObject.FromObject(item, new JsonSerializer
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            var jToken = Utils.ChangeTypeBaseItemsToIds(type, jObject);
            var chainLink = Chain.CreateChainLink<T>(item.Id, replaces);
            var result = table.Insert(jToken).Do_(e => GetTable(typeof(Chain)).Insert(chainLink)).RunResult(_connection);
            if (result.Errors > 0)
            {
                throw new Exception("Something went wrong");
            }

            return item;
        }

        public T ReadByIdDefault<T>(Id id, UserContext.ReadType readType, GraphQLDocument document) where T : class
        {
            var selectionSet = document != null ? GetSelectionSet(document) : null;

            switch (readType)
            {
                case UserContext.ReadType.WithDocument:
                    return GetWithDocument<T>(selectionSet, id);
                case UserContext.ReadType.Shallow:
                    return GetShallow<T>(id);
                default:
                    throw new ArgumentOutOfRangeException(nameof(readType), readType, null);
            }
        }

        public void Remove<T>(Id id)
        {
            var type = typeof(T);
            if (!id.IsIdentifierForType<T>())
                throw new Exception($"Id is not valid for type {type.Name}");
            var chainLink = Chain.CreateChainLink<T>(null, id);
            var result = GetTable(typeof(Chain)).Insert(chainLink).RunResult(_connection);
            if (result.Errors > 0)
            {
                throw new Exception("Something went wrong");
            }
        }

        public T[] GetArrayByIdDefault<T>(Id[] ids, GraphQLDocument document)
        {
            return GetWithDocument<T[]>(GetSelectionSet(document), ids);
        }

        public bool Restore<T>(Id id)
        {
            var type = typeof(T);
            if (!id.IsIdentifierForType<T>())
                throw new Exception($"Id is not valid for type {type.Name}");
            var exists = GetTable(type).GetAll(id.ToString())
                .Count()
                .Eq(1)
                .RunResult<bool>(_connection);
            if (!exists)
            {
                return false;
            }
            var chainLink = Chain.CreateChainLink<T>(id, id);
            var result = GetTable(typeof(Chain)).Insert(chainLink).RunResult(_connection);
            if (result.Errors > 0)
            {
                throw new Exception("Something went wrong");
            }
            return true;
        }
    }
}
