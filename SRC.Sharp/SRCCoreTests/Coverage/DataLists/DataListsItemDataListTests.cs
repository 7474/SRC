using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Exceptions;
using SRCCore.Models;
using SRCCore.TestLib;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// ItemDataList.Load (item.txt / アイテムデータ) の読み込み仕様のテスト。
    /// VB 版 ItemDataList.cls の Load を仕様として確認する。
    /// </summary>
    [TestClass]
    public class DataListsItemDataListTests
    {
        private static SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        private static ItemDataList Load(SRC src, string text, string fname = "item.txt")
        {
            src.IDList.Load(fname, new MemoryStream(Encoding.UTF8.GetBytes(text)));
            return src.IDList;
        }

        private static string[] Errors(SRC src) => src.DataErrors.Select(x => x.msg).ToArray();

        private const string Rifle =
            "ビームライフル\n" +
            "ビームライフル,びーむらいふる,武器,片手\n" +
            "特殊能力\n" +
            "射撃強化Lv1\n" +
            "100,-10,+50,-5,0\n" +
            "ビームライフル,1500,2,5,+10,8,10,-,AABA,+0,B\n" +
            "===\n" +
            "応急修理,回復Lv1,1,1,-,-,-\n" +
            "*連邦軍の標準装備。\n" +
            "*エネルギーパック式。\n";

        [TestMethod]
        public void Load_StandardItem_ParsesAllFields()
        {
            var src = CreateSrc();
            var id = Load(src, Rifle).Item("ビームライフル");

            Assert.AreEqual("ビームライフル", id.Nickname);
            Assert.AreEqual("びーむらいふる", id.KanaName);
            Assert.AreEqual("武器", id.Class);
            Assert.AreEqual("片手", id.Part);
            CollectionAssert.AreEqual(new[] { "射撃強化" }, id.Features.Select(x => x.Name).ToArray());
            // 修正値は符号付きで読み込む
            Assert.AreEqual(100, id.HP);
            Assert.AreEqual(-10, id.EN);
            Assert.AreEqual(50, id.Armor);
            Assert.AreEqual(-5, id.Mobility);
            Assert.AreEqual(0, id.Speed);
            Assert.AreEqual(1500, id.Weapons.Single().Power);
            Assert.AreEqual("応急修理", id.Abilities.Single().Name);
            Assert.AreEqual(0, src.DataErrors.Count);
        }

        [TestMethod]
        public void Load_StandardItem_JoinsCommentLines()
        {
            // 「*」で始まる行は解説として行頭の「*」を除いて連結する
            var src = CreateSrc();
            var id = Load(src, Rifle).Item("ビームライフル");

            var lines = id.Comment.Split('\n').Select(x => x.TrimEnd('\r')).ToArray();
            CollectionAssert.AreEqual(new[] { "連邦軍の標準装備。", "エネルギーパック式。" }, lines);
        }

        [TestMethod]
        public void Load_CommentDirectlyAfterWeapons_EndsWeaponList()
        {
            // 武器データは解説行「*」で終了する
            var src = CreateSrc();
            var id = Load(src,
                "剣\n剣,武器,片手\n特殊能力なし\n0,0,0,0,0\n" +
                "斬撃,1200,1,1,+0,-,-,-,AAAA,+0,武\n*切れ味鋭い。\n").Item("剣");

            Assert.AreEqual(1, id.Weapons.Count);
            Assert.AreEqual(0, id.Abilities.Count);
            Assert.AreEqual("切れ味鋭い。", id.Comment);
        }

        [TestMethod]
        public void Load_KanaOmitted_DerivesKanaFromNickname()
        {
            // 2行目のコンマが2個なら読み仮名は愛称をひらがな化したもの
            var src = CreateSrc();
            var id = Load(src, "盾\nシールド,盾,盾\n特殊能力\nシールド\n0,0,100,0,0\n").Item("盾");

            Assert.AreEqual("シールド", id.Nickname);
            Assert.AreEqual("しーるど", id.KanaName);
            Assert.AreEqual("盾", id.Class);
            Assert.AreEqual("盾", id.Part);
            Assert.AreEqual(100, id.Armor);
            Assert.AreEqual(0, id.Weapons.Count);
        }

        [TestMethod]
        public void Load_InvalidModifiers_ReportErrorsAndLeaveZero()
        {
            var src = CreateSrc();
            var id = Load(src, "盾\n盾,盾,盾\n特殊能力なし\nA,B,C,D,E\n").Item("盾");

            Assert.AreEqual(0, id.HP);
            Assert.AreEqual(0, id.EN);
            Assert.AreEqual(0, id.Armor);
            Assert.AreEqual(0, id.Mobility);
            Assert.AreEqual(0, id.Speed);
            CollectionAssert.AreEqual(new[]
            {
                "最大ＨＰ修正値の設定が間違っています。",
                "最大ＥＮ修正値の設定が間違っています。",
                "装甲修正値の設定が間違っています。",
                "運動性修正値の設定が間違っています。",
                "移動力修正値の設定が間違っています。",
            }, Errors(src));
            Assert.IsTrue(src.DataErrors.All(x => x.dname == "盾" && x.fname == "item.txt" && x.line_num == 4));
        }

        [TestMethod]
        [DataRow("盾,a\n盾,盾,盾\n", "名称の設定が抜けています。")]
        [DataRow("大 盾\n盾,盾,盾\n", "名称に半角スペースは使用出来ません。")]
        [DataRow("盾（大）\n盾,盾,盾\n", "名称に全角括弧は使用出来ません。")]
        [DataRow("盾\"\n盾,盾,盾\n", "名称に\"は使用出来ません。")]
        [DataRow("盾\n盾,盾\n", "設定に抜けがあります。")]
        [DataRow("盾\n盾,たて,盾,盾,盾\n", "余分な「,」があります。")]
        [DataRow("盾\n盾,盾,\n", "装備個所の設定が抜けています。")]
        [DataRow("盾\n盾,盾,盾\n0,0,0,0,0\n", "特殊能力の設定が抜けています。")]
        [DataRow("盾\n盾,盾,盾\n特殊能力なし\n0\n", "最大ＥＮ修正値の設定が抜けています。")]
        [DataRow("盾\n盾,盾,盾\n特殊能力なし\n0,0\n", "装甲修正値の設定が抜けています。")]
        [DataRow("盾\n盾,盾,盾\n特殊能力なし\n0,0,0\n", "運動性修正値の設定が抜けています。")]
        [DataRow("盾\n盾,盾,盾\n特殊能力なし\n0,0,0,0\n", "移動力修正値の設定が抜けています。")]
        [DataRow("盾\n盾,盾,盾\n特殊能力なし\n0,0,0,0,\n", "移動力修正値の設定が抜けています。")]
        public void Load_MalformedItem_ThrowsWithMessage(string text, string expected)
        {
            var src = CreateSrc();
            var ex = Assert.Throws<InvalidSrcDataException>(() => Load(src, text));
            Assert.AreEqual(expected, ex.InvalidDataList.Single().msg);
            Assert.AreEqual("item.txt", ex.InvalidDataList.Single().fname);
        }

        [TestMethod]
        public void Load_RedefinedItem_ReplacesWithNewData()
        {
            // 既に定義済みのアイテムは削除してから登録し直す (VB: Delete → Add)
            var src = CreateSrc();
            var list = Load(src, Rifle);
            var before = list.Item("ビームライフル");

            Load(src, "ビームライフル\nビームライフル,武器,両手\n特殊能力なし\n0,0,0,0,1\n");

            var after = list.Item("ビームライフル");
            Assert.AreNotSame(before, after);
            Assert.AreEqual(1, list.Count());
            Assert.AreEqual("両手", after.Part);
            Assert.AreEqual(1, after.Speed);
            Assert.AreEqual(0, after.Weapons.Count);
        }

        [TestMethod]
        public void Load_MultipleItems_AndDataComments()
        {
            var src = CreateSrc();
            var list = Load(src, "# アイテム一覧\n" + Rifle + "\n# 盾\n盾\n盾,盾,盾\n特殊能力なし\n0,0,0,0,0\n");

            Assert.AreEqual(2, list.Count());
            Assert.IsTrue(list.IsDefined("盾"));
            Assert.AreEqual("# アイテム一覧", list.DataComment);
            Assert.AreEqual("# 盾", list.Item("ビームライフル").DataComment);
            StringAssert.StartsWith(list.Item("盾").Raw, "盾");
        }

        [TestMethod]
        public void Clear_RemovesItemsAndRawData()
        {
            var src = CreateSrc();
            var list = Load(src, "# c\n" + Rifle);
            list.Clear();

            Assert.AreEqual(0, list.Count());
            Assert.AreEqual("", list.Raw);
            Assert.AreEqual("", list.DataComment);
        }
    }
}
