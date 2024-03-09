using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LandmarksR.Scripts.Experiment.Log
{
    public class LocalLogger : IAsyncDisposable
    {
        private readonly LoggerQueue _loggerQueue;
        private StreamWriter _streamWriter;
        private readonly SemaphoreSlim _asyncLock = new(1, 1);
        private readonly bool _ready;

        public LocalLogger(string fileName, int flushingInterval = 100)
        {
            if (!ValidateAndSetOutputFile(fileName)) return;
            _streamWriter = new StreamWriter(fileName, append: true) { AutoFlush = true };
            _loggerQueue = new LoggerQueue(WriteLogAsync, flushingInterval); // Assuming LoggerQueue now accepts Func<LogMessage, Task>
            _loggerQueue.StartProcessingTask();
            _ready = true;
        }

        public void Log(LogMessage message)
        {
            _loggerQueue.EnqueueMessage(message);
        }

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

        public async Task StopAsync()
        {
            if (!_ready) return;
            await _loggerQueue.StopAsync(); // Assuming StopAsync is implemented correctly in LoggerQueue
            await DisposeAsync();
        }

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
