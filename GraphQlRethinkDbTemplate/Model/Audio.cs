using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQlRethinkDbLibrary.Handlers;
using GraphQlRethinkDbLibrary.Schema.Types;
using Newtonsoft.Json;

namespace GraphQlRethinkDbTemplate.Model
{
    public class Audio : NodeBase<Audio>, IDefaultAudio
    {
        public Audio(AudioData[] audioData, string source, string contentType, int blockSize, int length)
        {
            ContentType = contentType;
            AudioData = audioData;
            BlockSize = blockSize;
            Source = source;
            Length = length;
        }

        public string ContentType { get; }
        public int BlockSize { get; }

        public int Length { get; }

        public AudioData[] AudioData { get; }

        public string Source { get; }

        IDefaultAudioData[] IDefaultAudio.AudioData => AudioData.Cast<IDefaultAudioData>().ToArray();
    }
}
