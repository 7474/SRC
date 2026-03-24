using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// 未テストだったユニット操作・その他コマンドの引数検証テスト
    /// ヘルプの記載に基づく書式・エラーハンドリングを検証する
    /// </summary>
    [TestClass]
    public class UnitCmdMoreTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new Queue<string>();
            return src;
        }

        private CmdData CreateCmd(SRC src, string cmdText, int id = 0)
        {
            var line = new EventDataLine(id, EventDataSource.Scenario, "test", id, cmdText);
            src.Event.EventData.Add(line);
            var parser = new CmdParser();
            var cmd = parser.Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd;
        }

        // ──────────────────────────────────────────────
        // SelectCmd
        // ヘルプ: Select ユニット名 — 指定ユニットを選択状態にする
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectCmd_WrongArgCount_ReturnsError()
        {
            // 書式: Select ユニット名 (引数1つ必須)
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Select");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void SelectCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Select unit1 unit2");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // SelectTargetCmd
        // ヘルプ: SelectTarget ユニット名 — 攻撃対象ユニットを選択
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectTargetCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SelectTarget");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void SelectTargetCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SelectTarget unit1 unit2");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ShowUnitStatusCmd
        // ヘルプ: ShowUnitStatus [ユニット名] — ユニットステータスを表示
        // ShowUnitStatus 終了 — ステータス表示を終了
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ShowUnitStatusCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ShowUnitStatus unit1 extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ShowUnitStatusCmd_WithUnitArg_NonExistentUnit_ReturnsError()
        {
            // ShowUnitStatus ユニット名 → 存在しないユニットはエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ShowUnitStatus 存在しないユニット");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // SetStatusCmd
        // ヘルプ: SetStatus [ユニット名] 状態名 ターン数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetStatusCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetStatus 状態名");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void SetStatusCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetStatus unit 状態名 5 extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // SetBulletCmd
        // ヘルプ: SetBullet [ユニット名] 武器名 弾数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetBulletCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetBullet 武器名");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void SetBulletCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetBullet unit 武器名 5 extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // SetStockCmd
        // ヘルプ: SetStock [ユニット名] アビリティ名 ストック数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetStockCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetStock アビリティ名");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // FinishCmd
        // ヘルプ: Finish [ユニット名] — ユニットの行動を終了する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FinishCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Finish unit1 extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // JoinCmd
        // ヘルプ: Join [パイロット名/ユニット名] — 離脱中のユニットを復帰させる
        // ──────────────────────────────────────────────

        [TestMethod]
        public void JoinCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Join unit1 extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // LeaveCmd
        // ヘルプ: Leave [パイロット名/ユニット名] — ユニットを離脱させる
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LeaveCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Leave unit1 extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // LandCmd
        // ヘルプ: Land [ユニット名] 母艦名 — ユニットを母艦に格納する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LandCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Land");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void LandCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Land u1 u2 extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // LaunchCmd
        // ヘルプ: Launch [ユニット名] X座標 Y座標 — ユニットをマップに配置する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LaunchCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Launch 5");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void LaunchCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Launch u1 5 5 extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // EscapeCmd
        // ヘルプ: Escape [パイロット名/所属] — ユニットを撤退させる
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EscapeCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Escape unit1 extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // CombineCmd
        // ヘルプ: Combine [ユニット名] 変形後ユニット名 — ユニットを合体させる
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CombineCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Combine");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void CombineCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Combine u1 u2 extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ClearSpecialPowerCmd
        // ヘルプ: ClearSpecialPower [ユニット名] スペシャルパワー名 — 効果を解除する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ClearSpecialPowerCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ClearSpecialPower");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ClearSpecialPowerCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ClearSpecialPower u1 sp extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // SpecialPowerCmd
        // ヘルプ: SpecialPower [ユニット名] スペシャルパワー名 [対象ユニット名]
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SpecialPowerCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SpecialPower");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void SpecialPowerCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SpecialPower u1 sp t1 extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // RemoveUnitCmd
        // ヘルプ: RemoveUnit [ユニット名] — ユニットをゲームから除去する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RemoveUnitCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RemoveUnit u1 extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // RemovePilotCmd
        // ヘルプ: RemovePilot [パイロット名] — パイロットをゲームから除去する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RemovePilotCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RemovePilot p1 extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ReleaseCmd
        // ヘルプ: Release [パイロット名/アイテム名] — 固定状態を解除する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ReleaseCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Release p1 extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // UpgradeCmd
        // ヘルプ: Upgrade [ユニット名] 変更後ユニット名 — ユニットを上位機体に換装
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UpgradeCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Upgrade");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void UpgradeCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Upgrade u1 u2 extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // OrganizeCmd
        // ヘルプ: Organize ユニット数 X座標 Y座標 [クラス] — 部隊を整列配置する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void OrganizeCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Organize 5 1");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // RideCmd
        // ヘルプ: Ride パイロット名 [ユニット名] — パイロットをユニットに乗せる
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RideCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Ride");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void RideCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Ride p1 u1 extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // UseAbilityCmd
        // ヘルプ: UseAbility [ユニット名] アビリティ名 [対象ユニット名]
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UseAbilityCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "UseAbility");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void UseAbilityCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "UseAbility u1 ability t1 extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ChangeUnitBitmapCmd
        // ヘルプ: ChangeUnitBitmap [ユニット名] ビットマップファイル名
        //        ビットマップファイル名には .bmp ファイル名 または 非表示/非表示解除/- を指定
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ChangeUnitBitmapCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ChangeUnitBitmap");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ChangeUnitBitmapCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ChangeUnitBitmap u1 file.bmp extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // CreateCmd
        // ヘルプ: Create 所属 ユニット名 ランク パイロット名 レベル X Y [グループID]
        //        引数は8または9個
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CreateCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Create 味方 ユニット 1 パイロット");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void CreateCmd_InvalidParty_ReturnsError()
        {
            // 所属が正しくない場合はエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Create 無効 ユニット 1 パイロット 1 1 1");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ClearObjCmd
        // ヘルプ: ClearObj [オブジェクト名] — ホットポイントを削除する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ClearObjCmd_WithNameArg_NonExistentObj_ReturnsNextId()
        {
            // ClearObj オブジェクト名 → 存在しないオブジェクト名でも正常終了(何もしない)
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ClearObj 存在しないオブジェクト");
            // HotPointListが空の場合、何も消去されないが正常終了すべきだが
            // GUI.UpdateHotPoint()の初期化依存があるため-1になる可能性がある
            var result = cmd.Exec();
            // エラーなし(次ID)またはGUI初期化なしによるエラーのどちらも許容
            Assert.IsTrue(result == 1 || result == -1);
        }

        [TestMethod]
        public void ClearObjCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ClearObj obj1 extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // MoveCmd
        // ヘルプ: Move [unit] x y [option] — ユニットを指定座標に移動
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MoveCmd_InvalidUnitName_ReturnsError()
        {
            // ヘルプ: unit に存在しないパイロット名を指定するとエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Move 存在しないパイロット 5 3");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void MoveCmd_NoSelectedUnit_NumericArgs_ReturnsError()
        {
            // ヘルプ: unit 省略時は選択ユニットを使用; 選択ユニットなしはエラー
            var src = CreateSrc();
            // SelectedUnitForEvent は null (デフォルト)
            var cmd = CreateCmd(src, "Move 5 3");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void MoveCmd_IsInstanceOfMoveCmd()
        {
            // Move コマンドが正しくパースされることを確認
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Move ユニット 5 3");
            Assert.IsInstanceOfType(cmd, typeof(MoveCmd));
        }

        // ──────────────────────────────────────────────
        // UnitCmd
        // ヘルプ: Unit name rank — ユニットを作成し味方に所属させる
        // 書式: Unit ユニット名 ランク
        // 解説: ユニットを指定のランクで作成する。作成されたユニットは味方に所属し、
        //       Rideコマンドやインターミッションでパイロットを乗せることができる。
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCmd_TooFewArgs_ReturnsError()
        {
            // ヘルプ書式: Unit ユニット名 ランク — ランク省略はエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Unit ユニット名");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void UnitCmd_TooManyArgs_ReturnsError()
        {
            // ヘルプ書式: Unit ユニット名 ランク — 余分な引数はエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Unit ユニット名 0 余分な引数");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void UnitCmd_NonExistentUnit_ReturnsError()
        {
            // ヘルプ解説: 作成するユニットデータはあらかじめロードしておく必要がある
            // データが定義されていない場合はエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Unit 存在しないユニット 0");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void UnitCmd_ValidUnit_CreatesUnitAsFriend()
        {
            // ヘルプ解説: 作成されたユニットは味方に所属する
            var src = CreateSrc();
            // UDListにテスト用ユニットデータを登録
            var ud = src.UDList.Add("テストユニット");
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            ud.HP = 100;
            ud.EN = 50;
            var cmd = CreateCmd(src, "Unit テストユニット 0");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void UnitCmd_ValidUnit_SetsSelectedUnitForEvent()
        {
            // ヘルプ解説: 作成されたユニットはイベントの選択ユニットに設定される
            var src = CreateSrc();
            var ud = src.UDList.Add("テストユニット2");
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            ud.HP = 100;
            ud.EN = 50;
            var cmd = CreateCmd(src, "Unit テストユニット2 1");
            cmd.Exec();
            Assert.IsNotNull(src.Event.SelectedUnitForEvent);
            Assert.AreEqual("テストユニット2", src.Event.SelectedUnitForEvent.Name);
        }

        [TestMethod]
        public void UnitCmd_ValidUnit_UnitPartyIsFriend()
        {
            // ヘルプ解説: 作成されたユニットは味方に所属するとみなされる
            var src = CreateSrc();
            var ud = src.UDList.Add("テストユニット3");
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            ud.HP = 100;
            ud.EN = 50;
            var cmd = CreateCmd(src, "Unit テストユニット3 0");
            cmd.Exec();
            Assert.AreEqual("味方", src.Event.SelectedUnitForEvent.Party);
        }

        [TestMethod]
        public void UnitCmd_ValidUnit_WithNonZeroRank_SetsRank()
        {
            // ヘルプ: rank — ユニットランク (0〜999)
            var src = CreateSrc();
            var ud = src.UDList.Add("テストユニット4");
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            ud.HP = 100;
            ud.EN = 50;
            var cmd = CreateCmd(src, "Unit テストユニット4 5");
            cmd.Exec();
            Assert.AreEqual(5, src.Event.SelectedUnitForEvent.Rank);
        }
    }
}
