using System;
using Zenject;
using Timecoder.Configuration;
using Timecoder.Network;
using BS_Utils.Utilities;
using SiraUtil.Logging;
using Timecoder.Network.Models;
using Newtonsoft.Json.Linq;
using Timecoder.Network.Models.Packets;

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
            _logger.DebugMode = true;
        }

        public void Initialize()
        {
            BSEvents.gameSceneLoaded += OnGameSceneLoaded;

            BSEvents.levelRestarted += OnLevelRestarted;

            BSEvents.levelCleared += OnLevelCleared;
            BSEvents.levelQuit += OnLevelQuit;
            BSEvents.levelFailed += OnLevelFailed;

            SetupSession();
        }

        public async void SetupSession()
        {
            var response = await _client.GetSession();

            if (!response.IsSuccess)
            {
                _logger.Error($"Server did not return a session! Response code is: {response.StatusCode} {response.ErrorMessage}. Response data: {response.ResponseData}");
                return; //For now anyway, not handling this
            }

            _logger.Debug(response);

            // Parse the JSON response using JObject
            var parsedObject = JObject.Parse(response.ResponseData);

            // Extract the SessionId
            Session = new Session(parsedObject["session"]["id"].ToString());

            _logger.Logger.Notice($"Got session id {Session.Id} from server!");
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

            var response = await _client.SendSubEventData(packet.ToString());

            _logger.Debug(response.ToString());

            if (!response.IsSuccess)
            {
                _logger.Error(response.ToString());
                return;
            }
        }

        private async void OnLevelFailed(StandardLevelScenesTransitionSetupDataSO arg1, LevelCompletionResults arg2)
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
                SubEvent = new SubEvent<SongEnd>(SubEventName.End, new SongEnd(SongEnd.SongEndReason.Failed, time)) 
            };

            var response = await _client.SendSubEventData(packet.ToString());

            _logger.Debug(response.ToString());

            if (!response.IsSuccess)
            {
                _logger.Error(response.ToString());
                return;
            }
        }

        private async void OnLevelQuit(StandardLevelScenesTransitionSetupDataSO arg1, LevelCompletionResults arg2)
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
                SubEvent = new SubEvent<SongEnd>(SubEventName.End, new SongEnd(SongEnd.SongEndReason.Quit, time))
            };

            var response = await _client.SendSubEventData(packet.ToString());

            _logger.Debug(response.ToString());

            if (!response.IsSuccess)
            {
                _logger.Error(response.ToString());
                return;
            }
        }

        private async void OnLevelCleared(StandardLevelScenesTransitionSetupDataSO arg1, LevelCompletionResults arg2)
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
                SubEvent = new SubEvent<SongEnd>(SubEventName.End, new SongEnd(SongEnd.SongEndReason.Finished, time))
            };

            var response = await _client.SendSubEventData(packet.ToString());

            _logger.Debug(response.ToString());

            if (!response.IsSuccess)
            {
                _logger.Error(response.ToString());
                return;
            }
        }

        private async void OnGameSceneLoaded()
        {
            var beatmapLevel = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.previewBeatmapLevel;

            var song = new SongInfo(
                beatmapLevel.levelID,
                beatmapLevel.songName,
                beatmapLevel.songSubName,
                beatmapLevel.songAuthorName,
                beatmapLevel.levelAuthorName);

            _logger.Logger.Notice($"Started level: {song}");

            if (Session == null)
            {
                _logger.Warn($"Empty session on GameSceneLoaded");
                return; // We dont want to do anything when we dont have a session
            }

            var time = DateTime.UtcNow;

            var packet = new Packet<object, SongStart>
            {
                Session = Session,
                Event = new Event<object>(EventName.Song, new object()),
                SubEvent = new SubEvent<SongStart>(SubEventName.Start, new SongStart(song, time))
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
