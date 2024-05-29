using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LandmarksR.Scripts.Experiment.Log
{
    /// <summary>
    /// Manages local logging of messages to a file.
    /// </summary>
    public class LocalLogger : IAsyncDisposable
    {
        private readonly LoggerQueue _loggerQueue;
        private StreamWriter _streamWriter;
        private readonly SemaphoreSlim _asyncLock = new(1, 1);
        private readonly bool _ready;

        /// <summary>
        /// Initializes a new instance of the LocalLogger class.
        /// </summary>
        /// <param name="fileName">The file name for the log file.</param>
        /// <param name="flushingInterval">The interval for flushing the log queue.</param>
        public LocalLogger(string fileName, int flushingInterval = 100)
        {
            if (!ValidateAndSetOutputFile(fileName)) return;
            _streamWriter = new StreamWriter(fileName, append: true) { AutoFlush = true };
            _loggerQueue = new LoggerQueue(WriteLogAsync, flushingInterval); // Assuming LoggerQueue now accepts Func<LogMessage, Task>
            _loggerQueue.StartProcessingTask();
            _ready = true;
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
        /// Writes a log message asynchronously to the log file.
        /// </summary>
        /// <param name="message">The log message to write.</param>
        private async Task WriteLogAsync(LogMessage message)
        {
            await _asyncLock.WaitAsync();
            try
            {
                await _streamWriter.WriteLineAsync(message.ToString());
                await _streamWriter.FlushAsync();
            }
            finally
            {
                _asyncLock.Release();
            }
        }

        /// <summary>
        /// Stops the local logger asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task StopAsync()
        {
            if (!_ready) return;
            await _loggerQueue.StopAsync(); // Assuming StopAsync is implemented correctly in LoggerQueue
            await DisposeAsync();
        }

        /// <summary>
        /// Disposes the local logger asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async ValueTask DisposeAsync()
        {
            await _asyncLock.WaitAsync();
            try
            {
                if (_streamWriter != null)
                {
                    await _streamWriter.DisposeAsync();
                    _streamWriter = null;
                }
            }
            finally
            {
                _asyncLock.Release();
                _asyncLock.Dispose();
            }
        }

        /// <summary>
        /// Validates and sets the output file.
        /// </summary>
        /// <param name="fileName">The file name for the log file.</param>
        /// <returns>True if the file was successfully validated and set; otherwise, false.</returns>
        private static bool ValidateAndSetOutputFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrEmpty(fileName))
                return false;

            try
            {
                if (File.Exists(fileName))
                    return true;

                // Create parent directory if it does not exist
                var directory = Path.GetDirectoryName(fileName);
                if (directory != null && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                // Create the file
                File.Create(fileName).Close();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
