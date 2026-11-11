using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.Expressions;
using SRCCore.Items;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Tests
{
    /// <summary>
    /// VB の String の変数・クラスの String のメンバーは "" で始まる。移した C# のフィールド・プロパティも "" で始まることのテスト。
    /// SRCPortingAnalyzers の SRCP003（7474/SRC#1555）で見つけた箇所。
    /// </summary>
    [TestClass]
    public class StringInitialValueVBMatchTests
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        private static void AssertEmpty(object obj, params string[] members)
        {
            var type = obj.GetType();
            foreach (var name in members)
            {
                object value = null;
                var found = false;
                for (var t = type; t != null && !found; t = t.BaseType)
                {
                    var field = t.GetField(name, Flags | BindingFlags.DeclaredOnly);
                    if (field != null)
                    {
                        value = field.GetValue(obj);
                        found = true;
                        continue;
                    }
                    var property = t.GetProperty(name, Flags | BindingFlags.DeclaredOnly);
                    if (property != null)
                    {
                        value = property.GetValue(obj);
                        found = true;
                    }
                }
                Assert.IsTrue(found, $"{type.Name}.{name} がありません");
                Assert.AreEqual("", value, $"{type.Name}.{name}");
            }
        }

        // Unit.cls: Public ID As String など（Class_Initialize は Status だけを入れる）
        [TestMethod]
        public void Unit_文字列のメンバーは空文字列で始まる()
        {
            AssertEmpty(new Unit(CreateSrc()),
                "ID", "Area", "strMode", "strName", "strParty", "strAdaption",
                "strAbsorb", "strImmune", "strResist", "strWeakness", "strEffective", "strSpecialEffectImmune");
        }

        // Unit.cls: Private strWeaponClass() As String（武器ごとの配列の要素）
        [TestMethod]
        public void UnitWeapon_文字列のメンバーは空文字列で始まる()
        {
            var src = CreateSrc();
            var u = new Unit(src);
            var wd = new WeaponData(src) { Name = "武器" };
            var uw = new UnitWeapon(src, u, wd);
            Assert.AreEqual("武器", uw.Name);
            AssertEmpty(uw, "strWeaponClass");
        }

        // Pilot.cls: Public ID As String・Public Party As String・Public Adaption As String
        [TestMethod]
        public void Pilot_文字列のメンバーは空文字列で始まる()
        {
            var src = CreateSrc();
            // Adaption は Update でパイロットのデータから入るので見ない
            AssertEmpty(new Pilot(src, src.PDList.Item("ステータス表示用ダミーパイロット(ザコ)")), "ID", "Party");
        }

        // Item.cls: Public ID As String
        [TestMethod]
        public void Item_文字列のメンバーは空文字列で始まる()
        {
            AssertEmpty(new Item(CreateSrc()), "ID");
        }

        // *Data.cls などのデータのクラスの Public 〜 As String
        [TestMethod]
        public void データのクラスの文字列のメンバーは空文字列で始まる()
        {
            var src = CreateSrc();
            AssertEmpty(new AbilityData(src), "Name");
            AssertEmpty(new AbilityEffect(), "Name", "Data");
            AssertEmpty(new AliasDataElement(), "strAliasType", "strAliasData", "strAliasNecessarySkill", "strAliasNecessaryCondition");
            AssertEmpty(new AliasDataType(), "Name");
            AssertEmpty(new BattleConfigData(src), "Name");
            AssertEmpty(new Condition(), "Name", "StrData");
            AssertEmpty(new Dialog(src), "Situation");
            AssertEmpty(new DialogData(src), "Name");
            AssertEmpty(new FeatureData(), "Name", "StrData", "NecessarySkill", "NecessaryCondition");
            AssertEmpty(new ItemData(src), "Name", "Class", "Part", "Comment", "proNickname", "proKanaName");
            AssertEmpty(new MessageData(src), "Name");
            AssertEmpty(new NonPilotData(src), "Name", "strNickname", "proBitmap");
            AssertEmpty(new PilotData(src), "Name", "last_sname", "last_sdata", "Sex", "Class", "Adaption", "Personality", "BGM",
                "proNickname", "proKanaName", "proBitmap");
            AssertEmpty(new SkillData(), "Name", "StrData");
            AssertEmpty(new SpecialPowerData(src), "Name", "KanaName", "ShortName", "TargetType", "Duration", "NecessaryCondition",
                "Animation", "Comment");
            AssertEmpty(new SpecialPowerEffect(), "strEffectType", "strEffectData");
            AssertEmpty(new TerrainData(), "Name", "Bitmap", "Class");
            AssertEmpty(new UnitData(src), "Name", "Class", "Adaption", "Transportation", "Size", "proNickname", "proKanaName", "proBitmap");
            AssertEmpty(new WeaponData(src), "Name", "Adaption", "Class", "NecessarySkill", "NecessaryCondition");
            AssertEmpty(new LabelData(src), "StrData");
            AssertEmpty(new VarData(), "Name", "StringValue");
            AssertEmpty(new CmdArgument(), "strArg");
        }

        // 標準モジュール（Command.bas・Effect.bas・Event.bas・Map.bas・Sound.bas）の String の変数
        [TestMethod]
        public void モジュールの文字列の変数は空文字列で始まる()
        {
            var src = CreateSrc();
            AssertEmpty(src.Commands, "CommandState", "PrevUnitArea", "PrevCommand");
            // MainForm.mnuUnitCommandItem(AbilityCmdID).Caption は、初めはフォーム（Main.frm）に書いてある「アビリティ」
            Assert.AreEqual("アビリティ", src.Commands.AbilityCommandCaption);
            AssertEmpty(src.Effect, "WeaponInHand", "CurrentWeaponType");
            AssertEmpty(src.Event, "ObjDrawOption", "LastUnitName");
            AssertEmpty(new ObjectDrawSetting(), "ObjDrawOption");
            AssertEmpty(src.Map, "MapFileName", "MapDrawMode");
            AssertEmpty(src.Sound, "BGMFileName");
        }

        // GUI.bas: ListItemComment()・ListItemID() As String（ReDim した要素は ""）、ListBox の lb_caption・lb_info
        [TestMethod]
        public void リストボックスの文字列は空文字列で始まる()
        {
            AssertEmpty(new ListBoxItem(), "Text", "ListItemComment", "ListItemID");
            AssertEmpty(new ListBoxArgs(), "lb_caption", "lb_info");
        }
    }
}
