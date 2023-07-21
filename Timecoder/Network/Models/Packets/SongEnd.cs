using Newtonsoft.Json;
using System;

namespace Timecoder.Network.Models.Packets
{
    [Serializable]
    internal class SongEnd
    {
        public enum SongEndReason
        {
            Cleared,
            Quit,
            Failed
        }

        public SongResults Results { get; }
        public string Reason { get; }
        public DateTime EndTime { get; }
        public long Timestamp { get; }

        public SongEnd(SongResults results, SongEndReason reason, DateTime endTime)
        {
            Results = results;
            Reason = reason.ToString();
            EndTime = endTime;
            Timestamp = endTime.ToUnixTime();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
