using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Log
{
    public class RemoteLogger
    {
        private readonly LoggerQueue _loggerQueue;
        private static readonly HttpClient HttpClient = new(); // Reuse instance
        private readonly string _logUrl;
        private readonly string _filePath;
        private bool _ready; // Changed to false by default

        public RemoteLogger(string filePath, string statusUrl, string logUrl, int flushingInterval = 100)
        {
            _filePath = filePath;
            _logUrl = logUrl;
            _loggerQueue = new LoggerQueue(WriteLogAsync, flushingInterval); // Assume WriteLogAsync is the new async method

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

        public void Log(LogMessage message)
        {
            _loggerQueue.EnqueueMessage(message);
        }

        private async Task WriteLogAsync(LogMessage message)
        {
            var content = new StringContent(message.ToJson(_filePath), Encoding.UTF8, "application/json");
            try
            {
                Debug.Log($"Sending log message: {message.ToJson(_filePath)}");
                var response = await HttpClient.PostAsync(_logUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    Debug.LogWarning("Failed to log message.");
                    // Consider retrying or logging locally
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception occurred while logging: {ex.Message}");
                // Consider retrying or logging locally
            }
        }

        public async Task StopAsync()
        {
            if (!_ready) return;
            await _loggerQueue.StopAsync();
        }

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

        private static async Task<string> GetStatus(string url)
        {
            var response = await HttpClient.GetAsync(url);
            response.EnsureSuccessStatusCode(); // This will throw an exception for non-success codes
            return await response.Content.ReadAsStringAsync();
        }
    }
}
