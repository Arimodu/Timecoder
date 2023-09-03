using IPA.Loader;
using System.Linq;
using Timecoder.Network.Models;

namespace Timecoder.Utilities
{
    internal class Utils
    {
        internal static ReplaySource GetReplaySource()
        {
            if (IsScoreSaberReplay()) return ReplaySource.ScoreSaber;
            else if (IsBeatLeaderReplay()) return ReplaySource.BeatLeader;
            else return ReplaySource.None;
        }

        internal static bool IsBeatLeaderReplay() => 
            PluginManager.EnabledPlugins.Any(plugin => plugin.Id == "BeatLeader") 
            && BeatLeader.Replayer.ReplayerLauncher.IsStartedAsReplay;

        internal static bool IsScoreSaberReplay()
        {
            if (PluginManager.EnabledPlugins.Any(plugin => plugin.Id == "ScoreSaber"))
            {
                return (bool)PluginManager.GetPluginFromId("ScoreSaber")
                    .Assembly.GetType("ScoreSaber.Plugin.ReplayState")
                    .GetProperty("IsPlaybackEnabled")
                    .GetValue(null, null);
            }
            // Default to false if ScoreSaber is not installed
            return false;
        }

        internal static bool IsReplay() => IsScoreSaberReplay() || IsBeatLeaderReplay();
        internal static int GetMaxScore() => ScoreModel.ComputeMaxMultipliedScoreForBeatmap(BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.transformedBeatmapData);
    }
}
