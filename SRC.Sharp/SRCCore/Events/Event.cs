using SRCCore.Maps;

namespace SRCCore.Events
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
