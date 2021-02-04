using System;
using System.Collections.Generic;
using System.Text;

namespace SRC.Core.Exceptions
{
    public class EventErrorException : Exception
    {
        public EventErrorException(string message) : base(message) { }
    }
}
