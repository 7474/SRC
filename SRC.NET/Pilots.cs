﻿using System;
using System.Collections;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    internal class Pilots : IEnumerable
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // 作成されたパイロットのデータを管理するリストクラス

        // パイロット一覧
        private Collection colPilots = new Collection();

        // クラスの解放
        // UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        private void Class_Terminate_Renamed()
        {
            short i;
            {
                var withBlock = colPilots;
                var loopTo = (short)withBlock.Count;
                for (i = 1; i <= loopTo; i++)
                    withBlock.Remove(1);
            }
            // UPGRADE_NOTE: オブジェクト colPilots をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            colPilots = null;
        }

        ~Pilots()
        {
            Class_Terminate_Renamed();
        }

        // ForEach用関数
        // UPGRADE_NOTE: NewEnum プロパティがコメント アウトされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3FC1610-34F3-43F5-86B7-16C984F0E88E"' をクリックしてください。
        // Public Function NewEnum() As stdole.IUnknown
        // NewEnum = colPilots.GetEnumerator
        // End Function

        public IEnumerator GetEnumerator()
        {
            return default;
            // UPGRADE_TODO: コレクション列挙子を返すには、コメントを解除して以下の行を変更してください。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="95F9AAD0-1319-4921-95F0-B9D3C4FF7F1C"' をクリックしてください。
            // GetEnumerator = colPilots.GetEnumerator
        }


        // パイロットを追加
        public Pilot Add(ref string pname, short plevel, ref string pparty, [Optional, DefaultParameterValue("")] ref string gid)
        {
            Pilot AddRet = default;
            var new_pilot = new Pilot();
            string key;
            short i;
            PilotData localItem() { object argIndex1 = pname; var ret = SRC.PDList.Item(ref argIndex1); return ret; }

            new_pilot.Name = localItem().Name;
            new_pilot.Level = plevel;
            new_pilot.Party = pparty;
            new_pilot.FullRecover();
            new_pilot.Alive = true;
            // UPGRADE_NOTE: オブジェクト new_pilot.Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            new_pilot.Unit_Renamed = null;
            if (string.IsNullOrEmpty(gid))
            {
                // グループＩＤが指定されていない場合
                if (Strings.InStr(new_pilot.Name, "(ザコ)") == 0 & Strings.InStr(new_pilot.Name, "(汎用)") == 0)
                {
                    key = new_pilot.Name;
                    object argIndex3 = key;
                    if (SRC.PList.IsDefined2(ref argIndex3))
                    {
                        Pilot localItem21() { object argIndex1 = key; var ret = SRC.PList.Item2(ref argIndex1); return ret; }

                        if ((localItem21().ID ?? "") == (key ?? ""))
                        {
                            // 一度作成されたパイロットを復活させる場合
                            Pilot localItem2() { object argIndex1 = key; var ret = SRC.PList.Item2(ref argIndex1); return ret; }

                            if (!localItem2().Alive)
                            {
                                object argIndex1 = key;
                                {
                                    var withBlock = SRC.PList.Item2(ref argIndex1);
                                    withBlock.Level = plevel;
                                    withBlock.Party = pparty;
                                    withBlock.FullRecover();
                                    withBlock.Alive = true;
                                }

                                object argIndex2 = key;
                                AddRet = SRC.PList.Item2(ref argIndex2);
                                return AddRet;
                            }

                            string argmsg = key + "というパイロットは既に登録されています";
                            GUI.ErrorMessage(ref argmsg);
                            return AddRet;
                        }
                    }
                }
                else
                {
                    i = (short)colPilots.Count;
                    object argIndex4 = key;
                    do
                    {
                        i = (short)(i + 1);
                        key = new_pilot.Name + "_" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(i);
                    }
                    while (SRC.PList.IsDefined2(ref argIndex4));
                }
            }
            else
            {
                // グループＩＤが指定されている場合
                key = gid;
                i = 1;
                object argIndex5 = key;
                while (SRC.PList.IsDefined2(ref argIndex5))
                {
                    i = (short)(i + 1);
                    key = gid + ":" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(i);
                }
            }

            new_pilot.ID = key;
            colPilots.Add(new_pilot, key);
            AddRet = new_pilot;
            return AddRet;
        }

        // 登録されているパイロットの総数
        public short Count()
        {
            short CountRet = default;
            CountRet = (short)colPilots.Count;
            return CountRet;
        }

        // リストからパイロットを削除
        public void Delete(ref object Index)
        {
            colPilots.Remove(Index);
        }

        // リストからパイロットを検索
        public Pilot Item(ref object Index)
        {
            Pilot ItemRet = default;
            Pilot p;
            string pname;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 4142


            Input:

                    On Error GoTo ErrorHandler

             */
            ItemRet = (Pilot)colPilots[Index];
            if (ItemRet.Alive)
            {
                return ItemRet;
            }

            ErrorHandler:
            ;

            // ＩＤで見つからなければ名称で検索
            // UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            pname = Conversions.ToString(Index);
            foreach (Pilot currentP in colPilots)
            {
                p = currentP;
                if ((p.Data.Name ?? "") == (pname ?? ""))
                {
                    if (p.Alive)
                    {
                        ItemRet = p;
                        return ItemRet;
                    }
                }
            }
            // それでも見つからなければ愛称で検索
            foreach (Pilot currentP1 in colPilots)
            {
                p = currentP1;
                if ((p.Data.Nickname ?? "") == (pname ?? ""))
                {
                    if (p.Alive)
                    {
                        ItemRet = p;
                        return ItemRet;
                    }
                }
            }

            return ItemRet;
        }

        // パイロットが定義されているか
        public bool IsDefined(ref object Index)
        {
            bool IsDefinedRet = default;
            Pilot p;
            string pname;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 4986


            Input:

                    On Error GoTo ErrorHandler

             */
            p = (Pilot)colPilots[Index];
            if (p.Alive)
            {
                IsDefinedRet = true;
                return IsDefinedRet;
            }

            ErrorHandler:
            ;

            // ＩＤで見つからなければ名称で検索
            // UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            pname = Conversions.ToString(Index);
            foreach (Pilot currentP in colPilots)
            {
                p = currentP;
                if ((p.Data.Name ?? "") == (pname ?? ""))
                {
                    if (p.Alive)
                    {
                        IsDefinedRet = true;
                        return IsDefinedRet;
                    }
                }
            }
            // それでも見つからなければ愛称で検索
            foreach (Pilot currentP1 in colPilots)
            {
                p = currentP1;
                if ((p.Data.Nickname ?? "") == (pname ?? ""))
                {
                    if (p.Alive)
                    {
                        IsDefinedRet = true;
                        return IsDefinedRet;
                    }
                }
            }

            return IsDefinedRet;
        }

        // パイロットを検索 (IDのみで検索)
        public Pilot Item2(ref object Index)
        {
            Pilot Item2Ret = default;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 5814


            Input:
                    On Error GoTo ErrorHandler

             */
            Item2Ret = (Pilot)colPilots[Index];
            return Item2Ret;
            ErrorHandler:
            ;

            // UPGRADE_NOTE: オブジェクト Item2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            Item2Ret = null;
        }

        // パイロットが定義されているか (IDのみで検索)
        public bool IsDefined2(ref object Index)
        {
            bool IsDefined2Ret = default;
            Pilot p;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 6274


            Input:

                    On Error GoTo ErrorHandler

             */
            p = (Pilot)colPilots[Index];
            IsDefined2Ret = true;
            return IsDefined2Ret;
            ErrorHandler:
            ;
            IsDefined2Ret = false;
        }

        // リストのアップデート
        public void Update()
        {
            Pilot p;
            short i;
            foreach (Pilot currentP in colPilots)
            {
                p = currentP;
                if (p.Party != "味方" | !p.Alive)
                {
                    // 味方でないパイロットや破棄されたパイロットは削除
                    object argIndex1 = p.ID;
                    Delete(ref argIndex1);
                }
                else if (p.IsAdditionalPilot)
                {
                    // 追加パイロットは削除
                    if (p.Unit_Renamed is object)
                    {
                        {
                            var withBlock = p.Unit_Renamed;
                            // UPGRADE_NOTE: オブジェクト p.Unit.pltAdditionalPilot をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                            withBlock.pltAdditionalPilot = null;
                            var loopTo = withBlock.CountOtherForm();
                            for (i = 1; i <= loopTo; i++)
                            {
                                // UPGRADE_NOTE: オブジェクト p.Unit.OtherForm().pltAdditionalPilot をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                                Unit localOtherForm() { object argIndex1 = i; var ret = withBlock.OtherForm(ref argIndex1); return ret; }

                                localOtherForm().pltAdditionalPilot = null;
                            }
                        }
                    }

                    object argIndex2 = p.ID;
                    Delete(ref argIndex2);
                }
                else if (p.IsAdditionalSupport)
                {
                    // 追加サポートは削除
                    if (p.Unit_Renamed is object)
                    {
                        {
                            var withBlock1 = p.Unit_Renamed;
                            // UPGRADE_NOTE: オブジェクト p.Unit.pltAdditionalSupport をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                            withBlock1.pltAdditionalSupport = null;
                            var loopTo1 = withBlock1.CountOtherForm();
                            for (i = 1; i <= loopTo1; i++)
                            {
                                // UPGRADE_NOTE: オブジェクト p.Unit.OtherForm().pltAdditionalSupport をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                                Unit localOtherForm1() { object argIndex1 = i; var ret = withBlock1.OtherForm(ref argIndex1); return ret; }

                                localOtherForm1().pltAdditionalSupport = null;
                            }
                        }
                    }

                    object argIndex3 = p.ID;
                    Delete(ref argIndex3);
                }
                else if (p.Nickname0 == "パイロット不在")
                {
                    // ダミーパイロットは削除
                    object argIndex4 = p.ID;
                    Delete(ref argIndex4);
                }
                else if (p.Unit_Renamed is object)
                {
                    string argfname = "召喚ユニット";
                    if (p.Unit_Renamed.IsFeatureAvailable(ref argfname))
                    {
                        // 召喚ユニットの追加パイロットも削除
                        object argIndex5 = p.ID;
                        Delete(ref argIndex5);
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


        // ファイルにデータをセーブ
        public void Save()
        {
            var num = default(short);
            Pilot p;

            // 追加パイロットや追加サポートを除いたパイロット数を算出
            foreach (Pilot currentP in colPilots)
            {
                p = currentP;
                if (!p.IsAdditionalPilot & !p.IsAdditionalSupport)
                {
                    num = (short)(num + 1);
                }
            }

            FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)num);
            foreach (Pilot currentP1 in colPilots)
            {
                p = currentP1;
                // 追加パイロットや追加サポートはセーブしない
                if (!p.IsAdditionalPilot & !p.IsAdditionalSupport)
                {
                    if ((p.Name ?? "") == (p.ID ?? ""))
                    {
                        FileSystem.WriteLine(SRC.SaveDataFileNumber, p.Name);
                    }
                    else
                    {
                        FileSystem.WriteLine(SRC.SaveDataFileNumber, p.Name + " " + p.ID);
                    }

                    FileSystem.WriteLine(SRC.SaveDataFileNumber, p.Level, p.Exp);
                    if (p.Unit_Renamed is null)
                    {
                        if (p.Away)
                        {
                            FileSystem.WriteLine(SRC.SaveDataFileNumber, "離脱");
                        }
                        else
                        {
                            FileSystem.WriteLine(SRC.SaveDataFileNumber, "-");
                        }
                    }
                    else
                    {
                        FileSystem.WriteLine(SRC.SaveDataFileNumber, p.Unit_Renamed.ID);
                    }
                }
            }
        }

        // ファイルからデータをロード (リンクは行わない)
        public void Load()
        {
            short i, num = default;
            var pname = default(string);
            short plevel = default, pexp = default;
            var dummy = default(string);
            FileSystem.Input(SRC.SaveDataFileNumber, ref num);
            var loopTo = num;
            for (i = 1; i <= loopTo; i++)
            {
                // Name
                FileSystem.Input(SRC.SaveDataFileNumber, ref pname);
                // Level, Exp
                FileSystem.Input(SRC.SaveDataFileNumber, ref plevel);
                FileSystem.Input(SRC.SaveDataFileNumber, ref pexp);
                // Unit
                FileSystem.Input(SRC.SaveDataFileNumber, ref dummy);
                if (GeneralLib.LLength(ref pname) == 1)
                {
                    bool localIsDefined1() { object argIndex1 = pname; var ret = SRC.PDList.IsDefined(ref argIndex1); return ret; }

                    if (!localIsDefined1())
                    {
                        if (Strings.InStr(pname, "(") > 0)
                        {
                            pname = Strings.Left(pname, Strings.InStr(pname, "(") - 1);
                        }

                        bool localIsDefined() { object argIndex1 = pname; var ret = SRC.PDList.IsDefined(ref argIndex1); return ret; }

                        if (!localIsDefined())
                        {
                            string argmsg = pname + "のデータが定義されていません";
                            GUI.ErrorMessage(ref argmsg);
                            SRC.TerminateSRC();
                            Environment.Exit(0);
                        }
                    }

                    string argpparty = "味方";
                    string arggid = "";
                    {
                        var withBlock = Add(ref pname, plevel, ref argpparty, gid: ref arggid);
                        withBlock.Exp = pexp;
                    }
                }
                else
                {
                    bool localIsDefined2() { object argIndex1 = GeneralLib.LIndex(ref pname, 1); var ret = SRC.PDList.IsDefined(ref argIndex1); return ret; }

                    if (!localIsDefined2())
                    {
                        string argmsg1 = GeneralLib.LIndex(ref pname, 1) + "のデータが定義されていません";
                        GUI.ErrorMessage(ref argmsg1);
                        SRC.TerminateSRC();
                        Environment.Exit(0);
                    }

                    string argpname = GeneralLib.LIndex(ref pname, 1);
                    string argpparty1 = "味方";
                    string arggid1 = GeneralLib.LIndex(ref pname, 2);
                    {
                        var withBlock1 = Add(ref argpname, plevel, ref argpparty1, ref arggid1);
                        withBlock1.Exp = pexp;
                    }
                }
            }
        }

        // ファイルからデータを読み込みリンク
        public void LoadLinkInfo()
        {
            short ret, i, num = default;
            string pname = default, uid = default;
            string dummy;
            Unit u;
            FileSystem.Input(SRC.SaveDataFileNumber, ref num);
            var loopTo = num;
            for (i = 1; i <= loopTo; i++)
            {
                // Name
                FileSystem.Input(SRC.SaveDataFileNumber, ref pname);
                // Level, Exp
                dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);
                // Unit
                FileSystem.Input(SRC.SaveDataFileNumber, ref uid);
                if (GeneralLib.LLength(ref pname) == 1)
                {
                    bool localIsDefined() { object argIndex1 = pname; var ret = IsDefined(ref argIndex1); return ret; }

                    if (!localIsDefined())
                    {
                        pname = Strings.Left(pname, Strings.InStr(pname, "(") - 1);
                    }
                }

                switch (uid ?? "")
                {
                    case "離脱":
                        {
                            // Leaveされたパイロット
                            if (GeneralLib.LLength(ref pname) == 1)
                            {
                                Pilot localItem() { object argIndex1 = pname; var ret = Item(ref argIndex1); return ret; }

                                localItem().Away = true;
                            }
                            else
                            {
                                Pilot localItem1() { object argIndex1 = GeneralLib.LIndex(ref pname, 2); var ret = Item(ref argIndex1); return ret; }

                                localItem1().Away = true;
                            }

                            goto NextPilot;
                            break;
                        }

                    case "-":
                    case "Dummy":
                        {
                            // ユニットに乗っていないパイロット
                            goto NextPilot;
                            break;
                        }
                }

                // 旧形式のユニットＩＤを新形式に変換
                if (SRC.SaveDataVersion < 10700)
                {
                    SRC.ConvertUnitID(ref uid);
                }

                object argIndex3 = uid;
                if (SRC.UList.IsDefined(ref argIndex3))
                {
                    // パイロットをユニットに乗せる
                    object argIndex1 = uid;
                    u = SRC.UList.Item(ref argIndex1);
                    if (GeneralLib.LLength(ref pname) == 1)
                    {
                        Pilot localItem2() { object argIndex1 = pname; var ret = Item(ref argIndex1); return ret; }

                        localItem2().Unit_Renamed = u;
                    }
                    else
                    {
                        Pilot localItem3() { object argIndex1 = GeneralLib.LIndex(ref pname, 2); var ret = Item(ref argIndex1); return ret; }

                        localItem3().Unit_Renamed = u;
                    }
                }
                else
                {
                    // 乗せるべきユニットが見つからなかった場合は強制的にユニットを
                    // 作って乗せる (バグ対策だったけど……不要？)
                    ret = (short)Strings.InStr(uid, ":");
                    uid = Strings.Left(uid, ret - 1);
                    object argIndex2 = uid;
                    if (SRC.UDList.IsDefined(ref argIndex2))
                    {
                        string arguparty = "味方";
                        u = SRC.UList.Add(ref uid, 0, ref arguparty);
                        if (GeneralLib.LLength(ref pname) == 1)
                        {
                            Pilot localItem4() { object argIndex1 = pname; var ret = Item(ref argIndex1); return ret; }

                            localItem4().Ride(ref u);
                        }
                        else
                        {
                            Pilot localItem5() { object argIndex1 = GeneralLib.LIndex(ref pname, 2); var ret = Item(ref argIndex1); return ret; }

                            localItem5().Ride(ref u);
                        }
                    }
                }

                NextPilot:
                ;
            }
        }


        // 一時中断用データをセーブする
        public void Dump()
        {
            Pilot p;
            var num = default(short);

            // 追加パイロットを除いたパイロット数を算出
            foreach (Pilot currentP in colPilots)
            {
                p = currentP;
                if (!p.IsAdditionalPilot)
                {
                    num = (short)(num + 1);
                }
            }

            FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)num);
            foreach (Pilot currentP1 in colPilots)
            {
                p = currentP1;
                // 追加パイロットはセーブしない
                if (!p.IsAdditionalPilot)
                {
                    p.Dump();
                }
            }
        }

        // 一時中断用データをファイルからロードする
        public void Restore()
        {
            short i, num = default;
            string buf;
            Pilot p;
            {
                var withBlock = colPilots;
                var loopTo = (short)withBlock.Count;
                for (i = 1; i <= loopTo; i++)
                    withBlock.Remove(1);
            }

            FileSystem.Input(SRC.SaveDataFileNumber, ref num);
            var loopTo1 = num;
            for (i = 1; i <= loopTo1; i++)
            {
                p = new Pilot();
                p.Restore();
                colPilots.Add(p, p.ID);
            }
        }

        // 一時中断用データのリンク情報をファイルからロードする
        public void RestoreLinkInfo()
        {
            short i, num = default;
            FileSystem.Input(SRC.SaveDataFileNumber, ref num);
            var loopTo = num;
            for (i = 1; i <= loopTo; i++)
                // UPGRADE_WARNING: オブジェクト colPilots().RestoreLinkInfo の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                colPilots[i].RestoreLinkInfo();
        }

        // 一時中断用データのパラメータ情報をファイルからロードする
        public void RestoreParameter()
        {
            short i, num = default;
            FileSystem.Input(SRC.SaveDataFileNumber, ref num);
            var loopTo = num;
            for (i = 1; i <= loopTo; i++)
                // UPGRADE_WARNING: オブジェクト colPilots().RestoreParameter の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                colPilots[i].RestoreParameter();
        }


        // リストをクリア
        public void Clear()
        {
            short i;
            var loopTo = Count();
            for (i = 1; i <= loopTo; i++)
            {
                object argIndex1 = 1;
                Delete(ref argIndex1);
            }
        }


        // パイロットの支援修正を更新
        public void UpdateSupportMod(Unit u = null)
        {
            short xx, i, yy;
            short max_range, range;
            if (string.IsNullOrEmpty(Map.MapFileName))
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

            {
                var withBlock = u.MainPilot();
                // メインパイロットを更新
                withBlock.UpdateSupportMod();

                // 支援範囲を算出
                max_range = withBlock.CommandRange();
                string argsname = "広域サポート";
                if (withBlock.IsSkillAvailable(ref argsname))
                {
                    max_range = (short)GeneralLib.MaxLng(max_range, 2);
                }

                string argoname1 = "信頼補正";
                if (Expression.IsOptionDefined(ref argoname1) & Strings.InStr(withBlock.Name, "(ザコ)") == 0)
                {
                    string argoname = "信頼補正範囲拡大";
                    if (Expression.IsOptionDefined(ref argoname))
                    {
                        max_range = (short)GeneralLib.MaxLng(max_range, 2);
                    }
                    else
                    {
                        max_range = (short)GeneralLib.MaxLng(max_range, 1);
                    }
                }
            }

            // 他のパイロットを更新
            var loopTo = u.CountPilot();
            for (i = 2; i <= loopTo; i++)
            {
                Pilot localPilot() { object argIndex1 = i; var ret = u.Pilot(ref argIndex1); return ret; }

                localPilot().UpdateSupportMod();
            }

            var loopTo1 = u.CountSupport();
            for (i = 1; i <= loopTo1; i++)
            {
                Pilot localSupport() { object argIndex1 = i; var ret = u.Support(ref argIndex1); return ret; }

                localSupport().UpdateSupportMod();
            }

            // 支援範囲が無いなら他のユニットに乗っているパイロットには影響無し
            if (max_range == 0)
            {
                return;
            }

            // 周りのユニットに乗っているパイロットの支援修正を更新
            var loopTo2 = (short)GeneralLib.MinLng(u.x + max_range, Map.MapWidth);
            for (xx = (short)GeneralLib.MaxLng(u.x - max_range, 1); xx <= loopTo2; xx++)
            {
                var loopTo3 = (short)GeneralLib.MinLng(u.y + max_range, Map.MapHeight);
                for (yy = (short)GeneralLib.MaxLng(u.y - max_range, 1); yy <= loopTo3; yy++)
                {
                    if (Map.MapDataForUnit[xx, yy] is null)
                    {
                        goto NextPoint;
                    }

                    // 支援範囲内にいるかチェック
                    range = (short)(Math.Abs((short)(u.x - xx)) + Math.Abs((short)(u.y - yy)));
                    if (range > max_range)
                    {
                        goto NextPoint;
                    }

                    if (range == 0)
                    {
                        goto NextPoint;
                    }

                    // 乗っているパイロット全員の支援修正を更新
                    {
                        var withBlock1 = Map.MapDataForUnit[xx, yy];
                        if (withBlock1.CountPilot() == 0)
                        {
                            goto NextPoint;
                        }

                        withBlock1.MainPilot().UpdateSupportMod();
                        var loopTo4 = withBlock1.CountPilot();
                        for (i = 2; i <= loopTo4; i++)
                        {
                            Pilot localPilot1() { object argIndex1 = i; var ret = withBlock1.Pilot(ref argIndex1); return ret; }

                            localPilot1().UpdateSupportMod();
                        }

                        var loopTo5 = withBlock1.CountSupport();
                        for (i = 1; i <= loopTo5; i++)
                        {
                            Pilot localSupport1() { object argIndex1 = i; var ret = withBlock1.Support(ref argIndex1); return ret; }

                            localSupport1().UpdateSupportMod();
                        }
                    }

                    NextPoint:
                    ;
                }
            }
        }

        // 破棄されたパイロットを削除する
        public void Clean()
        {
            foreach (Pilot p in colPilots)
            {
                if (!p.Alive)
                {
                    // 破棄されたパイロットを削除
                    object argIndex1 = p.ID;
                    Delete(ref argIndex1);
                }
            }
        }
    }
}