using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQlRethinkDbLibrary.Handlers;
using GraphQlRethinkDbLibrary.Schema.Types;
using GraphQL.Conventions;

namespace GraphQlRethinkDbTemplate.Model
{
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
