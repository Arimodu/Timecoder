using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timecoder.Network.Models
{
    [Serializable]
    internal class ReplayInfo : SongInfo
    {
        public string ReplaySource { get; }

        public ReplayInfo(string levelId, string songName, string songSubName, string songAuthorName, string levelAuthorName, ReplaySource replaySource)
            : base(levelId, songName, songSubName, songAuthorName, levelAuthorName)
        {
            ReplaySource = replaySource.ToString();
        }

        public override string ToString() => base.ToString();
    }
    public enum ReplaySource
    {
        None,
        ScoreSaber,
        BeatLeader
    }
}
