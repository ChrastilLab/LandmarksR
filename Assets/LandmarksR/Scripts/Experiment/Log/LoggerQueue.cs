using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace LandmarksR.Scripts.Experiment.Log
{
    public class LoggerQueue
    {
        private readonly ConcurrentQueue<LogMessage> _messages = new();
        private readonly Func<LogMessage, Task> _writeActionAsync;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly int _flushingInterval;
        private Task _processingTask;

        public LoggerQueue(Func<LogMessage, Task> writeActionAsync, int flushingInterval = 100)
        {
            _writeActionAsync = writeActionAsync ?? throw new ArgumentNullException(nameof(writeActionAsync));
            _flushingInterval = flushingInterval;
        }

        public void StartProcessingTask()
        {
            _processingTask = Task.Run(async () =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    if (_messages.TryDequeue(out var message))
                    {
                        await _writeActionAsync(message);
                    }
                    else
                    {
                        await Task.Delay(_flushingInterval, _cancellationTokenSource.Token);
                    }
                }

                // Optional: Process any remaining messages after cancellation is requested
                while (_messages.TryDequeue(out var remainingMessage))
                {
                    await _writeActionAsync(remainingMessage);
                }
            }, _cancellationTokenSource.Token);
        }

        public void EnqueueMessage(LogMessage message)
        {
            _messages.Enqueue(message);
        }

        public async Task StopAsync()
        {
            _cancellationTokenSource.Cancel();
            try
            {
                // Wait for the processing task to complete its current iteration and process all remaining messages
                await _processingTask;
            }
            catch (TaskCanceledException)
            {
                // Expected exception upon cancellation, can be safely ignored
            }
            catch (Exception)
            {
                // ignore
            }
        }
    }
}
