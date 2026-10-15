using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Coverage.Tests.HelpCoverage
{
    /// <summary>
    /// Help のテスト用に、パイロットが搭乗したユニットを組み立てる補助。
    /// </summary>
    internal sealed class HelpTestFixture
    {
        public SRC Src { get; }
        public MockGUI Gui { get; }
        public Models.UnitData UnitData { get; }
        public Models.PilotData PilotData { get; }

        public HelpTestFixture(string unitName = "テスト機", string pilotName = "テストパイロット")
        {
            Gui = new MockGUI();
            Src = new SRC { GUI = Gui };
            PilotData = Src.PDList.Add(pilotName);
            PilotData.SP = 50;
            PilotData.Sex = "男性";
            PilotData.Adaption = "AAAA";
            UnitData = Src.UDList.Add(unitName);
            UnitData.Nickname = unitName;
            UnitData.HP = 3000;
            UnitData.EN = 200;
            UnitData.Armor = 1000;
            UnitData.Mobility = 100;
            UnitData.Speed = 5;
            UnitData.PilotNum = 1;
            UnitData.Transportation = "陸";
            UnitData.Adaption = "AAAA";
            UnitData.Size = "M";
        }

        // 武器を追加する
        public Models.WeaponData AddWeapon(string name, string cls, int bullet = 0, int en = 0)
        {
            var wd = UnitData.AddWeapon(name);
            wd.Power = 1000;
            wd.MinRange = 1;
            wd.MaxRange = 1;
            wd.Adaption = "AAAA";
            wd.Class = cls;
            wd.Bullet = bullet;
            wd.ENConsumption = en;
            wd.NecessarySkill = "";
            wd.NecessaryCondition = "";
            return wd;
        }

        // アビリティを追加する
        public Models.AbilityData AddAbility(string name, string cls, int stock = 0)
        {
            var ad = UnitData.AddAbility(name);
            ad.MinRange = 1;
            ad.MaxRange = 1;
            ad.Class = cls;
            ad.Stock = stock;
            return ad;
        }

        // パイロットを生成してユニットに搭乗させる
        public Unit Build(int level = 1)
        {
            var p = Src.PList.Add(PilotData.Name, level, "味方");
            var u = Src.UList.Add(UnitData.Name, 0, "味方");
            p.Ride(u);
            u.Update();
            u.FullRecover();
            return u;
        }
    }
}
