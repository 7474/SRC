// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;
using SRCCore.VB;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SRCCore.TestLib
{
    /// <summary>
    /// 元の SRC（VB5）の形式のセーブデータを書く（テストの見本を作るためのもの。#1577）。
    /// SRC.bas の SaveData・DumpData と、Pilots・Units・Items・Pilot・Unit・Item の Save・Dump、
    /// Map.bas の DumpMapData、Event.bas の DumpEventData・SaveGlobalVariables・SaveLocalVariables を写した。
    /// SRC# のゲームは元の SRC の形式では書かない（セーブは JSON）。
    /// </summary>
    public sealed class VbSaveDataWriter
    {
        /// <summary>書く版（VB: 10000 * Major + 100 * Minor + Revision）。既定は Ver.2.2.33</summary>
        public int Version { get; set; } = 20233;

        /// <summary>
        /// マップのレイヤ（"Layer" の行から）を書くか。Ver.2.4.0 の手直し（240a）で足したもので、Ver.2.2.33 は書かないと読んだ（推定）
        /// </summary>
        public bool WriteLayer { get; set; }

        /// <summary>乱数系列（RndSeed・RndIndex）を書くか。古い版は書かない</summary>
        public bool WriteRandomSeries { get; set; } = true;

        /// <summary>クリアデータでアイテムを書くか。古い版は書かない</summary>
        public bool WriteItems { get; set; } = true;

        private readonly StringBuilder sb = new StringBuilder();

        /// <summary>VB の Write #。文字列は「"」で囲み、真偽値は #TRUE#・#FALSE#、数値はそのまま（小数点は「.」）書き、カンマで区切って改行する</summary>
        public VbSaveDataWriter Write(params object[] values)
        {
            sb.Append(string.Join(",", values.Select(Format)));
            sb.Append("\r\n");
            return this;
        }

        private static string Format(object value)
        {
            switch (value)
            {
                case null:
                    return "\"\"";
                case string s:
                    return "\"" + s + "\"";
                case bool b:
                    return b ? "#TRUE#" : "#FALSE#";
                case double d:
                    return d.ToString("R", CultureInfo.InvariantCulture);
                case IFormattable f:
                    return f.ToString(null, CultureInfo.InvariantCulture);
                default:
                    return "\"" + value + "\"";
            }
        }

        public override string ToString()
        {
            return sb.ToString();
        }

        /// <summary>Shift_JIS のバイト列（元の SRC が書くファイルの中身）</summary>
        public byte[] ToBytes()
        {
            return VbSaveDataReader.ShiftJis.GetBytes(sb.ToString());
        }

        // データをセーブ（VB: SRC.bas の SaveData）
        public VbSaveDataWriter SaveData(SRC src)
        {
            if (Version > 10000)
            {
                Write(Version);
            }
            Write(src.Titles.Count);
            foreach (var title in src.Titles)
            {
                Write(title);
            }
            Write(src.Expression.GetValueAsString("次ステージ"));
            Write(src.TotalTurn);
            Write(src.Money);
            // パーツ用のダミー
            Write(0);

            SaveVariables(src.Event.GlobalVariableList);

            // Pilots.Save（追加パイロットや追加サポートはセーブしない）
            var pilots = src.PList.Items.Where(p => !p.IsAdditionalPilot && !p.IsAdditionalSupport).ToList();
            Write(pilots.Count);
            foreach (var p in pilots)
            {
                Write(p.Name == p.ID ? p.Name : p.Name + " " + p.ID);
                Write(p.Level, p.Exp);
                if (p.Unit is null)
                {
                    Write(p.Away ? "離脱" : "-");
                }
                else
                {
                    Write(p.Unit.ID);
                }
            }

            // Units.Save
            Write(src.UList.IDCount);
            Write(src.UList.Count());
            foreach (var u in src.UList.Items)
            {
                Write(u.Name);
                Write(u.ID, u.Rank, u.Status);
                Write(u.CountOtherForm());
                u.OtherForms.ToList().ForEach(x => Write(x.ID));
                Write(u.Pilots.Count);
                u.Pilots.ToList().ForEach(x => Write(x.ID));
                Write(u.CountSupport());
                u.Supports.ToList().ForEach(x => Write(x.ID));
                Write(u.ItemList.Count);
                u.ItemList.ToList().ForEach(x => Write(x.ID));
            }

            // Items.Save
            if (WriteItems)
            {
                Write(src.IList.CurrentIDCount);
                Write((int)src.IList.Count());
                foreach (var it in src.IList.List)
                {
                    Write(it.Name);
                    Write(it.ID, it.Unit is null ? "-" : it.Unit.ID);
                }
            }
            return this;
        }

        // 一時中断用データをファイルにセーブする（VB: SRC.bas の DumpData）
        public VbSaveDataWriter DumpData(SRC src)
        {
            if (Version > 10000)
            {
                Write(Version);
            }
            Write(src.ToSavedPath(src.ScenarioFileName).Replace('/', '\\'));
            Write(src.Titles.Count);
            foreach (var title in src.Titles)
            {
                Write(title);
            }
            Write(src.Turn);
            Write(src.TotalTurn);
            Write(src.Money);

            // DumpEventData
            SaveVariables(src.Event.GlobalVariableList);
            SaveVariables(src.Event.LocalVariableList);
            Write(src.Event.colEventLabelList.Count);
            foreach (var lab in src.Event.colEventLabelList.List)
            {
                Write(lab.Enable);
            }
            if (Version > 20003)
            {
                var files = src.Event.AdditionalEventFileNames ?? new string[0];
                Write(files.Count);
                foreach (var fname in files)
                {
                    Write(fname);
                }
            }

            // PList.Dump（追加パイロットはセーブしない）
            var pilots = src.PList.Items.Where(p => !p.IsAdditionalPilot).ToList();
            Write(pilots.Count);
            foreach (var p in pilots)
            {
                Write(p.Name, p.ID, p.Party);
                Write(p.Level, p.Exp);
                Write(p.SP, p.Morale, p.Plana);
                Write(p.Alive, p.Away, p.SupportIndex);
                Write(p.Unit is null ? "-" : p.Unit.ID);
            }

            // IList.Dump
            Write((int)src.IList.Count());
            foreach (var it in src.IList.List)
            {
                Write(it.Name, it.ID, it.Exist);
                Write(it.Unit is null ? "-" : it.Unit.ID);
            }

            // UList.Dump
            Write(src.UList.Count());
            foreach (var u in src.UList.Items)
            {
                DumpUnit(u);
            }

            DumpMapData(src);

            // ＢＧＭは Midi のフォルダからの相対パス
            var bgm = (src.Sound.BGMFileName ?? "").Replace('/', '\\');
            var idx = bgm.IndexOf("\\midi\\", StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                bgm = bgm.Substring(idx + 6);
            }
            else if (bgm.Contains('\\'))
            {
                bgm = bgm.Substring(bgm.IndexOf('\\') + 1);
            }
            Write(bgm);
            Write(src.Sound.RepeatMode);
            Write(src.Sound.KeepBGM);
            Write(src.Sound.BossBGM);

            if (WriteRandomSeries)
            {
                Write(GeneralLib.RndSeed);
                Write(GeneralLib.RndIndex);
            }
            return this;
        }

        // VB: SaveGlobalVariables・SaveLocalVariables（数値の変数も Format$ の文字列で書く）
        private void SaveVariables(SrcCollection<Expressions.VarData> list)
        {
            Write(list.Count);
            foreach (var v in list.List)
            {
                if (v.VariableType == Expressions.ValueType.StringType)
                {
                    Write(v.Name, v.StringValue);
                }
                else
                {
                    Write(v.Name, SrcFormatter.Format(v.NumericValue));
                }
            }
        }

        // VB: Unit.cls の Dump
        private void DumpUnit(Units.Unit u)
        {
            Write(u.Name, u.ID, u.Party0);
            Write(u.Rank, u.BossRank, u.x, u.y);
            Write(u.Area, u.UsedAction, u.Mode, u.Status);
            Write(u.Master is null ? "-" : u.Master.ID);
            Write(u.Summoner is null ? "-" : u.Summoner.ID);
            Write(u.UsedSupportAttack, u.UsedSupportGuard);
            Write(u.UsedSyncAttack, u.UsedCounterAttack);

            Write(u.CountOtherForm());
            u.OtherForms.ToList().ForEach(x => Write(x.ID));
            Write(u.UnitOnBoards.Count);
            u.UnitOnBoards.ToList().ForEach(x => Write(x.ID));
            Write(u.Pilots.Count);
            u.Pilots.ToList().ForEach(x => Write(x.ID));
            Write(u.CountSupport());
            u.Supports.ToList().ForEach(x => Write(x.ID));
            if (Version >= 20210)
            {
                var additionalSupport = u.AdditionalSupport();
                Write(additionalSupport is null ? "" : additionalSupport.ID);
            }
            Write(u.Servants.Count);
            u.Servants.ToList().ForEach(x => Write(x.ID));
            Write(u.ItemList.Count);
            u.ItemList.ToList().ForEach(x => Write(x.ID));

            Write(u.SpecialPowersInEffect.Count);
            foreach (var cnd in u.SpecialPowersInEffect)
            {
                Write(cnd.Name, cnd.StrData);
            }
            Write(u.Conditions.Count);
            foreach (var cnd in u.Conditions)
            {
                Write(cnd.Name, cnd.Lifetime, cnd.Level, cnd.StrData);
            }

            Write(u.CountWeapon());
            for (var i = 1; i <= u.CountWeapon(); i++)
            {
                Write(u.Weapon(i).Bullet());
            }
            Write(u.CountAbility());
            for (var i = 1; i <= u.CountAbility(); i++)
            {
                Write(u.Ability(i).Stock());
            }
            Write(u.HP, u.EN);
        }

        // VB: Map.bas の DumpMapData
        private void DumpMapData(SRC src)
        {
            var map = src.Map;
            var fname = string.IsNullOrEmpty(map.MapFileName) ? "" : src.ToSavedPath(map.MapFileName).Replace('/', '\\');
            Write(fname, map.MapDrawIsMapOnly ? map.MapDrawMode + "(マップ限定)" : map.MapDrawMode);
            Write(map.MapWidth, map.MapHeight);
            for (var i = 1; i <= map.MapWidth; i++)
            {
                for (var j = 1; j <= map.MapHeight; j++)
                {
                    Write(map.MapData[i, j].TerrainType, map.MapData[i, j].BitmapNo);
                }
            }
            Write(src.GUI.MapX, src.GUI.MapY);
            if (WriteLayer)
            {
                Write("Layer");
                for (var i = 1; i <= map.MapWidth; i++)
                {
                    for (var j = 1; j <= map.MapHeight; j++)
                    {
                        var cell = map.MapData[i, j];
                        Write(cell.LayerType, cell.LayerBitmapNo, (int)cell.BoxType);
                    }
                }
            }
        }
    }
}
