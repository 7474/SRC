using SRC.Core.CmdDatas;
using SRC.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRC.Core.Exceptions
{
    public class EventErrorException : Exception
    {
        public EventDataLine EventData { get; }

        public EventErrorException(CmdData cmd, string message) : base(message)
        {
            EventData = cmd.EventData;
        }
    }
}
