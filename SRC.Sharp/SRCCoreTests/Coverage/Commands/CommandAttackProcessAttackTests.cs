using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Maps;
using SRCCore.Units;
using System.Linq;
using static SRCCore.Coverage.Tests.Commands.CommandAttackProcessFixture;

namespace SRCCore.Coverage.Tests.Commands
{
    /// <summary>
    /// Command.attack.cs（StartAttackCommand / FinishAttackCommand / MapAttackCommand）と
    /// Command.process.cs のターゲット選択・マップ攻撃使用時の処理のテスト。
    /// VB 版 Command.bas の StartAttackCommand / FinishAttackCommand / MapAttackCommand に対応する。
    /// </summary>
    [TestClass]
    public class CommandAttackProcessAttackTests
    {
        private static UnitWeapon SelectWeaponOnList(CommandAttackProcessFixture f, Unit u, string wname)
        {
            var w = u.Weapons.First(x => x.Name == wname);
            f.Headless.Handle("WeaponListBox", _ => w);
            return w;
        }

        private static CommandAttackProcessFixture CreateBattle(out Unit ally, string wclass = "", int minRange = 1, int maxRange = 1)
        {
            var f = new CommandAttackProcessFixture();
            var ud = f.AddUnitData("自機");
            f.AddWeapon(ud, "主武器", minRange: minRange, maxRange: maxRange, wclass: wclass);
            ally = f.Deploy("自機", "主人公", "味方", 5, 5);
            f.Commands.SelectedUnit = ally;
            f.Commands.CommandState = "コマンド選択";
            return f;
        }

        // ══════════════════════════════════════════════════════
        // StartAttackCommand
        // ══════════════════════════════════════════════════════

        [TestMethod]
        public void StartAttack_NormalWeapon_MasksAllButEnemiesInRange()
        {
            // 通常武器: 射程内の敵だけを選択可能（マスク解除）にしてターゲット選択に進む
            var f = CreateBattle(out var ally);
            f.AddUnitData("敵機");
            f.AddUnitData("僚機");
            var enemy = f.Deploy("敵機", "敵兵", "敵", 6, 5);
            var friend = f.Deploy("僚機", "相棒", "味方", 4, 5);
            var farEnemy = f.Deploy("敵機", "遠くの敵", "敵", 8, 5);
            SelectWeaponOnList(f, ally, "主武器");

            f.Commands.UnitCommand(new UiCommand(Command.AttackCmdID, "攻撃"));

            Assert.AreEqual("攻撃", f.Commands.SelectedCommand);
            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);
            Assert.AreEqual(1, f.Commands.SelectedWeapon);
            Assert.IsFalse(f.SRC.Map.MaskData[6, 5], "射程内の敵は選択できる");
            Assert.IsTrue(f.SRC.Map.MaskData[4, 5], "味方は選択できない");
            Assert.IsTrue(f.SRC.Map.MaskData[8, 5], "射程外の敵は選択できない");
            Assert.IsFalse(f.SRC.Map.MaskData[5, 5], "自分の位置はマスクしない");
            // 大型マップでなければ自機を中心に表示
            var center = f.Headless.CallsOf("Center").Last();
            CollectionAssert.AreEqual(new object[] { 5, 5 }, center.Args);
            Assert.IsTrue(f.Headless.CallsOf("MaskScreen").Any());
            // 武器選択は移動前用の一覧
            Assert.AreEqual("移動前", f.Headless.CallsOf("WeaponListBox").Single().Args[3]);
        }

        [TestMethod]
        public void StartAttack_LargeMapOption_DoesNotCenter()
        {
            // Option「大型マップ」では画面の中央寄せを行わない
            var f = CreateBattle(out var ally);
            f.SRC.Expression.DefineGlobalVariable("Option(大型マップ)");
            SelectWeaponOnList(f, ally, "主武器");

            f.Commands.UnitCommand(new UiCommand(Command.AttackCmdID, "攻撃"));

            Assert.AreEqual(0, f.Headless.CallsOf("Center").Count());
        }

        [TestMethod]
        public void StartAttack_AfterMove_EntersAfterMoveTargetSelect()
        {
            // 移動後の攻撃は移動後用の武器一覧を使い「移動後ターゲット選択」に進む
            var f = CreateBattle(out var ally);
            f.Commands.CommandState = "移動後コマンド選択";
            SelectWeaponOnList(f, ally, "主武器");

            f.Commands.UnitCommand(new UiCommand(Command.AttackCmdID, "攻撃"));

            Assert.AreEqual("移動後ターゲット選択", f.Commands.CommandState);
            Assert.AreEqual("移動後", f.Headless.CallsOf("WeaponListBox").Single().Args[3]);
        }

        [TestMethod]
        public void StartAttack_MapWeapon_SetsMapAttackCommand()
        {
            // マップ兵器（Ｍ属性）を選ぶと「マップ攻撃」になり、効果範囲内は全て選択可能
            var f = CreateBattle(out var ally, wclass: "Ｍ全", maxRange: 2);
            f.AddUnitData("僚機");
            f.Deploy("僚機", "相棒", "味方", 4, 5);
            SelectWeaponOnList(f, ally, "主武器");

            f.Commands.UnitCommand(new UiCommand(Command.AttackCmdID, "攻撃"));

            Assert.AreEqual("マップ攻撃", f.Commands.SelectedCommand);
            Assert.IsFalse(f.SRC.Map.MaskData[4, 5], "Ｍ全は味方も効果範囲に含む");
            Assert.IsFalse(f.SRC.Map.MaskData[7, 5]);
            Assert.IsTrue(f.SRC.Map.MaskData[8, 5], "射程外");
        }

        [TestMethod]
        public void StartAttack_IdentifyingMapWeapon_MasksAllies()
        {
            // 識別攻撃（識）のマップ兵器は効果範囲内の味方をマスクする
            var f = CreateBattle(out var ally, wclass: "Ｍ全識", maxRange: 2);
            f.AddUnitData("僚機");
            f.AddUnitData("敵機");
            f.Deploy("僚機", "相棒", "味方", 4, 5);
            f.Deploy("敵機", "敵兵", "敵", 6, 5);
            SelectWeaponOnList(f, ally, "主武器");

            f.Commands.UnitCommand(new UiCommand(Command.AttackCmdID, "攻撃"));

            Assert.IsTrue(f.SRC.Map.MaskData[4, 5], "味方は識別して除外");
            Assert.IsFalse(f.SRC.Map.MaskData[6, 5], "敵は対象");
        }

        [TestMethod]
        public void StartAttack_WeaponCannotReachTargetArea_MasksTarget()
        {
            // 武器の地形適応が対象の居る地形で無効（-）なら対象をマスクする
            var f = new CommandAttackProcessFixture();
            var ud = f.AddUnitData("自機");
            // 地形適応は 空陸水宇 の順。空中の敵には当たらない
            f.AddWeapon(ud, "対地武器", adaption: "-AAA");
            var ally = f.Deploy("自機", "主人公", "味方", 5, 5);
            var eud = f.AddUnitData("飛行敵");
            eud.Transportation = "空";
            var flying = f.Deploy("飛行敵", "敵兵", "敵", 6, 5);
            flying.Area = "空中";
            f.Commands.SelectedUnit = ally;
            f.Commands.CommandState = "コマンド選択";
            SelectWeaponOnList(f, ally, "対地武器");

            f.Commands.UnitCommand(new UiCommand(Command.AttackCmdID, "攻撃"));

            Assert.IsTrue(f.SRC.Map.MaskData[6, 5]);
        }

        [TestMethod]
        public void StartAttack_AutoMoveCursor_MovesCursorToWeakestNearestEnemy()
        {
            // カーソル自動移動: マスクされていない敵のうち HP が最も低いもの、同じなら近いものへ移動
            var f = CreateBattle(out var ally, minRange: 1, maxRange: 3);
            f.SRC.SystemConfig.AutoMoveCursor = true;
            f.AddUnitData("敵機");
            var far = f.Deploy("敵機", "遠い敵", "敵", 8, 5);
            var near = f.Deploy("敵機", "近い敵", "敵", 6, 5);
            var strong = f.Deploy("敵機", "元気な敵", "敵", 5, 6);
            strong.HP = strong.MaxHP;
            far.HP = 100;
            near.HP = 100;
            SelectWeaponOnList(f, ally, "主武器");

            f.Commands.UnitCommand(new UiCommand(Command.AttackCmdID, "攻撃"));

            var move = f.Headless.CallsOf("MoveCursorPos").Last();
            Assert.AreEqual("ユニット選択", move.Args[0]);
            Assert.AreSame(near, move.Args[1]);
        }

        [TestMethod]
        public void StartAttack_AutoMoveCursor_NoTarget_MovesCursorToSelf()
        {
            // 適当なターゲットが見つからなければ自分自身にカーソルを合わせる
            var f = CreateBattle(out var ally);
            f.SRC.SystemConfig.AutoMoveCursor = true;
            SelectWeaponOnList(f, ally, "主武器");

            f.Commands.UnitCommand(new UiCommand(Command.AttackCmdID, "攻撃"));

            Assert.AreSame(ally, f.Headless.CallsOf("MoveCursorPos").Last().Args[1]);
        }

        // ══════════════════════════════════════════════════════
        // ProceedTargetSelect（ターゲット選択）
        // ══════════════════════════════════════════════════════

        [TestMethod]
        public void TargetSelect_SelfForAttack_Ignored()
        {
            // 攻撃で自分自身を選んでも何も起きない
            var f = CreateBattle(out var ally);
            f.Commands.SelectedCommand = "攻撃";
            f.Commands.CommandState = "ターゲット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], ally);

            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);
            Assert.IsNull(f.Commands.SelectedTarget);
        }

        [TestMethod]
        public void TargetSelect_EmptyCellForAttack_NoTarget()
        {
            // ユニットを選ぶコマンドで空き地を選んでも何も起きない
            var f = CreateBattle(out var ally);
            f.Commands.SelectedCommand = "攻撃";
            f.Commands.CommandState = "ターゲット選択";

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[6, 5], null);

            Assert.AreEqual(6, f.Commands.SelectedX);
            Assert.AreEqual(5, f.Commands.SelectedY);
            Assert.IsNull(f.Commands.SelectedTarget);
            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);
        }

        [TestMethod]
        public void TargetSelect_MaskedCell_Ignored()
        {
            // マスクされた地点は選べない
            var f = CreateBattle(out var ally);
            f.Commands.SelectedCommand = "攻撃";
            f.Commands.CommandState = "ターゲット選択";
            f.SRC.Map.MaskData[6, 5] = true;

            f.Commands.ProceedCommand(false, GuiButton.Left, f.SRC.Map.MapData[6, 5], null);

            Assert.AreEqual(0, f.Commands.SelectedX);
        }

        [TestMethod]
        public void TargetSelect_Move_MovesUnitAndShowsAfterMoveMenu_CancelRestoresPosition()
        {
            // 移動先を選ぶとユニットが移動して移動後コマンド選択になり、
            // キャンセルすると元の位置・EN に戻って移動先の選択からやり直す
            var f = CreateBattle(out var ally);
            var en = ally.EN;
            f.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));
            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);
            Assert.AreEqual("移動", f.Commands.SelectedCommand);

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 7], null);

            Assert.AreEqual("移動後コマンド選択", f.Commands.CommandState);
            Assert.AreEqual(5, ally.x);
            Assert.AreEqual(7, ally.y);
            Assert.AreSame(ally, f.SRC.Map.MapDataForUnit[5, 7]);
            Assert.IsNotNull(f.LastUnitCommandMenu());
            Assert.IsTrue(f.Commands.SelectedUnitMoveCost > 0);

            f.Commands.CancelCommand();

            Assert.AreEqual(5, ally.x);
            Assert.AreEqual(5, ally.y);
            Assert.AreSame(ally, f.SRC.Map.MapDataForUnit[5, 5]);
            Assert.IsNull(f.SRC.Map.MapDataForUnit[5, 7]);
            Assert.AreEqual(en, ally.EN);
            Assert.AreEqual(0, f.Commands.SelectedUnitMoveCost);
            // StartMoveCommand をやり直すのでターゲット選択に戻る
            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);
            Assert.AreEqual("移動", f.Commands.SelectedCommand);
        }

        [TestMethod]
        public void CancelAfterMoveAttack_ReturnsToAfterMoveMenuWithPrevCommand()
        {
            // 移動後の攻撃対象選択をキャンセルすると移動後コマンド選択に戻り、
            // 直前のコマンド（移動）を SelectedCommand に戻してメニューを再表示する
            var f = CreateBattle(out var ally);
            f.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));
            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 7], null);
            SelectWeaponOnList(f, ally, "主武器");
            f.Commands.UnitCommand(new UiCommand(Command.AttackCmdID, "攻撃"));
            Assert.AreEqual("移動後ターゲット選択", f.Commands.CommandState);
            var menus = f.Headless.CallsOf("ShowUnitCommandMenu").Count();

            f.Commands.ProceedInput(GuiButton.Right, null, null);

            Assert.AreEqual("移動後コマンド選択", f.Commands.CommandState);
            Assert.AreEqual("移動", f.Commands.SelectedCommand);
            Assert.AreEqual(menus + 1, f.Headless.CallsOf("ShowUnitCommandMenu").Count());
            // ユニットは移動先に留まる
            Assert.AreEqual(7, ally.y);
            Assert.IsFalse(f.SRC.Map.MaskData[5, 7]);
        }

        [TestMethod]
        public void CancelTargetSelect_ReturnsToCommandMenu()
        {
            // 移動前の対象選択をキャンセルするとコマンド選択に戻ってメニューを再表示する
            var f = CreateBattle(out var ally);
            SelectWeaponOnList(f, ally, "主武器");
            f.Commands.UnitCommand(new UiCommand(Command.AttackCmdID, "攻撃"));
            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);

            f.Commands.ProceedInput(GuiButton.Right, null, null);

            Assert.AreEqual("コマンド選択", f.Commands.CommandState);
            Assert.IsNotNull(f.LastUnitCommandMenu());
            Assert.IsTrue(f.Headless.CallsOf("RedrawScreen").Any());
        }

        [TestMethod]
        public void CancelTargetSelect_ReMove_FinishesAction()
        {
            // 再移動（遊撃）の移動先選択をキャンセルするとその場で行動終了
            var f = CreateBattle(out var ally);
            f.Commands.SelectedCommand = "再移動";
            f.Commands.CommandState = "ターゲット選択";
            Assert.AreEqual(1, ally.Action);

            f.Commands.CancelCommand();

            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
            Assert.AreEqual(0, ally.Action);
        }

        // ══════════════════════════════════════════════════════
        // FinishAttackCommand
        // ══════════════════════════════════════════════════════

        [TestMethod]
        public void FinishAttack_DestroyEnemy_GainsMoneyAndEndsAction()
        {
            // 敵を破壊すると資金（敵の Value / 2）を得て行動終了する
            var f = new CommandAttackProcessFixture();
            var ud = f.AddUnitData("自機");
            var wd = f.AddWeapon(ud, "必殺武器", power: 99999);
            wd.Precision = 999;
            var ally = f.Deploy("自機", "主人公", "味方", 5, 5);
            var eud = f.AddUnitData("敵機");
            eud.HP = 10;
            eud.Armor = 0;
            eud.Mobility = 0;
            eud.Value = 2000;
            var enemy = f.Deploy("敵機", "敵兵", "敵", 6, 5);
            f.Commands.SelectedUnit = ally;
            f.Commands.CommandState = "コマンド選択";
            SelectWeaponOnList(f, ally, "必殺武器");
            f.Commands.UnitCommand(new UiCommand(Command.AttackCmdID, "攻撃"));
            var money = f.SRC.Money;

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[6, 5], enemy);

            Assert.AreEqual("破壊", enemy.Status);
            Assert.AreEqual(money + enemy.Value / 2, f.SRC.Money);
            Assert.IsTrue(f.Headless.CallsOf("DisplaySysMessage").Any(x => ((string)x.Args[0]).Contains("を得た")));
            Assert.AreEqual(0, ally.Action);
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
            Assert.AreEqual("必殺武器", f.Commands.SelectedWeaponName);
            // 反撃手段のない敵なので反撃武器名は空
            Assert.AreEqual("", f.Commands.SelectedTWeaponName);
            Assert.IsNull(f.Commands.SelectedTarget);
        }

        private static CommandAttackProcessFixture CreateDuel(out Unit ally, out Unit enemy, int allyPower, string allyClass = "", bool enemyArmed = true)
        {
            var f = new CommandAttackProcessFixture();
            var ud = f.AddUnitData("自機");
            ud.HP = 100000;
            var wd = f.AddWeapon(ud, "主武器", power: allyPower, wclass: allyClass);
            wd.Precision = 999;
            ally = f.Deploy("自機", "主人公", "味方", 5, 5);
            var eud = f.AddUnitData("敵機");
            eud.HP = 100000;
            eud.Value = 2000;
            if (enemyArmed)
            {
                var ewd = f.AddWeapon(eud, "反撃武器", power: 3000);
                ewd.Precision = 999;
            }
            enemy = f.Deploy("敵機", "敵兵", "敵", 6, 5);
            f.Commands.SelectedUnit = ally;
            f.Commands.CommandState = "コマンド選択";
            SelectWeaponOnList(f, ally, "主武器");
            f.Commands.UnitCommand(new UiCommand(Command.AttackCmdID, "攻撃"));
            return f;
        }

        [TestMethod]
        public void FinishAttack_EnemySurvives_CounterattacksAndAttackerGainsExp()
        {
            // 敵が生き残れば反撃武器で反撃し、攻撃側は「攻撃」の経験値を得て行動終了する
            var f = CreateDuel(out var ally, out var enemy, allyPower: 1000);
            var exp = ally.MainPilot().Exp;
            var money = f.SRC.Money;

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[6, 5], enemy);

            Assert.AreEqual("出撃", enemy.Status);
            Assert.AreEqual("反撃武器", f.Commands.SelectedTWeaponName);
            Assert.IsTrue(ally.HP < ally.MaxHP, "反撃を受けている");
            Assert.IsTrue(ally.MainPilot().Exp > exp, "経験値を得ている");
            Assert.AreEqual(money, f.SRC.Money, "破壊していないので資金は増えない");
            Assert.AreEqual(0, ally.Action);
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
        }

        [TestMethod]
        public void FinishAttack_IndirectWeapon_NoCounterattack()
        {
            // 間接攻撃（間）の武器に対しては反撃しない
            var f = CreateDuel(out var ally, out var enemy, allyPower: 1000, allyClass: "間");

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[6, 5], enemy);

            Assert.AreEqual("", f.Commands.SelectedTWeaponName);
            Assert.AreEqual(ally.MaxHP, ally.HP);
            Assert.IsTrue(enemy.HP < enemy.MaxHP);
        }

        [TestMethod]
        public void FinishAttack_RaidSkillWithAfterMoveWeapon_AllowsReMove()
        {
            // 遊撃技能のパイロットが移動後攻撃可能な武器で攻撃すると、行動を消費せず再移動に入る
            var f = new CommandAttackProcessFixture();
            var pd = f.AddPilotData("遊撃手");
            pd.AddSkill("遊撃", 1, "", 1);
            var ud = f.AddUnitData("自機");
            ud.HP = 100000;
            var wd = f.AddWeapon(ud, "格闘武器", power: 1000);
            wd.Precision = 999;
            var ally = f.Deploy("自機", "遊撃手", "味方", 5, 5);
            var eud = f.AddUnitData("敵機");
            eud.HP = 100000;
            var enemy = f.Deploy("敵機", "敵兵", "敵", 6, 5);
            f.Commands.SelectedUnit = ally;
            f.Commands.CommandState = "コマンド選択";
            SelectWeaponOnList(f, ally, "格闘武器");
            f.Commands.UnitCommand(new UiCommand(Command.AttackCmdID, "攻撃"));

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[6, 5], enemy);

            Assert.AreEqual("再移動", f.Commands.SelectedCommand);
            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);
            Assert.AreEqual(1, ally.Action, "再移動するまで行動は消費しない");
        }

        // ══════════════════════════════════════════════════════
        // MapAttackCommand / ProceedMapAttack
        // ══════════════════════════════════════════════════════

        private static CommandAttackProcessFixture CreateMapAttack(out Unit ally, string wclass, int maxRange, int power = 99999)
        {
            var f = new CommandAttackProcessFixture();
            // 画面座標＝マップ座標として扱う
            f.Headless.Handle("PixelToMapX", a => (int)a[0]);
            f.Headless.Handle("PixelToMapY", a => (int)a[0]);
            var ud = f.AddUnitData("自機");
            var wd = f.AddWeapon(ud, "マップ兵器", power: power, minRange: 1, maxRange: maxRange, wclass: wclass);
            wd.Precision = 999;
            ally = f.Deploy("自機", "主人公", "味方", 5, 5);
            f.Commands.SelectedUnit = ally;
            f.Commands.CommandState = "コマンド選択";
            SelectWeaponOnList(f, ally, "マップ兵器");
            f.Commands.UnitCommand(new UiCommand(Command.AttackCmdID, "攻撃"));
            return f;
        }

        private static void ClickAt(CommandAttackProcessFixture f, int x, int y)
        {
            f.SRC.GUI.MouseX = x;
            f.SRC.GUI.MouseY = y;
            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[x, y], f.SRC.Map.MapDataForUnit[x, y]);
        }

        [TestMethod]
        public void MapAttack_Throw_SelectingPointSetsBlastArea()
        {
            // Ｍ投: 目標地点を選ぶと「マップ攻撃使用」になり、目標地点を中心に Ｍ投 のレベルの範囲を効果範囲にする
            var f = CreateMapAttack(out var ally, "Ｍ投L1", maxRange: 4);
            Assert.AreEqual("マップ攻撃", f.Commands.SelectedCommand);

            ClickAt(f, 8, 5);

            Assert.AreEqual("マップ攻撃使用", f.Commands.CommandState);
            Assert.AreEqual(8, f.Commands.SelectedX);
            Assert.AreEqual(5, f.Commands.SelectedY);
            Assert.IsFalse(f.SRC.Map.MaskData[9, 5]);
            Assert.IsFalse(f.SRC.Map.MaskData[8, 6]);
            Assert.IsTrue(f.SRC.Map.MaskData[6, 5], "効果範囲外");
        }

        [TestMethod]
        public void MapAttack_Throw_CancelReturnsToTargetSelectWithWeaponRange()
        {
            // マップ攻撃使用中のキャンセルは射程表示に戻る（ターゲット選択）
            var f = CreateMapAttack(out var ally, "Ｍ投L1", maxRange: 4);
            ClickAt(f, 8, 5);

            f.Commands.ProceedInput(GuiButton.Right, null, null);

            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);
            Assert.IsFalse(f.SRC.Map.MaskData[6, 5], "射程内に戻る");
            Assert.IsTrue(f.SRC.Map.MaskData[10, 5], "射程外");
        }

        [TestMethod]
        public void MapAttack_Throw_ClickOutsideMapIgnored()
        {
            // マップ攻撃使用中にマップ外をクリックしても発動しない
            var f = CreateMapAttack(out var ally, "Ｍ投L1", maxRange: 4);
            ClickAt(f, 8, 5);
            f.SRC.GUI.MouseX = 0;
            f.SRC.GUI.MouseY = 0;

            f.Commands.ProceedCommand();

            Assert.AreEqual("マップ攻撃使用", f.Commands.CommandState);
            Assert.AreEqual(1, ally.Action);
        }

        [TestMethod]
        public void MapAttack_Throw_ConfirmInsideAreaDestroysEnemyAndEndsAction()
        {
            // 効果範囲内をクリックするとマップ攻撃を実行し、行動終了する
            var f = CreateMapAttack(out var ally, "Ｍ投L1", maxRange: 4);
            var eud = f.AddUnitData("敵機");
            eud.HP = 10;
            eud.Armor = 0;
            var enemy = f.Deploy("敵機", "敵兵", "敵", 8, 5);
            ClickAt(f, 8, 5);

            ClickAt(f, 8, 5);

            Assert.AreEqual("破壊", enemy.Status);
            Assert.AreEqual(0, ally.Action);
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
            Assert.IsNull(f.Commands.SelectedTarget);
        }

        [TestMethod]
        public void MapAttack_MoveType_TargetOccupied_StaysInTargetSelect()
        {
            // Ｍ移: 目標地点に他のユニットがいる場合は選択し直し
            var f = CreateMapAttack(out var ally, "Ｍ移", maxRange: 3);
            f.AddUnitData("敵機");
            f.Deploy("敵機", "敵兵", "敵", 7, 5);
            f.SRC.Map.MaskData[7, 5] = false;

            ClickAt(f, 7, 5);

            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);
        }

        [TestMethod]
        public void MapAttack_LineType_SelectingPointSetsLineArea()
        {
            // Ｍ線: 目標地点を選ぶと自機からその地点までの直線を効果範囲にする
            var f = CreateMapAttack(out var ally, "Ｍ線", maxRange: 4);

            ClickAt(f, 8, 5);

            Assert.AreEqual("マップ攻撃使用", f.Commands.CommandState);
            Assert.IsFalse(f.SRC.Map.MaskData[7, 5]);
            Assert.IsTrue(f.SRC.Map.MaskData[5, 7]);
        }
    }
}
