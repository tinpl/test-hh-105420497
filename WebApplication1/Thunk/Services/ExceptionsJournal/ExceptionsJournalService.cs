using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace Thunk.Services.ExceptionsJournal
{
  public class ExceptionsJournalService
  {
    private readonly ExceptionsJournalFacade _exceptionsJournalFacade;

    public ExceptionsJournalService(ExceptionsJournalFacade exceptionsJournalFacade)
    {
      _exceptionsJournalFacade = exceptionsJournalFacade;
    }

    public async Task<long> StoreException(Exception ex,
      long eventId,
      IEnumerable<KeyValuePair<string, StringValues>> queryParams,
      byte[] requestBody, CancellationToken cancellationToken)
    {
      var ret = await _exceptionsJournalFacade.JournaledExceptions.AddAsync(new JournaledException
      {
        TimeStamp = DateTime.UtcNow,
        Body = requestBody,
        EventId = eventId,
        StackTrace = ex.StackTrace,
        QueryParams = string.Join(',', queryParams.Select(x => $"{x.Key}:[{string.Join(',', x.Value)}]")),
        Message = ex.Message
      }, cancellationToken);

      await _exceptionsJournalFacade.SaveChangesAsync(cancellationToken);
      return ret.Entity.Id;
    }

    public Task<List<JournaledException>> GetRange(int skip, int take, DateTime? filterFrom, DateTime? filterTo,
      string? search, CancellationToken cancellationToken)
    {
      var query = _exceptionsJournalFacade.JournaledExceptions.AsQueryable();

      if (filterFrom != null)
        query = query.Where(x => x.TimeStamp >= filterFrom);
      if (filterTo != null)
        query = query.Where(x => x.TimeStamp < filterTo);
      if (search != null)
        query = query.Where(x => x.Message.Contains(search));

      query = query.Skip(skip).Take(take);

      return query.AsNoTracking().ToListAsync(cancellationToken);
    }

    public Task<JournaledException> GetSingle(long id, CancellationToken cancellationToken)
    {
      return _exceptionsJournalFacade.JournaledExceptions.SingleAsync(x => x.Id == id, cancellationToken);
    }
  }
}
