// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using Newtonsoft.Json;
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Pilots
{
    // 作成されたパイロットのデータを管理するリストクラス
    [JsonObject(MemberSerialization.OptIn)]
    public class Pilots
    {
        // パイロット一覧
        [JsonProperty]
        private SrcCollection<Pilot> colPilots = new SrcCollection<Pilot>();
        public IList<Pilot> Items => colPilots.List;
        public IEnumerable<Pilot> AlivePilots => Items.Where(x => x.Alive);

        protected SRC SRC;
        private IGUI GUI => SRC.GUI;

        public Pilots(SRC src)
        {
            SRC = src;
        }

        public void Restore(SRC src)
        {
            SRC = src;
            foreach (var p in Items)
            {
                p.Restore(src);
            }
        }

        // パイロットを追加
        public Pilot Add(string pname, int plevel, string pparty, string gid = null)
        {
            Pilot AddRet = default;
            string key;
            int i;

            if (!SRC.PDList.IsDefined(pname))
            {
                throw new TerminateException($"パイロットデータ「{pname}」が見つかりません。");
            }
            var new_pilot = new Pilot(SRC, SRC.PDList.Item(pname));
            new_pilot.Level = plevel;
            new_pilot.Party = pparty;
            new_pilot.FullRecover();
            new_pilot.Alive = true;
            new_pilot.Unit = null;
            if (string.IsNullOrEmpty(gid))
            {
                // グループＩＤが指定されていない場合
                if (Strings.InStr(new_pilot.Name, "(ザコ)") == 0 && Strings.InStr(new_pilot.Name, "(汎用)") == 0)
                {
                    key = new_pilot.Name;
                    if (SRC.PList.IsDefined2(key))
                    {
                        var p = SRC.PList.Item2(key);

                        if ((p.ID ?? "") == (key ?? ""))
                        {
                            // 一度作成されたパイロットを復活させる場合
                            if (!p.Alive)
                            {
                                p.Level = plevel;
                                p.Party = pparty;
                                p.FullRecover();
                                p.Alive = true;

                                return p;
                            }

                            GUI.ErrorMessage(key + "というパイロットは既に登録されています");
                            return null;
                        }
                    }
                }
                else
                {
                    i = colPilots.Count;
                    do
                    {
                        i = (i + 1);
                        key = new_pilot.Name + "_" + SrcFormatter.Format(i);
                    }
                    while (SRC.PList.IsDefined2(key));
                }
            }
            else
            {
                // グループＩＤが指定されている場合
                key = gid;
                i = 1;
                while (SRC.PList.IsDefined2(key))
                {
                    i = (i + 1);
                    key = gid + ":" + SrcFormatter.Format(i);
                }
            }

            new_pilot.ID = key;
            colPilots.Add(new_pilot, key);
            return new_pilot;
        }

        public IEnumerable<Pilot> ItemsByGroupId(string gid, bool without_first)
        {
            var results = new List<Pilot>();
            var first = Item2(gid);
            if (first != null)
            {
                if (!without_first)
                {
                    results.Add(first);
                }
                results.AddRange(Items.Where(x => x.ID.StartsWith(gid + ":")));
            }
            return results;
        }

        // 登録されているパイロットの総数
        public int Count()
        {
            return colPilots.Count;
        }

        // リストからパイロットを削除
        public void Delete(string Index)
        {
            colPilots.Remove(Index);
        }

        // リストからパイロットを検索
        // 元の SRC は、ＩＤで見つかったパイロットが破棄されていれば名称・愛称で探し直し、
        // それでも見つからなければＩＤで見つかった（破棄された）パイロットを返す（On Error GoTo の後も Item は代入されたまま）
        public Pilot Item(string Index)
        {
            Pilot p = colPilots[Index];
            if (p?.Alive ?? false)
            {
                return p;
            }

            return FindAliveByName(Index) ?? p;
        }

        // ＩＤで見つからなければ名称で、それでも見つからなければ愛称で、破棄されていないパイロットを検索
        private Pilot FindAliveByName(string pname)
        {
            // ＩＤで見つからなければ名称で検索
            foreach (Pilot p in AlivePilots)
            {
                if ((p.Data.Name ?? "") == (pname ?? ""))
                {
                    return p;
                }
            }
            // それでも見つからなければ愛称で検索
            foreach (Pilot p in AlivePilots)
            {
                if ((p.Data.Nickname ?? "") == (pname ?? ""))
                {
                    return p;
                }
            }

            return null;
        }

        // パイロットが定義されているか
        // Item と異なり、破棄されたパイロットは定義されていないものとする
        public bool IsDefined(string Index)
        {
            var p = colPilots[Index];
            if (p?.Alive ?? false)
            {
                return true;
            }

            return FindAliveByName(Index) != null;
        }

        // パイロットを検索 (IDのみで検索)
        public Pilot Item2(string Index)
        {
            return colPilots[Index];
        }

        // パイロットが定義されているか (IDのみで検索)
        public bool IsDefined2(string Index)
        {
            return Item2(Index) != null;
        }

        // リストのアップデート
        public void Update()
        {
            Pilot p;
            // XXX Clone List
            foreach (Pilot currentP in colPilots.List.ToList())
            {
                p = currentP;
                if (p.Party != "味方" || !p.Alive)
                {
                    // 味方でないパイロットや破棄されたパイロットは削除
                    Delete(p.ID);
                }
                else if (p.IsAdditionalPilot)
                {
                    // 追加パイロットは削除
                    if (p.Unit is object)
                    {
                        var u = p.Unit;
                        u.pltAdditionalPilot = null;
                        foreach (var of in u.OtherForms)
                        {
                            of.pltAdditionalPilot = null;
                        }
                    }

                    Delete(p.ID);
                }
                else if (p.IsAdditionalSupport)
                {
                    // 追加サポートは削除
                    if (p.Unit is object)
                    {
                        var u = p.Unit;
                        u.pltAdditionalSupport = null;
                        foreach (var of in u.OtherForms)
                        {
                            of.pltAdditionalSupport = null;
                        }
                    }

                    Delete(p.ID);
                }
                else if (p.Nickname0 == "パイロット不在")
                {
                    // ダミーパイロットは削除
                    Delete(p.ID);
                }
                else if (p.Unit != null)
                {
                    if (p.Unit.IsFeatureAvailable("召喚ユニット"))
                    {
                        // 召喚ユニットの追加パイロットも削除
                        Delete(p.ID);
                    }
                }
            }

            // 残ったパイロットを全回復
            foreach (Pilot currentP1 in colPilots)
            {
                p = currentP1;
                p.FullRecover();
            }
        }

        // パイロットの支援修正を更新
        public void UpdateSupportMod(Unit u = null)
        {
            int xx, i, yy;
            int max_range, range;
            
            if (string.IsNullOrEmpty(SRC.Map.MapFileName))
            {
                return;
            }

            // ユニット指定がなければ全パイロットを更新
            if (u is null)
            {
                foreach (Pilot p in colPilots)
                    p.UpdateSupportMod();
                return;
            }
            
            // ユニットにパイロットが乗っていなければそのまま終了
            if (u.CountPilot() == 0)
            {
                return;
            }

            var withBlock = u.MainPilot();
            // メインパイロットを更新
            withBlock.UpdateSupportMod();

            // 支援範囲を算出
            max_range = withBlock.CommandRange();
            if (withBlock.IsSkillAvailable("広域サポート"))
            {
                max_range = GeneralLib.MaxLng(max_range, 2);
            }

            if (SRC.Expression.IsOptionDefined("信頼補正") && Strings.InStr(withBlock.Name, "(ザコ)") == 0)
            {
                if (SRC.Expression.IsOptionDefined("信頼補正範囲拡大"))
                {
                    max_range = GeneralLib.MaxLng(max_range, 2);
                }
                else
                {
                    max_range = GeneralLib.MaxLng(max_range, 1);
                }
            }

            // 他のパイロットを更新
            foreach (var subPilot in u.SubPilots)
            {
                subPilot.UpdateSupportMod();
            }

            foreach (var supportPilot in u.Supports)
            {
                supportPilot.UpdateSupportMod();
            }

            // 支援範囲が無いなら他のユニットに乗っているパイロットには影響無し
            if (max_range == 0)
            {
                return;
            }

            // 周りのユニットに乗っているパイロットの支援修正を更新
            for (xx = GeneralLib.MaxLng(u.x - max_range, 1); xx <= GeneralLib.MinLng(u.x + max_range, SRC.Map.MapWidth); xx++)
            {
                for (yy = GeneralLib.MaxLng(u.y - max_range, 1); yy <= GeneralLib.MinLng(u.y + max_range, SRC.Map.MapHeight); yy++)
                {
                    if (SRC.Map.MapDataForUnit[xx, yy] is null)
                    {
                        continue;
                    }

                    // 支援範囲内にいるかチェック
                    range = (Math.Abs((u.x - xx)) + Math.Abs((u.y - yy)));
                    if (range > max_range)
                    {
                        continue;
                    }

                    if (range == 0)
                    {
                        continue;
                    }

                    // 乗っているパイロット全員の支援修正を更新
                    var withBlock1 = SRC.Map.MapDataForUnit[xx, yy];
                    if (withBlock1.CountPilot() == 0)
                    {
                        continue;
                    }

                    withBlock1.MainPilot().UpdateSupportMod();
                    foreach (var subPilot in withBlock1.SubPilots)
                    {
                        subPilot.UpdateSupportMod();
                    }

                    foreach (var supportPilot in withBlock1.Supports)
                    {
                        supportPilot.UpdateSupportMod();
                    }
                }
            }
        }

        // 破棄されたパイロットを削除する
        public void Clean()
        {
            // 破棄されたパイロットを削除
            var nonAlivePilots = Items.Where(x => !x.Alive).ToList();
            foreach (Pilot p in nonAlivePilots)
            {
                Delete(p.ID);
            }
        }
    }
}
