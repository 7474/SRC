// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using Newtonsoft.Json;
using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.VB;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Units
{
    // 全ユニットのデータを管理するリストのクラス
    [JsonObject(MemberSerialization.OptIn)]
    public partial class Units
    {
        // ユニットＩＤ作成用カウンタ
        [JsonProperty]
        public int IDCount = 0;

        // ユニット一覧
        [JsonProperty]
        private SrcCollection<Unit> colUnits = new SrcCollection<Unit>();
        public IList<Unit> Items => colUnits.List;

        protected SRC SRC;
        private IGUI GUI => SRC.GUI;

        public Units(SRC src)
        {
            SRC = src;
        }

        public void Restore(SRC src)
        {
            SRC = src;
            foreach (var u in Items)
            {
                u.Restore(src);
            }
        }

        // ユニットリストに新しいユニットを追加
        public Unit Add(string uname, int urank, string uparty)
        {
            // ユニットデータが定義されている？
            if (!SRC.UDList.IsDefined(uname))
            {
                return null;
            }

            var ud = SRC.UDList.Item(uname);
            var new_unit = new Unit(SRC)
            {
                Name = ud.Name,
                Rank = urank,
                Party = uparty,
                ID = CreateID(ud.Name),
            };
            new_unit.FullRecover();

            colUnits.Add(new_unit, new_unit.ID);

            // これ以降は本体以外の形態の追加
            var other_forms = new List<string>();

            // 変形先の形態
            {
                var fd = ud.Feature("変形");
                if (fd != null)
                {
                    foreach (var uname2 in fd.DataL.Skip(1))
                    {
                        if (!SRC.UDList.IsDefined(uname2))
                        {
                            GUI.ErrorMessage("ユニットデータ「" + uname + "」の変形先形態「" + uname2 + "」が見つかりません");
                            return null;
                        }
                        other_forms.Add(uname2);
                    }
                }
            }

            // ハイパーモード先の形態
            if (ud.IsFeatureAvailable("ハイパーモード"))
            {
                var fd = ud.Feature("ハイパーモード");
                var uname2 = fd.DataL.Skip(1).FirstOrDefault();
                if (string.IsNullOrEmpty(uname2))
                {
                    GUI.ErrorMessage("ユニットデータ「" + uname + "」のハイパーモード先形態が指定されていません");
                    return null;
                }
                else if (!SRC.UDList.IsDefined(uname2))
                {
                    GUI.ErrorMessage("ユニットデータ「" + uname + "」のハイパーモード先形態「" + uname2 + "」が見つかりません");
                    return null;
                }
                other_forms.Add(uname2);
            }

            // ノーマルモード先の形態
            if (ud.IsFeatureAvailable("ノーマルモード"))
            {
                var fd = ud.Feature("ノーマルモード");
                var uname2 = fd.DataL.FirstOrDefault();
                if (string.IsNullOrEmpty(uname2))
                {
                    GUI.ErrorMessage("ユニットデータ「" + uname + "」のノーマルモード先形態が指定されていません");
                    return null;
                }
                else if (!SRC.UDList.IsDefined(uname2))
                {
                    GUI.ErrorMessage("ユニットデータ「" + uname + "」のノーマルモード先形態「" + uname2 + "」が見つかりません");
                    return null;
                }
                other_forms.Add(uname2);
            }

            // パーツ分離先の形態
            if (ud.IsFeatureAvailable("パーツ分離"))
            {
                var fd = ud.Feature("パーツ分離");
                var uname2 = fd.DataL.Skip(1).FirstOrDefault();
                if (string.IsNullOrEmpty(uname2))
                {
                    GUI.ErrorMessage("ユニットデータ「" + uname + "」のパーツ分離先形態が指定されていません");
                    return null;
                }
                else if (!SRC.UDList.IsDefined(uname2))
                {
                    GUI.ErrorMessage("ユニットデータ「" + uname + "」のパーツ分離先形態「" + uname2 + "」が見つかりません");
                    return null;
                }
                other_forms.Add(uname2);
            }

            // パーツ合体先の形態
            if (ud.IsFeatureAvailable("パーツ合体"))
            {
                var fd = ud.Feature("パーツ合体");
                var uname2 = fd.DataL.FirstOrDefault();
                if (string.IsNullOrEmpty(uname2))
                {
                    GUI.ErrorMessage("ユニットデータ「" + uname + "」のパーツ合体先形態が指定されていません");
                    return null;
                }
                else if (!SRC.UDList.IsDefined(uname2))
                {
                    GUI.ErrorMessage("ユニットデータ「" + uname + "」のパーツ合体先形態「" + uname2 + "」が見つかりません");
                    return null;
                }
                other_forms.Add(uname2);
            }

            // 変形技の変形先の形態
            foreach (var fd in ud.Features.Where(x => x.Name == "変形技"))
            {
                var uname2 = fd.DataL.Skip(1).FirstOrDefault();
                if (string.IsNullOrEmpty(uname2))
                {
                    GUI.ErrorMessage("ユニットデータ「" + uname + "」の変形技使用後形態が指定されていません");
                    return null;
                }
                else if (!SRC.UDList.IsDefined(uname2))
                {
                    GUI.ErrorMessage("ユニットデータ「" + uname + "」の変形技使用後形態「" + uname2 + "」が見つかりません");
                    return null;
                }
                other_forms.Add(uname2);
            }

            // 換装先の形態
            {
                var fd = ud.Feature("換装");
                if (fd != null)
                {
                    foreach (var uname2 in fd.DataL)
                    {
                        if (!SRC.UDList.IsDefined(uname2))
                        {
                            GUI.ErrorMessage("ユニットデータ「" + uname + "」の換装先形態「" + uname2 + "」が見つかりません");
                            return null;
                        }
                        other_forms.Add(uname2);
                    }
                }
            }

            // 他形態で指定された形態
            {
                var fd = ud.Feature("他形態");
                if (fd != null)
                {
                    foreach (var uname2 in fd.DataL)
                    {
                        if (!SRC.UDList.IsDefined(uname2))
                        {
                            GUI.ErrorMessage("ユニットデータ「" + uname + "」の他形態「" + uname2 + "」が見つかりません");
                            return null;
                        }
                        other_forms.Add(uname2);
                    }
                }
            }

            // 形態を追加
            foreach (var other_form in other_forms)
            {
                if (!new_unit.IsOtherFormDefined(other_form))
                {
                    var new_form = new Unit(SRC);
                    new_form.Name = other_form;
                    new_form.Rank = urank;
                    new_form.Party = uparty;
                    new_form.ID = CreateID(ud.Name);
                    new_form.FullRecover();
                    new_form.Status = "他形態";

                    colUnits.Add(new_form, new_form.ID);
                    new_unit.AddOtherForm(new_form);
                }
            }

            // 追加した形態に対して自分自身を追加しておく
            foreach (var of1 in new_unit.OtherForms)
            {
                of1.AddOtherForm(new_unit);

                foreach (var of2 in new_unit.OtherForms.Where(x => x != of1))
                {
                    of1.AddOtherForm(of2);
                }
            }

            // 既に合体先 or 分離先のユニットが作成されていれば自分は他形態
            foreach (var fd in ud.Features)
            {
                if (fd.Name == "合体")
                {
                    if (SRC.UList.IsDefined(fd.DataL.Skip(1).FirstOrDefault() ?? ""))
                    {
                        new_unit.Status = "他形態";
                        break;
                    }
                }

                if (fd.Name == "分離")
                {
                    foreach (var ofname in fd.DataL.Skip(1))
                    {
                        if (SRC.UList.IsDefined(ofname))
                        {
                            new_unit.Status = "他形態";
                            break;
                        }
                    }
                }
            }

            return new_unit;
        }

        // ユニットリストにユニット u を追加
        public void Add2(Unit u)
        {
            colUnits.Add(u, u.ID);
        }

        // 新規ユニットIDを作成
        public string CreateID(string uname)
        {
            do
            {
                IDCount = IDCount + 1;
            } while (IsDefined(uname + ":" + SrcFormatter.Format(IDCount)));
            return uname + ":" + SrcFormatter.Format(IDCount);
        }

        // ユニットリストに登録されているユニット数を返す
        public int Count()
        {
            return colUnits.Count;
        }

        // ユニットリストからユニットを削除
        public void Delete(string Index)
        {
            colUnits.Remove(Index);
        }

        // ユニットリストから指定されたユニットを返す
        public Unit Item(string Index)
        {
            var _u = colUnits[Index];
            if (_u != null)
            {
                return _u;
            }

            // IDで見つからなければユニット名で検索
            var uname = Index;
            foreach (Unit u in colUnits)
            {
                if ((u.Name ?? "") == (uname ?? ""))
                {
                    if (u.Status != "破棄")
                    {
                        return u;
                    }
                }
            }
            return null;
        }

        // ユニットリストからユニットを検索 (IDのみ)
        public Unit Item2(string Index)
        {
            return colUnits[Index];
        }

        // ユニットリストに指定されたユニットが定義されているか？
        public bool IsDefined(string Index)
        {
            return Item(Index) != null;
        }

        // ユニットリストに指定されたユニットが定義されているか？ (IDのみ)
        public bool IsDefined2(string Index)
        {
            return Item2(Index) != null;
        }

        // ユニットリストをアップデート
        public void Update()
        {
            // 母艦に格納されたユニットを降ろす
            foreach (Unit u in colUnits.List)
            {
                foreach (var bu in u.UnitOnBoards.CloneList())
                {
                    u.UnloadUnit(bu.ID);
                }
            }

            // 破壊された味方ユニットがあるか検索
            var flag = false;
            foreach (Unit u in colUnits)
            {
                if (u.Party0 == "味方")
                {
                    if (u.Status == "破壊")
                    {
                        flag = true;
                        break;
                    }
                }
                else if (u.Party0 == "ＮＰＣ")
                {
                    if (u.Status == "破壊")
                    {
                        if (u.Summoner is object)
                        {
                            if (u.Summoner.Party0 == "味方")
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                }
            }

            // 破壊された味方ユニットがあれば修理
            if (flag)
            {
                GUI.OpenMessageForm(u1: null, u2: null);
                var prev_money = SRC.Money;
                foreach (Unit u in colUnits)
                {
                    if (u.Status != "破壊")
                    {
                        continue;
                    }

                    if (u.IsFeatureAvailable("召喚ユニット"))
                    {
                        continue;
                    }

                    switch (u.Party0 ?? "")
                    {
                        case "味方":
                            break;

                        case "ＮＰＣ":
                            if (u.Summoner is null)
                            {
                                continue;
                            }
                            else if (u.Summoner.Party0 != "味方")
                            {
                                continue;
                            }
                            break;

                        default:
                            continue;
                    }

                    SRC.IncrMoney(-u.Value);
                    u.Status = "待機";
                    if (!u.IsHero())
                    {
                        GUI.DisplayMessage("システム", u.Nickname + "を修理した;修理費 = " + SrcFormatter.Format(u.Value));
                    }
                    else
                    {
                        GUI.DisplayMessage("システム", u.Nickname + "を治療した;治療費 = " + SrcFormatter.Format(u.Value));
                    }
                }

                GUI.DisplayMessage("システム", "合計 = " + SrcFormatter.Format(prev_money - SRC.Money));
                GUI.CloseMessageForm();
            }

            // 全ユニットを待機状態に変更
            foreach (Unit u in colUnits)
            {
                switch (u.Status ?? "")
                {
                    case "出撃":
                    case "格納":
                        {
                            u.Status = "待機";
                            break;
                        }
                }
            }

            // ３段階までの変形・合体に対応
            for (var i = 1; i <= 3; i++)
            {
                // ノーマルモード・パーツ合体を行う
                foreach (Unit u in colUnits.List.CloneList())
                {
                    if (u.Party0 == "味方" && u.Status != "他形態" && u.Status != "旧主形態" && u.Status != "旧形態")
                    {
                        if (u.IsFeatureAvailable("ノーマルモード"))
                        {
                            u.Transform(GeneralLib.LIndex(u.FeatureData("ノーマルモード"), 1));
                        }
                        else if (u.IsFeatureAvailable("パーツ合体"))
                        {
                            if (GeneralLib.LLength(u.FeatureData("パーツ合体")) == 2)
                            {
                                u.Transform(GeneralLib.LIndex(u.FeatureData("パーツ合体"), 2));
                            }
                            else
                            {
                                u.Transform(GeneralLib.LIndex(u.FeatureData("パーツ合体"), 1));
                            }
                        }
                    }
                }

                // 分離を行う
                foreach (Unit u in colUnits.List.CloneList())
                {
                    if (CanSplitOnUpdate(u))
                    {
                        // 分離を実施
                        u.Split();
                    }
                }

                // 合体を行う
                foreach (Unit u in colUnits.List.CloneList())
                {
                    if (u.Party0 == "味方" && u.Status != "他形態" && u.Status != "旧主形態" && u.Status != "旧形態")
                    {
                        if (u.IsFeatureAvailable("合体"))
                        {
                            foreach (var fd in u.Features.Where(x => x.Name == "合体").ToList())
                            {
                                if (CanCombineOnUpdate(u, fd))
                                {
                                    // 合体を実施
                                    u.Combine(GeneralLib.LIndex(fd.Data, 2));
                                    break;
                                }
                            }
                        }
                    }
                }

                // 標準形態に変形
                foreach (Unit u in colUnits.List.CloneList())
                {
                    if (u.Party0 == "味方" && u.Status != "他形態" && u.Status != "旧主形態" && u.Status != "旧形態")
                    {
                        if (u.IsFeatureAvailable("変形"))
                        {
                            var uname = u.Name;
                            var buf = u.FeatureData("変形");
                            var loopTo = GeneralLib.LLength(buf);
                            for (var j = 2; j <= loopTo; j++)
                            {
                                var uname2 = GeneralLib.LIndex(buf, j);
                                if (SRC.UDList.IsDefined(uname2))
                                {
                                    if (SRC.UDList.Item(uname2).ID < SRC.UDList.Item(uname).ID)
                                    {
                                        uname = uname2;
                                    }
                                }
                                else
                                {
                                    GUI.ErrorMessage(uname + "の変形先ユニット「" + uname2 + "」のデータが定義されていません。");
                                }
                            }

                            if ((uname ?? "") != (u.Name ?? ""))
                            {
                                u.Transform(uname);
                            }
                        }
                    }
                }
            }

            // 暴走時パイロットを削除
            foreach (Unit u in colUnits)
            {
                if (u.IsFeatureAvailable("暴走時パイロット"))
                {
                    var berserkPilotName = u.FeatureData("暴走時パイロット");
                    if (SRC.PList.IsDefined(berserkPilotName))
                    {
                        SRC.PList.Delete(berserkPilotName);
                    }
                }
            }

            // ダミーパイロットを削除
            foreach (Unit u in colUnits)
            {
                if (u.CountPilot() > 0)
                {
                    if (u.Pilots.First().Nickname0 == "パイロット不在")
                    {
                        u.DeletePilot(u.Pilots.First());
                    }
                }
            }

            // 変身先の形態等、一時的な形態を削除
            foreach (Unit u in colUnits.List.CloneList())
            {
                if (u.Status == "待機")
                {
                    u.DeleteTemporaryOtherForm();
                }
            }

            // 破棄されたユニットを削除
            foreach (Unit u in colUnits.List.CloneList())
            {
                // 召喚ユニットは必ず破棄
                if (u.IsFeatureAvailable("召喚ユニット"))
                {
                    u.Status = "破棄";
                }
                // ダミーユニットを破棄
                if (u.IsFeatureAvailable("ダミーユニット"))
                {
                    u.Status = "破棄";
                }

                // 味方ユニット以外のユニットと破棄されたユニットを削除
                if (u.Party0 != "味方" || u.Status == "破棄")
                {
                    // ユニットが装備しているアイテムも破棄
                    foreach (var itm in u.ItemList)
                    {
                        itm.Exist = false;
                    }

                    Delete(u.ID);
                }
            }

            // ユニットの状態を回復
            foreach (Unit u in colUnits)
            {
                u.Reset();
            }

            // ステータスをアップデート
            foreach (Unit u in colUnits)
            {
                u.Update(true);
            }
        }

        // Update 時に分離を行うか判定する
        private bool CanSplitOnUpdate(Unit u)
        {
            if (!u.IsFeatureAvailable("分離"))
            {
                return false;
            }

            if (u.Party0 != "味方" || u.Status == "他形態" || u.Status == "旧主形態" || u.Status == "旧形態")
            {
                return false;
            }

            if (u.Status == "破棄")
            {
                if (u.CountPilot() == 0)
                {
                    return false;
                }
            }

            // 合体形態が主形態なら分離を行わない
            var splitData = u.FeatureData("分離");
            if (GeneralLib.LLength(splitData) > 3 && !u.IsFeatureAvailable("制限時間"))
            {
                return false;
            }

            if (u.IsFeatureAvailable("主形態"))
            {
                return false;
            }

            // パイロットが足らない場合は分離を行わない
            var n = 0;
            var loopTo = GeneralLib.LLength(splitData);
            for (var j = 2; j <= loopTo; j++)
            {
                var uname = GeneralLib.LIndex(splitData, j);
                if (SRC.UDList.IsDefined(uname))
                {
                    var ud = SRC.UDList.Item(uname);
                    if (!ud.IsFeatureAvailable("召喚ユニット"))
                    {
                        n = n + ud.PilotNum;
                    }
                }
            }

            if (u.CountPilot() < n)
            {
                return false;
            }

            // 分離先の形態が利用可能？
            for (var j = 2; j <= loopTo; j++)
            {
                var uname = GeneralLib.LIndex(splitData, j);
                if (!SRC.UList.IsDefined(uname))
                {
                    return false;
                }

                if (SRC.UList.Item(uname).CurrentForm().Status == "待機")
                {
                    return false;
                }
            }

            return true;
        }

        // Update 時に合体能力 fd による合体を行うか判定する
        private bool CanCombineOnUpdate(Unit u, FeatureData fd)
        {
            // 合体後の形態が利用可能？
            var uname = GeneralLib.LIndex(fd.Data, 2);
            if (!SRC.UList.IsDefined(uname))
            {
                return false;
            }

            var cu = SRC.UList.Item(uname);
            if (u.Status == "待機" && cu.CurrentForm().Status == "離脱")
            {
                return false;
            }

            if (cu.IsFeatureAvailable("制限時間"))
            {
                return false;
            }

            if (!cu.IsFeatureAvailable("主形態") && GeneralLib.LLength(fd.Data) == 3)
            {
                return false;
            }

            // 合体のパートナーが利用可能？
            var loopTo = GeneralLib.LLength(fd.Data);
            for (var k = 3; k <= loopTo; k++)
            {
                uname = GeneralLib.LIndex(fd.Data, k);
                if (!SRC.UList.IsDefined(uname))
                {
                    return false;
                }

                var pu = SRC.UList.Item(uname);
                if (u.Status == "待機")
                {
                    if (pu.CurrentForm().Status != "待機")
                    {
                        return false;
                    }
                }
                else if (pu.CurrentForm().Status != "離脱")
                {
                    return false;
                }
            }

            return true;
        }

        // ユニットリストをクリア
        public void Clear()
        {
            while (Count() > 0)
            {
                Delete(Items.First().ID);
            }
        }

        // ハイパーモードの自動発動チェック
        public void CheckAutoHyperMode()
        {
            foreach (Unit u in colUnits.List)
            {
                u.CheckAutoHyperMode();
            }
        }

        // ノーマルモードの自動発動チェック
        public void CheckAutoNormalMode()
        {
            var is_redraw_necessary = default(bool);
            foreach (Unit u in colUnits.List)
            {
                if (u.CheckAutoNormalMode(true))
                {
                    is_redraw_necessary = true;
                }
            }

            // 画面の再描画が必要？
            if (is_redraw_necessary)
            {
                GUI.RedrawScreen();
            }
        }

        // 破棄されたユニットを削除
        public void Clean()
        {
            // 出撃していないユニットは味方ユニット以外全て削除
            foreach (Unit u in colUnits.List.CloneList()
                .Where(u => u.Party0 != "味方"))
            {
                if (u.Status == "待機" || u.Status == "破壊")
                {
                    u.Status = "破棄";
                    foreach (var of in u.OtherForms)
                    {
                        of.Status = "破棄";
                    }
                }
            }

            // 破棄されたユニットを削除
            foreach (Unit u in colUnits.List.CloneList()
                .Where(u => u.Status == "破棄"))
            {
                // ユニットに乗っているパイロットも破棄
                foreach (var p in u.AllRawPilots)
                {
                    p.Alive = false;
                }

                // ユニットが装備しているアイテムも破棄
                foreach (var itm in u.ItemList)
                {
                    itm.Exist = false;
                }

                Delete(u.ID);
            }
        }
    }
}
