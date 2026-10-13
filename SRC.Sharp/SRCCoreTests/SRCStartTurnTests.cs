using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.Tests
{
    /// <summary>
    /// SRC.StartTurn（陣営のフェイズを実行する。元の SRC の StartTurn）
    /// </summary>
    [TestClass]
    public class SRCStartTurnTests
    {
        private static SRC CreateSrc()
        {
            var src = new SRC
            {
                GUI = new MockGUI
                {
                    CenterHandler = (x, y) => { },
                    RedrawScreenHandler = (late) => { },
                    RefreshScreenHandler = (a, b) => { },
                    UnlockGUIHandler = () => { },
                    LockGUIHandler = () => { },
                    DisplayTelopHandler = (m) => { },
                    SleepHandler = (ms, e) => { },
                    ResetDrawStringHandler = () => { },
                    UpdateScreenHandler = () => { },
                    MainFormShowHandler = () => { },
                },
            };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new Queue<string>();
            src.Titles = new List<string>();
            src.Turn = 0;
            return src;
        }

        [TestMethod]
        [Timeout(10000)]
        public void StartTurn_NoUnitsToOperate_DoesNotRunCPUPhases()
        {
            // 操作可能な味方ユニットも、CPU が操作するユニットもいない場合は
            // 敵フェイズ等を繰り返さずに味方フェイズを開始する
            // （判定が逆だと敵・中立・ＮＰＣフェイズと味方フェイズを無限に繰り返す）
            // マップ未ロード（ステータス表示用）なのでターン数は進まない
            var src = CreateSrc();

            src.StartTurn("味方");

            Assert.AreEqual(0, src.Turn);
            Assert.AreEqual("味方", src.Stage);
        }
    }
}
