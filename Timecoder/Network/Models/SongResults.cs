using Newtonsoft.Json;
using System;
using static Timecoder.Utilities.Utils;

namespace Timecoder.Network.Models
{
    [Serializable]
    internal class SongResults
    {
        public int Score { get; }
        public int MaxScore { get; }
        public float Accuracy { get; }
        public int Missed { get; }
        public int BadCuts { get; }
        public int GoodCuts { get; }
        public int MaxCombo { get; }
        public string Rank { get; }
        public bool FullCombo { get; }
        public SongModifiers Modifiers { get; }

        public SongResults(LevelCompletionResults results)
        {
            Score = results.modifiedScore;
            MaxScore = GetMaxScore();
            Accuracy = (float)Decimal.Divide(Score, MaxScore) * 100;
            Missed = results.missedCount;
            BadCuts = results.badCutsCount;
            GoodCuts = results.goodCutsCount;
            MaxCombo = results.maxCombo;
            Rank = results.rank.ToString();
            FullCombo = results.fullCombo;
            Modifiers = new SongModifiers(results.gameplayModifiers);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
