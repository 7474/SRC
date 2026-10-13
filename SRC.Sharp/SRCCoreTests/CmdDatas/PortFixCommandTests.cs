using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// 移植時の差異を修正したコマンドの動作を元の SRC (CmdData.cls) と比較するテスト
    /// </summary>
    [TestClass]
    public class PortFixCommandTests
    {
        private static (SRC src, MockGUI gui) CreateSrc()
        {
            var gui = new MockGUI();
            var src = new SRC { GUI = gui };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new List<string>();
            return (src, gui);
        }

        private static CmdData[] BuildEvent(SRC src, params string[] lines)
        {
            var parser = new CmdParser();
            var cmds = new CmdData[lines.Length];
            for (var i = 0; i < lines.Length; i++)
            {
                var line = new EventDataLine(i, EventDataSource.Scenario, "test", i, lines[i]);
                src.Event.EventData.Add(line);
                var cmd = parser.Parse(src, line);
                src.Event.EventCmd.Add(cmd);
                cmds[i] = cmd;
            }
            for (var i = 0; i < lines.Length; i++)
            {
                var trimmed = lines[i].Trim();
                if (trimmed.EndsWith(":"))
                {
                    src.Event.AddLabel(trimmed.Substring(0, trimmed.Length - 1).Trim(), i);
                }
            }
            return cmds;
        }

        private static void Run(CmdData[] cmds, int maxSteps = 1000)
        {
            var current = 0;
            for (var step = 0; step < maxSteps && current >= 0 && current < cmds.Length; step++)
            {
                current = cmds[current].Exec();
            }
        }

        private static Units.Unit CreateUnit(SRC src, string name, int rank = 0)
        {
            if (!src.UDList.IsDefined(name))
            {
                var ud = src.UDList.Add(name);
                ud.HP = 1000;
                ud.EN = 100;
            }
            var u = src.UList.Add(name, rank, "味方");
            u.Update();
            u.FullRecover();
            return u;
        }

        // Local コマンドの複数項の代入式は引用符を残したまま評価する
        [TestMethod]
        public void LocalCmd_MultiTermAssignment_KeepsQuotes()
        {
            var (src, _) = CreateSrc();
            var cmds = BuildEvent(src,
                "Call mysub",
                "Exit",
                "mysub:",
                "Local x = \"a b\" & \"c\"",
                "Set result x",
                "Return");
            Run(cmds);
            Assert.AreEqual("a bc", src.Expression.GetValueAsString("result"));
        }

        // RemoveItem アイテムID でユニットから取り外される
        [TestMethod]
        public void RemoveItemCmd_ByItemId_DeletesFromUnit()
        {
            var (src, _) = CreateSrc();
            var u = CreateUnit(src, "テスト機");
            src.IDList.Add("テスト装備");
            var itm = src.IList.Add("テスト装備");
            u.AddItem(itm);
            Assert.AreEqual(1, u.CountItem());

            var cmds = BuildEvent(src, "RemoveItem " + itm.ID);
            Assert.AreEqual(1, cmds[0].Exec());

            Assert.AreEqual(0, u.CountItem());
            Assert.IsNull(itm.Unit);
            Assert.IsFalse(itm.Exist);
        }

        // RemoveItem アイテム名で装備中のアイテムをユニットから取り外す
        [TestMethod]
        public void RemoveItemCmd_ByItemName_DeletesFromUnit()
        {
            var (src, _) = CreateSrc();
            var u = CreateUnit(src, "テスト機");
            src.IDList.Add("テスト装備");
            var itm = src.IList.Add("テスト装備");
            u.AddItem(itm);

            var cmds = BuildEvent(src, "RemoveItem テスト装備");
            Assert.AreEqual(1, cmds[0].Exec());

            Assert.AreEqual(0, u.CountItem());
            Assert.IsNull(itm.Unit);
            Assert.IsFalse(itm.Exist);
        }

        // RemoveItem 装備されていないアイテムが優先される
        [TestMethod]
        public void RemoveItemCmd_ByItemName_PrefersUnequipped()
        {
            var (src, _) = CreateSrc();
            var u = CreateUnit(src, "テスト機");
            src.IDList.Add("テスト装備");
            var equipped = src.IList.Add("テスト装備");
            u.AddItem(equipped);
            var free = src.IList.Add("テスト装備");

            var cmds = BuildEvent(src, "RemoveItem テスト装備");
            Assert.AreEqual(1, cmds[0].Exec());

            Assert.IsFalse(free.Exist);
            Assert.IsTrue(equipped.Exist);
            Assert.AreEqual(1, u.CountItem());
        }

        // RemoveItem 存在しないアイテム名の場合は何もしない
        [TestMethod]
        public void RemoveItemCmd_UnknownItemName_DoesNothing()
        {
            var (src, _) = CreateSrc();
            var cmds = BuildEvent(src, "RemoveItem 存在しない装備");
            Assert.AreEqual(1, cmds[0].Exec());
        }

        // Talk の引数なし行でオプションはメッセージではなく表示モードとして渡される
        [TestMethod]
        public void TalkCmd_NoArgLine_PassesOptionsAsMode()
        {
            var (src, gui) = CreateSrc();
            var calls = new List<(string pname, string msg, string mode)>();
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.CloseMessageFormHandler = () => { };
            gui.DisplayMessageHandler = (pname, msg, mode) => calls.Add((pname, msg, mode));
            src.Stage = "プロローグ";

            var cmds = BuildEvent(src,
                "Talk システム 白黒",
                "Talk",
                "End");
            cmds[0].Exec();

            Assert.IsTrue(calls.Count > 0);
            Assert.AreEqual("システム", calls[0].pname);
            Assert.AreEqual("", calls[0].msg);
            Assert.AreEqual("白黒 ", calls[0].mode);
        }

        // ForEach で未定義の名前を指定した場合は1回も実行しない
        [TestMethod]
        public void ForEachCmd_UndefinedName_IteratesZeroTimes()
        {
            var (src, _) = CreateSrc();
            var cmds = BuildEvent(src,
                "Set count 0",
                "ForEach item In 未定義配列",
                "Incr count",
                "Next",
                "Set done 1");
            Run(cmds);
            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("count"));
            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("done"));
        }

        // ForEach でリストの形をした引数はリストとして扱う
        [TestMethod]
        public void ForEachCmd_ListFunction_Iterates()
        {
            var (src, _) = CreateSrc();
            var cmds = BuildEvent(src,
                "Set count 0",
                "ForEach item In List(a,b,c)",
                "Incr count",
                "Next");
            Run(cmds);
            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("count"));
        }

        // Call のラベル名は存在すればそのまま使う（式として評価しない）
        [TestMethod]
        public void CallCmd_ExistingLabel_NotEvaluated()
        {
            var (src, _) = CreateSrc();
            src.Expression.SetVariableAsString("mysub", "othersub");
            var cmds = BuildEvent(src,
                "Call mysub",
                "Exit",
                "mysub:",
                "Set result \"mysub\"",
                "Return",
                "othersub:",
                "Set result \"othersub\"",
                "Return");
            Run(cmds);
            Assert.AreEqual("mysub", src.Expression.GetValueAsString("result"));
        }

        // Call のラベル名が存在しない場合は式として評価する
        [TestMethod]
        public void CallCmd_Expression_Evaluated()
        {
            var (src, _) = CreateSrc();
            src.Expression.SetVariableAsString("subname", "othersub");
            var cmds = BuildEvent(src,
                "Call subname",
                "Exit",
                "othersub:",
                "Set result \"othersub\"",
                "Return");
            Run(cmds);
            Assert.AreEqual("othersub", src.Expression.GetValueAsString("result"));
        }

        private static (Filesystem.FileHandle handle, MemoryStream ms) OpenWrite(SRC src)
        {
            var ms = new MemoryStream();
            var handle = src.FileHandleManager.Add(Filesystem.SafeOpenMode.Write, SRCCompatibilityMode.None, ms);
            return (handle, ms);
        }

        private static string Written(Filesystem.FileHandle handle, MemoryStream ms)
        {
            handle.Writer.Flush();
            return Encoding.UTF8.GetString(ms.ToArray()).TrimStart('﻿');
        }

        // Print の末尾が「;」の場合は改行しない
        [TestMethod]
        public void PrintCmd_TrailingSemicolon_NoNewLine()
        {
            var (src, _) = CreateSrc();
            var (handle, ms) = OpenWrite(src);
            var cmds = BuildEvent(src,
                $"Print {handle.Handle} abc;",
                $"Print {handle.Handle} def");
            Run(cmds);
            Assert.AreEqual("abcdef" + System.Environment.NewLine, Written(handle, ms));
        }

        // Write は VB の Write # と同様に引用符で囲んで出力する
        [TestMethod]
        public void WriteCmd_QuotesValues()
        {
            var (src, _) = CreateSrc();
            var (handle, ms) = OpenWrite(src);
            var cmds = BuildEvent(src, $"Write {handle.Handle} \"a,b\" 12");
            Run(cmds);
            var nl = System.Environment.NewLine;
            Assert.AreEqual("\"a,b\"" + nl + "\"12\"" + nl, Written(handle, ms));
        }

        // Read は VB の Input # と同様にカンマ区切り・引用符を解釈する
        [TestMethod]
        public void ReadCmd_ParsesCommaAndQuotes()
        {
            var (src, _) = CreateSrc();
            var content = Encoding.UTF8.GetBytes("\"a,b\",c  ,  d\r\n\"e\"\r\nf\n");
            var ms = new MemoryStream(content);
            var handle = src.FileHandleManager.Add(Filesystem.SafeOpenMode.Read, SRCCompatibilityMode.None, ms);
            var cmds = BuildEvent(src, $"Read {handle.Handle} A B C D E");
            Run(cmds);
            Assert.AreEqual("a,b", src.Expression.GetValueAsString("A"));
            Assert.AreEqual("c", src.Expression.GetValueAsString("B"));
            Assert.AreEqual("d", src.Expression.GetValueAsString("C"));
            Assert.AreEqual("e", src.Expression.GetValueAsString("D"));
            Assert.AreEqual("f", src.Expression.GetValueAsString("E"));
        }

        // Write で出力した内容を Read で読み戻せる
        [TestMethod]
        public void WriteThenRead_RoundTrip()
        {
            var (src, _) = CreateSrc();
            var (handle, ms) = OpenWrite(src);
            BuildEvent(src, $"Write {handle.Handle} \"x, y\" \"z\"")[0].Exec();
            var text = Written(handle, ms);
            using var reader = new StringReader(text);
            Assert.AreEqual("x, y", ReadCmd.ReadInputField(reader));
            Assert.AreEqual("z", ReadCmd.ReadInputField(reader));
        }

        // Equip でマップ表示中なら出撃中ユニットの画像を再描画する
        [TestMethod]
        public void EquipCmd_OnMap_RepaintsUnit()
        {
            var (src, gui) = CreateSrc();
            var painted = 0;
            gui.PaintUnitBitmapHandler = (u, m) => painted++;
            src.Map.MapFileName = "test.map";
            var unit = CreateUnit(src, "テスト機");
            unit.Status = "出撃";
            src.IDList.Add("テスト装備");
            src.Event.SelectedUnitForEvent = unit;

            var cmds = BuildEvent(src, "Equip テスト装備");
            Assert.AreEqual(1, cmds[0].Exec());

            Assert.AreEqual(1, unit.CountItem());
            Assert.AreEqual(1, painted);
        }

        // Equip でステータス表示中は再描画しない
        [TestMethod]
        public void EquipCmd_StatusView_DoesNotRepaint()
        {
            var (src, gui) = CreateSrc();
            var painted = 0;
            gui.PaintUnitBitmapHandler = (u, m) => painted++;
            src.Map.MapFileName = "";
            var unit = CreateUnit(src, "テスト機");
            unit.Status = "出撃";
            src.IDList.Add("テスト装備");
            src.Event.SelectedUnitForEvent = unit;

            var cmds = BuildEvent(src, "Equip テスト装備");
            Assert.AreEqual(1, cmds[0].Exec());
            Assert.AreEqual(0, painted);
        }

        // RankUp 合体可能なユニットの場合、他の分離ユニットのランクは相対的に変化する
        [TestMethod]
        public void RankUpCmd_NegativeOnCombinedPart_AppliesRelatively()
        {
            var (src, _) = CreateSrc();
            var cud = src.UDList.Add("合体機");
            cud.AddFeature("分離=分離 Ａパーツ Ｂパーツ");
            cud.AddFeature("主形態");
            var aud = src.UDList.Add("Ａパーツ");
            aud.HP = 1000;
            aud.EN = 100;
            aud.AddFeature("合体=合体 合体機 Ｂパーツ");
            var bud = src.UDList.Add("Ｂパーツ");
            bud.HP = 1000;
            bud.EN = 100;
            bud.AddFeature("合体=合体 合体機 Ａパーツ");
            var a = CreateUnit(src, "Ａパーツ", 3);
            var b = CreateUnit(src, "Ｂパーツ", 1);

            var cmds = BuildEvent(src, "RankUp Ａパーツ -1");
            Assert.AreEqual(1, cmds[0].Exec());

            Assert.AreEqual(2, a.Rank);
            Assert.AreEqual(0, b.Rank);
        }
    }
}
