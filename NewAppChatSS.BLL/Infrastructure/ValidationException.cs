using System;

namespace NewAppChatSS.BLL.Infrastructure
{
    public class ValidationException : Exception
    {
        public ValidationException(string message)
            : base(message)
        {
        }
    }
}
