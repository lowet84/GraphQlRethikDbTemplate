using System.IO;
using GraphQlRethinkDbLibrary.Handlers;
using GraphQlRethinkDbLibrary.Schema.Types;
using Newtonsoft.Json;

namespace GraphQlRethinkDbTemplate.Model
{
    public class AudioFile : NodeBase<AudioFile>, IDefaultAudio
    {
        public string FileName { get; }

        public AudioFile(string fileName)
        {
            FileName = fileName;
        }

        [JsonIgnore]
        public string ContentType => "audio/mpeg";

        [JsonIgnore]
        public int BlockSize => 200000;

        [JsonIgnore]
        public long Length => new FileInfo(FileName).Length;
    }
}
