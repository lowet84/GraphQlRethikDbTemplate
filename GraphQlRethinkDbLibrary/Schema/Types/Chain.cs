using System;
using GraphQlRethinkDbLibrary.Database;
using GraphQlRethinkDbLibrary.Schema.Converters;
using GraphQL.Conventions;
using Newtonsoft.Json;

namespace GraphQlRethinkDbLibrary.Schema.Types
{
    [Description("The chain")]
    public class Chain : NodeBase<Chain>
    {
        public static Chain CreateChainLink<T>(Id? currentId, Id? oldId = null)
        {
            var type = typeof(T);
            if (currentId!=null && !currentId.GetValueOrDefault().IsIdentifierForType<T>())
                throw new ArgumentException($"currentId is not id of type {type.Name}");
            if (oldId != null && !oldId.GetValueOrDefault().IsIdentifierForType<T>())
                throw new ArgumentException($"oldId is not id of type {type.Name}");
            if (currentId == null && oldId == null)
            {
                throw new ArgumentException("Both current and old is cannot be null at the same time");
            }

            Id linkId;
            long version = 0;
            if (oldId == null)
            {
                linkId = Utils.CreateNewId<Chain>();
            }
            else
            {
                var existingChain =
                    DbContext.Instance.FindChainLink(oldId.Value);
                linkId = existingChain.LinkId;
                version = existingChain.ChainVersion + 1;
            }

            return new Chain(currentId, linkId, version);
        }

        [JsonConverter(typeof(IdConverter))]
        public Id? CurrentId { get; }

        [JsonConverter(typeof(IdConverter))]
        public Id LinkId { get; }

        public long ChainVersion { get; }

        private Chain(Id? currentId, Id linkId, long chainVersion)
        {
            CurrentId = currentId;
            LinkId = linkId;
            ChainVersion = chainVersion;
        }
    }
}
