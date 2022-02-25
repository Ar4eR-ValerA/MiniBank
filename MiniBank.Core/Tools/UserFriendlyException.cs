using System;

namespace MiniBank.Core.Tools
{
    public class UserFriendlyException : Exception
    {
        public UserFriendlyException()
        {
        }

        public UserFriendlyException(string message)
            : base(message)
        {
        }

        public UserFriendlyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}