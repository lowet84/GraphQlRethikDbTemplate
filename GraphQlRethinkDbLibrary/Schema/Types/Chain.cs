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
        public static Chain CreateChainLink<T>(Id currentId, Id? oldId = null)
        {
            var type = typeof(T);
            if (!currentId.IsIdentifierForType<T>())
                throw new ArgumentException($"currentId is not id of type {type.Name}");
            if (oldId != null && !oldId.GetValueOrDefault().IsIdentifierForType<T>())
                throw new ArgumentException($"oldId is not id of type {type.Name}");

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

            return new Chain(currentId, oldId, linkId, version);
        }

        [JsonConverter(typeof(IdConverter))]
        public Id CurrentId { get; }

        [JsonConverter(typeof(IdConverter))]
        public Id? OldId { get; }

        [JsonConverter(typeof(IdConverter))]
        public Id LinkId { get; }

        public long ChainVersion { get; }

        private Chain(Id currentId, Id? oldId, Id linkId, long chainVersion)
        {
            CurrentId = currentId;
            OldId = oldId;
            LinkId = linkId;
            ChainVersion = chainVersion;
        }
    }
}
