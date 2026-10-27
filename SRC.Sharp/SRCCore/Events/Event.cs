using SRCCore.Commands;
using SRCCore.Maps;
using System.Collections.Generic;
using System.Linq;

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
            // VB: Event.bas RestoreEventData
            // 読み込んだファイル名を復元しておかないと、再開後に同じ Require を実行した際に
            // 同じファイルを再度読み込んでラベルが二重に登録されてしまう
            AdditionalEventFileNames = (data.AdditionalEventFileNames ?? new List<string>()).ToList();
            var loaded = false;
            foreach (var fname in AdditionalEventFileNames)
            {
                var path = SRC.FileSystem.IsAbsolutePath(fname)
                    ? fname
                    : SRC.FileSystem.PathCombine(SRC.ScenarioPath, fname);

                // 既に読み込まれている場合はスキップ
                if (!EventFileNames.Contains(path))
                {
                    LoadEventData2(path, EventDataSource.Scenario);
                    loaded = true;
                }
            }
            var label_error_found = RegisterLabel();
            if (loaded)
            {
                // コマンドデータ配列を設定
                ParseCommand(label_error_found);
            }

            // イベント用ラベルを設定
            foreach (var lab in colEventLabelList.List)
            {
                lab.Enable = !data.DisableEventLabels.Contains(lab.Data);
            }
        }
    }
}
