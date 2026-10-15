using SRCCore.TestLib;
using System.IO;
using System.Text;

namespace SRCCore.Coverage.Tests.ExprInfo
{
    /// <summary>
    /// Info 関数のテスト用に、データファイル形式の文字列からデータを読み込んだ SRC を作る
    /// </summary>
    internal static class ExprInfoFixture
    {
        public const string RobotData = @"テスト機
テスト機, てすとき, 汎用, 1, 2
陸, 4, M, 1200, 150
特殊能力
ＨＰ回復Lv2
バリア=バリア 1000 10
1000, 100, 600, 60
AABC, test_unit.bmp
テストビーム, 1800, 2, 4, +20, -, 10, 110, AAAA, +10, Ｂ
テストパンチ, 1500, 1, 1, +0, 5, -, -, AAAA, +5, 突
合体ビーム, 3500, 1, 6, +0, -, -, -, AAAA, +0, 合
===
テスト修理, 回復Lv2, 3, 2, 10, -, -

";

        public const string PilotData = @"テストパイロット
テスト, てすと, 男性, 汎用, AABA, 100
特殊能力
底力, 1
援護Lv2, 5
150, 140, 130, 120, 110, 100, 強気
ＳＰ, 50, 加速, 1, 必中, 5, 熱血, 50
test_pilot.bmp, test.mid

";

        public const string ItemData = @"テスト装甲
テスト装甲, てすとそうこう, 汎用, 強化パーツ
特殊能力
ＨＰ回復Lv1
500, 30, 100, 5, 1
アイテム弾, 1200, 1, 3, +30, 3, -, -, AAAA, +10, Ｐ
*説明文の1行目
*説明文の2行目

";

        public const string SpData = @"必中, ひっちゅう
必, 25, 自分, ターン, -, -, 必中
絶対命中
１ターンの間、命中率が100%になる

加速, かそく
加, 10, 自分, ターン, -, -, 加速
移動力強化Lv2 気力増加Lv1.5
１ターンの間、移動力+2

熱血, ねっけつ
熱, 40, 自分, 攻撃, -, -, 熱血
ダメージ増加Lv1
次の攻撃のダメージ2倍

";

        public const string NonPilotData = @"町長
マルタ町長, mayor.bmp

";

        private static Stream ToStream(string s)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(s));
        }

        public static SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.SPDList.Load("sp.txt", ToStream(SpData));
            src.UDList.Load("robot.txt", ToStream(RobotData));
            src.PDList.Load("pilot.txt", ToStream(PilotData));
            src.IDList.Load("item.txt", ToStream(ItemData));
            src.NPDList.Load("non_pilot.txt", ToStream(NonPilotData));
            return src;
        }

        /// <summary>
        /// テスト機にテストパイロットを乗せた味方ユニットを作る
        /// </summary>
        public static Units.Unit CreatePilotedUnit(SRC src, int rank = 0, int level = 1)
        {
            var p = src.PList.Add("テストパイロット", level, "味方");
            var u = src.UList.Add("テスト機", rank, "味方");
            p.Ride(u);
            u.Update();
            return u;
        }

        public static string Eval(SRC src, string expr)
        {
            return src.Expression.GetValueAsString(expr);
        }
    }
}
