namespace MiniBank.Core.Tools;

public class NotFoundException : UserFriendlyException
{
    public NotFoundException()
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }
}