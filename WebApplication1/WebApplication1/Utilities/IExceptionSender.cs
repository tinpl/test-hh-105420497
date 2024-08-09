using Microsoft.Extensions.Primitives;

namespace WebApi.Utilities;

public interface IExceptionSender
{
  Task<long> SendException(Exception ex, long eventId,
    IEnumerable<KeyValuePair<string, StringValues>> queryParams,
    byte[] requestBody, CancellationToken cancellationToken);
}