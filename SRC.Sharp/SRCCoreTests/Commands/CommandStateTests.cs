using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.state.cs で定義される選択状態管理メソッドのユニットテスト
    /// SaveSelections / RestoreSelections / SwapSelections
    /// </summary>
    [TestClass]
    public class CommandStateTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // SaveSelections / RestoreSelections
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SaveSelections_ThenRestoreSelections_RestoresCommand()
        {
            var src = CreateSrc();
            src.Commands.SelectedCommand = "攻撃";
            src.Commands.SaveSelections();
            src.Commands.SelectedCommand = "移動";
            src.Commands.RestoreSelections();
            Assert.AreEqual("攻撃", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void SaveSelections_ThenRestoreSelections_RestoresSelectedX()
        {
            var src = CreateSrc();
            src.Commands.SelectedX = 5;
            src.Commands.SaveSelections();
            src.Commands.SelectedX = 10;
            src.Commands.RestoreSelections();
            Assert.AreEqual(5, src.Commands.SelectedX);
        }

        [TestMethod]
        public void SaveSelections_ThenRestoreSelections_RestoresSelectedY()
        {
            var src = CreateSrc();
            src.Commands.SelectedY = 7;
            src.Commands.SaveSelections();
            src.Commands.SelectedY = 3;
            src.Commands.RestoreSelections();
            Assert.AreEqual(7, src.Commands.SelectedY);
        }

        [TestMethod]
        public void SaveSelections_ThenRestoreSelections_RestoresSelectedWeapon()
        {
            var src = CreateSrc();
            src.Commands.SelectedWeapon = 2;
            src.Commands.SaveSelections();
            src.Commands.SelectedWeapon = 9;
            src.Commands.RestoreSelections();
            Assert.AreEqual(2, src.Commands.SelectedWeapon);
        }

        [TestMethod]
        public void SaveSelections_ThenRestoreSelections_RestoresSelectedWeaponName()
        {
            var src = CreateSrc();
            src.Commands.SelectedWeaponName = "ビームライフル";
            src.Commands.SaveSelections();
            src.Commands.SelectedWeaponName = "バズーカ";
            src.Commands.RestoreSelections();
            Assert.AreEqual("ビームライフル", src.Commands.SelectedWeaponName);
        }

        [TestMethod]
        public void SaveAndRestoreSelections_MultipleStack_WorksLIFO()
        {
            // LIFO (後入れ先出し) の動作を確認
            var src = CreateSrc();
            src.Commands.SelectedCommand = "状態1";
            src.Commands.SaveSelections();
            src.Commands.SelectedCommand = "状態2";
            src.Commands.SaveSelections();
            src.Commands.SelectedCommand = "状態3";

            // 最後に保存した状態2 を復元
            src.Commands.RestoreSelections();
            Assert.AreEqual("状態2", src.Commands.SelectedCommand);

            // さらに前の状態1 を復元
            src.Commands.RestoreSelections();
            Assert.AreEqual("状態1", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void RestoreSelections_WithEmptyStack_DoesNotThrow()
        {
            // スタックが空のときは何もしないで正常終了することを確認
            var src = CreateSrc();
            src.Commands.SelectedCommand = "現在の状態";
            src.Commands.RestoreSelections(); // スタックは空
            // 状態は変わらない
            Assert.AreEqual("現在の状態", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void SaveSelections_IncreasesStackSize()
        {
            var src = CreateSrc();
            var before = 0;
            foreach (var _ in src.Commands.SavedStates) before++;

            src.Commands.SaveSelections();

            var after = 0;
            foreach (var _ in src.Commands.SavedStates) after++;

            Assert.AreEqual(before + 1, after);
        }

        [TestMethod]
        public void RestoreSelections_DecreasesStackSize()
        {
            var src = CreateSrc();
            src.Commands.SaveSelections();
            src.Commands.SaveSelections();

            var before = 0;
            foreach (var _ in src.Commands.SavedStates) before++;

            src.Commands.RestoreSelections();

            var after = 0;
            foreach (var _ in src.Commands.SavedStates) after++;

            Assert.AreEqual(before - 1, after);
        }

        [TestMethod]
        public void SaveSelections_CreatesDeepCopy_ModifyingAfterSave_DoesNotAffectSaved()
        {
            // Save後に状態を変更しても、保存済み状態が影響を受けないことを確認
            var src = CreateSrc();
            src.Commands.SelectedCommand = "元の値";
            src.Commands.SaveSelections();
            src.Commands.SelectedCommand = "変更後の値";

            src.Commands.RestoreSelections();
            Assert.AreEqual("元の値", src.Commands.SelectedCommand);
        }

        // ──────────────────────────────────────────────
        // SwapSelections
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SwapSelections_SwapsSelectedWeapon()
        {
            var src = CreateSrc();
            src.Commands.SelectedWeapon = 3;
            src.Commands.SelectedTWeapon = 7;
            src.Commands.SwapSelections();
            Assert.AreEqual(7, src.Commands.SelectedWeapon);
            Assert.AreEqual(3, src.Commands.SelectedTWeapon);
        }

        [TestMethod]
        public void SwapSelections_SwapsSelectedWeaponName()
        {
            var src = CreateSrc();
            src.Commands.SelectedWeaponName = "ビームライフル";
            src.Commands.SelectedTWeaponName = "バズーカ";
            src.Commands.SwapSelections();
            Assert.AreEqual("バズーカ", src.Commands.SelectedWeaponName);
            Assert.AreEqual("ビームライフル", src.Commands.SelectedTWeaponName);
        }

        [TestMethod]
        public void SwapSelections_SwapsSelectedUnit_AndSelectedTarget()
        {
            var src = CreateSrc();
            var unitA = new Unit(src);
            var unitB = new Unit(src);
            src.Commands.SelectedUnit = unitA;
            src.Commands.SelectedTarget = unitB;
            src.Commands.SwapSelections();
            Assert.AreSame(unitB, src.Commands.SelectedUnit);
            Assert.AreSame(unitA, src.Commands.SelectedTarget);
        }

        [TestMethod]
        public void SwapSelections_BothNullUnits_DoesNotThrow()
        {
            var src = CreateSrc();
            src.Commands.SelectedUnit = null;
            src.Commands.SelectedTarget = null;
            src.Commands.SwapSelections(); // null でも例外なし
            Assert.IsNull(src.Commands.SelectedUnit);
            Assert.IsNull(src.Commands.SelectedTarget);
        }

        [TestMethod]
        public void SwapSelections_WithSameWeaponValue_RemainsUnchanged()
        {
            var src = CreateSrc();
            src.Commands.SelectedWeapon = 5;
            src.Commands.SelectedTWeapon = 5;
            src.Commands.SwapSelections();
            Assert.AreEqual(5, src.Commands.SelectedWeapon);
            Assert.AreEqual(5, src.Commands.SelectedTWeapon);
        }

        [TestMethod]
        public void SwapSelections_CalledTwice_RestoresOriginalState()
        {
            var src = CreateSrc();
            src.Commands.SelectedWeapon = 2;
            src.Commands.SelectedTWeapon = 8;
            src.Commands.SelectedWeaponName = "ライフル";
            src.Commands.SelectedTWeaponName = "ナイフ";
            src.Commands.SwapSelections();
            src.Commands.SwapSelections();
            Assert.AreEqual(2, src.Commands.SelectedWeapon);
            Assert.AreEqual(8, src.Commands.SelectedTWeapon);
            Assert.AreEqual("ライフル", src.Commands.SelectedWeaponName);
            Assert.AreEqual("ナイフ", src.Commands.SelectedTWeaponName);
        }

        [TestMethod]
        public void SwapSelections_SwapsWeaponAndWeaponName_Independently()
        {
            // SelectedWeapon (int) と SelectedWeaponName (string) は独立して入れ替わる
            var src = CreateSrc();
            src.Commands.SelectedWeapon = 1;
            src.Commands.SelectedTWeapon = 2;
            src.Commands.SelectedWeaponName = "主武器";
            src.Commands.SelectedTWeaponName = "反撃武器";
            src.Commands.SwapSelections();
            // int: 1↔2
            Assert.AreEqual(2, src.Commands.SelectedWeapon);
            Assert.AreEqual(1, src.Commands.SelectedTWeapon);
            // string: "主武器"↔"反撃武器"
            Assert.AreEqual("反撃武器", src.Commands.SelectedWeaponName);
            Assert.AreEqual("主武器", src.Commands.SelectedTWeaponName);
        }
    }
}
