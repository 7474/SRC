using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests.UnitsCoverage
{
    /// <summary>
    /// Unit.PilotMessage / IsMessageDefined / PilotMassageIfDefined のテスト。
    /// 元の SRC の Unit.cls PilotMessage / IsMessageDefined に従い、
    /// シチュエーションごとにどのメッセージが選ばれるかを検証する。
    /// </summary>
    [TestClass]
    public class UnitMessageAnimePilotMessageTests
    {
        private const string PName = "テストパイロット";

        private static Unit CreateArmedUnit(UnitMessageAnimeFixture f, string sex = "男性")
        {
            return f.CreateUnit(pname: PName, sex: sex, setup: ud =>
            {
                var saber = ud.AddWeapon("ビームサーベル");
                saber.Class = "格";
                saber.MinRange = 1;
                saber.MaxRange = 1;
                var rifle = ud.AddWeapon("ビームライフル");
                rifle.Class = "射";
                rifle.MinRange = 1;
                rifle.MaxRange = 5;
            });
        }

        private static MessageData AddMessages(UnitMessageAnimeFixture f, params (string sit, string msg)[] entries)
        {
            var md = f.Src.MDList.Item(PName) ?? f.Src.MDList.Add(PName);
            foreach (var (sit, msg) in entries)
            {
                md.AddMessage(sit, msg);
            }
            return md;
        }

        // ──────────────────────────────────────────────
        // 基本のシチュエーションと代替シチュエーション
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PilotMessage_DefinedSituation_DisplaysMessageWithPilotId()
        {
            // メッセージデータに定義されたシチュエーションのメッセージをパイロットの ID で表示する
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            AddMessages(f, ("回避", "当たるものか"), ("ダメージ小", "くっ"));

            u.PilotMessage("回避");

            CollectionAssert.AreEqual(new[] { (u.MainPilot().ID, "当たるものか") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_DefenseAbilitySituation_FallsBackToEvade()
        {
            // 切り払い等のメッセージが無ければ「回避」のメッセージを使う (Unit.cls PilotMessage)
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            AddMessages(f, ("回避", "見切った"));

            u.PilotMessage("切り払い");

            CollectionAssert.AreEqual(new[] { (PName, "見切った") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_DefenseAbilitySituation_PrefersOwnMessage()
        {
            // 切り払い自体のメッセージがあれば「回避」より優先する
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            AddMessages(f, ("回避", "見切った"), ("切り払い", "切り払った"));

            u.PilotMessage("切り払い");

            CollectionAssert.AreEqual(new[] { (PName, "切り払った") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_ShieldDefense_FallsBackToSmallDamage()
        {
            // シールド防御・ビーム無効化等は「ダメージ小」で代用する
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            AddMessages(f, ("ダメージ小", "かすり傷だ"), ("回避", "回避"));

            u.PilotMessage("シールド防御");

            CollectionAssert.AreEqual(new[] { (PName, "かすり傷だ") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_UndefinedSituation_DisplaysNothing()
        {
            // 対応するメッセージが無ければ何も表示しない
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            AddMessages(f, ("回避", "当たるものか"));

            u.PilotMessage("破壊");

            Assert.AreEqual(0, f.Messages.Count);
        }

        [TestMethod]
        public void PilotMessage_CancelMessage_DisplaysNothing()
        {
            // メッセージが「-」なら表示をキャンセルする (PlayMessage)
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            AddMessages(f, ("回避", "-"));

            u.PilotMessage("回避");

            Assert.AreEqual(0, f.Messages.Count);
        }

        // ──────────────────────────────────────────────
        // 攻撃メッセージ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PilotMessage_AttackOnOwnPhase_UsesInfightOrShootingByWeaponClass()
        {
            // 自分のフェイズの攻撃では、武器が格闘系なら「格闘」、そうでなければ「射撃」を使う
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            f.Src.Stage = "味方";
            AddMessages(f, ("格闘", "斬る！"), ("射撃", "撃つ！"));

            u.PilotMessage("ビームサーベル", "攻撃");
            u.PilotMessage("ビームライフル", "攻撃");

            CollectionAssert.AreEqual(new[] { (PName, "斬る！"), (PName, "撃つ！") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_AttackWithWeaponMessage_PrefersWeaponNameAndSubstitutesWeapon()
        {
            // 武器名のシチュエーションが「射撃」より優先され、$(武器) は武器名に置換される
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            f.Src.Stage = "味方";
            AddMessages(f, ("射撃", "撃つ！"), ("ビームライフル", "$(武器)、発射！"));

            u.PilotMessage("ビームライフル", "攻撃");

            CollectionAssert.AreEqual(new[] { (PName, "ビームライフル、発射！") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_AttackOnEnemyPhase_PrefersCounterMessage()
        {
            // 相手のフェイズの攻撃 (反撃) では「射撃(反撃)」が「射撃」より優先される
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            f.Src.Stage = "敵";
            AddMessages(f, ("射撃", "撃つ！"), ("射撃(反撃)", "反撃だ！"));

            u.PilotMessage("ビームライフル", "攻撃");

            CollectionAssert.AreEqual(new[] { (PName, "反撃だ！") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_CounterModeOnEnemyPhase_DoesNotUseGenericCounterMessage()
        {
            // カウンター攻撃では「武器名(反撃)」「武器名」「射撃」のみを探し、
            // 「射撃(反撃)」は使わない (Unit.cls PilotMessage の msg_mode = "カウンター")
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            f.Src.Stage = "敵";
            AddMessages(f, ("射撃", "撃つ！"), ("射撃(反撃)", "反撃だ！"));

            u.PilotMessage("ビームライフル", "カウンター");

            CollectionAssert.AreEqual(new[] { (PName, "撃つ！") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_HitSubSituationOnOwnPhase_UsesInfightHitMessage()
        {
            // 「武器名(命中)」が無ければ「格闘(命中)」を使う
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            f.Src.Stage = "味方";
            AddMessages(f, ("格闘(命中)", "手応えあり"), ("射撃(命中)", "当たった"));

            u.PilotMessage("ビームサーベル(命中)");

            CollectionAssert.AreEqual(new[] { (PName, "手応えあり") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_HitSubSituationOnEnemyPhase_PrefersCounterVariant()
        {
            // 相手のフェイズでは「射撃(命中)(反撃)」が「射撃(命中)」より優先される
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            f.Src.Stage = "敵";
            AddMessages(f, ("射撃(命中)", "当たった"), ("射撃(命中)(反撃)", "返り討ちだ"));

            u.PilotMessage("ビームライフル(命中)");

            CollectionAssert.AreEqual(new[] { (PName, "返り討ちだ") }, f.Messages);
        }

        // ──────────────────────────────────────────────
        // しゃべれない状態
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PilotMessage_Petrified_DisplaysNothing()
        {
            // 石化・凍結・麻痺は意識不明のため何も表示しない
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            AddMessages(f, ("回避", "当たるものか"));
            u.AddCondition("石化", 1);

            u.PilotMessage("回避");

            Assert.AreEqual(0, f.Messages.Count);
        }

        [TestMethod]
        public void PilotMessage_Silenced_DisplaysEllipsis()
        {
            // 沈黙中は「…………」を表示する
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            AddMessages(f, ("回避", "当たるものか"));
            u.AddCondition("沈黙", 1);

            u.PilotMessage("回避");

            CollectionAssert.AreEqual(new[] { (PName, "…………") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_SilencedDamageWithDamageFace_UsesDamageFace()
        {
            // 沈黙中のダメージ大では「名前(ダメージ)」の表情があればそれで「…………！」を表示する
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            f.Src.NPDList.Add(PName + "(ダメージ)");
            u.AddCondition("沈黙", 1);

            u.PilotMessage("ダメージ大");

            CollectionAssert.AreEqual(new[] { (PName + "(ダメージ)", "…………！") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_SilencedAttackWithAttackFace_UsesAttackFace()
        {
            // 沈黙中の攻撃では「名前(攻撃)」の表情があればそれで「…………！」を表示する
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            f.Src.Stage = "味方";
            f.Src.NPDList.Add(PName + "(攻撃)");
            u.AddCondition("沈黙", 1);

            u.PilotMessage("ビームライフル", "攻撃");

            CollectionAssert.AreEqual(new[] { (PName + "(攻撃)", "…………！") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_SilencedShoutOrSubSituation_DisplaysNothing()
        {
            // 沈黙中のかけ声や、括弧付きのサブシチュエーションでは何も表示しない
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            f.Src.Stage = "味方";
            u.AddCondition("沈黙", 1);

            // Dice(3) = 3 に固定し、かけ声の乱数ではなく沈黙の判定で表示が抑止されることを確かめる
            using (UnitMessageAnimeFixture.FixDice(0.99))
            {
                u.PilotMessage("かけ声");
            }
            u.PilotMessage("ビームライフル(命中)");

            Assert.AreEqual(0, f.Messages.Count);
        }

        [TestMethod]
        public void PilotMessage_Sleeping_DisplaysSleepTalk()
        {
            // 睡眠中は寝言「ＺＺＺ……」を表示する
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            AddMessages(f, ("ダメージ中", "ぐっ"));
            u.AddCondition("睡眠", 1);

            u.PilotMessage("ダメージ中");

            CollectionAssert.AreEqual(new[] { (PName, "ＺＺＺ……") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_SelectedMessageOverridesSilence()
        {
            // SetMessage で設定されたメッセージは沈黙中でもそちらを使う
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            u.AddCondition("沈黙", 1);
            f.Src.Expression.SetVariableAsString("Message(" + PName + ",回避)", "設定済み");

            u.PilotMessage("回避");

            CollectionAssert.AreEqual(new[] { (PName, "設定済み") }, f.Messages);
        }

        // ──────────────────────────────────────────────
        // 恐怖・混乱
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PilotMessage_FearMaleWithoutFearMessage_DisplaysPanicMessage()
        {
            // 恐怖状態で「恐怖」メッセージが無い場合は性別に応じた悲鳴を表示する
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f, sex: "男性");
            AddMessages(f, ("ダメージ中", "ぐっ"));
            u.AddCondition("恐怖", 1);

            using (UnitMessageAnimeFixture.FixDice(0.0))
            {
                u.PilotMessage("ダメージ中");
            }

            // Dice(4) = 1 → 「う、うわああああっ！」
            CollectionAssert.AreEqual(new[] { (PName, "う、うわああああっ！") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_FearFemaleWithCryingFace_UsesCryingFace()
        {
            // 女性の悲鳴は「名前(泣き)」の表情があればそれを使う
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f, sex: "女性");
            f.Src.NPDList.Add(PName + "(泣き)");
            f.Src.NPDList.Add(PName + "(ダメージ)");
            u.AddCondition("恐怖", 1);

            using (UnitMessageAnimeFixture.FixDice(0.99))
            {
                u.PilotMessage("ダメージ大");
            }

            // Dice(4) = 4 → 「た、助けてええっ！」
            CollectionAssert.AreEqual(new[] { (PName + "(泣き)", "た、助けてええっ！") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_FearWithDamageFaceOnly_UsesDamageFace()
        {
            // 「(泣き)」が無ければ「(ダメージ)」の表情を使う
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f, sex: "女性");
            f.Src.NPDList.Add(PName + "(ダメージ)");
            u.AddCondition("恐怖", 1);

            using (UnitMessageAnimeFixture.FixDice(0.0))
            {
                u.PilotMessage("回避");
            }

            CollectionAssert.AreEqual(new[] { (PName + "(ダメージ)", "きゃあああああっ！") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_FearWithFearMessage_UsesFearMessage()
        {
            // 「恐怖」メッセージが定義されていれば、どのシチュエーションでもそれを使う
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            AddMessages(f, ("回避", "当たるものか"), ("恐怖", "怖い……"));
            u.AddCondition("恐怖", 1);

            u.PilotMessage("回避");

            CollectionAssert.AreEqual(new[] { (PName, "怖い……") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_ConfusionWithConfusionMessage_UsesConfusionMessage()
        {
            // 混乱状態で「混乱」メッセージが定義されていればそれを使う
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            AddMessages(f, ("回避", "当たるものか"), ("混乱", "ここはどこだ"));
            u.AddCondition("混乱", 1);

            u.PilotMessage("回避");

            CollectionAssert.AreEqual(new[] { (PName, "ここはどこだ") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_ShoutIsSkippedWhenDiceIsOne()
        {
            // かけ声は 3 分の 1 (Dice(3) = 1) の確率で表示しない
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            AddMessages(f, ("かけ声", "いくぞ！"));

            using (UnitMessageAnimeFixture.FixDice(0.0))
            {
                u.PilotMessage("かけ声");
            }
            Assert.AreEqual(0, f.Messages.Count);

            using (UnitMessageAnimeFixture.FixDice(0.99))
            {
                u.PilotMessage("かけ声");
            }
            CollectionAssert.AreEqual(new[] { (PName, "いくぞ！") }, f.Messages);
        }

        // ──────────────────────────────────────────────
        // SetMessage コマンドによる指定 (ローカル変数 Message(ID,状況))
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PilotMessage_SelectedMessageText_DisplaysAndConsumesVariable()
        {
            // メッセージデータに無い文字列はそのまま表示し、変数は一度使うと消去される
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            var vname = "Message(" + PName + ",回避)";
            f.Src.Expression.SetVariableAsString(vname, "今だ！");

            u.PilotMessage("回避");

            CollectionAssert.AreEqual(new[] { (PName, "今だ！") }, f.Messages);
            Assert.IsFalse(f.Src.Expression.IsLocalVariableDefined(vname));
        }

        [TestMethod]
        public void PilotMessage_SelectedMessageWithFace_UsesGivenSpeaker()
        {
            // 「表情::メッセージ」形式なら表情 (話者) を指定して表示する
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            f.Src.Expression.SetVariableAsString("Message(" + PName + ",回避)", PName + "(笑)::余裕だな");

            u.PilotMessage("回避");

            CollectionAssert.AreEqual(new[] { (PName + "(笑)", "余裕だな") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_SelectedMessageNumber_SelectsNthMessageOfSituation()
        {
            // 数値指定ならそのシチュエーションの n 番目のメッセージを選ぶ
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            AddMessages(f, ("回避", "一つ目"), ("ダメージ小", "別"), ("回避", "二つ目"));
            f.Src.Expression.SetVariableAsString("Message(" + PName + ",回避)", "2");

            u.PilotMessage("回避");

            CollectionAssert.AreEqual(new[] { (PName, "二つ目") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_SelectedMessageSituationName_SelectsThatSituation()
        {
            // 指定がシチュエーション名ならそのシチュエーションのメッセージを使う
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            AddMessages(f, ("回避", "よけた"), ("回避(特別)", "特別な回避"));
            f.Src.Expression.SetVariableAsString("Message(" + PName + ",回避)", "回避(特別)");

            u.PilotMessage("回避");

            CollectionAssert.AreEqual(new[] { (PName, "特別な回避") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_SelectedMessageDash_DisplaysNothing()
        {
            // 「-」が指定されていれば何も表示しない
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            AddMessages(f, ("回避", "よけた"));
            f.Src.Expression.SetVariableAsString("Message(" + PName + ",回避)", "-");

            u.PilotMessage("回避");

            Assert.AreEqual(0, f.Messages.Count);
        }

        [TestMethod]
        public void PilotMessage_SelectedAttackMessage_AppliesToInfightAndShooting()
        {
            // 「格闘」「射撃」の代わりに Message(ID,攻撃) の指定も参照する
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            f.Src.Stage = "味方";
            f.Src.Expression.SetVariableAsString("Message(" + PName + ",攻撃)", "くらえ！");

            u.PilotMessage("ビームサーベル", "攻撃");

            CollectionAssert.AreEqual(new[] { (PName, "くらえ！") }, f.Messages);
        }

        // ──────────────────────────────────────────────
        // ダイアログデータ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PilotMessage_DialogData_TakesPriorityOverMessageData()
        {
            // ダイアログデータがあればメッセージデータより優先し、ダイアログの全メッセージを順に表示する
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            AddMessages(f, ("回避", "メッセージデータ"));
            f.Src.NPDList.Add("オペレーター");
            var dialog = f.Src.DDList.Add(PName).AddDialog("回避");
            dialog.AddMessage(PName, "よけろ！");
            dialog.AddMessage("オペレーター", "回避成功です");

            u.PilotMessage("回避");

            CollectionAssert.AreEqual(new[] { (PName, "よけろ！"), ("オペレーター", "回避成功です") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_SelectedMessageNumber_SelectsNthDialog()
        {
            // SetMessage の数値指定はダイアログデータにも適用される
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            var dd = f.Src.DDList.Add(PName);
            dd.AddDialog("回避").AddMessage(PName, "一つ目");
            dd.AddDialog("回避").AddMessage(PName, "二つ目");
            f.Src.Expression.SetVariableAsString("Message(" + PName + ",回避)", "2");

            u.PilotMessage("回避");

            CollectionAssert.AreEqual(new[] { (PName, "二つ目") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_MultiPilotUnit_UsesMainPilotDialogEvenIfMessageDataExists()
        {
            // 元の SRC では「メッセージデータがあれば打ち切る」判定は追加パイロット用の 4 番目の候補だけに行う。
            // 2 人乗りユニットでも、メインパイロット単独のダイアログデータはメッセージデータより優先される。
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit(pname: PName, setup: ud => ud.PilotNum = 2);
            f.Src.PDList.Add("副パイロット").Sex = "男性";
            f.Src.PList.Add("副パイロット", 1, "味方").Ride(u);
            u.Update();
            AddMessages(f, ("回避", "メッセージデータ"));
            f.Src.DDList.Add(PName).AddDialog("回避").AddMessage(PName, "ダイアログデータ");

            u.PilotMessage("回避");

            CollectionAssert.AreEqual(new[] { (PName, "ダイアログデータ") }, f.Messages);
        }

        [TestMethod]
        public void PilotMessage_MultiPilotUnit_UsesMessageDataOfChosenPilot()
        {
            // 元の SRC では Dice(CountPilot + CountSupport) で選ばれたパイロット i の
            // メッセージデータ (MDList.Item(Pilot(i).MessageType)) を使い、そのパイロットとして表示する
            var f = new UnitMessageAnimeFixture();
            var u = f.CreateUnit(pname: PName, setup: ud => ud.PilotNum = 2);
            f.Src.PDList.Add("副パイロット").Sex = "男性";
            var sub = f.Src.PList.Add("副パイロット", 1, "味方");
            sub.Ride(u);
            u.Update();
            f.Src.MDList.Add("副パイロット").AddMessage("回避", "副パイロットの回避");

            using (UnitMessageAnimeFixture.FixDice(0.99))
            {
                // Dice(2) = 2 → 2 人目のパイロットが選ばれる
                u.PilotMessage("回避");
            }

            CollectionAssert.AreEqual(new[] { (sub.ID, "副パイロットの回避") }, f.Messages);
        }

        [TestMethod]
        public void PlayDialog_PartnerAndCancel()
        {
            // 「@名前」は合体技パートナーの指定で、@ を除いた名前で表示する。
            // 「-」のメッセージがあればそこで表示を打ち切る
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            var dd = new Dialog(f.Src);
            dd.AddMessage("@相棒", "合わせるぞ");
            dd.AddMessage(PName, "おう！");
            dd.AddMessage(PName, "-");
            dd.AddMessage(PName, "表示されない");

            u.PlayDialog(dd, "");

            CollectionAssert.AreEqual(new[] { ("相棒", "合わせるぞ"), (PName, "おう！") }, f.Messages);
        }

        // ──────────────────────────────────────────────
        // IsMessageDefined
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsMessageDefined_ReflectsMessageDataAndConditions()
        {
            // メッセージデータにあれば真。しゃべれない状態では ignore_condition を指定しない限り偽
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            AddMessages(f, ("回避", "よけた"));

            Assert.IsTrue(u.IsMessageDefined("回避"));
            Assert.IsFalse(u.IsMessageDefined("破壊"));

            u.AddCondition("睡眠", 1);
            Assert.IsFalse(u.IsMessageDefined("回避"));
            Assert.IsTrue(u.IsMessageDefined("回避", true));
        }

        [TestMethod]
        public void IsMessageDefined_FearOnlyAllowsFearSituation()
        {
            // 恐怖状態では「恐怖」シチュエーションだけを判定する
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            AddMessages(f, ("回避", "よけた"), ("恐怖", "怖い"));
            u.AddCondition("恐怖", 1);

            Assert.IsFalse(u.IsMessageDefined("回避"));
            Assert.IsTrue(u.IsMessageDefined("恐怖"));
        }

        [TestMethod]
        public void IsMessageDefined_TrueForSelectedMessageVariableOrDialog()
        {
            // SetMessage の変数やダイアログデータでも定義済みとみなす
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            f.Src.Expression.SetVariableAsString("Message(" + PName + ",破壊)", "やられた");
            f.Src.DDList.Add(PName).AddDialog("ダメージ大").AddMessage(PName, "ぐわっ");

            Assert.IsTrue(u.IsMessageDefined("破壊"));
            Assert.IsTrue(u.IsMessageDefined("ダメージ大"));
            Assert.IsFalse(u.IsMessageDefined("ダメージ小"));
        }

        // ──────────────────────────────────────────────
        // PilotMassageIfDefined
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PilotMassageIfDefined_DisplaysFirstDefinedSituationInMessageForm()
        {
            // 定義されている最初のシチュエーションのメッセージを、メッセージウィンドウを開いて表示する
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);
            AddMessages(f, ("ダメージ小", "くっ"), ("回避", "よけた"));

            u.PilotMassageIfDefined(new[] { "破壊", "回避", "ダメージ小" });

            CollectionAssert.AreEqual(new[] { (PName, "よけた") }, f.Messages);
            CollectionAssert.AreEqual(
                new[] { "Center", "RefreshScreen", "OpenMessageForm", "DisplayBattleMessage", "CloseMessageForm" },
                f.Calls);
        }

        [TestMethod]
        public void PilotMassageIfDefined_NoneDefined_DoesNotOpenMessageForm()
        {
            // どれも定義されていなければメッセージウィンドウも開かない
            var f = new UnitMessageAnimeFixture();
            var u = CreateArmedUnit(f);

            u.PilotMassageIfDefined(new[] { "破壊", "回避" });

            Assert.AreEqual(0, f.Calls.Count);
        }
    }
}
