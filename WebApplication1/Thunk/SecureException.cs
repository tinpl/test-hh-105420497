namespace Thunk;

public class SecureException : Exception
{
  public new string Message { get; init; }

  public SecureException(string message)
  {
    Message = message;
  }

}