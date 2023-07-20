using Newtonsoft.Json;
using System;

namespace Timecoder.Network.Models
{
    [Serializable]
    internal class SubEvent<T>
    {
        public string Name { get; }
        public T Data { get; }

        public SubEvent(SubEventName name, T data)
        {
            Name = name.ToString();
            Data = data;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public enum SubEventName
    {
        Start,
        End,
        Restart
    }
}
