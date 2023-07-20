using SiraUtil.Logging;
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
    internal class TCClient : IInitializable, IDisposable
    {
        private TCConfig Config { get; set; }
        private HttpClient HttpClient { get; set; }

        private readonly SiraLog _logger;
        internal TCClient(TCConfig config, SiraLog log)
        {
            Config = config;
            _logger = log;
        }

        public void Initialize()
        {
            HttpClient = new HttpClient();
        }

        /// <summary>
        /// Retrieves session info and user data by sending a GET request to the /me endpoint.
        /// </summary>
        /// <returns>An <see cref="ApiResponse"/> object representing the response from the server.</returns>
        public async Task<ApiResponse> GetSession()
        {
            try
            {
                // Add authorization header if the token is available
                if (!string.IsNullOrEmpty(Config.Token.TokenString))
                {
                    HttpClient.DefaultRequestHeaders.Add("Authorization", $"{Config.Token.Version} {Config.Token.TokenString}");
                }

                var sessionUri = Config.GetSessionURI();

                // Send the GET request
                var response = await HttpClient.GetAsync(sessionUri);

                // Handle the response
                var responseContent = await response.Content.ReadAsStringAsync();

                return new ApiResponse(response.IsSuccessStatusCode, response.StatusCode, responseContent, response.ReasonPhrase);
            }
            catch (Exception e)
            {
                return new ApiResponse(false, HttpStatusCode.InternalServerError, null, e.ToString());
            }
            finally
            {
                // Remove the authorization header
                if (!string.IsNullOrEmpty(Config.Token.TokenString))
                {
                    HttpClient.DefaultRequestHeaders.Remove("Authorization");
                }
            }
        }

        /// <summary>
        /// Sends event data by sending a POST request to the EventPush endpoint.
        /// </summary>
        /// <param name="eventData">The event data to be sent.</param>
        /// <returns>An <see cref="ApiResponse"/> object representing the response from the server.</returns>
        public async Task<ApiResponse> SendEventData(string jsonData)
        {
            _logger.Debug($"Sending JSON to server: {jsonData}");

            try
            {
                // Add authorization header if the token is available
                if (!string.IsNullOrEmpty(Config.Token.TokenString))
                {
                    HttpClient.DefaultRequestHeaders.Add("Authorization", $"{Config.Token.Version} {Config.Token.TokenString}");
                }

                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Send the POST request
                var response = await HttpClient.PostAsync(Config.GetEventPushURI(), content);

                // Handle the response
                var responseContent = await response.Content.ReadAsStringAsync();

                return new ApiResponse(response.IsSuccessStatusCode, response.StatusCode, responseContent, response.ReasonPhrase);
            }
            catch (Exception e)
            {
                return new ApiResponse(false, HttpStatusCode.InternalServerError, null, e.ToString());
            }
            finally
            {
                // Remove the authorization header
                if (!string.IsNullOrEmpty(Config.Token.TokenString))
                {
                    HttpClient.DefaultRequestHeaders.Remove("Authorization");
                }
            }
        }

        /// <summary>
        /// Sends sub-event data by sending a POST request to the SubEventPush endpoint.
        /// </summary>
        /// <param name="subeventData">The sub-event data to be sent.</param>
        /// <returns>An <see cref="ApiResponse"/> object representing the response from the server.</returns>
        public async Task<ApiResponse> SendSubEventData(string jsonData)
        {
            _logger.Debug($"Sending JSON to server: {jsonData}");

            try
            {
                // Add authorization header if the token is available
                if (!string.IsNullOrEmpty(Config.Token.TokenString))
                {
                    HttpClient.DefaultRequestHeaders.Add("Authorization", $"{Config.Token.Version} {Config.Token.TokenString}");
                }

                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Send the POST request
                var response = await HttpClient.PostAsync(Config.GetSubEventPushURI(), content);

                // Handle the response
                var responseContent = await response.Content.ReadAsStringAsync();

                return new ApiResponse(response.IsSuccessStatusCode, response.StatusCode, responseContent, response.ReasonPhrase);
            }
            catch (Exception e)
            {
                return new ApiResponse(false, HttpStatusCode.InternalServerError, null, e.Message);
            }
            finally
            {
                // Remove the authorization header
                if (!string.IsNullOrEmpty(Config.Token.TokenString))
                {
                    HttpClient.DefaultRequestHeaders.Remove("Authorization");
                }
            }
        }

        public void Dispose()
        {
            HttpClient.Dispose();
        }
    }
}
