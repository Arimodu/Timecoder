using Newtonsoft.Json;
using System;

namespace Timecoder.Network.Models.Packets
{
    [Serializable]
    internal class Packet<EventType, SubEventType>
    {
        public Session Session { get; set; }
        public Event<EventType> Event { get; set; }
        public SubEvent<SubEventType> SubEvent { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    [Serializable]
    internal class Packet<T>
    {
        public Session Session { get; set; }
        public Event Event { get; set; }
        public SubEvent<T> SubEvent { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
