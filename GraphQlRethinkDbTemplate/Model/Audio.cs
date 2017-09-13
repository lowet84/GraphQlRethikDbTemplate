using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQlRethinkDbLibrary.Handlers;
using GraphQlRethinkDbLibrary.Schema.Types;

namespace GraphQlRethinkDbTemplate.Model
{
    public class Audio : NodeBase<Audio>, IDefaultAudio
    {
        public Audio(string audioData, string source, string contentType)
        {
            ContentType = contentType;
            AudioData = audioData;
            Source = source;
        }

        public string ContentType { get; }
        public string AudioData { get; }

        public string Source { get; }
    }
}
