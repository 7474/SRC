using SRCCore.VB;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SRCCore.Events
{
    public partial class Event
    {
        // イベントデータの消去
        // ただしグローバル変数のデータは残しておく
        public void ClearEventData()
        {
            SelectedUnitForEvent = null;
            SelectedTargetForEvent = null;
            // システム側のイベントデータだけを残す（VB: ReDim Preserve EventData(SysEventDataSize)）
            EventData = EventData.Where(x => x.IsSystemData).ToList();
            colNormalLabelList.Clear();
            // システムのイベント（本体側の Lib など）が無い場合はすべてのラベルを消す（元の SRC の SysEventDataSize = 0）
            var maxEventId = EventData.Count > 0 ? EventData.Max(x => x.ID) : -1;
            foreach (var label in colEventLabelList.List.Where(x => x.EventDataId > maxEventId).ToList())
            {
                colEventLabelList.Remove(label);
            }

            EventQue.Clear();
            CallDepth = 0;
            ArgIndex = 0;
            // VB の VarIndex = 0（空）は、C# では -1
            VarIndex = -1;
            ForIndex = 0;
            UpVarLevel = 0;
            // VB の ReDim HotPointList(0)（要素の無い一覧）。配列にすると HotPoint で追加できない
            HotPointList = new List<HotPoint>();
            ObjColor = Color.White;
            ObjFillColor = Color.White;
            ObjFillStyle = FillStyle.VbFSTransparent;
            ObjDrawWidth = 1;
            ObjDrawOption = "";
            GUI.IsPictureVisible = false;
            GUI.IsCursorVisible = false;
            GUI.PaintedAreaX1 = GUI.MainPWidth;
            GUI.PaintedAreaY1 = GUI.MainPHeight;
            GUI.PaintedAreaX2 = -1;
            GUI.PaintedAreaY2 = -1;
            LocalVariableList.Clear();
        }

        // VB の ClearAllEventData は VB でも呼ばれていないので移植しない。
        // 一時中断用データの DumpEventData・RestoreEventData・SkipEventData と、
        // 変数の SaveGlobalVariables・LoadGlobalVariables・SaveLocalVariables・LoadLocalVariables は、
        // JSON のセーブ（SRC.save.cs の SRCSaveData・SRCSuspendData と Event.Restore）に置き換えている。
    }
}
