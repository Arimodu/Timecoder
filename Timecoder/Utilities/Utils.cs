using System.Reflection;
using HarmonyLib;

namespace Timecoder.Utilities
{
    internal class Utils
    {
        // Stolen from SongPlayHistory
        // REPOSITORY: https://github.com/qe201020335/SongPlayHistory
        // FILE: https://github.com/qe201020335/SongPlayHistory/blob/master/SongPlayHistory/Utils/Utils.cs
        // LICENSE: https://github.com/qe201020335/SongPlayHistory/blob/master/LICENSE

        private static readonly MethodBase ScoreSaber_playbackEnabled =
            AccessTools.Method("ScoreSaber.Core.ReplaySystem.HarmonyPatches.PatchHandleHMDUnmounted:Prefix");

        private static readonly MethodBase GetBeatLeaderIsStartedAsReplay =
            AccessTools.Property(AccessTools.TypeByName("BeatLeader.Replayer.ReplayerLauncher"), "IsStartedAsReplay")?.GetGetMethod(false);


        internal static bool IsReplay()
        {
            var ssReplay = ScoreSaber_playbackEnabled != null && (bool)ScoreSaber_playbackEnabled.Invoke(null, null) == false;

            var blReplay = GetBeatLeaderIsStartedAsReplay != null && (bool)GetBeatLeaderIsStartedAsReplay.Invoke(null, null);

            return ssReplay || blReplay;
        }

        // ----------------------------------
        // FILE: https://github.com/qe201020335/SongPlayHistory/blob/master/SongPlayHistory/Utils/ScoreUtils.cs

        internal static int CalculateV2MaxScore(int noteCount)
        {
            int effectiveNoteCount = 0;
            int multiplier;
            for (multiplier = 1; multiplier < 8; multiplier *= 2)
            {
                if (noteCount < multiplier * 2)
                {
                    effectiveNoteCount += multiplier * noteCount;
                    noteCount = 0;
                    break;
                }
                effectiveNoteCount += multiplier * multiplier * 2 + multiplier;
                noteCount -= multiplier * 2;
            }
            effectiveNoteCount += noteCount * multiplier;
            return effectiveNoteCount * 115;
        }
    }
}
