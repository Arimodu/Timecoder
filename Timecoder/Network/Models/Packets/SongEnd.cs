using Newtonsoft.Json;
using System;

namespace Timecoder.Network.Models.Packets
{
    [Serializable]
    internal class SongEnd
    {
        public enum SongEndReason
        {
            Finished,
            Quit,
            Failed
        }

        public string Reason { get; }
        public DateTime EndTime { get; }
        public long Timestamp { get; }

        public SongEnd(SongEndReason reason, DateTime endTime)
        {
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
