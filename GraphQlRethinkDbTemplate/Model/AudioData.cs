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
        public AudioData(string data)
        {
            Data = data;
        }

        public string Data { get; }
    }
}
