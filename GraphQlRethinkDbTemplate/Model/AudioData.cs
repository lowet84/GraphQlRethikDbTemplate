using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQlRethinkDbLibrary.Handlers;
using GraphQlRethinkDbLibrary.Schema.Types;

namespace GraphQlRethinkDbTemplate.Model
{
    public class AudioData : NodeBase<AudioData>, IDefaultAudioData
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
