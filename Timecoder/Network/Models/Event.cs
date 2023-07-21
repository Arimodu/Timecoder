using Newtonsoft.Json;
using System;

namespace Timecoder.Network.Models
{
    [Serializable]
    internal class Event<T>
    {
        public string Name { get; }
        public T Data { get; }

        public Event(EventName name, T data)
        {
            Name = name.ToString();
            Data = data;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    [Serializable]
    internal class Event
    {
        public string Id { get; }

        public Event(string id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public enum EventName
    {
        Song,
        Replay
    }
}
