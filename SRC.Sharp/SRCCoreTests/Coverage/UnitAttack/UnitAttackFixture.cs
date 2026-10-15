using SRCCore.Filesystem;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SRCCore.Coverage.Tests.UnitAttack
{
    /// <summary>
    /// Unit.Attack / Unit.MapAttack のテスト用に、空マップ・ユニット・パイロットを用意するヘルパー。
    /// GUI は HeadlessGUI で呼び出しを記録する（戦闘アニメ無し）。
    /// 命中・クリティカルは武器の命中補正／CT補正を極端な値にして乱数に依存しないようにする。
    /// </summary>
    internal class UnitAttackFixture
    {
        public SRC Src { get; }
        public HeadlessGUI Gui { get; }

        public UnitAttackFixture(int width = 10, int height = 10)
        {
            Src = new SRC();
            Gui = new HeadlessGUI();
            Gui.Install(Src);
            Src.SystemConfig.BattleAnimation = false;
            // 効果音ファイルを探しに行かないようにする
            Src.SystemConfig.AppPath = "";
            Src.FileSystem = new LocalFileSystem();
            // イベントは定義しない（HandleEvent が空のイベントキューで動くようにする）
            Src.Event.InitEventData();
            var map = Src.Map;
            map.MapFileName = "";
            map.MapWidth = width;
            map.MapHeight = height;
            map.MapData = new Src2DArray<MapCell>(width, height);
            for (var x = 1; x <= width; x++)
            {
                for (var y = 1; y <= height; y++)
                {
                    map.MapData[x, y] = MapCell.EmptyCell;
                }
            }
            map.MapDataForUnit = new Src2DArray<Unit>(width, height);
            map.MaskData = new Src2DArray<bool>(width, height);
        }

        /// <summary>
        /// ユニットを作成して (x, y) に出撃させる。
        /// </summary>
        public Unit AddUnit(string name, string party, int x, int y,
            int hp = 1000, int en = 100, int armor = 0,
            IEnumerable<WeaponData> weapons = null,
            IEnumerable<string> features = null,
            string personality = "普通",
            IEnumerable<(string name, double level)> skills = null)
        {
            var ud = Src.UDList.Add(name);
            ud.Nickname = name;
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            ud.HP = hp;
            ud.EN = en;
            ud.Armor = armor;
            ud.Mobility = 0;
            ud.Speed = 5;
            ud.Size = "M";
            ud.PilotNum = 1;
            ud.Class = "汎用";
            foreach (var f in features ?? Enumerable.Empty<string>())
            {
                ud.AddFeature(f);
            }
            foreach (var wd in weapons ?? Enumerable.Empty<WeaponData>())
            {
                var nw = ud.AddWeapon(wd.Name);
                nw.Power = wd.Power;
                nw.MinRange = wd.MinRange;
                nw.MaxRange = wd.MaxRange;
                nw.Precision = wd.Precision;
                nw.Bullet = wd.Bullet;
                nw.ENConsumption = wd.ENConsumption;
                nw.NecessaryMorale = wd.NecessaryMorale;
                nw.Adaption = wd.Adaption;
                nw.Critical = wd.Critical;
                nw.Class = wd.Class;
                nw.NecessarySkill = wd.NecessarySkill ?? "";
                nw.NecessaryCondition = wd.NecessaryCondition ?? "";
            }

            var pname = name + "パイロット";
            var pd = Src.PDList.Add(pname);
            pd.Nickname = pname;
            pd.Adaption = "AAAA";
            pd.Class = "汎用";
            pd.Infight = 100;
            pd.Shooting = 100;
            pd.Hit = 100;
            pd.Dodge = 100;
            pd.Intuition = 100;
            pd.Technique = 100;
            pd.Personality = personality;
            pd.SP = 0;
            foreach (var (sname, slevel) in skills ?? Enumerable.Empty<(string, double)>())
            {
                pd.AddSkill(sname, slevel, "", 1);
            }
            var p = Src.PList.Add(pname, 1, party);

            var u = Src.UList.Add(name, 0, party);
            p.Ride(u);
            u.FullRecover();
            u.StandBy(x, y);
            u.Update();
            return u;
        }

        /// <summary>
        /// 必中（命中補正が非常に大きい）・クリティカル無し（CT補正が非常に小さい）の武器データ。
        /// </summary>
        public WeaponData Weapon(string name, int power, string wclass = "",
            int minRange = 1, int maxRange = 1, int bullet = 0, int en = 0,
            int precision = 1000, int critical = -1000)
        {
            return new WeaponData(Src)
            {
                Name = name,
                Power = power,
                MinRange = minRange,
                MaxRange = maxRange,
                Precision = precision,
                Critical = critical,
                Bullet = bullet,
                ENConsumption = en,
                Adaption = "AAAA",
                Class = wclass,
                NecessarySkill = "",
                NecessaryCondition = "",
            };
        }

        /// <summary>
        /// GeneralLib.Dice の乱数系列をすべて value に固定する（Dice(max) = (int)(max * value) + 1）。
        /// 戻り値の Dispose で元の系列とインデックスに戻す。
        /// </summary>
        public static System.IDisposable FixDice(double value)
        {
            var field = typeof(GeneralLib).GetField("RndHistory", BindingFlags.NonPublic | BindingFlags.Static);
            var history = (double[])field.GetValue(null);
            var saved = (double[])history.Clone();
            var savedIndex = GeneralLib.RndIndex;
            for (var i = 0; i < history.Length; i++)
            {
                history[i] = value;
            }
            return new Restore(() =>
            {
                System.Array.Copy(saved, history, saved.Length);
                GeneralLib.RndIndex = savedIndex;
            });
        }

        private sealed class Restore : System.IDisposable
        {
            private readonly System.Action action;
            public Restore(System.Action action) { this.action = action; }
            public void Dispose() { action(); }
        }

        public IList<string> SysMessages()
        {
            return Gui.CallsOf(nameof(IGUI.DisplaySysMessage)).Select(x => (string)x.Args[0]).ToList();
        }

        public IList<HeadlessGUICall> Calls(string member)
        {
            return Gui.CallsOf(member).ToList();
        }
    }
}
