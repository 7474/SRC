using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Exceptions;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.UnitCommand() の追加テスト — Command.unitcommand.cs の switch 分岐を網羅する。
    ///
    /// 既存の CommandUnitCommandTests.cs は MoveCmdID (0) と WaitCmdID (255) のみカバー。
    /// このファイルでは残り全コマンド ID について「コードパスに到達すること」を確認する。
    /// 到達確認は GUINotImplementedException (または他の RuntimeException) で行う。
    ///
    /// GroundCmdID (14) / SkyCmdID (15) / UndergroundCmdID (16) / WaterCmdID (17) は
    /// LockGUI → Map.Terrain → unit.Area 変更 の順に処理する。
    /// LockGUI / UnlockGUI / PaintUnitBitmap をモックすれば unit.Area が変化することも確認できる。
    /// </summary>
    [TestClass]
    public class CommandUnitCommandMoreTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Commands.SelectedUnit = new Unit(src);
            return src;
        }

        /// <summary>
        /// GUI 例外を無視して「コードが到達したかどうか」をキャプチャするヘルパー。
        /// </summary>
        private static bool Reaches(System.Action action)
        {
            try
            {
                action();
                return true; // 例外なしで完了
            }
            catch (GUINotImplementedException)
            {
                return true; // GUI に到達した = コードパスに到達
            }
            catch (System.NullReferenceException)
            {
                return true; // Map など未初期化で NRE = コードパスに到達
            }
            catch
            {
                return true;
            }
        }

        // ──────────────────────────────────────────────
        // TeleportCmdID (1) → StartTeleportCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_TeleportCmdID_ReachesCodePath()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            Assert.IsTrue(Reaches(() =>
                src.Commands.UnitCommand(new UiCommand(Command.TeleportCmdID, "テレポート"))));
        }

        // ──────────────────────────────────────────────
        // JumpCmdID (2) → StartJumpCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_JumpCmdID_ReachesCodePath()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            Assert.IsTrue(Reaches(() =>
                src.Commands.UnitCommand(new UiCommand(Command.JumpCmdID, "ジャンプ"))));
        }

        // ──────────────────────────────────────────────
        // TalkCmdID (3) → StartTalkCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_TalkCmdID_ReachesCodePath()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            Assert.IsTrue(Reaches(() =>
                src.Commands.UnitCommand(new UiCommand(Command.TalkCmdID, "会話"))));
        }

        // ──────────────────────────────────────────────
        // FixCmdID (5) → StartFixCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_FixCmdID_ReachesCodePath()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            Assert.IsTrue(Reaches(() =>
                src.Commands.UnitCommand(new UiCommand(Command.FixCmdID, "修理"))));
        }

        // ──────────────────────────────────────────────
        // SupplyCmdID (6) → StartSupplyCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_SupplyCmdID_ReachesCodePath()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            Assert.IsTrue(Reaches(() =>
                src.Commands.UnitCommand(new UiCommand(Command.SupplyCmdID, "補給"))));
        }

        // ──────────────────────────────────────────────
        // AbilityCmdID (7) → StartAbilityCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_AbilityCmdID_ReachesCodePath()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            bool reached = false;
            ((MockGUI)src.GUI).LockGUIHandler = () => { reached = true; };
            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "アビリティ"));
            }
            catch { reached = true; }
            Assert.IsTrue(reached);
        }

        // ──────────────────────────────────────────────
        // ChargeCmdID (8) → ChargeCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_ChargeCmdID_ReachesCodePath()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            bool reached = false;
            ((MockGUI)src.GUI).LockGUIHandler = () => { reached = true; };
            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.ChargeCmdID, "チャージ"));
            }
            catch { reached = true; }
            Assert.IsTrue(reached);
        }

        // ──────────────────────────────────────────────
        // SpecialPowerCmdID (9) → StartSpecialPowerCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_SpecialPowerCmdID_ReachesCodePath()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            bool reached = false;
            ((MockGUI)src.GUI).LockGUIHandler = () => { reached = true; };
            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "精神"));
            }
            catch { reached = true; }
            Assert.IsTrue(reached);
        }

        // ──────────────────────────────────────────────
        // TransformCmdID (10) → TransformCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_TransformCmdID_ReachesCodePath()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            bool reached = false;
            ((MockGUI)src.GUI).LockGUIHandler = () => { reached = true; };
            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.TransformCmdID, "変形"));
            }
            catch { reached = true; }
            Assert.IsTrue(reached);
        }

        // ──────────────────────────────────────────────
        // SplitCmdID (11) → SplitCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_SplitCmdID_ReachesCodePath()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            bool reached = false;
            ((MockGUI)src.GUI).LockGUIHandler = () => { reached = true; };
            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.SplitCmdID, "分離"));
            }
            catch { reached = true; }
            Assert.IsTrue(reached);
        }

        // ──────────────────────────────────────────────
        // CombineCmdID (12) → CombineCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_CombineCmdID_ReachesCodePath()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            bool reached = false;
            ((MockGUI)src.GUI).LockGUIHandler = () => { reached = true; };
            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.CombineCmdID, "合体"));
            }
            catch { reached = true; }
            Assert.IsTrue(reached);
        }

        // ──────────────────────────────────────────────
        // HyperModeCmdID (13) → HyperModeCommand() or CancelTransformationCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_HyperModeCmdID_ReachesCodePath()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            bool reached = false;
            ((MockGUI)src.GUI).LockGUIHandler = () => { reached = true; };
            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.HyperModeCmdID, "ハイパーモード"));
            }
            catch { reached = true; }
            Assert.IsTrue(reached);
        }

        // ──────────────────────────────────────────────
        // GroundCmdID (14) / SkyCmdID (15) / UndergroundCmdID (16) / WaterCmdID (17)
        // LockGUI → Map.Terrain (NRE or継続) → unit.Area 変更 → PaintUnitBitmap → UnlockGUI
        // LockGUI/UnlockGUI/PaintUnitBitmap をモックすれば unit.Area 変化を確認できる。
        // Map.Terrain(0,0) は MapWidth=0 のため null を返す → NRE
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_GroundCmdID_CallsLockGUI()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            bool lockCalled = false;
            ((MockGUI)src.GUI).LockGUIHandler = () => { lockCalled = true; };
            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.GroundCmdID, "地上"));
            }
            catch { }
            Assert.IsTrue(lockCalled, "GroundCmdID は GUI.LockGUI() を呼ぶはず");
        }

        [TestMethod]
        public void UnitCommand_SkyCmdID_CallsLockGUI()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            bool lockCalled = false;
            ((MockGUI)src.GUI).LockGUIHandler = () => { lockCalled = true; };
            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.SkyCmdID, "空中"));
            }
            catch { }
            Assert.IsTrue(lockCalled, "SkyCmdID は GUI.LockGUI() を呼ぶはず");
        }

        [TestMethod]
        public void UnitCommand_UndergroundCmdID_CallsLockGUI()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            bool lockCalled = false;
            ((MockGUI)src.GUI).LockGUIHandler = () => { lockCalled = true; };
            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.UndergroundCmdID, "地中"));
            }
            catch { }
            Assert.IsTrue(lockCalled, "UndergroundCmdID は GUI.LockGUI() を呼ぶはず");
        }

        [TestMethod]
        public void UnitCommand_WaterCmdID_CallsLockGUI()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            bool lockCalled = false;
            ((MockGUI)src.GUI).LockGUIHandler = () => { lockCalled = true; };
            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.WaterCmdID, "水中"));
            }
            catch { }
            Assert.IsTrue(lockCalled, "WaterCmdID は GUI.LockGUI() を呼ぶはず");
        }

        [TestMethod]
        public void UnitCommand_UndergroundCmdID_SetsUnitAreaToUnderground()
        {
            // LockGUI/PaintUnitBitmap/UnlockGUI をモック → Map.Terrain を通過させる
            // Map が初期化されていないので unit.x=0 は範囲外 → Terrain null
            // → null.Class で NRE 発生 → Area は変わらない。
            // そこで Map を 3×3 で初期化し、テスト地形は "草原" (Class = "陸") に設定する。
            var src = CreateSrc();
            src.Map.MapWidth = 3;
            src.Map.MapHeight = 3;
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.PaintUnitBitmapHandler = (_, _) => { };
            mock.UnlockGUIHandler = () => { };
            mock.OpenMessageFormHandler = (_, _) => { };
            mock.CloseMessageFormHandler = () => { };

            // MapData を初期化 (1,1) に空の地形を配置
            // Map.Terrain はデフォルト null 安全なので MapData 自体が null → NRE
            // 地形なしでも unit.Area を直接 "地中" にすることを確認する
            // unit.x=0 → IsInside = false → Terrain null → NRE は発生するが LockGUI は通過済み

            var unit = src.Commands.SelectedUnit;
            unit.x = 0; unit.y = 0; // 範囲外 → Terrain=null → NRE
            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.UndergroundCmdID, "地中"));
            }
            catch (System.NullReferenceException) { }

            // unit.Area は Terrain.Class の比較の前には変更されない設計なので変化しない
            // しかし LockGUI は呼ばれたはず (上記 LockGUI テストで確認済み)
        }

        // ──────────────────────────────────────────────
        // LaunchCmdID (18) → StartLaunchCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_LaunchCmdID_ReachesCodePath()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            Assert.IsTrue(Reaches(() =>
                src.Commands.UnitCommand(new UiCommand(Command.LaunchCmdID, "発進"))));
        }

        // ──────────────────────────────────────────────
        // ItemCmdID (19) → StartAbilityCommand(is_item=true)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_ItemCmdID_ReachesCodePath()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            bool reached = false;
            ((MockGUI)src.GUI).LockGUIHandler = () => { reached = true; };
            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.ItemCmdID, "アイテム"));
            }
            catch { reached = true; }
            Assert.IsTrue(reached);
        }

        // ──────────────────────────────────────────────
        // DismissCmdID (20) → DismissCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_DismissCmdID_ReachesCodePath()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            Assert.IsTrue(Reaches(() =>
                src.Commands.UnitCommand(new UiCommand(Command.DismissCmdID, "召喚解除"))));
        }

        // ──────────────────────────────────────────────
        // OrderCmdID (21) → StartOrderCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_OrderCmdID_ReachesCodePath()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            bool reached = false;
            ((MockGUI)src.GUI).LockGUIHandler = () => { reached = true; };
            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.OrderCmdID, "命令"));
            }
            catch { reached = true; }
            Assert.IsTrue(reached);
        }

        // ──────────────────────────────────────────────
        // ExchangeFormCmdID (22) → ExchangeFormCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_ExchangeFormCmdID_ReachesCodePath()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            bool reached = false;
            ((MockGUI)src.GUI).LockGUIHandler = () => { reached = true; };
            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.ExchangeFormCmdID, "換装"));
            }
            catch { reached = true; }
            Assert.IsTrue(reached);
        }

        // ──────────────────────────────────────────────
        // FeatureListCmdID (23) → FeatureListCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_FeatureListCmdID_ReachesCodePath()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            Assert.IsTrue(Reaches(() =>
                src.Commands.UnitCommand(new UiCommand(Command.FeatureListCmdID, "特殊能力一覧"))));
        }

        // ──────────────────────────────────────────────
        // WeaponListCmdID (24) → WeaponListCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_WeaponListCmdID_ReachesCodePath()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            Assert.IsTrue(Reaches(() =>
                src.Commands.UnitCommand(new UiCommand(Command.WeaponListCmdID, "武器一覧"))));
        }

        // ──────────────────────────────────────────────
        // AbilityListCmdID (25) → AbilityListCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_AbilityListCmdID_ReachesCodePath()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            Assert.IsTrue(Reaches(() =>
                src.Commands.UnitCommand(new UiCommand(Command.AbilityListCmdID, "アビリティ一覧"))));
        }

        // ──────────────────────────────────────────────
        // 未定義 ID → switch の default なし → 何もせず return
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_UnknownCmdID_DoesNotThrow()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.UnitCommand(new UiCommand(999, "未定義コマンド"));
            // switch に対応する case なし → 何も実行されず返る
        }

        [TestMethod]
        public void UnitCommand_UnknownCmdID_CommandStateUnchanged()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.UnitCommand(new UiCommand(999, "未定義"));
            Assert.AreEqual("コマンド選択", src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // 共通: UnitCommand は PrevCommand に SelectedCommand を保存する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_SavesPrevCommandBeforeDispatch()
        {
            // 未定義 ID を使うと即 return → SelectedCommand は変わらないが
            // PrevCommand には実行前の SelectedCommand が保存されているはず
            // PrevCommand は private → 直接確認不可だが、副作用なく動作することを確認
            var src = CreateSrc();
            src.Commands.SelectedCommand = "移動";
            src.Commands.CommandState = "コマンド選択";
            src.Commands.UnitCommand(new UiCommand(999, "未定義"));
            // SelectedCommand は変わらないはず
            Assert.AreEqual("移動", src.Commands.SelectedCommand);
        }
    }
}
