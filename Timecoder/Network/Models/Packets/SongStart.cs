using System;
using Newtonsoft.Json;

namespace Timecoder.Network.Models.Packets
{
    [Serializable]
    internal class SongStart
    {
        public SongInfo Song { get; }
        public DateTime StartTime { get; }
        public long Timestamp { get; }

        public SongStart(SongInfo song, DateTime startTime)
        {
            Song = song;
            StartTime = startTime;
            Timestamp = startTime.ToUnixTime();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
