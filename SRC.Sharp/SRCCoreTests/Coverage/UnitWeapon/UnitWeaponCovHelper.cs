using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Units;
using System;
using System.Linq;

namespace SRCCore.Coverage.Tests.UnitWeaponCov
{
    /// <summary>
    /// UnitWeapon のテスト用に、地形付きの小さなマップと
    /// パイロット搭乗済みのユニットを組み立てる。
    /// </summary>
    internal static class UnitWeaponCovHelper
    {
        public static SRC CreateSrc(int width = 10, int height = 10, string terrainClass = "陸", int hitMod = 0, int damageMod = 0)
        {
            var src = new SRC { GUI = new MockGUI() };
            // SetMapSize は GUI のマップの実装を要するため、フィールドに直接置く
            src.Map.MapWidth = width;
            src.Map.MapHeight = height;
            src.Map.MapDataForUnit = new Src2DArray<Unit>(width, height);
            src.Map.MaskData = new Src2DArray<bool>(width, height);
            src.Map.MapData = new Src2DArray<MapCell>(width, height);
            for (var x = 1; x <= width; x++)
            {
                for (var y = 1; y <= height; y++)
                {
                    SetTerrain(src, x, y, "平地", terrainClass, hitMod, damageMod);
                }
            }
            return src;
        }

        public static void SetTerrain(SRC src, int x, int y, string name, string terrainClass, int hitMod = 0, int damageMod = 0)
        {
            src.Map.MapData[x, y] = new MapCell
            {
                X = x,
                Y = y,
                UnderTerrain = new TerrainData { Name = name, Class = terrainClass, HitMod = hitMod, DamageMod = damageMod },
            };
        }

        /// <summary>
        /// ユニットを作成してマップに置く。configure で UnitData（武器・特殊能力など）を、
        /// configurePilot で PilotData（技能など）を設定できる。
        /// パイロットの能力値は計算を追いやすいよう格闘・射撃 100、その他 0、気力 100 にそろえる。
        /// </summary>
        public static Unit CreateUnit(
            SRC src,
            string party = "味方",
            int x = 1,
            int y = 1,
            string area = "地上",
            Action<UnitData> configure = null,
            Action<PilotData> configurePilot = null,
            bool withPilot = true)
        {
            var name = "機体_" + Guid.NewGuid().ToString("N");
            var ud = src.UDList.Add(name);
            ud.HP = 1000;
            ud.EN = 100;
            ud.Armor = 0;
            ud.Mobility = 0;
            ud.Adaption = "AAAA";
            ud.Transportation = "陸";
            ud.Size = "M";
            ud.Speed = 5;
            ud.PilotNum = 1;
            configure?.Invoke(ud);

            var u = src.UList.Add(name, 0, party);
            if (withPilot)
            {
                var pname = "パイロット_" + Guid.NewGuid().ToString("N");
                var pd = src.PDList.Add(pname);
                pd.SP = 10;
                pd.Adaption = "AAAA";
                configurePilot?.Invoke(pd);
                var p = src.PList.Add(pname, 1, party);
                p.Ride(u);
            }
            u.Update();
            u.Status = "出撃";
            u.x = x;
            u.y = y;
            u.Area = area;
            u.HP = u.MaxHP;
            u.EN = u.MaxEN;
            src.Map.MapDataForUnit[x, y] = u;
            if (withPilot)
            {
                var p = u.MainPilot();
                p.Infight = 100;
                p.Shooting = 100;
                p.Hit = 0;
                p.Dodge = 0;
                p.Intuition = 0;
                p.Technique = 0;
                p.Morale = 100;
            }
            return u;
        }

        public static WeaponData AddWeapon(UnitData ud, string name, int power = 1000, string wclass = "",
            int minRange = 1, int maxRange = 1, int precision = 0, int critical = 0,
            int bullet = 0, int en = 0, string adaption = "AAAA", int morale = 0)
        {
            var wd = ud.AddWeapon(name);
            wd.Power = power;
            wd.Class = wclass;
            wd.MinRange = minRange;
            wd.MaxRange = maxRange;
            wd.Precision = precision;
            wd.Critical = critical;
            wd.Bullet = bullet;
            wd.ENConsumption = en;
            wd.Adaption = adaption;
            wd.NecessaryMorale = morale;
            wd.NecessarySkill = "";
            wd.NecessaryCondition = "";
            return wd;
        }

        public static UnitWeapon Weapon(Unit u, string name)
        {
            return u.Weapons.First(w => w.Name == name);
        }
    }
}
