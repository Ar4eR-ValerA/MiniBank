namespace MiniBank.Core.Tools
{
    public class ValidationException : UserFriendlyException
    {
        public ValidationException()
        {
        }

        public ValidationException(string message)
            : base(message)
        {
        }
    }
}