using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace LandmarksR.Scripts.Experiment.Log
{
    /// <summary>
    /// Manages a queue of log messages to be processed asynchronously.
    /// </summary>
    public class LoggerQueue
    {
        private readonly ConcurrentQueue<LogMessage> _messages = new();
        private readonly Func<LogMessage, Task> _writeActionAsync;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly int _flushingInterval;
        private Task _processingTask;

        /// <summary>
        /// Initializes a new instance of the LoggerQueue class.
        /// </summary>
        /// <param name="writeActionAsync">The asynchronous action to write a log message.</param>
        /// <param name="flushingInterval">The interval for flushing the log queue.</param>
        public LoggerQueue(Func<LogMessage, Task> writeActionAsync, int flushingInterval = 100)
        {
            _writeActionAsync = writeActionAsync ?? throw new ArgumentNullException(nameof(writeActionAsync));
            _flushingInterval = flushingInterval;
        }

        /// <summary>
        /// Starts the task for processing log messages from the queue.
        /// </summary>
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

                // Process any remaining messages after cancellation is requested
                while (_messages.TryDequeue(out var remainingMessage))
                {
                    await _writeActionAsync(remainingMessage);
                }
            }, _cancellationTokenSource.Token);
        }

        /// <summary>
        /// Enqueues a log message to be processed.
        /// </summary>
        /// <param name="message">The log message to enqueue.</param>
        public void EnqueueMessage(LogMessage message)
        {
            _messages.Enqueue(message);
        }

        /// <summary>
        /// Stops the processing task asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
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
                // Handle other exceptions as necessary
            }
        }
    }
}
