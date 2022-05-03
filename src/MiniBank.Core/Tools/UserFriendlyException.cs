namespace MiniBank.Core.Tools;

public class UserFriendlyException : Exception
{
    public UserFriendlyException()
    {
    }

    public UserFriendlyException(string message)
        : base(message)
    {
    }
}