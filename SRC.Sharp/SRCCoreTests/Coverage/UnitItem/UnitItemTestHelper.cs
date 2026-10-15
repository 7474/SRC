using SRCCore.Items;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Coverage.Tests.UnitItem
{
    /// <summary>
    /// UnitItem カバレッジテスト用のユニット・アイテム生成ヘルパー
    /// </summary>
    internal static class UnitItemTestHelper
    {
        public static SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ユニットデータを定義してユニットを作成する
        public static Unit CreateUnit(SRC src, string name, string uclass = "汎用", int itemNum = 4, string party = "味方", params string[] features)
        {
            var ud = src.UDList.Add(name);
            ud.Class = uclass;
            ud.ItemNum = itemNum;
            ud.HP = 1000;
            ud.EN = 100;
            ud.Armor = 100;
            ud.Mobility = 50;
            ud.Speed = 4;
            ud.PilotNum = 1;
            foreach (var f in features)
            {
                ud.AddFeature(f);
            }
            var u = src.UList.Add(name, 0, party);
            u.Update();
            return u;
        }

        // アイテムデータを定義してアイテムを作成する
        public static Item CreateItem(SRC src, string name, string part, string iclass = "汎用", params string[] features)
        {
            var id = src.IDList.IsDefined(name) ? src.IDList.Item(name) : src.IDList.Add(name);
            id.Class = iclass;
            id.Part = part;
            id.Nickname = name;
            foreach (var f in features)
            {
                id.AddFeature(f);
            }
            return src.IList.Add(name);
        }

        // パイロットを作成して搭乗させる
        public static Pilots.Pilot RidePilot(SRC src, Unit u, string name, string sex)
        {
            var pd = src.PDList.Add(name);
            pd.Sex = sex;
            pd.SP = 100;
            var p = src.PList.Add(name, 1, u.Party0);
            p.Ride(u);
            u.Update();
            return p;
        }
    }
}
