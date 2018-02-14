using System.IO;
using System.Linq;
using GraphQlRethinkDbCore.Schema.Types;
using GraphQlRethinkDbLibrary.Handlers;
using Newtonsoft.Json;

namespace SampleApp.Model
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

        string IDefaultAudio.Key => Id.ToString();

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

    public class AudioFile : NodeBase<AudioFile>, IDefaultAudio
    {
        public string FileName { get; }

        public AudioFile(string fileName)
        {
            FileName = fileName;
        }


        string IDefaultAudio.Key => Id.ToString();

        [JsonIgnore]
        public string ContentType => "audio/mpeg";

        [JsonIgnore]
        public int BlockSize => 200000;

        [JsonIgnore]
        public long Length => new FileInfo(FileName).Length;
    }
}
