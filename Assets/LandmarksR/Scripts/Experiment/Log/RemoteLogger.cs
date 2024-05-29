using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Log
{
    /// <summary>
    /// Manages remote logging of messages.
    /// </summary>
    public class RemoteLogger
    {
        private readonly LoggerQueue _loggerQueue;
        private static readonly HttpClient HttpClient = new(); // Reuse instance
        private readonly string _logUrl;
        private readonly string _filePath;
        private bool _ready; // Changed to false by default

        /// <summary>
        /// Initializes a new instance of the RemoteLogger class.
        /// </summary>
        /// <param name="filePath">The file path for local logging.</param>
        /// <param name="statusUrl">The URL to check the status of the remote logging server.</param>
        /// <param name="logUrl">The URL to send log messages to.</param>
        /// <param name="flushingInterval">The interval for flushing the log queue.</param>
        public RemoteLogger(string filePath, string statusUrl, string logUrl, int flushingInterval = 100)
        {
            _filePath = filePath;
            _logUrl = logUrl;
            _loggerQueue = new LoggerQueue(WriteLogAsync, flushingInterval);

            ValidateAPIAsync(statusUrl).ContinueWith(task =>
            {
                if (!task.Result)
                {
                    Debug.LogWarning("Invalid status url or server is currently down.");
                }
                else
                {
                    _ready = true; // Set to true after successful initialization
                    _loggerQueue.StartProcessingTask();
                }
            });
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Log(LogMessage message)
        {
            _loggerQueue.EnqueueMessage(message);
        }

        /// <summary>
        /// Writes a log message asynchronously to the remote server.
        /// </summary>
        /// <param name="message">The log message to write.</param>
        private async Task WriteLogAsync(LogMessage message)
        {
            var content = new StringContent(message.ToJson(_filePath), Encoding.UTF8, "application/json");

            try
            {
                var response = await HttpClient.PostAsync(_logUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    Debug.LogWarning("Failed to log message remotely.");
                    // Consider retrying or logging locally
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception occurred while logging remotely: {ex.Message}");
                // Consider retrying or logging locally
            }
        }

        /// <summary>
        /// Stops the remote logger asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task StopAsync()
        {
            if (!_ready) return;
            await _loggerQueue.StopAsync();
        }

        /// <summary>
        /// Validates the remote logging server API asynchronously.
        /// </summary>
        /// <param name="url">The URL to check the status of the remote logging server.</param>
        /// <returns>True if the API is valid; otherwise, false.</returns>
        private static async Task<bool> ValidateAPIAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return false;
            }

            try
            {
                var result = await GetStatus(url);
                var status = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
                return status.ContainsKey("status") && status["status"] == "ok";
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the status of the remote logging server asynchronously.
        /// </summary>
        /// <param name="url">The URL to check the status of the remote logging server.</param>
        /// <returns>The status response as a string.</returns>
        private static async Task<string> GetStatus(string url)
        {
            var response = await HttpClient.GetAsync(url);
            response.EnsureSuccessStatusCode(); // This will throw an exception for non-success codes
            return await response.Content.ReadAsStringAsync();
        }
    }
}
