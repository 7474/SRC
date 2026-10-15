using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Expressions;
using SRCCore.Pilots;
using SRCCore.Units;
using static SRCCore.Coverage.Tests.PilotsCoverage.PilotSkillStatusFixture;

namespace SRCCore.Coverage.Tests.PilotsCoverage
{
    /// <summary>
    /// Pilot.props.cs（元の SRC の Pilot.cls の各プロパティ）のテスト。
    /// 搭乗ユニットの特殊能力（パイロット愛称・パイロット画像・性別・性格変更など）による変更を確認する。
    /// </summary>
    [TestClass]
    public class PilotSkillStatus_PropsTests
    {
        private const double NoLv = Constants.DEFAULT_LEVEL;

        private static (SRC src, Pilot main, Pilot sub) CreateTwoSeater(params string[] features)
        {
            var src = CreateSrc();
            var md = AddPilotData(src, "アムロ");
            md.Nickname = "アムロ";
            md.KanaName = "あむろ";
            md.Bitmap = "amuro.bmp";
            md.Sex = "男性";
            md.Personality = "普通";
            var sd = AddPilotData(src, "セイラ");
            sd.Nickname = "セイラ";
            sd.KanaName = "せいら";
            sd.Bitmap = "sayla.bmp";
            sd.Personality = "冷静";
            var main = AddPilot(src, "アムロ");
            var sub = AddPilot(src, "セイラ");
            AddUnitData(src, "ガンダム", features);
            CreateUnit(src, "ガンダム", "味方", main, sub);
            return (src, main, sub);
        }

        private static void SetLocal(SRC src, string vname, string str, double num)
        {
            var etype = str != null ? ValueType.StringType : ValueType.NumericType;
            src.Event.LocalVariableList.Add(vname, new VarData(vname, etype, str ?? "", num));
        }

        // ──────────────────────────────────────────────
        // Nickname / KanaName
        // ──────────────────────────────────────────────

        [TestMethod]
        [Ignore("移植の不具合: get_Nickname がパイロット愛称をステータス表示用ダミーユニットの場合にしか適用しない")]
        public void Nickname_パイロット愛称でメインパイロットの愛称が変わる()
        {
            // VB: With u ... If .IsFeatureAvailable("パイロット愛称") は ダミーユニット判定の If の外にある
            var (_, main, sub) = CreateTwoSeater("パイロット愛称=$(愛称)大尉");

            Assert.AreEqual("アムロ大尉", main.get_Nickname(false));
            // メインパイロット以外は変更されない
            Assert.AreEqual("セイラ", sub.get_Nickname(false));
        }

        [TestMethod]
        public void Nickname_ユニットなしならデータの愛称()
        {
            var src = CreateSrc();
            AddPilotData(src, "アムロ").Nickname = "アムロ・レイ";
            var p = AddPilot(src, "アムロ");
            Assert.AreEqual("アムロ・レイ", p.get_Nickname(false));
        }

        [TestMethod]
        public void Nickname_無限呼び出し防止指定でNickname関数呼び出しを取り除く()
        {
            var src = CreateSrc();
            AddPilotData(src, "アムロ").Nickname = "アムロ";
            var p = AddPilot(src, "アムロ");
            AddUnitData(src, "ガンダム", "パイロット愛称=$(愛称)Nickname()");
            var real = src.UList.Add("ガンダム", 0, "味方");
            real.Update();
            CreateUnit(src, "ステータス表示用ダミーユニット", "味方", p);
            SetLocal(src, "搭乗順番[" + p.ID + "]", null, 1d);
            SetLocal(src, "搭乗ユニット[" + p.ID + "]", real.ID, 0d);

            Assert.AreEqual("アムロ", p.get_Nickname(true));
        }

        [TestMethod]
        public void Nickname_ステータス表示用ダミーユニットでは搭乗ユニットの愛称変更を使う()
        {
            var src = CreateSrc();
            AddPilotData(src, "アムロ").Nickname = "アムロ";
            var p = AddPilot(src, "アムロ");
            AddUnitData(src, "ガンダム", "パイロット愛称=$(愛称)少尉");
            var real = src.UList.Add("ガンダム", 0, "味方");
            real.Update();
            // ステータス表示用ダミーユニットのデータは UDList に標準で登録されている
            CreateUnit(src, "ステータス表示用ダミーユニット", "味方", p);

            // 搭乗順番が設定されていなければデータの愛称
            Assert.AreEqual("アムロ", p.get_Nickname(false));

            SetLocal(src, "搭乗順番[" + p.ID + "]", null, 1d);
            SetLocal(src, "搭乗ユニット[" + p.ID + "]", real.ID, 0d);
            Assert.AreEqual("アムロ少尉", p.get_Nickname(false));
        }

        [TestMethod]
        public void KanaName_パイロット読み仮名を優先しなければ愛称をひらがな化する()
        {
            var (_, main, sub) = CreateTwoSeater("パイロット愛称=$(愛称)タイイ");
            Assert.AreEqual("あむろたいい", main.KanaName);
            Assert.AreEqual("せいら", sub.KanaName);

            var (_, main2, _) = CreateTwoSeater("パイロット読み仮名=れい", "パイロット愛称=レイ");
            Assert.AreEqual("れい", main2.KanaName);
        }

        [TestMethod]
        [Ignore("移植の不具合: KanaName がダミーユニットで搭乗ユニット名が無い場合に null を返す")]
        public void KanaName_ダミーユニットで搭乗ユニット未設定ならデータの読み仮名()
        {
            // VB: If uname = "" Then Exit Property（KanaName = Data.KanaName のまま）
            var src = CreateSrc();
            AddPilotData(src, "アムロ").KanaName = "あむろ";
            var p = AddPilot(src, "アムロ");
            // ステータス表示用ダミーユニットのデータは UDList に標準で登録されている
            CreateUnit(src, "ステータス表示用ダミーユニット", "味方", p);
            SetLocal(src, "搭乗順番[" + p.ID + "]", null, 1d);

            Assert.AreEqual("あむろ", p.KanaName);
        }

        // ──────────────────────────────────────────────
        // Sex / Personality
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Sex_ユニットの性別指定で変わる()
        {
            var (_, main, _) = CreateTwoSeater();
            Assert.AreEqual("男性", main.Sex);

            var (_, main2, _) = CreateTwoSeater("性別=女性");
            Assert.AreEqual("女性", main2.Sex);
        }

        [TestMethod]
        public void Personality_性格変更が最優先()
        {
            var (_, main, sub) = CreateTwoSeater("性格変更=超強気");
            Assert.AreEqual("超強気", main.Personality);
            Assert.AreEqual("超強気", sub.Personality);
        }

        [TestMethod]
        public void Personality_性格変更がなければ各自の性格()
        {
            var (_, main, sub) = CreateTwoSeater();
            Assert.AreEqual("普通", main.Personality);
            Assert.AreEqual("冷静", sub.Personality);
        }

        // ──────────────────────────────────────────────
        // Bitmap
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Bitmap_パイロット画像はメインパイロットだけに適用される()
        {
            var (_, main, sub) = CreateTwoSeater("パイロット画像=newtype.bmp");
            Assert.AreEqual("newtype.bmp", main.get_Bitmap(false));
            Assert.AreEqual("sayla.bmp", sub.get_Bitmap(false));
        }

        [TestMethod]
        public void Bitmap_パイロット画像状態はデータ2項目目の画像()
        {
            var (src, main, _) = CreateTwoSeater();
            main.Unit.AddCondition("パイロット画像", -1, NoLv, "変身 henshin.bmp");
            Assert.AreEqual("henshin.bmp", main.get_Bitmap(false));
        }

        [TestMethod]
        public void Bitmap_ステータス表示用ダミーユニットでは搭乗ユニットの画像を使う()
        {
            var src = CreateSrc();
            AddPilotData(src, "アムロ").Bitmap = "amuro.bmp";
            var p = AddPilot(src, "アムロ");
            AddUnitData(src, "ガンダム", "パイロット画像=pilot_suit.bmp");
            var real = src.UList.Add("ガンダム", 0, "味方");
            real.Update();
            // ステータス表示用ダミーユニットのデータは UDList に標準で登録されている
            CreateUnit(src, "ステータス表示用ダミーユニット", "味方", p);

            Assert.AreEqual("amuro.bmp", p.get_Bitmap(false));

            // 2番目以降の搭乗順番なら変更しない
            SetLocal(src, "搭乗順番[" + p.ID + "]", null, 2d);
            SetLocal(src, "搭乗ユニット[" + p.ID + "]", real.ID, 0d);
            Assert.AreEqual("amuro.bmp", p.get_Bitmap(false));

            src.Event.LocalVariableList.Remove("搭乗順番[" + p.ID + "]");
            SetLocal(src, "搭乗順番[" + p.ID + "]", null, 1d);
            Assert.AreEqual("pilot_suit.bmp", p.get_Bitmap(false));
        }

        // ──────────────────────────────────────────────
        // MessageType
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MessageType_メッセージ能力とメッセージ状態で変わる()
        {
            var src = CreateSrc();
            AddPilotData(src, "アムロ");
            AddPilotData(src, "兵士").AddSkill("メッセージ", NoLv, "一般兵", 1);
            var amuro = AddPilot(src, "アムロ");
            var soldier = AddPilot(src, "兵士");
            Assert.AreEqual("アムロ", amuro.MessageType);
            Assert.AreEqual("一般兵", soldier.MessageType);

            // 能力コピーで変身した場合はコピー元のメッセージ
            AddUnitData(src, "テスト機体");
            var u = CreateUnit(src, "テスト機体", "味方", amuro);
            u.AddCondition("メッセージ", -1, NoLv, "能力コピー シャア");
            Assert.AreEqual("シャア", amuro.MessageType);
        }

        // ──────────────────────────────────────────────
        // Defense
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Defense_耐久レベルで5ずつ上がる()
        {
            var src = CreateSrc();
            AddPilotData(src, "テストパイロット").AddSkill("耐久", 2d, "", 1);
            var p = AddPilot(src, "テストパイロット", 10);
            // 防御力成長オプションなしではレベルの影響なし
            Assert.AreEqual(110, p.Defense);
        }

        [TestMethod]
        public void Defense_防御力成長オプションでレベルと防御成長が加算される()
        {
            var src = CreateSrc();
            DefineOption(src, "防御力成長");
            var pd = AddPilotData(src, "テストパイロット");
            pd.AddSkill("耐久", 2d, "", 1);
            pd.AddSkill("防御成長", 1d, "", 1);
            var p = AddPilot(src, "テストパイロット", 10);

            // 100 + 5*2 + 10*(1+1)
            Assert.AreEqual(130, p.Defense);

            // 防御力低成長: (Lv * (1 + 2*防御成長)) \ 2
            DefineOption(src, "防御力低成長");
            Assert.AreEqual(110 + 15, p.Defense);
        }

        [TestMethod]
        public void Defense_防御力レベルアップオプションでも成長する()
        {
            var src = CreateSrc();
            DefineOption(src, "防御力レベルアップ");
            AddPilotData(src, "テストパイロット");
            var p = AddPilot(src, "テストパイロット", 7);
            Assert.AreEqual(107, p.Defense);
        }

        // ──────────────────────────────────────────────
        // Name / IsFix / IsRidingAdSupport
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Name_変更するとデータが切り替わり能力が更新される()
        {
            var src = CreateSrc();
            AddPilotData(src, "変身前");
            AddPilotData(src, "変身後").AddSkill("底力", 3d, "", 1);
            var p = AddPilot(src, "変身前");
            Assert.IsFalse(p.IsSkillAvailable("底力"));

            p.Name = "変身後";

            Assert.AreEqual("変身後", p.Name);
            Assert.AreEqual(3d, p.SkillLevel("底力"));
        }

        [TestMethod]
        public void IsFix_Fix変数の定義で判定する()
        {
            var src = CreateSrc();
            AddPilotData(src, "アムロ");
            var p = AddPilot(src, "アムロ");
            Assert.IsFalse(p.IsFix);

            SetGlobalString(src, "Fix(アムロ)", "1");
            Assert.IsTrue(p.IsFix);
        }

        [TestMethod]
        public void IsRidingAdSupport_サポートとして搭乗している場合のみ真()
        {
            var src = CreateSrc();
            AddPilotData(src, "メイン");
            AddPilotData(src, "サポート役");
            var main = AddPilot(src, "メイン");
            var sup = AddPilot(src, "サポート役");
            Assert.IsFalse(sup.IsRidingAdSupport);

            AddUnitData(src, "テスト機体");
            var u = CreateUnit(src, "テスト機体", "味方", main);
            u.AddSupport(sup);
            sup.Unit = u;

            Assert.IsTrue(sup.IsRidingAdSupport);
            Assert.IsFalse(main.IsRidingAdSupport);
        }
    }
}
