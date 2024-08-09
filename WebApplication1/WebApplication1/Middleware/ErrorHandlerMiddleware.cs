using System.Buffers;
using System.Net;
using System.Text.Json;
using Thunk;
using WebApi.Utilities;

namespace WebApi.Middleware
{
  class ErrorResponse
  {
    public string Type;
    public long Id;
    public Dictionary<string, string> Data = new();
  }

  public class ErrorHandlerMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(RequestDelegate next,
      ILogger<ErrorHandlerMiddleware> logger)
    {
      _next = next;
      _logger = logger;
    }

    // todo: do something real here, at least GUIDs which do not intersect between different machines
    long GenerateEventId() => new Random().Next();

    public async Task Invoke(HttpContext context, IExceptionSender exceptionSender)
    {
      try
      {
        await _next(context);
      }
      catch (Exception error)
      {
        var eventId = GenerateEventId();

        var response = context.Response;
        response.ContentType = "application/json";

        long id = 0;
        try
        {
          id = await exceptionSender.SendException(error,
            eventId,
            queryParams: context.Request.Query,
            requestBody: (await context.Request.BodyReader.ReadAsync()).Buffer.ToArray(),
            cancellationToken: context.RequestAborted
          );
        }
        catch (Exception storeExceptionException)
        {
          _logger.LogCritical(storeExceptionException.Message);
        }

        var responseObject = new ErrorResponse { Id = id };

        if (error.GetType().IsSubclassOf(typeof(SecureException)))
        {
          response.StatusCode = (int)HttpStatusCode.InternalServerError;
          responseObject.Type = "Secure";
          responseObject.Data.Add("message", (error as SecureException)!.Message);
        }
        else
        {
          responseObject.Type = "Exception";
          responseObject.Data.Add("message", $"Internal server error ID = {id}");
        }

        await response.WriteAsync(JsonSerializer.Serialize(responseObject));
      }
    }

  }
}
