using Newtonsoft.Json;
using System;

namespace Timecoder.Network.Models.Packets
{
    [Serializable]
    internal class SongRestart
    {
        public DateTime RestartTime { get; }
        public long Timestamp { get; }

        public SongRestart(DateTime restartTime)
        {
            RestartTime = restartTime;
            Timestamp = restartTime.ToUnixTime();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
