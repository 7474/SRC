using System;

namespace SRCCore.Exceptions
{
    public class TerminateException : Exception
    {
        public TerminateException(string message) : base(message)
        {
        }

        public TerminateException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
