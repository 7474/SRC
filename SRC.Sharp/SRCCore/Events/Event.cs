using SRCCore.Commands;
using SRCCore.Maps;

namespace SRCCore.Events
{
    public partial class Event
    {
        protected SRC SRC { get; }
        private IGUI GUI => SRC.GUI;
        private IGUIStatus Status => SRC.GUIStatus;
        private Map Map => SRC.Map;
        private Expressions.Expression Expression => SRC.Expression;
        private Command Commands => SRC.Commands;

        public Event(SRC src)
        {
            SRC = src;
        }
        public void Restore(SRCSuspendData data)
        {
            // Requireコマンドで追加されたイベントファイル
            foreach (var fname in data.AdditionalEventFileNames)
            {
                if (!EventFileNames.Contains(fname))
                {
                    LoadEventData2(fname, EventDataSource.Scenario);
                }
            }
            RegisterLabel();

            // イベント用ラベルを設定
            foreach (var lab in colEventLabelList.List)
            {
                lab.Enable = !data.DisableEventLabels.Contains(lab.Data);
            }
        }
    }
}
