using SRCCore.CmdDatas;
using SRCCore.Events;
using System;

namespace SRCCore.Exceptions
{
    public class EventErrorException : Exception
    {
        public EventDataLine EventData { get; }

        public EventErrorException(CmdData cmd, string message) : base(message)
        {
            EventData = cmd?.EventData;
        }
    }
}
