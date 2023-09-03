using System;
using Zenject;
using BS_Utils.Utilities;
using SiraUtil.Logging;
using Newtonsoft.Json.Linq;
using Timecoder.Network;
using Timecoder.Network.Models;
using Timecoder.Network.Models.Packets;
using static Timecoder.Utilities.Utils;
using System.Threading.Tasks;
using ReplayInfo = Timecoder.Network.Models.ReplayInfo;
using System.Text;
using static SliderController.Pool;

namespace Timecoder
{
    internal class TCEventManager : IInitializable
    {
        private readonly UWRWrapper UWR;
        private readonly SiraLog SiraLogger;

        private Session Session;
        private string SongEventId;
        private bool IsReplay;
        private StringBuilder EndNotice;

        internal TCEventManager(UWRWrapper uwr, SiraLog log) 
        {
            UWR = uwr;
            SiraLogger = log;

#if DEBUG
            SiraLogger.DebugMode = true;
#endif
        }

        public void Initialize() => InitializeAsync();

        private async void InitializeAsync()
        {
            var sessionSuccess = await SetupSession();

            if (sessionSuccess) BindEvents();
        }

        private void BindEvents()
        {
            BSEvents.gameSceneLoaded += OnGameSceneLoaded;

            BSEvents.levelRestarted += OnLevelRestarted;
            BSEvents.levelCleared += OnLevelCleared;
            BSEvents.levelQuit += OnLevelQuit;
            BSEvents.levelFailed += OnLevelFailed;
        }

        private void UnbindEvents()
        {
            BSEvents.gameSceneLoaded -= OnGameSceneLoaded;

            BSEvents.levelRestarted -= OnLevelRestarted;
            BSEvents.levelCleared -= OnLevelCleared;
            BSEvents.levelQuit -= OnLevelQuit;
            BSEvents.levelFailed -= OnLevelFailed;
        }

        public async Task<bool> SetupSession()
        {
            var response = await UWR.GetSession();

            if (!response.IsSuccess)
            {
                SiraLogger.Error($"Server did not return a session! Response code is: {response.StatusCode} {response.ErrorMessage}. Response data: {response.ResponseData}");
                return false; //For now anyway, not handling this
            }

            //SiraLogger.Debug($"Session response:\n{response}");

            // Parse the JSON response using JObject
            var parsedObject = JObject.Parse(response.ResponseData);

            // Extract the SessionId
            Session = new Session(parsedObject["session"]["id"].ToString());

            SiraLogger.Logger.Notice($"Got session id {Session.Id} from server!");

            return true;
        }

        private async void OnLevelRestarted(StandardLevelScenesTransitionSetupDataSO arg1, LevelCompletionResults arg2)
        {
            if (Session == null || SongEventId == string.Empty)
            {
                SiraLogger.Warn($"Empty session or song not started on LevelRestarted");
                return; // We dont want to do anything when we dont have a session
            }

            var time = DateTime.UtcNow;

            var packet = new Packet<SongRestart>
            {
                Session = Session,
                Event = new Event(SongEventId),
                SubEvent = new SubEvent<SongRestart>(SubEventName.Restart, new SongRestart(time))
            };


            SiraLogger.Logger.Notice($"Restarted {EndNotice}");

            if (!await SendLevelEndData(packet)) return;
        }

        private async Task<bool> SendLevelEndData<T>(Packet<T> packet)
        {
            var response = await UWR.SendSubEventData(packet);

            if (!response.IsSuccess)
            {
                SiraLogger.Error(response.ToString());
                return false;
            }

            //SiraLogger.Debug(response.ToString());

            return true;
        }

        private async void OnLevelFailed(StandardLevelScenesTransitionSetupDataSO arg1, LevelCompletionResults levelCompletionResults)
        {
            EndNotice.Insert(0, "Failed ");
            await SetupLevelEndData(levelCompletionResults, SongEnd.SongEndReason.Failed);
        }

        private async void OnLevelQuit(StandardLevelScenesTransitionSetupDataSO arg1, LevelCompletionResults levelCompletionResults)
        {
            EndNotice.Insert(0, IsReplay ? "Finished " : "Quit ");
            await SetupLevelEndData(levelCompletionResults, SongEnd.SongEndReason.Quit);
        }

        private async void OnLevelCleared(StandardLevelScenesTransitionSetupDataSO arg1, LevelCompletionResults levelCompletionResults)
        {
            EndNotice.Insert(0, "Cleared ");
            await SetupLevelEndData(levelCompletionResults, SongEnd.SongEndReason.Cleared);
        }

        private async Task SetupLevelEndData(LevelCompletionResults levelCompletionResults, SongEnd.SongEndReason reason)
        {
            if (Session == null || SongEventId == string.Empty)
            {
                //SiraLogger.Warn($"Empty session, or song not started on LevelRestarted");
                return; // We dont want to do anything when we dont have a session
            }

            var time = DateTime.UtcNow;

            var packet = new Packet<SongEnd>
            {
                Session = Session,
                Event = new Event(SongEventId),
                SubEvent = new SubEvent<SongEnd>(SubEventName.End, new SongEnd(new SongResults(levelCompletionResults), reason, time))
            };

            // Print the end notice
            SiraLogger.Logger.Notice(EndNotice.ToString());

            // Clear song event ID, we dont need it anymore
            // Should fix double event invocation, tho this is an ugly fix....
            SongEventId = string.Empty;

            if (!await SendLevelEndData(packet)) return;
        }

        private void OnGameSceneLoaded()
        {
            if (Session == null)
            {
                SiraLogger.Warn($"Empty session on GameSceneLoaded");
                return; // We don't want to do anything when we don't have a session
            }

            IsReplay = IsReplay();

            if (IsReplay) OnReplayStarted();
            else OnSongStarted();
        }

        public void OnSongStarted()
        {
            var beatmapLevel = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.previewBeatmapLevel;
            var songInfo = new SongInfo(
                beatmapLevel.levelID,
                beatmapLevel.songName,
                beatmapLevel.songSubName,
                beatmapLevel.songAuthorName,
                beatmapLevel.levelAuthorName);

            SiraLogger.Logger.Notice($"Started level: {songInfo}");
            EndNotice = new StringBuilder();
            EndNotice.Append($"level: {songInfo}");

            SendLevelStartData(songInfo);
        }

        public void OnReplayStarted()
        {
            var beatmapLevel = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.previewBeatmapLevel;
            ReplaySource replaySource = GetReplaySource();

            var replayInfo = new ReplayInfo(
                beatmapLevel.levelID,
                beatmapLevel.songName,
                beatmapLevel.songSubName,
                beatmapLevel.songAuthorName,
                beatmapLevel.levelAuthorName,
                replaySource);

            SiraLogger.Logger.Notice($"Started replay: {replayInfo}");
            EndNotice = new StringBuilder();
            EndNotice.Append($"replay: {replayInfo}");

            SendLevelStartData(replayInfo);
        }

        private async void SendLevelStartData<T>(T tInfo)
        {
            var time = DateTime.UtcNow;

            var packet = new Packet<T, SongStart>
            {
                Session = Session,
                Event = new Event<T>(EventName.Replay, tInfo),
                SubEvent = new SubEvent<SongStart>(SubEventName.Start, new SongStart(time))
            };

            var response = await UWR.SendSubEventData(packet);

            if (!response.IsSuccess)
            {
                SiraLogger.Error(response.ToString());
                return;
            }

            //SiraLogger.Debug(response.ToString());

            // Parse the JSON response using JObject
            var parsedObject = JObject.Parse(response.ResponseData);

            // Extract the Song Event id
            SongEventId = parsedObject["event"]["id"].ToString();
        }
    }
}
