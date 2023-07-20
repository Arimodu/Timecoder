using System;

namespace Timecoder.Network.Models
{
    [Serializable]
    internal class SongInfo
    {
        public string LevelID { get; }
        public string SongName { get; }
        public string SongSubName { get; }
        public string SongAuthorName { get; }
        public string LevelAuthorName { get; }

        public SongInfo(string levelID, string songName, string songSubName, string songAuthorName, string levelAuthorName)
        {
            LevelID = levelID;
            SongName = songName;
            SongSubName = songSubName;
            SongAuthorName = songAuthorName;
            LevelAuthorName = levelAuthorName;
        }

        public override string ToString()
        {
            return $"{SongName} by {SongAuthorName} mapped by {LevelAuthorName}";
        }
    }
}
