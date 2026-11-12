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
            // VB: RestoreEventData は保存した有効・無効を colEventLabelList の順に戻し、
            // 保存したものより多いラベルは有効にする（#1529）
            if (data.EventLabelEnabled != null)
            {
                // 元の SRC の形式では、保存したラベルが今のラベルより多ければ、多い分は先頭にあったとみなして後ろからそろえる。
                // 元の SRC は本体の Lib（スペシャルパワー.eve・汎用戦闘アニメ\include.eve の「*プロローグ:」など）のイベントのラベルを
                // シナリオのラベルより前に並べるので、本体に汎用戦闘アニメなどを入れた環境で保存したデータは、それの無い環境では
                // シナリオのラベルの位置がずれる（元の SRC の実機の中断データで分かった。#1577）
                var offset = data.AlignEventLabelsToEnd ? System.Math.Max(0, data.EventLabelEnabled.Count - colEventLabelList.Count) : 0;
                var i = offset;
                foreach (var lab in colEventLabelList.List)
                {
                    lab.Enable = i < data.EventLabelEnabled.Count ? data.EventLabelEnabled[i] : true;
                    i++;
                }
            }
            else
            {
                // 古いセーブ（DisableEventLabels だけのもの）は、ラベルの文字列で戻す
                var disabled = data.DisableEventLabels ?? new List<string>();
                foreach (var lab in colEventLabelList.List)
                {
                    lab.Enable = !disabled.Contains(lab.Data);
                }
            }
        }
    }
}
