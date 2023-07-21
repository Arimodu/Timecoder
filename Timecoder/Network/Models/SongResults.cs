using Newtonsoft.Json;
using System;
using static Timecoder.Utilities.Utils;

namespace Timecoder.Network.Models
{
    [Serializable]
    internal class SongResults
    {
        public int Score { get; }
        public int CalculatedMaxScoreV2 { get; }
        public float CalculatedAccuracyV2 { get; }
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
            CalculatedMaxScoreV2 = CalculateV2MaxScore(results.goodCutsCount + results.badCutsCount + results.missedCount);
            CalculatedAccuracyV2 = (float)Decimal.Divide(Score, CalculatedMaxScoreV2) * 100;
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
