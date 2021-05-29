using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRCCore.Maps
{
    public class UnitMoveProps
    {
        public bool is_trans_available_on_ground { get; private set; }
        public bool is_trans_available_in_water { get; private set; }
        public bool is_trans_available_in_sky { get; private set; }
        public bool is_trans_available_in_moon_sky { get; private set; }
        public bool is_adaptable_in_water { get; private set; }
        public bool is_adaptable_in_space { get; private set; }
        public bool is_trans_available_on_water { get; private set; }
        public bool is_swimable { get; private set; }
        public IList<string> adopted_terrain { get; private set; }
        public IList<string> allowed_terrains { get; private set; }
        public IList<string> prohibited_terrains { get; private set; }

        public UnitMoveProps(Unit u)
        {
            is_trans_available_on_ground = u.IsTransAvailable("陸") && u.get_Adaption(2) != 0;
            is_trans_available_in_water = u.IsTransAvailable("水") && u.get_Adaption(3) != 0;
            is_trans_available_in_sky = u.IsTransAvailable("空") && u.get_Adaption(1) != 0;
            is_trans_available_in_moon_sky = is_trans_available_in_sky || u.IsTransAvailable("宇宙") && u.get_Adaption(4) != 0;
            is_adaptable_in_water = Strings.Mid(u.Data.Adaption, 3, 1) != "-" || u.IsFeatureAvailable("水中移動");
            is_adaptable_in_space = Strings.Mid(u.Data.Adaption, 4, 1) != "-" || u.IsFeatureAvailable("宇宙移動");
            is_trans_available_on_water = u.IsFeatureAvailable("水上移動") || u.IsFeatureAvailable("ホバー移動");
            adopted_terrain = GeneralLib.ToL(u.FeatureData("地形適応")).Skip(1).ToList();
            allowed_terrains = new List<string>();
            if (u.IsFeatureAvailable("移動制限"))
            {
                if (u.Area != "空中" && u.Area != "地中")
                {
                    allowed_terrains = u.Feature("移動制限").DataL.Skip(1).ToList();
                }
            }
            prohibited_terrains = new List<string>();
            if (u.IsFeatureAvailable("進入不可"))
            {
                if (u.Area != "空中" && u.Area != "地中")
                {
                    prohibited_terrains = u.Feature("進入不可").DataL.Skip(1).ToList();
                }
            }
            is_swimable = u.IsFeatureAvailable("水泳");
        }

        public bool IsAdopted(TerrainData td)
        {
            return adopted_terrain.Contains(td.Name);
        }

        public bool IsAllowed(TerrainData td)
        {
            // 移動制限が無ければ許可されているとみなす
            return !allowed_terrains.Any() || allowed_terrains.Contains(td.Name);
        }

        public bool IsProhibited(TerrainData td)
        {
            return prohibited_terrains.Contains(td.Name);
        }
    }
}
