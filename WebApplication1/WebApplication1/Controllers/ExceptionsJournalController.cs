using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Thunk.Services.ExceptionsJournal;
using WebApi.Utilities;

namespace WebApi.Controllers
{
  public class JournalGetRangeFilter
  {
    public DateTime? From = null;
    public DateTime? To = null;
    public string? Search = null;
  }

  public record JournaledExceptionVm(string Text, long Id, long EventId, DateTime CreatedAt);
  public record JournaledExceptionInfoVm(long Id, long EventId, DateTime CreatedAt);

  [ApiController]
  [Route("api.user.journal")]
  [Produces("application/json")]
  public class ExceptionsJournalController : ControllerBase
  {
    private readonly ExceptionsJournalService _exceptionsJournalService;

    public ExceptionsJournalController(ExceptionsJournalService exceptionsJournalService)
    {
      _exceptionsJournalService = exceptionsJournalService;
    }

    [HttpPost("getRange")]
    public async Task<ReturnRange<JournaledExceptionInfoVm>> GetRange(
      [Range(0, int.MaxValue)] int skip,
      [Range(0, int.MaxValue)] int take, 
      JournalGetRangeFilter filter,
      CancellationToken cancellationToken)
    {
      var result =
        await _exceptionsJournalService.GetRange(skip, take, filter.From, filter.To, filter.Search, cancellationToken);

      return new ReturnRange<JournaledExceptionInfoVm>(skip, take,
        Items: result.Select(x => new JournaledExceptionInfoVm(x.Id, x.EventId, x.TimeStamp)).ToArray());
    }

    [HttpPost("getSingle")]
    public async Task<JournaledExceptionVm> GetSingle(
      [Range(0, int.MaxValue)] long id, 
      CancellationToken cancellationToken)
    {
      var result = await _exceptionsJournalService.GetSingle(id, cancellationToken);
      return new JournaledExceptionVm(result.Message, result.Id, result.EventId, result.TimeStamp);
    }

  }
}
