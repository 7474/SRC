using SRC.Core.Maps;
using System;

namespace SRC.Core.Events
{
    public partial class Event
    {
        protected SRC SRC { get; }
        private IGUI GUI => SRC.GUI;
        private Map Map => SRC.Map;

        public Event(SRC src)
        {
            SRC = src;
        }
    }
}
