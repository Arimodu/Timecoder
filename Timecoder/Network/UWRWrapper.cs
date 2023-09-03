using SiraUtil.Logging;
using SiraUtil.Web;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Timecoder.Configuration;
using Timecoder.Network.Models;
using Zenject;

namespace Timecoder.Network
{
    internal class UWRWrapper
    {
        private TCConfig Config { get; set; }
        private readonly IHttpService HttpService;
        private readonly SiraLog SiraLogger;

        private string SessionUri;
        private string EventPushUri;
        private string SubEventPushUri;

        internal UWRWrapper(TCConfig config, SiraLog log, IHttpService httpService)
        {
            Config = config;
            SiraLogger = log;
            HttpService = httpService;

            HttpService.Headers.Add("Authorization", Config.Token.ToString());
            HttpService.BaseURL = Config.BaseUrl;

            SessionUri = Config.SessionEndpoint;
            EventPushUri = Config.EventPush;
            SubEventPushUri = Config.SubEventPush;

            Config.OnChanged += Config_OnChanged;
#if DEBUG
            SiraLogger.DebugMode = true;
#endif
        }

        private void Config_OnChanged(TCConfig newConfig)
        {
            if (HttpService.Headers["Authorization"] != newConfig.Token.ToString())
            {
                SiraLogger.Debug($"Token changed from {HttpService.Headers["Authorization"]} to {newConfig.Token}");
                HttpService.Headers["Authorization"] = newConfig.Token.ToString();
            }

            if (HttpService.BaseURL != newConfig.BaseUrl)
            {
                SiraLogger.Debug($"BaseUrl changed from {HttpService.BaseURL} to {newConfig.BaseUrl}");
                HttpService.BaseURL = newConfig.BaseUrl;
            }

            if (SessionUri != newConfig.SessionEndpoint)
            {
                SiraLogger.Debug($"SessionEndpoint changed from {SessionUri} to {newConfig.SessionEndpoint}");
                SessionUri = newConfig.SessionEndpoint;
            }

            if (EventPushUri != newConfig.EventPush)
            {
                SiraLogger.Debug($"EventPush changed from {EventPushUri} to {newConfig.EventPush}");
                EventPushUri = newConfig.EventPush;
            }

            if (SubEventPushUri != newConfig.SubEventPush)
            {
                SiraLogger.Debug($"SubEventPush changed from {SubEventPushUri} to {newConfig.SubEventPush}");
                SubEventPushUri = newConfig.SubEventPush;
            }
        }


        /// <summary>
        /// Retrieves session info and user data by sending a GET request to the /me endpoint.
        /// </summary>
        /// <returns>An <see cref="ApiResponse"/> object representing the response from the server.</returns>
        public async Task<ApiResponse> GetSession()
        {
            try
            {
                var response = await HttpService.GetAsync(SessionUri);

                var responseContent = await response.ReadAsStringAsync();

                return new ApiResponse(response.Successful, response.Code, responseContent, await response.Error());
            }
            catch (Exception e)
            {
                return new ApiResponse(false, 500, null, e.ToString());
            }
        }

        /// <summary>
        /// Sends event data by sending a POST request to the EventPush endpoint.
        /// </summary>
        /// <param name="eventData">The event data to be sent.</param>
        /// <returns>An <see cref="ApiResponse"/> object representing the response from the server.</returns>
        public async Task<ApiResponse> SendEventData(object data)
        {
            SiraLogger.Debug($"Sending JSON to server: {data}");

            try
            {
                var response = await HttpService.PostAsync(EventPushUri, data);

                var responseContent = await response.ReadAsStringAsync();

                return new ApiResponse(response.Successful, response.Code, responseContent, await response.Error());
            }
            catch (Exception e)
            {
                return new ApiResponse(false, 500, null, e.ToString());
            }
        }

        /// <summary>
        /// Sends sub-event data by sending a POST request to the SubEventPush endpoint.
        /// </summary>
        /// <param name="subeventData">The sub-event data to be sent.</param>
        /// <returns>An <see cref="ApiResponse"/> object representing the response from the server.</returns>
        public async Task<ApiResponse> SendSubEventData(object data)
        {
            SiraLogger.Debug($"Sending JSON to server: {data}");

            try
            {
                var response = await HttpService.PostAsync(SubEventPushUri, data);

                var responseContent = await response.ReadAsStringAsync();

                return new ApiResponse(response.Successful, response.Code, responseContent, await response.Error());
            }
            catch (Exception e)
            {
                return new ApiResponse(false, 500, null, e.Message);
            }
        }
    }
}
