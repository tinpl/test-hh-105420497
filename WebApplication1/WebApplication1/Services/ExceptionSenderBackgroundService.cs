using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;
using WebApi.Utilities;

namespace WebApi.Services
{
  public record ExceptionData(Exception Ex, long EventId, IEnumerable<KeyValuePair<string, StringValues>> QueryParams,
    byte[] RequestBody);

  public class ExceptionSenderBackgroundService : BackgroundService
  {
    private readonly TimeSpan _waitDelay = TimeSpan.FromMilliseconds(10);

    private readonly IExceptionSender _exceptionSender;
    private readonly ILogger<ExceptionSenderBackgroundService> _logger;

    private readonly ConcurrentQueue<ExceptionData> _queue = new ConcurrentQueue<ExceptionData>();

    public ExceptionSenderBackgroundService(IExceptionSender exceptionSender,
      ILogger<ExceptionSenderBackgroundService> logger)
    {
      _exceptionSender = exceptionSender;
      _logger = logger;
    }

    public void EnqueueException(ExceptionData ex)
    {
      _queue.Enqueue(ex);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      while (!stoppingToken.IsCancellationRequested && !_queue.IsEmpty)
      {
        if (!_queue.TryDequeue(out ExceptionData data))
        {
          await Task.Delay(_waitDelay, stoppingToken);
          continue;
        }

        if (data == null)
          continue;

        // todo: use Polly for custom back-off?
        while (!stoppingToken.IsCancellationRequested)
        {
          try
          {
            await _exceptionSender.SendException(data!.Ex, data.EventId, data.QueryParams, data.RequestBody,
              stoppingToken);
          }
          catch (Exception storeExceptionException)
          {
            _logger.LogError(storeExceptionException.Message);
            await Task.Delay(_waitDelay, stoppingToken);
          }
        }
      }
    }
  }
}
