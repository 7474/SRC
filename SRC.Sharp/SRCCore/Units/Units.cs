// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using Newtonsoft.Json;
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
            foreach(var u in Items)
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
                    foreach (var uname2 in fd.DataL.Skip(1))
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
                    foreach (var uname2 in fd.DataL.Skip(1))
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
            // TODO Impl Update
            //            Unit u;
            //            int k, i, j, n;
            //            int prev_money;
            //            var flag = default(bool);
            //            string pname, uname, uname2, buf;

            //            // 母艦に格納されたユニットを降ろす
            //            foreach (Unit currentU in colUnits)
            //            {
            //                u = currentU;
            //                var loopTo = u.CountUnitOnBoard();
            //                for (i = 1; i <= loopTo; i++)
            //                {
            //                    u.UnloadUnit(1);
            //                }
            //            }

            //            // 破壊された味方ユニットがあるか検索
            //            foreach (Unit currentU1 in colUnits)
            //            {
            //                u = currentU1;
            //                if (u.Party0 == "味方")
            //                {
            //                    if (u.Status_Renamed == "破壊")
            //                    {
            //                        flag = true;
            //                        break;
            //                    }
            //                }
            //                else if (u.Party0 == "ＮＰＣ")
            //                {
            //                    if (u.Status_Renamed == "破壊")
            //                    {
            //                        if (u.Summoner is object)
            //                        {
            //                            if (u.Summoner.Party0 == "味方")
            //                            {
            //                                flag = true;
            //                                break;
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            // 破壊された味方ユニットがあれば修理
            //            if (flag)
            //            {
            //                GUI.OpenMessageForm(u1: null, u2: null);
            //                prev_money = SRC.Money;
            //                foreach (Unit currentU2 in colUnits)
            //                {
            //                    u = currentU2;
            //                    if (u.Status_Renamed != "破壊")
            //                    {
            //                        goto NextDestroyedUnit;
            //                    }

            //                    if (u.IsFeatureAvailable("召喚ユニット"))
            //                    {
            //                        goto NextDestroyedUnit;
            //                    }

            //                    switch (u.Party0 ?? "")
            //                    {
            //                        case "味方":
            //                            {
            //                                break;
            //                            }

            //                        case "ＮＰＣ":
            //                            {
            //                                if (u.Summoner is null)
            //                                {
            //                                    goto NextDestroyedUnit;
            //                                }
            //                                else if (u.Summoner.Party0 != "味方")
            //                                {
            //                                    goto NextDestroyedUnit;
            //                                }

            //                                break;
            //                            }

            //                        default:
            //                            {
            //                                goto NextDestroyedUnit;
            //                                break;
            //                            }
            //                    }

            //                    SRC.IncrMoney(-u.Value);
            //                    u.Status_Renamed = "待機";
            //                    if (!u.IsHero())
            //                    {
            //                        GUI.DisplayMessage("システム", u.Nickname + "を修理した;修理費 = " + SrcFormatter.Format(u.Value));
            //                    }
            //                    else
            //                    {
            //                        GUI.DisplayMessage("システム", u.Nickname + "を治療した;治療費 = " + SrcFormatter.Format(u.Value));
            //                    }

            //                NextDestroyedUnit:
            //                    ;
            //                }

            //                GUI.DisplayMessage("システム", "合計 = " + SrcFormatter.Format(prev_money - SRC.Money));
            //                GUI.CloseMessageForm();
            //            }

            //            // 全ユニットを待機状態に変更
            //            foreach (Unit currentU3 in colUnits)
            //            {
            //                u = currentU3;
            //                switch (u.Status_Renamed ?? "")
            //                {
            //                    case "出撃":
            //                    case "格納":
            //                        {
            //                            u.Status_Renamed = "待機";
            //                            break;
            //                        }
            //                }
            //            }

            //            // ３段階までの変形・合体に対応
            //            for (i = 1; i <= 3; i++)
            //            {
            //                // ノーマルモード・パーツ合体を行う
            //                foreach (Unit currentU4 in colUnits)
            //                {
            //                    u = currentU4;
            //                    if (u.Party0 == "味方" & u.Status_Renamed != "他形態" & u.Status_Renamed != "旧主形態" & u.Status_Renamed != "旧形態")
            //                    {
            //                        if (u.IsFeatureAvailable("ノーマルモード"))
            //                        {
            //                            string localLIndex() { object argIndex1 = "ノーマルモード"; string arglist = u.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //                            u.Transform(localLIndex());
            //                        }
            //                        else if (u.IsFeatureAvailable("パーツ合体"))
            //                        {
            //                            if (GeneralLib.LLength(u.FeatureData(argIndex2)) == 2)
            //                            {
            //                                string localLIndex1() { object argIndex1 = "パーツ合体"; string arglist = u.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                                u.Transform(localLIndex1());
            //                            }
            //                            else
            //                            {
            //                                string localLIndex2() { object argIndex1 = "パーツ合体"; string arglist = u.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //                                u.Transform(localLIndex2());
            //                            }
            //                        }
            //                    }
            //                }

            //                // 分離を行う
            //                foreach (Unit currentU5 in colUnits)
            //                {
            //                    u = currentU5;
            //                    if (!u.IsFeatureAvailable("分離"))
            //                    {
            //                        goto NextLoop1;
            //                    }

            //                    if (u.Party0 != "味方" | u.Status_Renamed == "他形態" | u.Status_Renamed == "旧主形態" | u.Status_Renamed == "旧形態")
            //                    {
            //                        goto NextLoop1;
            //                    }

            //                    if (u.Status_Renamed == "破棄")
            //                    {
            //                        if (u.CountPilot() == 0)
            //                        {
            //                            goto NextLoop1;
            //                        }
            //                    }

            //                    // 合体形態が主形態なら分離を行わない

            //                    int localLLength() { object argIndex1 = "分離"; string arglist = u.FeatureData(argIndex1); var ret = GeneralLib.LLength(arglist); return ret; }

            //                    if (localLLength() > 3 & !u.IsFeatureAvailable("制限時間"))
            //                    {
            //                        goto NextLoop1;
            //                    }

            //                    if (u.IsFeatureAvailable("主形態"))
            //                    {
            //                        goto NextLoop1;
            //                    }

            //                    // パイロットが足らない場合は分離を行わない
            //                    n = 0;
            //                    var loopTo1 = GeneralLib.LLength(u.FeatureData(argIndex6));
            //                    for (j = 2; j <= loopTo1; j++)
            //                    {
            //                        uname = GeneralLib.LIndex(u.FeatureData(argIndex3), j);
            //                        if (SRC.UDList.IsDefined(uname))
            //                        {
            //                            {
            //                                var withBlock = SRC.UDList.Item(uname);
            //                                if (!withBlock.IsFeatureAvailable("召喚ユニット"))
            //                                {
            //                                    n = (n + withBlock.PilotNum);
            //                                }
            //                            }
            //                        }
            //                    }

            //                    if (u.CountPilot() < n)
            //                    {
            //                        goto NextLoop1;
            //                    }

            //                    // 分離先の形態が利用可能？
            //                    var loopTo2 = GeneralLib.LLength(u.FeatureData(argIndex8));
            //                    for (j = 2; j <= loopTo2; j++)
            //                    {
            //                        uname = GeneralLib.LIndex(u.FeatureData(argIndex7), j);
            //                        bool localIsDefined() { object argIndex1 = uname; var ret = SRC.UList.IsDefined(argIndex1); return ret; }

            //                        if (!localIsDefined())
            //                        {
            //                            goto NextLoop1;
            //                        }

            //                        Unit localItem() { object argIndex1 = uname; var ret = SRC.UList.Item(argIndex1); return ret; }

            //                        if (localItem().CurrentForm().Status_Renamed == "待機")
            //                        {
            //                            goto NextLoop1;
            //                        }
            //                    }

            //                    // 分離を実施
            //                    u.Split_Renamed();
            //                NextLoop1:
            //                    ;
            //                }

            //                // 合体を行う
            //                foreach (Unit currentU6 in colUnits)
            //                {
            //                    u = currentU6;
            //                    if (u.Party0 == "味方" & u.Status_Renamed != "他形態" & u.Status_Renamed != "旧主形態" & u.Status_Renamed != "旧形態")
            //                    {
            //                        if (u.IsFeatureAvailable("合体"))
            //                        {
            //                            var loopTo3 = u.CountFeature();
            //                            for (j = 1; j <= loopTo3; j++)
            //                            {
            //                                if (u.Feature(j) != "合体")
            //                                {
            //                                    goto NextLoop2;
            //                                }

            //                                // 合体後の形態が利用可能？
            //                                string localFeatureData() { object argIndex1 = j; var ret = u.FeatureData(argIndex1); return ret; }

            //                                uname = GeneralLib.LIndex(localFeatureData(), 2);
            //                                bool localIsDefined1() { object argIndex1 = uname; var ret = SRC.UList.IsDefined(argIndex1); return ret; }

            //                                if (!localIsDefined1())
            //                                {
            //                                    goto NextLoop2;
            //                                }

            //                                {
            //                                    var withBlock1 = SRC.UList.Item(uname);
            //                                    if (u.Status_Renamed == "待機" & withBlock1.CurrentForm().Status_Renamed == "離脱")
            //                                    {
            //                                        goto NextLoop2;
            //                                    }

            //                                    if (withBlock1.IsFeatureAvailable("制限時間"))
            //                                    {
            //                                        goto NextLoop2;
            //                                    }

            //                                    string localFeatureData1() { object argIndex1 = j; var ret = u.FeatureData(argIndex1); return ret; }

            //                                    int localLLength1() { string arglist = hs7335ed59602e416aa1b2600f4949cf4c(); var ret = GeneralLib.LLength(arglist); return ret; }

            //                                    if (!withBlock1.IsFeatureAvailable("主形態") & localLLength1() == 3)
            //                                    {
            //                                        goto NextLoop2;
            //                                    }
            //                                }

            //                                // 合体のパートナーが利用可能？
            //                                string localFeatureData3() { object argIndex1 = j; var ret = u.FeatureData(argIndex1); return ret; }

            //                                var loopTo4 = GeneralLib.LLength(localFeatureData3());
            //                                for (k = 3; k <= loopTo4; k++)
            //                                {
            //                                    string localFeatureData2() { object argIndex1 = j; var ret = u.FeatureData(argIndex1); return ret; }

            //                                    uname = GeneralLib.LIndex(localFeatureData2(), k);
            //                                    bool localIsDefined2() { object argIndex1 = uname; var ret = SRC.UList.IsDefined(argIndex1); return ret; }

            //                                    if (!localIsDefined2())
            //                                    {
            //                                        goto NextLoop2;
            //                                    }

            //                                    {
            //                                        var withBlock2 = SRC.UList.Item(uname);
            //                                        if (u.Status_Renamed == "待機")
            //                                        {
            //                                            if (withBlock2.CurrentForm().Status_Renamed != "待機")
            //                                            {
            //                                                goto NextLoop2;
            //                                            }
            //                                        }
            //                                        else if (withBlock2.CurrentForm().Status_Renamed != "離脱")
            //                                        {
            //                                            goto NextLoop2;
            //                                        }
            //                                    }
            //                                }

            //                                // 合体を実施
            //                                string localFeatureData4() { object argIndex1 = j; var ret = u.FeatureData(argIndex1); return ret; }

            //                                string localLIndex3() { string arglist = hs1e082bda318043228d140b6ae8f6a2c1(); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //                                u.Combine(localLIndex3());
            //                                break;
            //                            NextLoop2:
            //                                ;
            //                            }
            //                        }
            //                    }
            //                }

            //                // 標準形態に変形
            //                foreach (Unit currentU7 in colUnits)
            //                {
            //                    u = currentU7;
            //                    if (u.Party0 == "味方" & u.Status_Renamed != "他形態" & u.Status_Renamed != "旧主形態" & u.Status_Renamed != "旧形態")
            //                    {
            //                        if (u.IsFeatureAvailable("変形"))
            //                        {
            //                            uname = u.Name;
            //                            buf = u.FeatureData("変形");
            //                            var loopTo5 = GeneralLib.LLength(buf);
            //                            for (j = 2; j <= loopTo5; j++)
            //                            {
            //                                uname2 = GeneralLib.LIndex(buf, j);
            //                                if (SRC.UDList.IsDefined(uname2))
            //                                {
            //                                    UnitData localItem1() { object argIndex1 = uname2; var ret = SRC.UDList.Item(argIndex1); return ret; }

            //                                    UnitData localItem2() { object argIndex1 = uname; var ret = SRC.UDList.Item(argIndex1); return ret; }

            //                                    if (localItem1().ID < localItem2().ID)
            //                                    {
            //                                        uname = uname2;
            //                                    }
            //                                }
            //                                else
            //                                {
            //                                    GUI.ErrorMessage(uname + "の変形先ユニット「" + uname2 + "」のデータが定義されていません。");
            //                                }
            //                            }

            //                            if ((uname ?? "") != (u.Name ?? ""))
            //                            {
            //                                u.Transform(uname);
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            // 暴走時パイロットを削除
            //            foreach (Unit currentU8 in colUnits)
            //            {
            //                u = currentU8;
            //                if (u.IsFeatureAvailable("暴走時パイロット"))
            //                {
            //                    if (SRC.PList.IsDefined(u.FeatureData(argIndex16)))
            //                    {
            //                        SRC.PList.Delete(u.FeatureData(argIndex14));
            //                    }
            //                }
            //            }

            //            // ダミーパイロットを削除
            //            foreach (Unit currentU9 in colUnits)
            //            {
            //                u = currentU9;
            //                if (u.CountPilot() > 0)
            //                {
            //                    if (u.Pilot(1).Nickname0 == "パイロット不在")
            //                    {
            //                        u.DeletePilot(1);
            //                    }
            //                }
            //            }

            //            // 変身先の形態等、一時的な形態を削除
            //            foreach (Unit currentU10 in colUnits)
            //            {
            //                u = currentU10;
            //                if (u.Status_Renamed == "待機")
            //                {
            //                    u.DeleteTemporaryOtherForm();
            //                }
            //            }

            //            // 破棄されたユニットを削除
            //            foreach (Unit currentU11 in colUnits)
            //            {
            //                u = currentU11;
            //                // 召喚ユニットは必ず破棄
            //                if (u.IsFeatureAvailable("召喚ユニット"))
            //                {
            //                    u.Status_Renamed = "破棄";
            //                }
            //                // ダミーユニットを破棄
            //                if (u.IsFeatureAvailable("ダミーユニット"))
            //                {
            //                    u.Status_Renamed = "破棄";
            //                }

            //                // 味方ユニット以外のユニットと破棄されたユニットを削除
            //                if (u.Party0 != "味方" | u.Status_Renamed == "破棄")
            //                {
            //                    // ユニットが装備しているアイテムも破棄
            //                    var loopTo6 = u.CountItem();
            //                    for (i = 1; i <= loopTo6; i++)
            //                    {
            //                        Item localItem3() { object argIndex1 = i; var ret = u.Item(argIndex1); return ret; }

            //                        localItem3().Exist = false;
            //                    }

            //                    Delete("パーツ合体"0);
            //                }
            //            }

            //            // ユニットの状態を回復
            //            foreach (Unit currentU12 in colUnits)
            //            {
            //                u = currentU12;
            //                u.Reset_Renamed();
            //            }

            //            // ステータスをアップデート
            //            foreach (Unit currentU13 in colUnits)
            //            {
            //                u = currentU13;
            //                u.Update(true);
            //            }
        }


        //        // ユニットリストに登録されたユニットの情報をセーブ
        //        public void Save()
        //        {
        //            int i;
        //            FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)IDCount);
        //            FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)Count());
        //            foreach (Unit u in colUnits)
        //            {
        //                FileSystem.WriteLine(SRC.SaveDataFileNumber, u.Name);
        //                FileSystem.WriteLine(SRC.SaveDataFileNumber, u.ID, u.Rank, u.Status_Renamed);
        //                FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)u.CountOtherForm());
        //                var loopTo = u.CountOtherForm();
        //                for (i = 1; i <= loopTo; i++)
        //                {
        //                    Unit localOtherForm() { object argIndex1 = i; var ret = u.OtherForm(argIndex1); return ret; }

        //                    FileSystem.WriteLine(SRC.SaveDataFileNumber, localOtherForm().ID);
        //                }

        //                FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)u.CountPilot());
        //                var loopTo1 = u.CountPilot();
        //                for (i = 1; i <= loopTo1; i++)
        //                {
        //                    Pilot localPilot() { object argIndex1 = i; var ret = u.Pilot(argIndex1); return ret; }

        //                    FileSystem.WriteLine(SRC.SaveDataFileNumber, localPilot().ID);
        //                }

        //                FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)u.CountSupport());
        //                var loopTo2 = u.CountSupport();
        //                for (i = 1; i <= loopTo2; i++)
        //                {
        //                    Pilot localSupport() { object argIndex1 = i; var ret = u.Support(argIndex1); return ret; }

        //                    FileSystem.WriteLine(SRC.SaveDataFileNumber, localSupport().ID);
        //                }

        //                FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)u.CountItem());
        //                var loopTo3 = u.CountItem();
        //                for (i = 1; i <= loopTo3; i++)
        //                {
        //                    Item localItem() { object argIndex1 = i; var ret = u.Item(argIndex1); return ret; }

        //                    FileSystem.WriteLine(SRC.SaveDataFileNumber, localItem().ID);
        //                }
        //            }
        //        }

        //        // ユニットリストにユニットの情報をロード
        //        // (リンクは後で行う)
        //        public void Load()
        //        {
        //            int num = default, num2 = default;
        //            Unit new_unit;
        //            var Name = default(string);
        //            // UPGRADE_NOTE: Status は Status_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        //            string ID = default, Status_Renamed = default;
        //            var Rank = default;
        //            int i, j;
        //            string dummy;
        //            FileSystem.Input(SRC.SaveDataFileNumber, IDCount);
        //            FileSystem.Input(SRC.SaveDataFileNumber, num);
        //            var loopTo = num;
        //            for (i = 1; i <= loopTo; i++)
        //            {
        //                new_unit = new Unit();
        //                // Name
        //                FileSystem.Input(SRC.SaveDataFileNumber, Name);
        //                bool localIsDefined() { object argIndex1 = Name; var ret = SRC.UDList.IsDefined(argIndex1); return ret; }

        //                if (!localIsDefined())
        //                {
        //                    GUI.ErrorMessage(Name + "のデータが定義されていません");
        //                    SRC.TerminateSRC();
        //                    Environment.Exit(0);
        //                }

        //                // ID, Rank, Status
        //                FileSystem.Input(SRC.SaveDataFileNumber, ID);
        //                FileSystem.Input(SRC.SaveDataFileNumber, Rank);
        //                FileSystem.Input(SRC.SaveDataFileNumber, Status_Renamed);

        //                // 旧形式のユニットＩＤを新形式に変換
        //                if (SRC.SaveDataVersion < 10700)
        //                {
        //                    SRC.ConvertUnitID(ID);
        //                }

        //                new_unit.Name = Name;
        //                new_unit.ID = ID;
        //                new_unit.Rank = Rank;
        //                new_unit.Party = "味方";
        //                new_unit.Status_Renamed = Status_Renamed;
        //                new_unit.FullRecover();
        //                colUnits.Add(new_unit, new_unit.ID);

        //                // OtherForm
        //                FileSystem.Input(SRC.SaveDataFileNumber, num2);
        //                var loopTo1 = num2;
        //                for (j = 1; j <= loopTo1; j++)
        //                    dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);

        //                // Pilot
        //                FileSystem.Input(SRC.SaveDataFileNumber, num2);
        //                var loopTo2 = num2;
        //                for (j = 1; j <= loopTo2; j++)
        //                    dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);

        //                // Support
        //                FileSystem.Input(SRC.SaveDataFileNumber, num2);
        //                var loopTo3 = num2;
        //                for (j = 1; j <= loopTo3; j++)
        //                    dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);

        //                // Item
        //                FileSystem.Input(SRC.SaveDataFileNumber, num2);
        //                var loopTo4 = num2;
        //                for (j = 1; j <= loopTo4; j++)
        //                    dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);
        //            }
        //        }

        //        // ユニットリストにユニットの情報をロードし、リンクを行う
        //        public void LoadLinkInfo()
        //        {
        //            int num = default, num2 = default;
        //            string ID = default, ID2 = default;
        //            int i, j;
        //            var int_dummy = default;
        //            string str_dummy;
        //            FileSystem.Input(SRC.SaveDataFileNumber, IDCount);
        //            FileSystem.Input(SRC.SaveDataFileNumber, num);
        //            var loopTo = num;
        //            for (i = 1; i <= loopTo; i++)
        //            {
        //                // Name
        //                str_dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);
        //                // ID, Rank, Status
        //                FileSystem.Input(SRC.SaveDataFileNumber, ID);
        //                FileSystem.Input(SRC.SaveDataFileNumber, int_dummy);
        //                FileSystem.Input(SRC.SaveDataFileNumber, str_dummy);

        //                // 旧形式のユニットＩＤを新形式に変換
        //                if (SRC.SaveDataVersion < 10700)
        //                {
        //                    SRC.ConvertUnitID(ID);
        //                }

        //                {
        //                    var withBlock = Item(ID);
        //                    // OtherForm
        //                    FileSystem.Input(SRC.SaveDataFileNumber, num2);
        //                    var loopTo1 = num2;
        //                    for (j = 1; j <= loopTo1; j++)
        //                    {
        //                        FileSystem.Input(SRC.SaveDataFileNumber, ID2);
        //                        SRC.ConvertUnitID(ID2);
        //                        if (IsDefined(ID2))
        //                        {
        //                            Unit localItem() { object argIndex1 = ID2; var ret = Item(argIndex1); return ret; }

        //                            withBlock.AddOtherForm(localItem());
        //                        }
        //                    }

        //                    // Pilot
        //                    FileSystem.Input(SRC.SaveDataFileNumber, num2);
        //                    var loopTo2 = num2;
        //                    for (j = 1; j <= loopTo2; j++)
        //                    {
        //                        FileSystem.Input(SRC.SaveDataFileNumber, ID2);
        //                        if (SRC.PList.IsDefined(ID2))
        //                        {
        //                            Pilot localItem1() { object argIndex1 = ID2; var ret = SRC.PList.Item(argIndex1); return ret; }

        //                            withBlock.AddPilot(localItem1());
        //                            if (withBlock.Status_Renamed == "離脱")
        //                            {
        //                                Pilot localItem2() { object argIndex1 = ID2; var ret = SRC.PList.Item(argIndex1); return ret; }

        //                                localItem2().Away = true;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            ID2 = Strings.Left(ID2, Strings.InStr(ID2, "(") - 1);
        //                            if (SRC.PList.IsDefined(ID2))
        //                            {
        //                                Pilot localItem3() { object argIndex1 = ID2; var ret = SRC.PList.Item(argIndex1); return ret; }

        //                                withBlock.AddPilot(localItem3());
        //                                if (withBlock.Status_Renamed == "離脱")
        //                                {
        //                                    Pilot localItem4() { object argIndex1 = ID2; var ret = SRC.PList.Item(argIndex1); return ret; }

        //                                    localItem4().Away = true;
        //                                }
        //                            }
        //                        }
        //                    }

        //                    // Support
        //                    FileSystem.Input(SRC.SaveDataFileNumber, num2);
        //                    var loopTo3 = num2;
        //                    for (j = 1; j <= loopTo3; j++)
        //                    {
        //                        FileSystem.Input(SRC.SaveDataFileNumber, ID2);
        //                        if (SRC.PList.IsDefined(ID2))
        //                        {
        //                            Pilot localItem5() { object argIndex1 = ID2; var ret = SRC.PList.Item(argIndex1); return ret; }

        //                            withBlock.AddSupport(localItem5());
        //                            if (withBlock.Status_Renamed == "離脱")
        //                            {
        //                                Pilot localItem6() { object argIndex1 = ID2; var ret = SRC.PList.Item(argIndex1); return ret; }

        //                                localItem6().Away = true;
        //                            }
        //                        }
        //                    }

        //                    // Unit
        //                    FileSystem.Input(SRC.SaveDataFileNumber, num2);
        //                    var loopTo4 = num2;
        //                    for (j = 1; j <= loopTo4; j++)
        //                    {
        //                        FileSystem.Input(SRC.SaveDataFileNumber, ID2);
        //                        bool localIsDefined() { object argIndex1 = ID2; var ret = SRC.IDList.IsDefined(argIndex1); return ret; }

        //                        if (SRC.IList.IsDefined(ID2))
        //                        {
        //                            Item localItem8() { object argIndex1 = ID2; var ret = SRC.IList.Item(argIndex1); return ret; }

        //                            if (localItem8().Unit is null)
        //                            {
        //                                Item localItem7() { object argIndex1 = ID2; var ret = SRC.IList.Item(argIndex1); return ret; }

        //                                withBlock.CurrentForm().AddItem0(localItem7());
        //                            }
        //                        }
        //                        else if (localIsDefined())
        //                        {
        //                            withBlock.CurrentForm().AddItem0(SRC.IList.Add(ID2));
        //                        }
        //                    }
        //                }
        //            }

        //            foreach (Unit u in colUnits)
        //                u.Update(true);
        //        }


        //        // 一時中断用データをファイルにセーブする
        //        public void Dump()
        //        {
        //            FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)Count());
        //            foreach (Unit u in colUnits)
        //                u.Dump();
        //        }

        //        // 一時中断用データをファイルからロードする
        //        public void Restore()
        //        {
        //            int i, num = default;
        //            Unit u;
        //            {
        //                var withBlock = colUnits;
        //                var loopTo = withBlock.Count;
        //                for (i = 1; i <= loopTo; i++)
        //                    withBlock.Remove(1);
        //            }

        //            FileSystem.Input(SRC.SaveDataFileNumber, num);
        //            var loopTo1 = num;
        //            for (i = 1; i <= loopTo1; i++)
        //            {
        //                u = new Unit();
        //                u.Restore();
        //                colUnits.Add(u, u.ID);
        //            }
        //        }

        //        // 一時中断用データのリンク情報をファイルからロードする
        //        public void RestoreLinkInfo()
        //        {
        //            var num = default;
        //            FileSystem.Input(SRC.SaveDataFileNumber, num);
        //            foreach (Unit u in colUnits)
        //                u.RestoreLinkInfo();
        //        }

        //        // 一時中断用データのパラメータ情報をファイルからロードする
        //        public void RestoreParameter()
        //        {
        //            var num = default;
        //            FileSystem.Input(SRC.SaveDataFileNumber, num);
        //            foreach (Unit u in colUnits)
        //                u.RestoreParameter();
        //        }


        // ユニットリストをクリア
        public void Clear()
        {
            while (Count() > 0)
            {
                Delete(Items.First().ID);
            }
        }

        //        // ユニットリストに登録されたユニットのビットマップIDをクリア
        //        public void ClearUnitBitmap()
        //        {

        //            {
        //                var withBlock = GUI.MainForm.picUnitBitmap;
        //                if (withBlock.Width == 32)
        //                {
        //                    // 既にクリアされていればそのまま終了
        //                    return;
        //                }

        //                // 画像をクリア
        //                withBlock.Picture = Image.FromFile("");
        //                withBlock.Move(0, 0, 32, 96);
        //            }

        //            // BitmapIDをクリア
        //            foreach (Unit u in colUnits)
        //                u.BitmapID = 0;
        //        }


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

        //        // 破棄されたユニットを削除
        //        public void Clean()
        //        {
        //            Unit u;
        //            int i;
        //            foreach (Unit currentU in colUnits)
        //            {
        //                u = currentU;
        //                // 出撃していないユニットは味方ユニット以外全て削除
        //                if (u.Party0 != "味方")
        //                {
        //                    if (u.Status_Renamed == "待機" | u.Status_Renamed == "破壊")
        //                    {
        //                        u.Status_Renamed = "破棄";
        //                        var loopTo = u.CountOtherForm();
        //                        for (i = 1; i <= loopTo; i++)
        //                        {
        //                            Unit localOtherForm() { object argIndex1 = i; var ret = u.OtherForm(argIndex1); return ret; }

        //                            localOtherForm().Status_Renamed = "破棄";
        //                        }
        //                    }
        //                }
        //            }

        //            foreach (Unit currentU1 in colUnits)
        //            {
        //                u = currentU1;
        //                // 破棄されたユニットを削除
        //                if (u.Status_Renamed == "破棄")
        //                {
        //                    // ユニットに乗っているパイロットも破棄
        //                    var loopTo1 = u.CountPilot();
        //                    for (i = 1; i <= loopTo1; i++)
        //                    {
        //                        Pilot localPilot() { object argIndex1 = i; var ret = u.Pilot(argIndex1); return ret; }

        //                        localPilot().Alive = false;
        //                    }

        //                    var loopTo2 = u.CountSupport();
        //                    for (i = 1; i <= loopTo2; i++)
        //                    {
        //                        Pilot localSupport() { object argIndex1 = i; var ret = u.Support(argIndex1); return ret; }

        //                        localSupport().Alive = false;
        //                    }

        //                    // ユニットが装備しているアイテムも破棄
        //                    var loopTo3 = u.CountItem();
        //                    for (i = 1; i <= loopTo3; i++)
        //                    {
        //                        Item localItem() { object argIndex1 = i; var ret = u.Item(argIndex1); return ret; }

        //                        localItem().Exist = false;
        //                    }

        //                    Delete(u.ID);
        //                }
        //            }
        //        }
    }
}
