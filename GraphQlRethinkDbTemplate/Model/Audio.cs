using System.Linq;
using GraphQlRethinkDbLibrary.Handlers;
using GraphQlRethinkDbLibrary.Schema.Types;
using Newtonsoft.Json;

namespace GraphQlRethinkDbTemplate.Model
{
    public class Audio : NodeBase<Audio>, IDefaultAudio
    {
        public Audio(AudioData[] audioData, string source, string contentType, int blockSize)
        {
            ContentType = contentType;
            AudioData = audioData;
            BlockSize = blockSize;
            Source = source;
            Length = audioData.Sum(d => d.Length);
        }

        public string ContentType { get; }

        public int BlockSize { get; }

        public long Length { get; }

        public AudioData[] AudioData { get; }

        public string Source { get; }
    }
}
