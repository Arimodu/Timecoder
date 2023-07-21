using System;
using Newtonsoft.Json;

namespace Timecoder.Network.Models.Packets
{
    [Serializable]
    internal class SongStart
    {
        public DateTime StartTime { get; }
        public long Timestamp { get; }

        public SongStart( DateTime startTime)
        {
            StartTime = startTime;
            Timestamp = startTime.ToUnixTime();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
