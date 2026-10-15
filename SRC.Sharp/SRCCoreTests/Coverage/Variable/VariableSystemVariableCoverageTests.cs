using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using System;
using VT = SRCCore.Expressions.ValueType;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Variable.cs の GetVariable / GetVariableObject によるシステム変数・コンフィグ変数の参照テスト
    /// （VB: Expression.bas GetVariable、ヘルプ: 変数.md）
    /// </summary>
    [TestClass]
    public class VariableSystemVariableCoverageTests
    {
        private static SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        private static Units.Unit CreateUnit(SRC src, string name, string party = "味方")
        {
            if (!src.UDList.IsDefined(name))
            {
                var ud = src.UDList.Add(name);
                ud.HP = 1000;
                ud.EN = 100;
                ud.PilotNum = 1;
            }
            var u = src.UList.Add(name, 0, party);
            u.Update();
            u.FullRecover();
            return u;
        }

        private static Pilots.Pilot CreatePilot(SRC src, string name)
        {
            var pd = src.PDList.Add(name);
            pd.SP = 50;
            return src.PList.Add(name, 1, "味方");
        }

        private static string GetStr(SRC src, string name, out VT type)
        {
            type = src.Expression.GetVariable(name, VT.StringType, out var s, out _);
            return s;
        }

        private static double GetNum(SRC src, string name, out VT type)
        {
            type = src.Expression.GetVariable(name, VT.NumericType, out _, out var n);
            return n;
        }

        // ──────────────────────────────────────────────
        // 未定義の変数・配列要素
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetVariable_UndefinedScalar_ReturnsNameAndUndefinedType()
        {
            // 未定義の変数は変数名そのものを返す
            var src = CreateSrc();
            var s = GetStr(src, "未定義変数", out var type);

            Assert.AreEqual(VT.UndefinedType, type);
            Assert.AreEqual("未定義変数", s);
        }

        [TestMethod]
        public void GetVariable_UndefinedArrayElement_ReturnsEmptyString()
        {
            // 値が設定されていない配列要素を読み出すと空文字列（ヘルプ: 変数.md）
            var src = CreateSrc();
            src.Expression.SetVariableAsString("arr[1]", "a");

            var s = GetStr(src, "arr[2]", out var type);

            Assert.AreEqual(VT.UndefinedType, type);
            Assert.AreEqual("", s);
        }

        [TestMethod]
        public void GetVariable_ArrayElementWithExpressionIndex_ResolvesIndex()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("k", 2d);
            src.Expression.SetVariableAsDouble("arr[3]", 30d);

            Assert.AreEqual(30d, GetNum(src, "arr[k + 1]", out var type));
            Assert.AreEqual(VT.NumericType, type);
        }

        [TestMethod]
        public void GetVariable_GlobalVariable_StringReferenceOfNumeric_IsFormatted()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("g");
            src.Expression.SetVariableAsDouble("g", 2.5d);

            Assert.AreEqual("2.5", GetStr(src, "g", out var type));
            Assert.AreEqual(VT.StringType, type);
        }

        // ──────────────────────────────────────────────
        // 対象ユニット・相手ユニット
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectedUnitVariables_ReturnMainPilotIdAndUnitId()
        {
            // 対象ユニット/対象パイロットはメインパイロットのID、対象ユニットＩＤはユニットのID
            var src = CreateSrc();
            var u = CreateUnit(src, "対象機");
            var p = CreatePilot(src, "対象乗員");
            p.Ride(u);
            var t = CreateUnit(src, "相手機", "敵");
            var tp = CreatePilot(src, "相手乗員");
            tp.Ride(t);
            src.Event.SelectedUnitForEvent = u;
            src.Event.SelectedTargetForEvent = t;

            Assert.AreEqual(p.ID, GetStr(src, "対象ユニット", out var t1));
            Assert.AreEqual(VT.StringType, t1);
            Assert.AreEqual(p.ID, GetStr(src, "対象パイロット", out _));
            Assert.AreEqual(tp.ID, GetStr(src, "相手ユニット", out _));
            Assert.AreEqual(tp.ID, GetStr(src, "相手パイロット", out _));
            Assert.AreEqual(u.ID, GetStr(src, "対象ユニットＩＤ", out _));
            Assert.AreEqual(t.ID, GetStr(src, "相手ユニットＩＤ", out _));
        }

        [TestMethod]
        public void SelectedUnitVariables_NoUnitOrNoPilot_ReturnEmptyString()
        {
            // ユニット未選択、またはパイロット不在の場合は空文字列（変数名ではない）
            var src = CreateSrc();
            var empty = CreateUnit(src, "無人機");
            src.Event.SelectedUnitForEvent = empty;
            src.Event.SelectedTargetForEvent = null;

            Assert.AreEqual("", GetStr(src, "対象ユニット", out var type));
            Assert.AreEqual(VT.StringType, type);
            Assert.AreEqual("", GetStr(src, "相手ユニット", out _));
            Assert.AreEqual("", GetStr(src, "相手ユニットＩＤ", out _));

            src.Event.SelectedUnitForEvent = null;
            Assert.AreEqual("", GetStr(src, "対象ユニットＩＤ", out _));
        }

        [TestMethod]
        public void UsedWeapon_AttackerSide_ReturnsSelectedWeapon()
        {
            // 対象ユニットが攻撃側（SelectedUnit）なら SelectedWeapon、相手ユニットが防御側なら SelectedTWeapon
            var src = CreateSrc();
            var atk = CreateUnit(src, "攻撃機");
            var def = CreateUnit(src, "防御機", "敵");
            src.Commands.SelectedUnit = atk;
            src.Commands.SelectedTarget = def;
            src.Event.SelectedUnitForEvent = atk;
            src.Event.SelectedTargetForEvent = def;
            src.Commands.SelectedWeapon = 2;
            src.Commands.SelectedWeaponName = "ビームライフル";
            src.Commands.SelectedTWeapon = 1;
            src.Commands.SelectedTWeaponName = "バルカン";

            Assert.AreEqual("ビームライフル", GetStr(src, "対象ユニット使用武器", out _));
            Assert.AreEqual(2d, GetNum(src, "対象ユニット使用武器番号", out var t1));
            Assert.AreEqual(VT.NumericType, t1);
            Assert.AreEqual("バルカン", GetStr(src, "相手ユニット使用武器", out _));
            Assert.AreEqual(1d, GetNum(src, "相手ユニット使用武器番号", out _));
        }

        [TestMethod]
        public void UsedWeapon_DefenderSide_ReturnsDefenseOptionWhenNoCounterWeapon()
        {
            // 対象ユニットが防御側で反撃武器が無い場合は防御方法（防御/回避など）を返す
            var src = CreateSrc();
            var atk = CreateUnit(src, "攻撃機", "敵");
            var def = CreateUnit(src, "防御機");
            src.Commands.SelectedUnit = atk;
            src.Commands.SelectedTarget = def;
            src.Event.SelectedUnitForEvent = def;
            src.Event.SelectedTargetForEvent = atk;
            src.Commands.SelectedWeapon = 3;
            src.Commands.SelectedWeaponName = "ミサイル";
            src.Commands.SelectedTWeapon = 0;
            src.Commands.SelectedDefenseOption = "防御";

            Assert.AreEqual("防御", GetStr(src, "対象ユニット使用武器", out _));
            Assert.AreEqual(0d, GetNum(src, "対象ユニット使用武器番号", out _));
            Assert.AreEqual("ミサイル", GetStr(src, "相手ユニット使用武器", out _));
            Assert.AreEqual(3d, GetNum(src, "相手ユニット使用武器番号", out _));
        }

        [TestMethod]
        public void UsedWeapon_AttackerWithoutWeapon_ReturnsEmpty()
        {
            var src = CreateSrc();
            var atk = CreateUnit(src, "攻撃機");
            var def = CreateUnit(src, "防御機", "敵");
            src.Commands.SelectedUnit = atk;
            src.Commands.SelectedTarget = def;
            src.Event.SelectedUnitForEvent = atk;
            src.Event.SelectedTargetForEvent = atk;
            src.Commands.SelectedWeapon = 0;
            src.Commands.SelectedWeaponName = "前回の武器";

            Assert.AreEqual("", GetStr(src, "対象ユニット使用武器", out _));
            Assert.AreEqual("", GetStr(src, "相手ユニット使用武器", out _));
        }

        [TestMethod]
        public void UsedAbilityAndSpecialPower_OnlyForSelectedUnit()
        {
            // アビリティ・スペシャルパワーは対象ユニットが SelectedUnit のときだけ値を持つ
            var src = CreateSrc();
            var u = CreateUnit(src, "使用機");
            var other = CreateUnit(src, "別機");
            src.Commands.SelectedUnit = u;
            src.Commands.SelectedAbility = 4;
            src.Commands.SelectedAbilityName = "修理装置";
            src.Commands.SelectedSpecialPower = "熱血";

            src.Event.SelectedUnitForEvent = u;
            Assert.AreEqual("修理装置", GetStr(src, "対象ユニット使用アビリティ", out _));
            Assert.AreEqual(4d, GetNum(src, "対象ユニット使用アビリティ番号", out _));
            Assert.AreEqual("熱血", GetStr(src, "対象ユニット使用スペシャルパワー", out _));

            src.Commands.SelectedAbility = 0;
            Assert.AreEqual("", GetStr(src, "対象ユニット使用アビリティ", out _));

            src.Event.SelectedUnitForEvent = other;
            Assert.AreEqual(0d, GetNum(src, "対象ユニット使用アビリティ番号", out _));
            Assert.AreEqual("", GetStr(src, "対象ユニット使用スペシャルパワー", out _));
        }

        [TestMethod]
        public void SupportUnitIds_ReturnIdOrEmpty()
        {
            var src = CreateSrc();
            var sa = CreateUnit(src, "援護機");

            Assert.AreEqual("", GetStr(src, "サポートアタックユニットＩＤ", out var type));
            Assert.AreEqual(VT.StringType, type);
            Assert.AreEqual("", GetStr(src, "サポートガードユニットＩＤ", out _));

            src.Commands.SupportAttackUnit = sa;
            src.Commands.SupportGuardUnit = sa;
            Assert.AreEqual(sa.ID, GetStr(src, "サポートアタックユニットＩＤ", out _));
            Assert.AreEqual(sa.ID, GetStr(src, "サポートガードユニットＩＤ", out _));
        }

        // ──────────────────────────────────────────────
        // 選択・ターン・フェイズ・資金・陣営数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Selection_NumericAndTextAlternative()
        {
            // 選択 は Ask 等で選ばれた選択肢。数値として参照すれば数値になる
            var src = CreateSrc();
            src.Event.SelectedAlternative = "3";
            Assert.AreEqual(3d, GetNum(src, "選択", out var t1));
            Assert.AreEqual(VT.NumericType, t1);
            Assert.AreEqual("3", GetStr(src, "選択", out _));

            src.Event.SelectedAlternative = "はい";
            Assert.AreEqual("はい", GetStr(src, "選択", out var t2));
            Assert.AreEqual(VT.StringType, t2);
        }

        [TestMethod]
        public void TurnPhaseAndMoney_ReflectSrcState()
        {
            var src = CreateSrc();
            src.Turn = 4;
            src.TotalTurn = 25;
            src.Stage = "敵";
            src.Money = 12345;

            Assert.AreEqual(4d, GetNum(src, "ターン数", out _));
            Assert.AreEqual("4", GetStr(src, "ターン数", out _));
            Assert.AreEqual(25d, GetNum(src, "総ターン数", out _));
            Assert.AreEqual("敵", GetStr(src, "フェイズ", out var t1));
            Assert.AreEqual(VT.StringType, t1);
            Assert.AreEqual(12345d, GetNum(src, "資金", out _));
        }

        [TestMethod]
        public void PartyCounts_CountOnlySortiedOrStoredUnitsOfParty()
        {
            // 味方数等は 出撃・格納 状態のユニット数（ヘルプ: 変数.md）
            var src = CreateSrc();
            CreateUnit(src, "味方1").Status = "出撃";
            CreateUnit(src, "味方2").Status = "格納";
            CreateUnit(src, "味方3").Status = "待機";
            CreateUnit(src, "味方4").Status = "破壊";
            CreateUnit(src, "敵1", "敵").Status = "出撃";
            CreateUnit(src, "中立1", "中立").Status = "出撃";
            CreateUnit(src, "中立2", "中立").Status = "格納";

            Assert.AreEqual(2d, GetNum(src, "味方数", out _));
            Assert.AreEqual(1d, GetNum(src, "敵数", out _));
            Assert.AreEqual(2d, GetNum(src, "中立数", out _));
            Assert.AreEqual(0d, GetNum(src, "ＮＰＣ数", out var t));
            Assert.AreEqual(VT.NumericType, t);
            Assert.AreEqual("0", GetStr(src, "ＮＰＣ数", out _));
        }

        // ──────────────────────────────────────────────
        // 英字のシステム変数（大文字小文字を区別しない）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AlphabetSystemVariables_AreCaseInsensitive()
        {
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            src.Event.BaseX = 10;
            src.Event.BaseY = 20;
            mock.MouseX = 33;
            mock.MouseY = 44;
            src.ScenarioPath = "/scenario/";
            src.SystemConfig.ExtDataPath = "/ext1/";
            src.SystemConfig.ExtDataPath2 = "/ext2/";

            Assert.AreEqual(10d, GetNum(src, "BaseX", out _));
            Assert.AreEqual(20d, GetNum(src, "BASEY", out _));
            Assert.AreEqual(33d, GetNum(src, "MouseX", out _));
            Assert.AreEqual(44d, GetNum(src, "mousey", out _));
            Assert.AreEqual("/scenario/", GetStr(src, "ScenarioPath", out _));
            Assert.AreEqual("/ext1/", GetStr(src, "ExtDataPath", out _));
            Assert.AreEqual("/ext2/", GetStr(src, "extdatapath2", out _));
            Assert.AreEqual(src.AppPath, GetStr(src, "AppPath", out var t));
            Assert.AreEqual(VT.StringType, t);
        }

        [TestMethod]
        public void AppVersion_IsMajor10000PlusMinor100PlusRevision()
        {
            // VB: 10000 * App.Major + 100 * App.Minor + App.Revision
            var src = CreateSrc();
            src.CompatibleVersion = new Version(2, 2, 33);
            Assert.AreEqual(20233d, GetNum(src, "AppVersion", out _));

            // 3 番目の数が無い場合は 0 として扱う
            src.CompatibleVersion = new Version(3, 1);
            Assert.AreEqual(30100d, GetNum(src, "appversion", out _));
        }

        [TestMethod]
        public void ArgNum_IsArgumentCountOfCurrentSubroutine()
        {
            // 引数の数 = ArgIndex - ArgIndexStack(CallDepth - 1)
            var src = CreateSrc();
            src.Event.CallDepth = 2;
            src.Event.ArgIndexStack[0] = 1;
            src.Event.ArgIndexStack[1] = 4;
            src.Event.ArgIndex = 6;
            src.Event.UpVarLevel = 0;

            Assert.AreEqual(2d, GetNum(src, "argnum", out _));
        }

        [TestMethod]
        public void ArgNum_UnderUpVar_ReferencesCallerArguments()
        {
            // UpVar 中は呼び出し元のサブルーチンの引数の数（UpVarLevel だけ遡る）
            var src = CreateSrc();
            src.Event.CallDepth = 2;
            src.Event.ArgIndexStack[0] = 1;
            src.Event.ArgIndexStack[1] = 4;
            src.Event.ArgIndex = 6;
            src.Event.UpVarLevel = 1;
            src.Event.UpVarLevelStack[1] = 0;

            Assert.AreEqual(5d, GetNum(src, "ArgNum", out _));
        }

        // ──────────────────────────────────────────────
        // バトルコンフィグ変数
        // ──────────────────────────────────────────────

        private static (Units.Unit u, Pilots.Pilot p) SetupBattleConfig(SRC src)
        {
            var u = CreateUnit(src, "計算機");
            var p = CreatePilot(src, "計算乗員");
            p.Ride(u);
            var def = CreateUnit(src, "被弾機", "敵");
            var bc = src.Event.BCVariable;
            bc.IsConfig = true;
            bc.MeUnit = u;
            bc.AtkUnit = u;
            bc.DefUnit = def;
            bc.WeaponNumber = 2;
            bc.AttackExp = 1500;
            bc.TerrainAdaption = 1.2d;
            bc.SizeMod = 0.8d;
            bc.WeaponPower = 2000;
            bc.Armor = 900;
            bc.LastVariable = 1100;
            bc.AttackVariable = 30;
            bc.DffenceVariable = 40;
            bc.CommonEnemy = 50;
            return (u, p);
        }

        [TestMethod]
        public void BattleConfig_NotActive_ConfigVariablesAreUndefined()
        {
            // バトルコンフィグ中でなければ 攻撃値 等は通常の未定義変数
            var src = CreateSrc();
            src.Event.BCVariable.IsConfig = false;

            Assert.AreEqual("攻撃値", GetStr(src, "攻撃値", out var t));
            Assert.AreEqual(VT.UndefinedType, t);
        }

        [TestMethod]
        public void BattleConfig_CalculationVariables_ReflectBCVariable()
        {
            var src = CreateSrc();
            var (u, _) = SetupBattleConfig(src);
            var def = src.Event.BCVariable.DefUnit;

            Assert.AreEqual(1500d, GetNum(src, "攻撃値", out var t1));
            Assert.AreEqual(VT.NumericType, t1);
            Assert.AreEqual("1500", GetStr(src, "攻撃値", out _));
            Assert.AreEqual(u.ID, GetStr(src, "攻撃側ユニットＩＤ", out _));
            Assert.AreEqual(def.ID, GetStr(src, "防御側ユニットＩＤ", out _));
            Assert.AreEqual(2d, GetNum(src, "武器番号", out _));
            Assert.AreEqual(1.2d, GetNum(src, "地形適応", out _), 1e-9);
            Assert.AreEqual(2000d, GetNum(src, "武器威力", out _));
            Assert.AreEqual(0.8d, GetNum(src, "サイズ補正", out _), 1e-9);
            Assert.AreEqual(900d, GetNum(src, "装甲値", out _));
            Assert.AreEqual(1100d, GetNum(src, "最終値", out _));
            Assert.AreEqual(30d, GetNum(src, "攻撃側補正", out _));
            Assert.AreEqual(40d, GetNum(src, "防御側補正", out _));
            Assert.AreEqual(50d, GetNum(src, "ザコ補正", out _));
        }

        [TestMethod]
        public void BattleConfig_PilotVariables_ReflectMeUnitMainPilot()
        {
            var src = CreateSrc();
            var (_, p) = SetupBattleConfig(src);

            Assert.AreEqual((double)p.Defense, GetNum(src, "耐久", out _));
            Assert.AreEqual((double)p.Level, GetNum(src, "ＬＶ", out _));
            Assert.AreEqual((double)p.Exp, GetNum(src, "経験", out _));
            Assert.AreEqual((double)p.SP, GetNum(src, "ＳＰ", out _));
            Assert.AreEqual((double)p.Plana, GetNum(src, "霊力", out _));
            Assert.AreEqual((double)p.Infight, GetNum(src, "格闘", out _));
            Assert.AreEqual((double)p.Shooting, GetNum(src, "射撃", out _));
            Assert.AreEqual((double)p.Hit, GetNum(src, "命中", out _));
            Assert.AreEqual((double)p.Dodge, GetNum(src, "回避", out _));
            Assert.AreEqual((double)p.Technique, GetNum(src, "技量", out _));
            Assert.AreEqual((double)p.Intuition, GetNum(src, "反応", out _));
        }

        [TestMethod]
        public void BattleConfig_Morale_AppliesSmallMoraleEffectOption()
        {
            // 気力は補正込みの値。オプション「気力効果小」では 50 + 気力 \ 2
            var src = CreateSrc();
            var (_, p) = SetupBattleConfig(src);
            p.Morale = 121;
            p.MoraleMod = 0;

            Assert.AreEqual(121d, GetNum(src, "気力", out _));

            src.Expression.DefineGlobalVariable("Option(気力効果小)");
            Assert.AreEqual(110d, GetNum(src, "気力", out _));
            Assert.AreEqual("110", GetStr(src, "気力", out _));
        }

        [TestMethod]
        public void BattleConfig_UnitVariables_ReflectMeUnit()
        {
            var src = CreateSrc();
            var (u, _) = SetupBattleConfig(src);
            u.HP = 600;
            u.EN = 70;

            Assert.AreEqual((double)u.MaxHP, GetNum(src, "最大ＨＰ", out _));
            Assert.AreEqual(600d, GetNum(src, "現在ＨＰ", out _));
            Assert.AreEqual((double)u.MaxEN, GetNum(src, "最大ＥＮ", out _));
            Assert.AreEqual(70d, GetNum(src, "現在ＥＮ", out _));
            Assert.AreEqual((double)u.Speed, GetNum(src, "移動力", out _));
            Assert.AreEqual((double)u.get_Armor(""), GetNum(src, "装甲", out _));
            Assert.AreEqual((double)u.get_Mobility(""), GetNum(src, "運動性", out _));
        }
    }
}
