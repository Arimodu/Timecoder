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

namespace Timecoder
{
    internal class TCEventManager : IInitializable
    {
        private readonly TCClient _client;
        private readonly SiraLog _logger;

        private Session Session;
        private string SongEventId;

        internal TCEventManager(TCClient client, SiraLog log) 
        {
            _client = client;
            _logger = log;

#if DEBUG
            _logger.DebugMode = true;
#endif
        }

        public async void Initialize()
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
            var response = await _client.GetSession();

            if (!response.IsSuccess)
            {
                _logger.Error($"Server did not return a session! Response code is: {response.StatusCode} {response.ErrorMessage}. Response data: {response.ResponseData}");
                return false; //For now anyway, not handling this
            }

            _logger.Debug(response);

            // Parse the JSON response using JObject
            var parsedObject = JObject.Parse(response.ResponseData);

            // Extract the SessionId
            Session = new Session(parsedObject["session"]["id"].ToString());

            _logger.Logger.Notice($"Got session id {Session.Id} from server!");

            return true;
        }

        private async void OnLevelRestarted(StandardLevelScenesTransitionSetupDataSO arg1, LevelCompletionResults arg2)
        {
            if (Session == null || SongEventId == string.Empty)
            {
                _logger.Warn($"Empty session or song not started on LevelRestarted");
                return; // We dont want to do anything when we dont have a session
            }

            var time = DateTime.UtcNow;

            var packet = new Packet<SongRestart>
            {
                Session = Session,
                Event = new Event(SongEventId),
                SubEvent = new SubEvent<SongRestart>(SubEventName.Restart, new SongRestart(time))
            };

            if (!await SendSubEventPacket(packet)) return;
        }

        private async Task<bool> SendSubEventPacket<T>(Packet<T> packet)
        {
            var response = await _client.SendSubEventData(packet.ToString());

            _logger.Debug(response.ToString());

            if (!response.IsSuccess)
            {
                _logger.Error(response.ToString());
                return false;
            }

            return true;
        }

        private async void OnLevelFailed(StandardLevelScenesTransitionSetupDataSO arg1, LevelCompletionResults levelCompletionResults)
        {
            await SendLevelEndData(levelCompletionResults, SongEnd.SongEndReason.Failed);
        }

        private async void OnLevelQuit(StandardLevelScenesTransitionSetupDataSO arg1, LevelCompletionResults levelCompletionResults)
        {
            await SendLevelEndData(levelCompletionResults, SongEnd.SongEndReason.Quit);
        }

        private async void OnLevelCleared(StandardLevelScenesTransitionSetupDataSO arg1, LevelCompletionResults levelCompletionResults)
        {
            await SendLevelEndData(levelCompletionResults, SongEnd.SongEndReason.Cleared);
        }

        private async Task SendLevelEndData(LevelCompletionResults levelCompletionResults, SongEnd.SongEndReason reason)
        {
            if (Session == null || SongEventId == string.Empty)
            {
                _logger.Warn($"Empty session or song not started on LevelRestarted");
                return; // We dont want to do anything when we dont have a session
            }

            var time = DateTime.UtcNow;

            var packet = new Packet<SongEnd>
            {
                Session = Session,
                Event = new Event(SongEventId),
                SubEvent = new SubEvent<SongEnd>(SubEventName.End, new SongEnd(new SongResults(levelCompletionResults), reason, time))
            };

            if (!await SendSubEventPacket(packet)) return;
        }

        private void OnGameSceneLoaded()
        {
            if (Session == null)
            {
                _logger.Warn($"Empty session on GameSceneLoaded");
                return; // We don't want to do anything when we don't have a session
            }

            if (IsReplay()) OnReplayStarted();
            else OnSongStarted();
        }

        public void OnSongStarted()
        {
            var beatmapLevel = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.previewBeatmapLevel;
            var song = new SongInfo(
                beatmapLevel.levelID,
                beatmapLevel.songName,
                beatmapLevel.songSubName,
                beatmapLevel.songAuthorName,
                beatmapLevel.levelAuthorName);

            _logger.Logger.Notice($"Started level: {song}");

            SendLevelStartData(song);
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

            _logger.Logger.Notice($"Started replay: {replayInfo}");

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

            var response = await _client.SendSubEventData(packet.ToString());

            if (!response.IsSuccess)
            {
                _logger.Error(response.ToString());
                return;
            }

            _logger.Debug(response.ToString());

            // Parse the JSON response using JObject
            var parsedObject = JObject.Parse(response.ResponseData);

            // Extract the Song Event id
            SongEventId = parsedObject["event"]["id"].ToString();
        }
    }
}
