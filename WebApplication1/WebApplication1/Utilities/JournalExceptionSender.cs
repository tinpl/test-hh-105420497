using Microsoft.Extensions.Primitives;
using Thunk.Services.ExceptionsJournal;

namespace WebApi.Utilities;

public class ExceptionSender : IExceptionSender
{
  private readonly ExceptionsJournalService _exceptionsJournalService;

  public ExceptionSender(ExceptionsJournalService exceptionsJournalService)
  {
    _exceptionsJournalService = exceptionsJournalService;
  }

  public Task<long> SendException(Exception ex, long eventId,
    IEnumerable<KeyValuePair<string, StringValues>> queryParams,
    byte[] requestBody, CancellationToken cancellationToken)
  {
    return _exceptionsJournalService.StoreException(ex, eventId, queryParams,
      requestBody, cancellationToken);
  }
}
