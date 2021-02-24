// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Pilots
{
    // 作成されたパイロットのデータを管理するリストクラス
    public class Pilots
    {
        // パイロット一覧
        private SrcCollection<Pilot> colPilots = new SrcCollection<Pilot>();
        public IList<Pilot> Items => colPilots.List;
        public IEnumerable<Pilot> AlivePilots => Items.Where(x => x.Alive);

        protected SRC SRC { get; }
        private IGUI GUI => SRC.GUI;

        public Pilots(SRC src)
        {
            SRC = src;
        }

        // パイロットを追加
        public Pilot Add(string pname, int plevel, string pparty, string gid = null)
        {
            Pilot AddRet = default;
            string key;
            int i;

            var new_pilot = new Pilot(SRC, SRC.PDList.Item(pname));
            new_pilot.Level = plevel;
            new_pilot.Party = pparty;
            new_pilot.FullRecover();
            new_pilot.Alive = true;
            new_pilot.Unit = null;
            if (string.IsNullOrEmpty(gid))
            {
                // グループＩＤが指定されていない場合
                if (Strings.InStr(new_pilot.Name, "(ザコ)") == 0 & Strings.InStr(new_pilot.Name, "(汎用)") == 0)
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

                            string argmsg = key + "というパイロットは既に登録されています";
                            GUI.ErrorMessage(argmsg);
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
        public Pilot Item(string Index)
        {
            Pilot p = colPilots[Index];
            if (p?.Alive ?? false)
            {
                return p;
            }

            // ＩＤで見つからなければ名称で検索
            var pname = Index;
            foreach (Pilot currentP in AlivePilots)
            {
                p = currentP;
                if ((p.Data.Name ?? "") == (pname ?? ""))
                {
                    if (p.Alive)
                    {
                        return p;
                    }
                }
            }
            // それでも見つからなければ愛称で検索
            foreach (Pilot currentP in AlivePilots)
            {
                p = currentP;
                if ((p.Data.Nickname ?? "") == (pname ?? ""))
                {
                    if (p.Alive)
                    {
                        return p;
                    }
                }
            }

            return null;
        }

        // パイロットが定義されているか
        public bool IsDefined(string Index)
        {
            return Item(Index) != null;
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

        //        // リストのアップデート
        //        public void Update()
        //        {
        //            Pilot p;
        //            int i;
        //            foreach (Pilot currentP in colPilots)
        //            {
        //                p = currentP;
        //                if (p.Party != "味方" | !p.Alive)
        //                {
        //                    // 味方でないパイロットや破棄されたパイロットは削除
        //                    object argIndex1 = p.ID;
        //                    Delete(argIndex1);
        //                }
        //                else if (p.IsAdditionalPilot)
        //                {
        //                    // 追加パイロットは削除
        //                    if (p.Unit_Renamed is object)
        //                    {
        //                        {
        //                            var withBlock = p.Unit_Renamed;
        //                            // UPGRADE_NOTE: オブジェクト p.Unit.pltAdditionalPilot をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
        //                            withBlock.pltAdditionalPilot = null;
        //                            var loopTo = withBlock.CountOtherForm();
        //                            for (i = 1; i <= loopTo; i++)
        //                            {
        //                                // UPGRADE_NOTE: オブジェクト p.Unit.OtherForm().pltAdditionalPilot をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
        //                                Unit localOtherForm() { object argIndex1 = i; var ret = withBlock.OtherForm(argIndex1); return ret; }

        //                                localOtherForm().pltAdditionalPilot = null;
        //                            }
        //                        }
        //                    }

        //                    object argIndex2 = p.ID;
        //                    Delete(argIndex2);
        //                }
        //                else if (p.IsAdditionalSupport)
        //                {
        //                    // 追加サポートは削除
        //                    if (p.Unit_Renamed is object)
        //                    {
        //                        {
        //                            var withBlock1 = p.Unit_Renamed;
        //                            // UPGRADE_NOTE: オブジェクト p.Unit.pltAdditionalSupport をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
        //                            withBlock1.pltAdditionalSupport = null;
        //                            var loopTo1 = withBlock1.CountOtherForm();
        //                            for (i = 1; i <= loopTo1; i++)
        //                            {
        //                                // UPGRADE_NOTE: オブジェクト p.Unit.OtherForm().pltAdditionalSupport をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
        //                                Unit localOtherForm1() { object argIndex1 = i; var ret = withBlock1.OtherForm(argIndex1); return ret; }

        //                                localOtherForm1().pltAdditionalSupport = null;
        //                            }
        //                        }
        //                    }

        //                    object argIndex3 = p.ID;
        //                    Delete(argIndex3);
        //                }
        //                else if (p.Nickname0 == "パイロット不在")
        //                {
        //                    // ダミーパイロットは削除
        //                    object argIndex4 = p.ID;
        //                    Delete(argIndex4);
        //                }
        //                else if (p.Unit_Renamed is object)
        //                {
        //                    string argfname = "召喚ユニット";
        //                    if (p.Unit_Renamed.IsFeatureAvailable(argfname))
        //                    {
        //                        // 召喚ユニットの追加パイロットも削除
        //                        object argIndex5 = p.ID;
        //                        Delete(argIndex5);
        //                    }
        //                }
        //            }

        //            // 残ったパイロットを全回復
        //            foreach (Pilot currentP1 in colPilots)
        //            {
        //                p = currentP1;
        //                p.FullRecover();
        //            }
        //        }

        //        // ファイルにデータをセーブ
        //        public void Save()
        //        {
        //            var num = default;
        //            Pilot p;

        //            // 追加パイロットや追加サポートを除いたパイロット数を算出
        //            foreach (Pilot currentP in colPilots)
        //            {
        //                p = currentP;
        //                if (!p.IsAdditionalPilot & !p.IsAdditionalSupport)
        //                {
        //                    num = (num + 1);
        //                }
        //            }

        //            FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)num);
        //            foreach (Pilot currentP1 in colPilots)
        //            {
        //                p = currentP1;
        //                // 追加パイロットや追加サポートはセーブしない
        //                if (!p.IsAdditionalPilot & !p.IsAdditionalSupport)
        //                {
        //                    if ((p.Name ?? "") == (p.ID ?? ""))
        //                    {
        //                        FileSystem.WriteLine(SRC.SaveDataFileNumber, p.Name);
        //                    }
        //                    else
        //                    {
        //                        FileSystem.WriteLine(SRC.SaveDataFileNumber, p.Name + " " + p.ID);
        //                    }

        //                    FileSystem.WriteLine(SRC.SaveDataFileNumber, p.Level, p.Exp);
        //                    if (p.Unit_Renamed is null)
        //                    {
        //                        if (p.Away)
        //                        {
        //                            FileSystem.WriteLine(SRC.SaveDataFileNumber, "離脱");
        //                        }
        //                        else
        //                        {
        //                            FileSystem.WriteLine(SRC.SaveDataFileNumber, "-");
        //                        }
        //                    }
        //                    else
        //                    {
        //                        FileSystem.WriteLine(SRC.SaveDataFileNumber, p.Unit_Renamed.ID);
        //                    }
        //                }
        //            }
        //        }

        //        // ファイルからデータをロード (リンクは行わない)
        //        public void Load()
        //        {
        //            int i, num = default;
        //            var pname = default(string);
        //            int plevel = default, pexp = default;
        //            var dummy = default(string);
        //            FileSystem.Input(SRC.SaveDataFileNumber, num);
        //            var loopTo = num;
        //            for (i = 1; i <= loopTo; i++)
        //            {
        //                // Name
        //                FileSystem.Input(SRC.SaveDataFileNumber, pname);
        //                // Level, Exp
        //                FileSystem.Input(SRC.SaveDataFileNumber, plevel);
        //                FileSystem.Input(SRC.SaveDataFileNumber, pexp);
        //                // Unit
        //                FileSystem.Input(SRC.SaveDataFileNumber, dummy);
        //                if (GeneralLib.LLength(pname) == 1)
        //                {
        //                    bool localIsDefined1() { object argIndex1 = pname; var ret = SRC.PDList.IsDefined(argIndex1); return ret; }

        //                    if (!localIsDefined1())
        //                    {
        //                        if (Strings.InStr(pname, "(") > 0)
        //                        {
        //                            pname = Strings.Left(pname, Strings.InStr(pname, "(") - 1);
        //                        }

        //                        bool localIsDefined() { object argIndex1 = pname; var ret = SRC.PDList.IsDefined(argIndex1); return ret; }

        //                        if (!localIsDefined())
        //                        {
        //                            string argmsg = pname + "のデータが定義されていません";
        //                            GUI.ErrorMessage(argmsg);
        //                            SRC.TerminateSRC();
        //                            Environment.Exit(0);
        //                        }
        //                    }

        //                    string argpparty = "味方";
        //                    string arggid = "";
        //                    {
        //                        var withBlock = Add(pname, plevel, argpparty, gid: arggid);
        //                        withBlock.Exp = pexp;
        //                    }
        //                }
        //                else
        //                {
        //                    bool localIsDefined2() { object argIndex1 = GeneralLib.LIndex(pname, 1); var ret = SRC.PDList.IsDefined(argIndex1); return ret; }

        //                    if (!localIsDefined2())
        //                    {
        //                        string argmsg1 = GeneralLib.LIndex(pname, 1) + "のデータが定義されていません";
        //                        GUI.ErrorMessage(argmsg1);
        //                        SRC.TerminateSRC();
        //                        Environment.Exit(0);
        //                    }

        //                    string argpname = GeneralLib.LIndex(pname, 1);
        //                    string argpparty1 = "味方";
        //                    string arggid1 = GeneralLib.LIndex(pname, 2);
        //                    {
        //                        var withBlock1 = Add(argpname, plevel, argpparty1, arggid1);
        //                        withBlock1.Exp = pexp;
        //                    }
        //                }
        //            }
        //        }

        //        // ファイルからデータを読み込みリンク
        //        public void LoadLinkInfo()
        //        {
        //            int ret, i, num = default;
        //            string pname = default, uid = default;
        //            string dummy;
        //            Unit u;
        //            FileSystem.Input(SRC.SaveDataFileNumber, num);
        //            var loopTo = num;
        //            for (i = 1; i <= loopTo; i++)
        //            {
        //                // Name
        //                FileSystem.Input(SRC.SaveDataFileNumber, pname);
        //                // Level, Exp
        //                dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);
        //                // Unit
        //                FileSystem.Input(SRC.SaveDataFileNumber, uid);
        //                if (GeneralLib.LLength(pname) == 1)
        //                {
        //                    bool localIsDefined() { object argIndex1 = pname; var ret = IsDefined(argIndex1); return ret; }

        //                    if (!localIsDefined())
        //                    {
        //                        pname = Strings.Left(pname, Strings.InStr(pname, "(") - 1);
        //                    }
        //                }

        //                switch (uid ?? "")
        //                {
        //                    case "離脱":
        //                        {
        //                            // Leaveされたパイロット
        //                            if (GeneralLib.LLength(pname) == 1)
        //                            {
        //                                Pilot localItem() { object argIndex1 = pname; var ret = Item(argIndex1); return ret; }

        //                                localItem().Away = true;
        //                            }
        //                            else
        //                            {
        //                                Pilot localItem1() { object argIndex1 = GeneralLib.LIndex(pname, 2); var ret = Item(argIndex1); return ret; }

        //                                localItem1().Away = true;
        //                            }

        //                            goto NextPilot;
        //                            break;
        //                        }

        //                    case "-":
        //                    case "Dummy":
        //                        {
        //                            // ユニットに乗っていないパイロット
        //                            goto NextPilot;
        //                            break;
        //                        }
        //                }

        //                // 旧形式のユニットＩＤを新形式に変換
        //                if (SRC.SaveDataVersion < 10700)
        //                {
        //                    SRC.ConvertUnitID(uid);
        //                }

        //                object argIndex3 = uid;
        //                if (SRC.UList.IsDefined(argIndex3))
        //                {
        //                    // パイロットをユニットに乗せる
        //                    object argIndex1 = uid;
        //                    u = SRC.UList.Item(argIndex1);
        //                    if (GeneralLib.LLength(pname) == 1)
        //                    {
        //                        Pilot localItem2() { object argIndex1 = pname; var ret = Item(argIndex1); return ret; }

        //                        localItem2().Unit_Renamed = u;
        //                    }
        //                    else
        //                    {
        //                        Pilot localItem3() { object argIndex1 = GeneralLib.LIndex(pname, 2); var ret = Item(argIndex1); return ret; }

        //                        localItem3().Unit_Renamed = u;
        //                    }
        //                }
        //                else
        //                {
        //                    // 乗せるべきユニットが見つからなかった場合は強制的にユニットを
        //                    // 作って乗せる (バグ対策だったけど……不要？)
        //                    ret = Strings.InStr(uid, ":");
        //                    uid = Strings.Left(uid, ret - 1);
        //                    object argIndex2 = uid;
        //                    if (SRC.UDList.IsDefined(argIndex2))
        //                    {
        //                        string arguparty = "味方";
        //                        u = SRC.UList.Add(uid, 0, arguparty);
        //                        if (GeneralLib.LLength(pname) == 1)
        //                        {
        //                            Pilot localItem4() { object argIndex1 = pname; var ret = Item(argIndex1); return ret; }

        //                            localItem4().Ride(u);
        //                        }
        //                        else
        //                        {
        //                            Pilot localItem5() { object argIndex1 = GeneralLib.LIndex(pname, 2); var ret = Item(argIndex1); return ret; }

        //                            localItem5().Ride(u);
        //                        }
        //                    }
        //                }

        //            NextPilot:
        //                ;
        //            }
        //        }


        //        // 一時中断用データをセーブする
        //        public void Dump()
        //        {
        //            Pilot p;
        //            var num = default;

        //            // 追加パイロットを除いたパイロット数を算出
        //            foreach (Pilot currentP in colPilots)
        //            {
        //                p = currentP;
        //                if (!p.IsAdditionalPilot)
        //                {
        //                    num = (num + 1);
        //                }
        //            }

        //            FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)num);
        //            foreach (Pilot currentP1 in colPilots)
        //            {
        //                p = currentP1;
        //                // 追加パイロットはセーブしない
        //                if (!p.IsAdditionalPilot)
        //                {
        //                    p.Dump();
        //                }
        //            }
        //        }

        //        // 一時中断用データをファイルからロードする
        //        public void Restore()
        //        {
        //            int i, num = default;
        //            string buf;
        //            Pilot p;
        //            {
        //                var withBlock = colPilots;
        //                var loopTo = withBlock.Count;
        //                for (i = 1; i <= loopTo; i++)
        //                    withBlock.Remove(1);
        //            }

        //            FileSystem.Input(SRC.SaveDataFileNumber, num);
        //            var loopTo1 = num;
        //            for (i = 1; i <= loopTo1; i++)
        //            {
        //                p = new Pilot();
        //                p.Restore();
        //                colPilots.Add(p, p.ID);
        //            }
        //        }

        //        // 一時中断用データのリンク情報をファイルからロードする
        //        public void RestoreLinkInfo()
        //        {
        //            int i, num = default;
        //            FileSystem.Input(SRC.SaveDataFileNumber, num);
        //            var loopTo = num;
        //            for (i = 1; i <= loopTo; i++)
        //                // UPGRADE_WARNING: オブジェクト colPilots().RestoreLinkInfo の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //                colPilots[i].RestoreLinkInfo();
        //        }

        //        // 一時中断用データのパラメータ情報をファイルからロードする
        //        public void RestoreParameter()
        //        {
        //            int i, num = default;
        //            FileSystem.Input(SRC.SaveDataFileNumber, num);
        //            var loopTo = num;
        //            for (i = 1; i <= loopTo; i++)
        //                // UPGRADE_WARNING: オブジェクト colPilots().RestoreParameter の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
        //                colPilots[i].RestoreParameter();
        //        }

        //        // リストをクリア
        //        public void Clear()
        //        {
        //            int i;
        //            var loopTo = Count();
        //            for (i = 1; i <= loopTo; i++)
        //            {
        //                object argIndex1 = 1;
        //                Delete(argIndex1);
        //            }
        //        }


        // パイロットの支援修正を更新
        public void UpdateSupportMod(Unit u = null)
        {
            // TODO Impl
            //int xx, i, yy;
            //int max_range, range;
            //if (string.IsNullOrEmpty(Map.MapFileName))
            //{
            //    return;
            //}

            //// ユニット指定がなければ全パイロットを更新
            //if (u is null)
            //{
            //    foreach (Pilot p in colPilots)
            //        p.UpdateSupportMod();
            //    return;
            //}
            //// ユニットにパイロットが乗っていなければそのまま終了
            //if (u.CountPilot() == 0)
            //{
            //    return;
            //}

            //{
            //    var withBlock = u.MainPilot();
            //    // メインパイロットを更新
            //    withBlock.UpdateSupportMod();

            //    // 支援範囲を算出
            //    max_range = withBlock.CommandRange();
            //    string argsname = "広域サポート";
            //    if (withBlock.IsSkillAvailable(argsname))
            //    {
            //        max_range = GeneralLib.MaxLng(max_range, 2);
            //    }

            //    string argoname1 = "信頼補正";
            //    if (Expression.IsOptionDefined(argoname1) & Strings.InStr(withBlock.Name, "(ザコ)") == 0)
            //    {
            //        string argoname = "信頼補正範囲拡大";
            //        if (Expression.IsOptionDefined(argoname))
            //        {
            //            max_range = GeneralLib.MaxLng(max_range, 2);
            //        }
            //        else
            //        {
            //            max_range = GeneralLib.MaxLng(max_range, 1);
            //        }
            //    }
            //}

            //// 他のパイロットを更新
            //var loopTo = u.CountPilot();
            //for (i = 2; i <= loopTo; i++)
            //{
            //    Pilot localPilot() { object argIndex1 = i; var ret = u.Pilot(argIndex1); return ret; }

            //    localPilot().UpdateSupportMod();
            //}

            //var loopTo1 = u.CountSupport();
            //for (i = 1; i <= loopTo1; i++)
            //{
            //    Pilot localSupport() { object argIndex1 = i; var ret = u.Support(argIndex1); return ret; }

            //    localSupport().UpdateSupportMod();
            //}

            //// 支援範囲が無いなら他のユニットに乗っているパイロットには影響無し
            //if (max_range == 0)
            //{
            //    return;
            //}

            //// 周りのユニットに乗っているパイロットの支援修正を更新
            //var loopTo2 = GeneralLib.MinLng(u.x + max_range, Map.MapWidth);
            //for (xx = GeneralLib.MaxLng(u.x - max_range, 1); xx <= loopTo2; xx++)
            //{
            //    var loopTo3 = GeneralLib.MinLng(u.y + max_range, Map.MapHeight);
            //    for (yy = GeneralLib.MaxLng(u.y - max_range, 1); yy <= loopTo3; yy++)
            //    {
            //        if (Map.MapDataForUnit[xx, yy] is null)
            //        {
            //            goto NextPoint;
            //        }

            //        // 支援範囲内にいるかチェック
            //        range = (Math.Abs((u.x - xx)) + Math.Abs((u.y - yy)));
            //        if (range > max_range)
            //        {
            //            goto NextPoint;
            //        }

            //        if (range == 0)
            //        {
            //            goto NextPoint;
            //        }

            //        // 乗っているパイロット全員の支援修正を更新
            //        {
            //            var withBlock1 = Map.MapDataForUnit[xx, yy];
            //            if (withBlock1.CountPilot() == 0)
            //            {
            //                goto NextPoint;
            //            }

            //            withBlock1.MainPilot().UpdateSupportMod();
            //            var loopTo4 = withBlock1.CountPilot();
            //            for (i = 2; i <= loopTo4; i++)
            //            {
            //                Pilot localPilot1() { object argIndex1 = i; var ret = withBlock1.Pilot(argIndex1); return ret; }

            //                localPilot1().UpdateSupportMod();
            //            }

            //            var loopTo5 = withBlock1.CountSupport();
            //            for (i = 1; i <= loopTo5; i++)
            //            {
            //                Pilot localSupport1() { object argIndex1 = i; var ret = withBlock1.Support(argIndex1); return ret; }

            //                localSupport1().UpdateSupportMod();
            //            }
            //        }

            //    NextPoint:
            //        ;
            //    }
            //}
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