using SRCCore.CmdDatas;
using SRCCore.Config;
using SRCCore.Events;
using SRCCore.Lib;
using SRCCore.TestLib;
using SRCCore.Units;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SRCCore.Coverage.Tests.UnitsCoverage
{
    /// <summary>
    /// Unit.message.cs / Unit.anime.cs / Unit.se.cs のテスト用の共通環境。
    /// GUI 呼び出しを記録する MockGUI と、パイロットの乗ったユニットを用意する。
    /// </summary>
    internal sealed class UnitMessageAnimeFixture
    {
        public SRC Src { get; }
        public MockGUI Gui { get; }

        // DisplayBattleMessage に渡された (話者, メッセージ)
        public List<(string pname, string msg)> Messages { get; } = new List<(string, string)>();
        // GUI 呼び出しの順序
        public List<string> Calls { get; } = new List<string>();
        // OpenMessageForm の引数
        public List<(Unit u1, Unit u2)> OpenArgs { get; } = new List<(Unit, Unit)>();
        // Sleep の待ち時間
        public List<int> Sleeps { get; } = new List<int>();
        // DisplayBattleMessage 時点の SelectedTargetForEvent
        public List<Unit> TargetsAtDisplay { get; } = new List<Unit>();

        public bool RButtonPressed { get; set; }

        public UnitMessageAnimeFixture()
        {
            Gui = new MockGUI();
            Src = new SRC { GUI = Gui };
            Src.Event.EventData = new List<EventDataLine>();
            Src.Event.EventCmd = new List<CmdData>();
            Src.Event.EventFileNames = new List<string>();
            Src.Event.AdditionalEventFileNames = new List<string>();
            Src.Event.EventQue = new List<string>();

            Gui.DisplayBattleMessageHandler = (p, m, mode) =>
            {
                Messages.Add((p, m));
                TargetsAtDisplay.Add(Src.Event.SelectedTargetForEvent);
                Calls.Add("DisplayBattleMessage");
            };
            Gui.OpenMessageFormHandler = (u1, u2) =>
            {
                Calls.Add("OpenMessageForm");
                OpenArgs.Add((u1, u2));
                Gui.MessageFormVisible = true;
            };
            Gui.CloseMessageFormHandler = () =>
            {
                Calls.Add("CloseMessageForm");
                Gui.MessageFormVisible = false;
            };
            Gui.ClearPictureHandler = () => Calls.Add("ClearPicture");
            Gui.UpdateScreenHandler = () => Calls.Add("UpdateScreen");
            Gui.CenterHandler = (x, y) => Calls.Add("Center");
            Gui.RefreshScreenHandler = (a, b) => Calls.Add("RefreshScreen");
            Gui.RedrawScreenHandler = _ => Calls.Add("RedrawScreen");
            Gui.SleepHandler = (ms, ev) => { Sleeps.Add(ms); Calls.Add("Sleep"); };
            Gui.IsRButtonPressedHandler = _ => RButtonPressed;
        }

        public LocalFileConfig Config => (LocalFileConfig)Src.SystemConfig;

        /// <summary>
        /// パイロット 1 名が乗ったユニットを作成する。
        /// </summary>
        public Unit CreateUnit(
            string uname = "テスト機",
            string unickname = null,
            string uclass = "テスト系",
            string pname = "テストパイロット",
            string pnickname = null,
            string sex = "男性",
            string party = "味方",
            Action<Models.UnitData> setup = null)
        {
            var pd = Src.PDList.Add(pname);
            pd.Nickname = pnickname ?? pname;
            pd.Sex = sex;
            pd.SP = 100;
            var p = Src.PList.Add(pname, 1, party);

            var ud = Src.UDList.Add(uname);
            ud.Nickname = unickname ?? uname;
            ud.Class = uclass;
            ud.HP = 1000;
            ud.EN = 100;
            ud.PilotNum = 1;
            setup?.Invoke(ud);

            var u = Src.UList.Add(uname, 0, party);
            p.Ride(u);
            u.Update();
            u.FullRecover();
            return u;
        }

        // ── 乱数系列の制御 ──
        // GeneralLib.Dice は静的な乱数系列 RndHistory を参照する。
        // テスト中だけ系列を固定値で埋め、終了時に元へ戻す。
        private static readonly FieldInfo RndHistoryField =
            typeof(GeneralLib).GetField("RndHistory", BindingFlags.NonPublic | BindingFlags.Static);

        public static IDisposable FixDice(double value)
        {
            var history = (double[])RndHistoryField.GetValue(null);
            var saved = (double[])history.Clone();
            var savedIndex = GeneralLib.RndIndex;
            for (var i = 0; i < history.Length; i++)
            {
                history[i] = value;
            }
            return new Restore(() =>
            {
                Array.Copy(saved, history, saved.Length);
                GeneralLib.RndIndex = savedIndex;
            });
        }

        private sealed class Restore : IDisposable
        {
            private readonly Action action;
            public Restore(Action action) { this.action = action; }
            public void Dispose() { action(); }
        }
    }
}
