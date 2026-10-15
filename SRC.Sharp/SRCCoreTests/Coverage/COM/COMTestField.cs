using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Coverage.Tests.COMTests
{
    /// <summary>
    /// COM（コンピューターの思考ルーチン）テスト用の小さな戦場を作る補助クラス。
    /// </summary>
    internal sealed class COMTestField
    {
        public SRC Src { get; }
        public HeadlessGUI GUI { get; }
        public TerrainData Plain { get; }

        public COMTestField(int width = 10, int height = 10)
        {
            // 描画系の呼び出しは記録だけして何もしない
            GUI = new HeadlessGUI();
            Src = new SRC();
            GUI.Install(Src);

            // 平地 (陸、地形修正なし) だけのマップ
            // SetMapSize は GUIMap を要するため配列を直接用意する
            Plain = new TerrainData
            {
                ID = 0,
                Name = "平地",
                Bitmap = "plain",
                Class = "陸",
                MoveCost = 2,
                HitMod = 0,
                DamageMod = 0,
            };
            var map = Src.Map;
            map.MapFileName = "";
            map.MapWidth = width;
            map.MapHeight = height;
            map.MapData = new Src2DArray<MapCell>(width, height);
            for (var x = 1; x <= width; x++)
            {
                for (var y = 1; y <= height; y++)
                {
                    map.MapData[x, y] = new MapCell
                    {
                        X = x,
                        Y = y,
                        UnderTerrain = Plain,
                        UpperTerrain = Plain,
                    };
                }
            }
            map.MapDataForUnit = new Src2DArray<Unit>(width, height);
            map.MaskData = new Src2DArray<bool>(width, height);
            map.TotalMoveCost = new int[width + 2, height + 2];
            map.PointInZOC = new int[width + 2, height + 2];
        }

        /// <summary>
        /// ユニットデータを定義する。武器は AddWeapon で後から追加する。
        /// </summary>
        public UnitData DefineUnit(string name, int hp = 3000, int en = 200, int armor = 1000, int mobility = 100, int speed = 5)
        {
            var ud = Src.UDList.Add(name);
            ud.Nickname = name;
            ud.Class = "汎用";
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            ud.Size = "M";
            ud.HP = hp;
            ud.EN = en;
            ud.Armor = armor;
            ud.Mobility = mobility;
            ud.Speed = speed;
            ud.PilotNum = 1;
            return ud;
        }

        public static WeaponData AddWeapon(UnitData ud, string name, int power, int minRange = 1, int maxRange = 1,
            int precision = 0, int critical = 0, string wclass = "突", int bullet = 0, int en = 0, int morale = 0)
        {
            var wd = ud.AddWeapon(name);
            wd.Power = power;
            wd.MinRange = minRange;
            wd.MaxRange = maxRange;
            wd.Precision = precision;
            wd.Critical = critical;
            wd.Class = wclass;
            wd.Bullet = bullet;
            wd.ENConsumption = en;
            wd.NecessaryMorale = morale;
            wd.Adaption = "AAAA";
            return wd;
        }

        public PilotData DefinePilot(string name, int stat = 100, int technique = 100)
        {
            var pd = Src.PDList.Add(name);
            pd.Nickname = name;
            pd.Sex = "男性";
            pd.Class = "汎用";
            pd.Adaption = "AAAA";
            pd.Infight = stat;
            pd.Shooting = stat;
            pd.Hit = stat;
            pd.Dodge = stat;
            pd.Intuition = stat;
            pd.Technique = technique;
            pd.Personality = "普通";
            pd.SP = 50;
            return pd;
        }

        /// <summary>
        /// ユニットを作成してパイロットを乗せ、(x, y) に出撃させる。
        /// </summary>
        public Unit Deploy(string unitName, string pilotName, string party, int x, int y)
        {
            if (!Src.PDList.IsDefined(pilotName))
            {
                DefinePilot(pilotName);
            }
            var u = Src.UList.Add(unitName, 0, party);
            var p = Src.PList.Add(pilotName, 1, party);
            p.Ride(u);
            u.FullRecover();
            u.StandBy(x, y);
            return u;
        }
    }
}
