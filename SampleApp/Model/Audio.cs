using System.Linq;
using GraphQlRethinkDbLibrary.Handlers;
using GraphQlRethinkDbLibrary.Schema.Types;

namespace GraphQlRethinkDbTemplate.Model
{
    public class Audio : NodeBase<Audio>, IDefaultAudio
    {
        public Audio(AudioData[] audioData, string source, string contentType, int blockSize)
        {
            ContentType = contentType;
            AudioDataParts = audioData;
            BlockSize = blockSize;
            Source = source;
            Length = audioData.Sum(d => d.Length);
        }

        public string ContentType { get; }

        public int BlockSize { get; }

        public long Length { get; }

        public AudioData[] AudioDataParts { get; }

        public string Source { get; }


        public class AudioData : NodeBase<AudioData>
        {
            public AudioData(string data, int length)
            {
                Data = data;
                Length = length;
            }

            public string Data { get; }

            public int Length { get; }
        }
    }
}
