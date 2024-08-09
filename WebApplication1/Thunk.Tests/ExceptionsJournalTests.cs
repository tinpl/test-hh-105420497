using System.Buffers;
using FluentAssertions;
using Microsoft.Extensions.Primitives;
using Thunk.Services.ExceptionsJournal;

namespace Thunk.Tests
{
  public class ExceptionsJournalTests
  {
    [Fact]
    public async Task Test1()
    {
      await using var ctx = new ExceptionsJournalFacade();
      var exceptionsJournalService = new ExceptionsJournalService(ctx);

      var random = new Random();
      var someBodyData = ArrayPool<byte>.Shared.Rent(1000);

      List<long> exceptionsIds = new List<long>();

      int startEventId = random.Next();
      const int genEventsCount = 100;

      for (int eventId = startEventId; eventId < startEventId + genEventsCount; ++eventId)
      {
        random.NextBytes(someBodyData);

        Exception ex;
        try
        {
          throw new Exception($"test_exception_message_{eventId}_postfix");
        }
        catch (Exception createException)
        {
          ex = createException;
        }

        var exceptionId = await exceptionsJournalService.StoreException(
          ex,
          eventId,
          new List<KeyValuePair<string, StringValues>>
          {
            new("query_param1", new StringValues(new[] { "1", "2", "3" })),
            new("query_param2", StringValues.Empty),
          },
          someBodyData,
          CancellationToken.None
        );

        exceptionsIds.Add(exceptionId);
      }

      ArrayPool<byte>.Shared.Return(someBodyData);

      exceptionsIds.Should().HaveCount(100);

      var manyEvents = await exceptionsJournalService.GetRange(
        0, 100_000_000, null, null, null,
        CancellationToken.None);
      manyEvents.Should().HaveCountGreaterOrEqualTo(100);

      int lookupEventId = random.Next(startEventId, startEventId + genEventsCount - 1);

      var filteredEvents = await exceptionsJournalService.GetRange(
        0, 100_000_000, DateTime.MinValue, DateTime.MaxValue,
        $"{lookupEventId}",
        CancellationToken.None);
      filteredEvents.Should().HaveCount(1);
    }
  }
}
