using System;

namespace SRC.Core.Events
{
    public partial class Event
    {
        private WeakReference<SRC> _src;
        private SRC SRC
        {
            get
            {
                SRC res;
                _src.TryGetTarget(out res);
                return res;
            }
        }
        private IGUI GUI => SRC.GUI;

        public Event(SRC src)
        {
            // XXX これでいいかは知らん。
            _src = new WeakReference<SRC>(src, true);
        }
    }
}
