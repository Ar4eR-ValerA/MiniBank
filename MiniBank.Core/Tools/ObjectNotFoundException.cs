namespace MiniBank.Core.Tools;

public class ObjectNotFoundException : UserFriendlyException
{
    public ObjectNotFoundException()
    {
    }

    public ObjectNotFoundException(string message)
        : base(message)
    {
    }
}