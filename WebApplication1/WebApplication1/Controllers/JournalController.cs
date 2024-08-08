using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
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
  public class JournalController : ControllerBase
  {
    [HttpPost("getRange")]
    public Task<ReturnRange<JournaledExceptionInfoVm>> GetRange(int skip, int take, JournalGetRangeFilter filter)
    {
      throw new NotImplementedException();
    }

    [HttpPost("getSingle")]
    public Task<JournaledExceptionVm> GetSingle(long id)
    {
      throw new NotImplementedException();
    }

  }
}
