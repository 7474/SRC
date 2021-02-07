// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.VB;
using System;
using System.Collections;

namespace SRCCore.Units
{
    // 全ユニットのデータを管理するリストのクラス
    public partial class Units : IEnumerable
    {
        // ユニットＩＤ作成用カウンタ
        public int IDCount = 0;

        // ユニット一覧
        private SrcCollection<Unit> colUnits = new SrcCollection<Unit>();

        protected SRC SRC { get; }
        private IGUI GUI => SRC.GUI;

        public Units(SRC src)
        {
            SRC = src;
        }

        public IEnumerator GetEnumerator()
        {
            return colUnits.Values.GetEnumerator();
        }

//        // ユニットリストに新しいユニットを追加
//        public Unit Add(ref string uname, short urank, ref string uparty)
//        {
//            Unit AddRet = default;
//            Unit new_unit;
//            Unit new_form;
//            UnitData ud;
//            string uname2;
//            string[] other_forms;
//            short i, j;
//            string list;

//            // ユニットデータが定義されている？
//            bool localIsDefined() { object argIndex1 = uname; var ret = SRC.UDList.IsDefined(ref argIndex1); return ret; }

//            if (!localIsDefined())
//            {
//                // UPGRADE_NOTE: オブジェクト Add をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
//                AddRet = null;
//                return AddRet;
//            }

//            object argIndex1 = uname;
//            ud = SRC.UDList.Item(ref argIndex1);
//            new_unit = new Unit();
//            AddRet = new_unit;
//            {
//                var withBlock = new_unit;
//                withBlock.Name = ud.Name;
//                withBlock.Rank = urank;
//                withBlock.Party = uparty;
//                withBlock.ID = CreateID(ref ud.Name);
//                withBlock.FullRecover();
//            }

//            colUnits.Add(new_unit, new_unit.ID);

//            // これ以降は本体以外の形態の追加
//            other_forms = new string[1];

//            // 変形先の形態
//            object argIndex2 = "変形";
//            list = ud.FeatureData(ref argIndex2);
//            var loopTo = GeneralLib.LLength(ref list);
//            for (i = 2; i <= loopTo; i++)
//            {
//                uname2 = GeneralLib.LIndex(ref list, i);
//                bool localIsDefined1() { object argIndex1 = uname2; var ret = SRC.UDList.IsDefined(ref argIndex1); return ret; }

//                if (!localIsDefined1())
//                {
//                    string argmsg = "ユニットデータ「" + uname + "」の変形先形態「" + uname2 + "」が見つかりません";
//                    GUI.ErrorMessage(ref argmsg);
//                    // UPGRADE_NOTE: オブジェクト Add をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
//                    AddRet = null;
//                    return AddRet;
//                }

//                Array.Resize(ref other_forms, Information.UBound(other_forms) + 1 + 1);
//                other_forms[Information.UBound(other_forms)] = uname2;
//            }

//            // ハイパーモード先の形態
//            string argfname = "ハイパーモード";
//            if (ud.IsFeatureAvailable(ref argfname))
//            {
//                object argIndex3 = "ハイパーモード";
//                list = ud.FeatureData(ref argIndex3);
//                uname2 = GeneralLib.LIndex(ref list, 2);
//                bool localIsDefined2() { object argIndex1 = uname2; var ret = SRC.UDList.IsDefined(ref argIndex1); return ret; }

//                if (!localIsDefined2())
//                {
//                    if (string.IsNullOrEmpty(uname))
//                    {
//                        string argmsg1 = "ユニットデータ「" + uname + "」のハイパーモード先形態が指定されていません";
//                        GUI.ErrorMessage(ref argmsg1);
//                    }
//                    else
//                    {
//                        string argmsg2 = "ユニットデータ「" + uname + "」のハイパーモード先形態「" + uname2 + "」が見つかりません";
//                        GUI.ErrorMessage(ref argmsg2);
//                    }
//                    // UPGRADE_NOTE: オブジェクト Add をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
//                    AddRet = null;
//                    return AddRet;
//                }

//                Array.Resize(ref other_forms, Information.UBound(other_forms) + 1 + 1);
//                other_forms[Information.UBound(other_forms)] = uname2;
//            }

//            // ノーマルモード先の形態
//            string argfname1 = "ノーマルモード";
//            if (ud.IsFeatureAvailable(ref argfname1))
//            {
//                object argIndex4 = "ノーマルモード";
//                list = ud.FeatureData(ref argIndex4);
//                uname2 = GeneralLib.LIndex(ref list, 1);
//                bool localIsDefined3() { object argIndex1 = uname2; var ret = SRC.UDList.IsDefined(ref argIndex1); return ret; }

//                if (!localIsDefined3())
//                {
//                    if (string.IsNullOrEmpty(uname2))
//                    {
//                        string argmsg3 = "ユニットデータ「" + uname + "」のノーマルモード先形態が指定されていません";
//                        GUI.ErrorMessage(ref argmsg3);
//                    }
//                    else
//                    {
//                        string argmsg4 = "ユニットデータ「" + uname + "」のノーマルモード先形態「" + uname2 + "」が見つかりません";
//                        GUI.ErrorMessage(ref argmsg4);
//                    }
//                    // UPGRADE_NOTE: オブジェクト Add をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
//                    AddRet = null;
//                    return AddRet;
//                }

//                Array.Resize(ref other_forms, Information.UBound(other_forms) + 1 + 1);
//                other_forms[Information.UBound(other_forms)] = uname2;
//            }

//            // パーツ分離先の形態
//            string argfname2 = "パーツ分離";
//            if (ud.IsFeatureAvailable(ref argfname2))
//            {
//                object argIndex5 = "パーツ分離";
//                string arglist = ud.FeatureData(ref argIndex5);
//                uname2 = GeneralLib.LIndex(ref arglist, 2);
//                bool localIsDefined4() { object argIndex1 = uname2; var ret = SRC.UDList.IsDefined(ref argIndex1); return ret; }

//                if (!localIsDefined4())
//                {
//                    if (string.IsNullOrEmpty(uname2))
//                    {
//                        string argmsg5 = "ユニットデータ「" + uname + "」のパーツ分離先形態が指定されていません";
//                        GUI.ErrorMessage(ref argmsg5);
//                    }
//                    else
//                    {
//                        string argmsg6 = "ユニットデータ「" + uname + "」のパーツ分離先形態「" + uname2 + "」が見つかりません";
//                        GUI.ErrorMessage(ref argmsg6);
//                    }
//                    // UPGRADE_NOTE: オブジェクト Add をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
//                    AddRet = null;
//                    return AddRet;
//                }

//                Array.Resize(ref other_forms, Information.UBound(other_forms) + 1 + 1);
//                other_forms[Information.UBound(other_forms)] = uname2;
//            }

//            // パーツ合体先の形態
//            string argfname3 = "パーツ合体";
//            if (ud.IsFeatureAvailable(ref argfname3))
//            {
//                object argIndex6 = "パーツ合体";
//                uname2 = ud.FeatureData(ref argIndex6);
//                bool localIsDefined5() { object argIndex1 = uname2; var ret = SRC.UDList.IsDefined(ref argIndex1); return ret; }

//                if (!localIsDefined5())
//                {
//                    if (string.IsNullOrEmpty(uname))
//                    {
//                        string argmsg7 = "ユニットデータ「" + uname + "」のパーツ合体先形態が指定されていません";
//                        GUI.ErrorMessage(ref argmsg7);
//                    }
//                    else
//                    {
//                        string argmsg8 = "ユニットデータ「" + uname + "」のパーツ合体先形態「" + uname2 + "」が見つかりません";
//                        GUI.ErrorMessage(ref argmsg8);
//                    }
//                    // UPGRADE_NOTE: オブジェクト Add をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
//                    AddRet = null;
//                    return AddRet;
//                }

//                Array.Resize(ref other_forms, Information.UBound(other_forms) + 1 + 1);
//                other_forms[Information.UBound(other_forms)] = uname2;
//            }

//            // 変形技の変形先の形態
//            string argfname4 = "変形技";
//            if (ud.IsFeatureAvailable(ref argfname4))
//            {
//                var loopTo1 = ud.CountFeature();
//                for (i = 1; i <= loopTo1; i++)
//                {
//                    object argIndex7 = i;
//                    if (ud.Feature(ref argIndex7) == "変形技")
//                    {
//                        string localFeatureData() { object argIndex1 = i; var ret = ud.FeatureData(ref argIndex1); return ret; }

//                        string arglist1 = localFeatureData();
//                        uname2 = GeneralLib.LIndex(ref arglist1, 2);
//                        bool localIsDefined6() { object argIndex1 = uname2; var ret = SRC.UDList.IsDefined(ref argIndex1); return ret; }

//                        if (!localIsDefined6())
//                        {
//                            if (string.IsNullOrEmpty(uname2))
//                            {
//                                string argmsg9 = "ユニットデータ「" + uname + "」の変形技使用後形態が指定されていません";
//                                GUI.ErrorMessage(ref argmsg9);
//                            }
//                            else
//                            {
//                                string argmsg10 = "ユニットデータ「" + uname + "」の変形技使用後形態「" + uname2 + "」が見つかりません";
//                                GUI.ErrorMessage(ref argmsg10);
//                            }
//                            // UPGRADE_NOTE: オブジェクト Add をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
//                            AddRet = null;
//                            return AddRet;
//                        }

//                        Array.Resize(ref other_forms, Information.UBound(other_forms) + 1 + 1);
//                        other_forms[Information.UBound(other_forms)] = uname2;
//                    }
//                }
//            }

//            // 換装先の形態
//            object argIndex8 = "換装";
//            list = ud.FeatureData(ref argIndex8);
//            var loopTo2 = GeneralLib.LLength(ref list);
//            for (i = 1; i <= loopTo2; i++)
//            {
//                uname2 = GeneralLib.LIndex(ref list, i);
//                bool localIsDefined7() { object argIndex1 = uname2; var ret = SRC.UDList.IsDefined(ref argIndex1); return ret; }

//                if (!localIsDefined7())
//                {
//                    string argmsg11 = "ユニットデータ「" + uname + "」の換装先形態「" + uname2 + "」が見つかりません";
//                    GUI.ErrorMessage(ref argmsg11);
//                    // UPGRADE_NOTE: オブジェクト Add をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
//                    AddRet = null;
//                    return AddRet;
//                }

//                Array.Resize(ref other_forms, Information.UBound(other_forms) + 1 + 1);
//                other_forms[Information.UBound(other_forms)] = uname2;
//            }

//            // 他形態で指定された形態
//            object argIndex9 = "他形態";
//            list = ud.FeatureData(ref argIndex9);
//            var loopTo3 = GeneralLib.LLength(ref list);
//            for (i = 1; i <= loopTo3; i++)
//            {
//                uname2 = GeneralLib.LIndex(ref list, i);
//                bool localIsDefined8() { object argIndex1 = uname2; var ret = SRC.UDList.IsDefined(ref argIndex1); return ret; }

//                if (!localIsDefined8())
//                {
//                    string argmsg12 = "ユニットデータ「" + uname + "」の他形態「" + uname2 + "」が見つかりません";
//                    GUI.ErrorMessage(ref argmsg12);
//                    // UPGRADE_NOTE: オブジェクト Add をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
//                    AddRet = null;
//                    return AddRet;
//                }

//                Array.Resize(ref other_forms, Information.UBound(other_forms) + 1 + 1);
//                other_forms[Information.UBound(other_forms)] = uname2;
//            }

//            // 形態を追加
//            var loopTo4 = (short)Information.UBound(other_forms);
//            for (i = 1; i <= loopTo4; i++)
//            {
//                if (!new_unit.IsOtherFormDefined(ref other_forms[i]))
//                {
//                    new_form = new Unit();
//                    {
//                        var withBlock1 = new_form;
//                        withBlock1.Name = other_forms[i];
//                        withBlock1.Rank = urank;
//                        withBlock1.Party = uparty;
//                        withBlock1.ID = CreateID(ref ud.Name);
//                        withBlock1.FullRecover();
//                        withBlock1.Status_Renamed = "他形態";
//                    }

//                    colUnits.Add(new_form, new_form.ID);
//                    new_unit.AddOtherForm(ref new_form);
//                }
//            }

//            // 追加した形態に対して自分自身を追加しておく
//            var loopTo5 = new_unit.CountOtherForm();
//            for (i = 1; i <= loopTo5; i++)
//            {
//                Unit localOtherForm() { object argIndex1 = i; var ret = new_unit.OtherForm(ref argIndex1); return ret; }

//                localOtherForm().AddOtherForm(ref new_unit);
//                var loopTo6 = new_unit.CountOtherForm();
//                for (j = 1; j <= loopTo6; j++)
//                {
//                    if (!(i == j))
//                    {
//                        Unit localOtherForm1() { object argIndex1 = i; var ret = new_unit.OtherForm(ref argIndex1); return ret; }

//                        Unit localOtherForm2() { object argIndex1 = j; var ret = new_unit.OtherForm(ref argIndex1); return ret; }

//                        var argu = localOtherForm2();
//                        localOtherForm1().AddOtherForm(ref argu);
//                    }
//                }
//            }

//            // 既に合体先 or 分離先のユニットが作成されていれば自分は他形態
//            var loopTo7 = ud.CountFeature();
//            for (i = 1; i <= loopTo7; i++)
//            {
//                object argIndex11 = i;
//                if (ud.Feature(ref argIndex11) == "合体")
//                {
//                    string localFeatureData1() { object argIndex1 = i; var ret = ud.FeatureData(ref argIndex1); return ret; }

//                    string localLIndex() { string arglist = hsac6574ea324b48e3a31bae276fa1258d(); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

//                    object argIndex10 = localLIndex();
//                    if (SRC.UList.IsDefined(ref argIndex10))
//                    {
//                        new_unit.Status_Renamed = "他形態";
//                        return AddRet;
//                    }
//                }

//                object argIndex13 = i;
//                if (ud.Feature(ref argIndex13) == "分離")
//                {
//                    string localFeatureData3() { object argIndex1 = i; var ret = ud.FeatureData(ref argIndex1); return ret; }

//                    string arglist2 = localFeatureData3();
//                    var loopTo8 = GeneralLib.LLength(ref arglist2);
//                    for (j = 2; j <= loopTo8; j++)
//                    {
//                        string localFeatureData2() { object argIndex1 = i; var ret = ud.FeatureData(ref argIndex1); return ret; }

//                        string localLIndex1() { string arglist = hs68135e533f0745c1aa8b425ec504a876(); var ret = GeneralLib.LIndex(ref arglist, j); return ret; }

//                        object argIndex12 = localLIndex1();
//                        if (SRC.UList.IsDefined(ref argIndex12))
//                        {
//                            new_unit.Status_Renamed = "他形態";
//                            return AddRet;
//                        }
//                    }
//                }
//            }

//            return AddRet;
//        }

//        // ユニットリストにユニット u を追加
//        public void Add2(ref Unit u)
//        {
//            colUnits.Add(u, u.ID);
//        }

//        // 新規ユニットIDを作成
//        public string CreateID(ref string uname)
//        {
//            string CreateIDRet = default;
//            bool localIsDefined() { object argIndex1 = (object)(uname + ":" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(IDCount)); var ret = IsDefined(ref argIndex1); return ret; }

//            do
//                IDCount = IDCount + 1;
//            while (localIsDefined());
//            CreateIDRet = uname + ":" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(IDCount);
//            return CreateIDRet;
//        }

//        // ユニットリストに登録されているユニット数を返す
//        public short Count()
//        {
//            short CountRet = default;
//            CountRet = (short)colUnits.Count;
//            return CountRet;
//        }

//        // ユニットリストからユニットを削除
//        public void Delete(ref object Index)
//        {
//            colUnits.Remove(Index);
//        }

//        // ユニットリストから指定されたユニットを返す
//        public Unit Item(ref object Index)
//        {
//            Unit ItemRet = default;
//            string uname;
//            ;
//#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
//            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 12097


//            Input:

//                    On Error GoTo ErrorHandler

//             */
//            ItemRet = (Unit)colUnits[Index];
//            return ItemRet;
//        ErrorHandler:
//            ;

//            // IDで見つからなければユニット名で検索
//            // UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
//            uname = Conversions.ToString(Index);
//            foreach (Unit u in colUnits)
//            {
//                if ((u.Name ?? "") == (uname ?? ""))
//                {
//                    if (u.Status_Renamed != "破棄")
//                    {
//                        ItemRet = u;
//                        return ItemRet;
//                    }
//                }
//            }
//            // UPGRADE_NOTE: オブジェクト Item をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
//            ItemRet = null;
//        }

//        // ユニットリストからユニットを検索 (IDのみ)
//        public Unit Item2(ref object Index)
//        {
//            Unit Item2Ret = default;
//            ;
//#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
//            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 12937


//            Input:
//                    On Error GoTo ErrorHandler

//             */
//            Item2Ret = (Unit)colUnits[Index];
//            return Item2Ret;
//        ErrorHandler:
//            ;

//            // UPGRADE_NOTE: オブジェクト Item2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
//            Item2Ret = null;
//        }

//        // ユニットリストに指定されたユニットが定義されているか？
//        public bool IsDefined(ref object Index)
//        {
//            bool IsDefinedRet = default;
//            Unit u;
//            string uname;
//            ;
//#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
//            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 13420


//            Input:

//                    On Error GoTo ErrorHandler

//             */
//            u = (Unit)colUnits[Index];
//            IsDefinedRet = true;
//            return IsDefinedRet;
//        ErrorHandler:
//            ;

//            // IDで見つからなければユニット名で検索
//            // UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
//            uname = Conversions.ToString(Index);
//            foreach (Unit currentU in colUnits)
//            {
//                u = currentU;
//                if ((u.Name ?? "") == (uname ?? ""))
//                {
//                    if (u.Status_Renamed != "破棄")
//                    {
//                        IsDefinedRet = true;
//                        return IsDefinedRet;
//                    }
//                }
//            }

//            IsDefinedRet = false;
//        }

//        // ユニットリストに指定されたユニットが定義されているか？ (IDのみ)
//        public bool IsDefined2(ref object Index)
//        {
//            bool IsDefined2Ret = default;
//            Unit u;
//            ;
//#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
//            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 14138


//            Input:

//                    On Error GoTo ErrorHandler

//             */
//            u = (Unit)colUnits[Index];
//            IsDefined2Ret = true;
//            return IsDefined2Ret;
//        ErrorHandler:
//            ;
//            IsDefined2Ret = false;
//        }

//        // ユニットリストをアップデート
//        public void Update()
//        {
//            Unit u;
//            short k, i, j, n;
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
//                    object argIndex1 = 1;
//                    u.UnloadUnit(ref argIndex1);
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
//                Unit argu1 = null;
//                Unit argu2 = null;
//                GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
//                prev_money = SRC.Money;
//                foreach (Unit currentU2 in colUnits)
//                {
//                    u = currentU2;
//                    if (u.Status_Renamed != "破壊")
//                    {
//                        goto NextDestroyedUnit;
//                    }

//                    string argfname = "召喚ユニット";
//                    if (u.IsFeatureAvailable(ref argfname))
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
//                        string argpname = "システム";
//                        GUI.DisplayMessage(ref argpname, u.Nickname + "を修理した;修理費 = " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(u.Value));
//                    }
//                    else
//                    {
//                        string argpname1 = "システム";
//                        GUI.DisplayMessage(ref argpname1, u.Nickname + "を治療した;治療費 = " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(u.Value));
//                    }

//                NextDestroyedUnit:
//                    ;
//                }

//                string argpname2 = "システム";
//                GUI.DisplayMessage(ref argpname2, "合計 = " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(prev_money - SRC.Money));
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
//                        string argfname1 = "ノーマルモード";
//                        string argfname2 = "パーツ合体";
//                        if (u.IsFeatureAvailable(ref argfname1))
//                        {
//                            string localLIndex() { object argIndex1 = "ノーマルモード"; string arglist = u.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

//                            string argnew_form = localLIndex();
//                            u.Transform(ref argnew_form);
//                        }
//                        else if (u.IsFeatureAvailable(ref argfname2))
//                        {
//                            object argIndex2 = "パーツ合体";
//                            string arglist = u.FeatureData(ref argIndex2);
//                            if (GeneralLib.LLength(ref arglist) == 2)
//                            {
//                                string localLIndex1() { object argIndex1 = "パーツ合体"; string arglist = u.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

//                                string argnew_form1 = localLIndex1();
//                                u.Transform(ref argnew_form1);
//                            }
//                            else
//                            {
//                                string localLIndex2() { object argIndex1 = "パーツ合体"; string arglist = u.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

//                                string argnew_form2 = localLIndex2();
//                                u.Transform(ref argnew_form2);
//                            }
//                        }
//                    }
//                }

//                // 分離を行う
//                foreach (Unit currentU5 in colUnits)
//                {
//                    u = currentU5;
//                    string argfname3 = "分離";
//                    if (!u.IsFeatureAvailable(ref argfname3))
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

//                    short localLLength() { object argIndex1 = "分離"; string arglist = u.FeatureData(ref argIndex1); var ret = GeneralLib.LLength(ref arglist); return ret; }

//                    string argfname4 = "制限時間";
//                    if (localLLength() > 3 & !u.IsFeatureAvailable(ref argfname4))
//                    {
//                        goto NextLoop1;
//                    }

//                    string argfname5 = "主形態";
//                    if (u.IsFeatureAvailable(ref argfname5))
//                    {
//                        goto NextLoop1;
//                    }

//                    // パイロットが足らない場合は分離を行わない
//                    n = 0;
//                    object argIndex6 = "分離";
//                    string arglist2 = u.FeatureData(ref argIndex6);
//                    var loopTo1 = GeneralLib.LLength(ref arglist2);
//                    for (j = 2; j <= loopTo1; j++)
//                    {
//                        object argIndex3 = "分離";
//                        string arglist1 = u.FeatureData(ref argIndex3);
//                        uname = GeneralLib.LIndex(ref arglist1, j);
//                        object argIndex5 = uname;
//                        if (SRC.UDList.IsDefined(ref argIndex5))
//                        {
//                            object argIndex4 = uname;
//                            {
//                                var withBlock = SRC.UDList.Item(ref argIndex4);
//                                string argfname6 = "召喚ユニット";
//                                if (!withBlock.IsFeatureAvailable(ref argfname6))
//                                {
//                                    n = (short)(n + withBlock.PilotNum);
//                                }
//                            }
//                        }
//                    }

//                    if (u.CountPilot() < n)
//                    {
//                        goto NextLoop1;
//                    }

//                    // 分離先の形態が利用可能？
//                    object argIndex8 = "分離";
//                    string arglist4 = u.FeatureData(ref argIndex8);
//                    var loopTo2 = GeneralLib.LLength(ref arglist4);
//                    for (j = 2; j <= loopTo2; j++)
//                    {
//                        object argIndex7 = "分離";
//                        string arglist3 = u.FeatureData(ref argIndex7);
//                        uname = GeneralLib.LIndex(ref arglist3, j);
//                        bool localIsDefined() { object argIndex1 = uname; var ret = SRC.UList.IsDefined(ref argIndex1); return ret; }

//                        if (!localIsDefined())
//                        {
//                            goto NextLoop1;
//                        }

//                        Unit localItem() { object argIndex1 = uname; var ret = SRC.UList.Item(ref argIndex1); return ret; }

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
//                        string argfname9 = "合体";
//                        if (u.IsFeatureAvailable(ref argfname9))
//                        {
//                            var loopTo3 = u.CountFeature();
//                            for (j = 1; j <= loopTo3; j++)
//                            {
//                                object argIndex9 = j;
//                                if (u.Feature(ref argIndex9) != "合体")
//                                {
//                                    goto NextLoop2;
//                                }

//                                // 合体後の形態が利用可能？
//                                string localFeatureData() { object argIndex1 = j; var ret = u.FeatureData(ref argIndex1); return ret; }

//                                string arglist5 = localFeatureData();
//                                uname = GeneralLib.LIndex(ref arglist5, 2);
//                                bool localIsDefined1() { object argIndex1 = uname; var ret = SRC.UList.IsDefined(ref argIndex1); return ret; }

//                                if (!localIsDefined1())
//                                {
//                                    goto NextLoop2;
//                                }

//                                object argIndex10 = uname;
//                                {
//                                    var withBlock1 = SRC.UList.Item(ref argIndex10);
//                                    if (u.Status_Renamed == "待機" & withBlock1.CurrentForm().Status_Renamed == "離脱")
//                                    {
//                                        goto NextLoop2;
//                                    }

//                                    string argfname7 = "制限時間";
//                                    if (withBlock1.IsFeatureAvailable(ref argfname7))
//                                    {
//                                        goto NextLoop2;
//                                    }

//                                    string localFeatureData1() { object argIndex1 = j; var ret = u.FeatureData(ref argIndex1); return ret; }

//                                    short localLLength1() { string arglist = hs7335ed59602e416aa1b2600f4949cf4c(); var ret = GeneralLib.LLength(ref arglist); return ret; }

//                                    string argfname8 = "主形態";
//                                    if (!withBlock1.IsFeatureAvailable(ref argfname8) & localLLength1() == 3)
//                                    {
//                                        goto NextLoop2;
//                                    }
//                                }

//                                // 合体のパートナーが利用可能？
//                                string localFeatureData3() { object argIndex1 = j; var ret = u.FeatureData(ref argIndex1); return ret; }

//                                string arglist7 = localFeatureData3();
//                                var loopTo4 = GeneralLib.LLength(ref arglist7);
//                                for (k = 3; k <= loopTo4; k++)
//                                {
//                                    string localFeatureData2() { object argIndex1 = j; var ret = u.FeatureData(ref argIndex1); return ret; }

//                                    string arglist6 = localFeatureData2();
//                                    uname = GeneralLib.LIndex(ref arglist6, k);
//                                    bool localIsDefined2() { object argIndex1 = uname; var ret = SRC.UList.IsDefined(ref argIndex1); return ret; }

//                                    if (!localIsDefined2())
//                                    {
//                                        goto NextLoop2;
//                                    }

//                                    object argIndex11 = uname;
//                                    {
//                                        var withBlock2 = SRC.UList.Item(ref argIndex11);
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
//                                string localFeatureData4() { object argIndex1 = j; var ret = u.FeatureData(ref argIndex1); return ret; }

//                                string localLIndex3() { string arglist = hs1e082bda318043228d140b6ae8f6a2c1(); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

//                                string arguname = localLIndex3();
//                                u.Combine(ref arguname);
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
//                        string argfname10 = "変形";
//                        if (u.IsFeatureAvailable(ref argfname10))
//                        {
//                            uname = u.Name;
//                            object argIndex12 = "変形";
//                            buf = u.FeatureData(ref argIndex12);
//                            var loopTo5 = GeneralLib.LLength(ref buf);
//                            for (j = 2; j <= loopTo5; j++)
//                            {
//                                uname2 = GeneralLib.LIndex(ref buf, j);
//                                object argIndex13 = uname2;
//                                if (SRC.UDList.IsDefined(ref argIndex13))
//                                {
//                                    UnitData localItem1() { object argIndex1 = uname2; var ret = SRC.UDList.Item(ref argIndex1); return ret; }

//                                    UnitData localItem2() { object argIndex1 = uname; var ret = SRC.UDList.Item(ref argIndex1); return ret; }

//                                    if (localItem1().ID < localItem2().ID)
//                                    {
//                                        uname = uname2;
//                                    }
//                                }
//                                else
//                                {
//                                    string argmsg = uname + "の変形先ユニット「" + uname2 + "」のデータが定義されていません。";
//                                    GUI.ErrorMessage(ref argmsg);
//                                }
//                            }

//                            if ((uname ?? "") != (u.Name ?? ""))
//                            {
//                                u.Transform(ref uname);
//                            }
//                        }
//                    }
//                }
//            }

//            // 暴走時パイロットを削除
//            foreach (Unit currentU8 in colUnits)
//            {
//                u = currentU8;
//                string argfname11 = "暴走時パイロット";
//                if (u.IsFeatureAvailable(ref argfname11))
//                {
//                    object argIndex16 = "暴走時パイロット";
//                    object argIndex17 = u.FeatureData(ref argIndex16);
//                    if (SRC.PList.IsDefined(ref argIndex17))
//                    {
//                        object argIndex14 = "暴走時パイロット";
//                        object argIndex15 = u.FeatureData(ref argIndex14);
//                        SRC.PList.Delete(ref argIndex15);
//                    }
//                }
//            }

//            // ダミーパイロットを削除
//            foreach (Unit currentU9 in colUnits)
//            {
//                u = currentU9;
//                if (u.CountPilot() > 0)
//                {
//                    object argIndex19 = 1;
//                    if (u.Pilot(ref argIndex19).Nickname0 == "パイロット不在")
//                    {
//                        object argIndex18 = 1;
//                        u.DeletePilot(ref argIndex18);
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
//                string argfname12 = "召喚ユニット";
//                if (u.IsFeatureAvailable(ref argfname12))
//                {
//                    u.Status_Renamed = "破棄";
//                }
//                // ダミーユニットを破棄
//                string argfname13 = "ダミーユニット";
//                if (u.IsFeatureAvailable(ref argfname13))
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
//                        Item localItem3() { object argIndex1 = i; var ret = u.Item(ref argIndex1); return ret; }

//                        localItem3().Exist = false;
//                    }

//                    object argIndex20 = u.ID;
//                    Delete(ref argIndex20);
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
//        }


//        // ユニットリストに登録されたユニットの情報をセーブ
//        public void Save()
//        {
//            short i;
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
//                    Unit localOtherForm() { object argIndex1 = i; var ret = u.OtherForm(ref argIndex1); return ret; }

//                    FileSystem.WriteLine(SRC.SaveDataFileNumber, localOtherForm().ID);
//                }

//                FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)u.CountPilot());
//                var loopTo1 = u.CountPilot();
//                for (i = 1; i <= loopTo1; i++)
//                {
//                    Pilot localPilot() { object argIndex1 = i; var ret = u.Pilot(ref argIndex1); return ret; }

//                    FileSystem.WriteLine(SRC.SaveDataFileNumber, localPilot().ID);
//                }

//                FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)u.CountSupport());
//                var loopTo2 = u.CountSupport();
//                for (i = 1; i <= loopTo2; i++)
//                {
//                    Pilot localSupport() { object argIndex1 = i; var ret = u.Support(ref argIndex1); return ret; }

//                    FileSystem.WriteLine(SRC.SaveDataFileNumber, localSupport().ID);
//                }

//                FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)u.CountItem());
//                var loopTo3 = u.CountItem();
//                for (i = 1; i <= loopTo3; i++)
//                {
//                    Item localItem() { object argIndex1 = i; var ret = u.Item(ref argIndex1); return ret; }

//                    FileSystem.WriteLine(SRC.SaveDataFileNumber, localItem().ID);
//                }
//            }
//        }

//        // ユニットリストにユニットの情報をロード
//        // (リンクは後で行う)
//        public void Load()
//        {
//            short num = default, num2 = default;
//            Unit new_unit;
//            var Name = default(string);
//            // UPGRADE_NOTE: Status は Status_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
//            string ID = default, Status_Renamed = default;
//            var Rank = default(short);
//            short i, j;
//            string dummy;
//            FileSystem.Input(SRC.SaveDataFileNumber, ref IDCount);
//            FileSystem.Input(SRC.SaveDataFileNumber, ref num);
//            var loopTo = num;
//            for (i = 1; i <= loopTo; i++)
//            {
//                new_unit = new Unit();
//                // Name
//                FileSystem.Input(SRC.SaveDataFileNumber, ref Name);
//                bool localIsDefined() { object argIndex1 = Name; var ret = SRC.UDList.IsDefined(ref argIndex1); return ret; }

//                if (!localIsDefined())
//                {
//                    string argmsg = Name + "のデータが定義されていません";
//                    GUI.ErrorMessage(ref argmsg);
//                    SRC.TerminateSRC();
//                    Environment.Exit(0);
//                }

//                // ID, Rank, Status
//                FileSystem.Input(SRC.SaveDataFileNumber, ref ID);
//                FileSystem.Input(SRC.SaveDataFileNumber, ref Rank);
//                FileSystem.Input(SRC.SaveDataFileNumber, ref Status_Renamed);

//                // 旧形式のユニットＩＤを新形式に変換
//                if (SRC.SaveDataVersion < 10700)
//                {
//                    SRC.ConvertUnitID(ref ID);
//                }

//                new_unit.Name = Name;
//                new_unit.ID = ID;
//                new_unit.Rank = Rank;
//                new_unit.Party = "味方";
//                new_unit.Status_Renamed = Status_Renamed;
//                new_unit.FullRecover();
//                colUnits.Add(new_unit, new_unit.ID);

//                // OtherForm
//                FileSystem.Input(SRC.SaveDataFileNumber, ref num2);
//                var loopTo1 = num2;
//                for (j = 1; j <= loopTo1; j++)
//                    dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);

//                // Pilot
//                FileSystem.Input(SRC.SaveDataFileNumber, ref num2);
//                var loopTo2 = num2;
//                for (j = 1; j <= loopTo2; j++)
//                    dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);

//                // Support
//                FileSystem.Input(SRC.SaveDataFileNumber, ref num2);
//                var loopTo3 = num2;
//                for (j = 1; j <= loopTo3; j++)
//                    dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);

//                // Item
//                FileSystem.Input(SRC.SaveDataFileNumber, ref num2);
//                var loopTo4 = num2;
//                for (j = 1; j <= loopTo4; j++)
//                    dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);
//            }
//        }

//        // ユニットリストにユニットの情報をロードし、リンクを行う
//        public void LoadLinkInfo()
//        {
//            short num = default, num2 = default;
//            string ID = default, ID2 = default;
//            short i, j;
//            var int_dummy = default(short);
//            string str_dummy;
//            FileSystem.Input(SRC.SaveDataFileNumber, ref IDCount);
//            FileSystem.Input(SRC.SaveDataFileNumber, ref num);
//            var loopTo = num;
//            for (i = 1; i <= loopTo; i++)
//            {
//                // Name
//                str_dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);
//                // ID, Rank, Status
//                FileSystem.Input(SRC.SaveDataFileNumber, ref ID);
//                FileSystem.Input(SRC.SaveDataFileNumber, ref int_dummy);
//                FileSystem.Input(SRC.SaveDataFileNumber, ref str_dummy);

//                // 旧形式のユニットＩＤを新形式に変換
//                if (SRC.SaveDataVersion < 10700)
//                {
//                    SRC.ConvertUnitID(ref ID);
//                }

//                object argIndex6 = ID;
//                {
//                    var withBlock = Item(ref argIndex6);
//                    // OtherForm
//                    FileSystem.Input(SRC.SaveDataFileNumber, ref num2);
//                    var loopTo1 = num2;
//                    for (j = 1; j <= loopTo1; j++)
//                    {
//                        FileSystem.Input(SRC.SaveDataFileNumber, ref ID2);
//                        SRC.ConvertUnitID(ref ID2);
//                        object argIndex1 = ID2;
//                        if (IsDefined(ref argIndex1))
//                        {
//                            Unit localItem() { object argIndex1 = ID2; var ret = Item(ref argIndex1); return ret; }

//                            var argu = localItem();
//                            withBlock.AddOtherForm(ref argu);
//                        }
//                    }

//                    // Pilot
//                    FileSystem.Input(SRC.SaveDataFileNumber, ref num2);
//                    var loopTo2 = num2;
//                    for (j = 1; j <= loopTo2; j++)
//                    {
//                        FileSystem.Input(SRC.SaveDataFileNumber, ref ID2);
//                        object argIndex3 = ID2;
//                        if (SRC.PList.IsDefined(ref argIndex3))
//                        {
//                            Pilot localItem1() { object argIndex1 = ID2; var ret = SRC.PList.Item(ref argIndex1); return ret; }

//                            var argp = localItem1();
//                            withBlock.AddPilot(ref argp);
//                            if (withBlock.Status_Renamed == "離脱")
//                            {
//                                Pilot localItem2() { object argIndex1 = ID2; var ret = SRC.PList.Item(ref argIndex1); return ret; }

//                                localItem2().Away = true;
//                            }
//                        }
//                        else
//                        {
//                            ID2 = Strings.Left(ID2, Strings.InStr(ID2, "(") - 1);
//                            object argIndex2 = ID2;
//                            if (SRC.PList.IsDefined(ref argIndex2))
//                            {
//                                Pilot localItem3() { object argIndex1 = ID2; var ret = SRC.PList.Item(ref argIndex1); return ret; }

//                                var argp1 = localItem3();
//                                withBlock.AddPilot(ref argp1);
//                                if (withBlock.Status_Renamed == "離脱")
//                                {
//                                    Pilot localItem4() { object argIndex1 = ID2; var ret = SRC.PList.Item(ref argIndex1); return ret; }

//                                    localItem4().Away = true;
//                                }
//                            }
//                        }
//                    }

//                    // Support
//                    FileSystem.Input(SRC.SaveDataFileNumber, ref num2);
//                    var loopTo3 = num2;
//                    for (j = 1; j <= loopTo3; j++)
//                    {
//                        FileSystem.Input(SRC.SaveDataFileNumber, ref ID2);
//                        object argIndex4 = ID2;
//                        if (SRC.PList.IsDefined(ref argIndex4))
//                        {
//                            Pilot localItem5() { object argIndex1 = ID2; var ret = SRC.PList.Item(ref argIndex1); return ret; }

//                            var argp2 = localItem5();
//                            withBlock.AddSupport(ref argp2);
//                            if (withBlock.Status_Renamed == "離脱")
//                            {
//                                Pilot localItem6() { object argIndex1 = ID2; var ret = SRC.PList.Item(ref argIndex1); return ret; }

//                                localItem6().Away = true;
//                            }
//                        }
//                    }

//                    // Unit
//                    FileSystem.Input(SRC.SaveDataFileNumber, ref num2);
//                    var loopTo4 = num2;
//                    for (j = 1; j <= loopTo4; j++)
//                    {
//                        FileSystem.Input(SRC.SaveDataFileNumber, ref ID2);
//                        bool localIsDefined() { object argIndex1 = ID2; var ret = SRC.IDList.IsDefined(ref argIndex1); return ret; }

//                        object argIndex5 = ID2;
//                        if (SRC.IList.IsDefined(ref argIndex5))
//                        {
//                            Item localItem8() { object argIndex1 = ID2; var ret = SRC.IList.Item(ref argIndex1); return ret; }

//                            if (localItem8().Unit is null)
//                            {
//                                Item localItem7() { object argIndex1 = ID2; var ret = SRC.IList.Item(ref argIndex1); return ret; }

//                                var argitm = localItem7();
//                                withBlock.CurrentForm().AddItem0(ref argitm);
//                            }
//                        }
//                        else if (localIsDefined())
//                        {
//                            var argitm1 = SRC.IList.Add(ref ID2);
//                            withBlock.CurrentForm().AddItem0(ref argitm1);
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
//            short i, num = default;
//            Unit u;
//            {
//                var withBlock = colUnits;
//                var loopTo = (short)withBlock.Count;
//                for (i = 1; i <= loopTo; i++)
//                    withBlock.Remove(1);
//            }

//            FileSystem.Input(SRC.SaveDataFileNumber, ref num);
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
//            var num = default(short);
//            FileSystem.Input(SRC.SaveDataFileNumber, ref num);
//            foreach (Unit u in colUnits)
//                u.RestoreLinkInfo();
//        }

//        // 一時中断用データのパラメータ情報をファイルからロードする
//        public void RestoreParameter()
//        {
//            var num = default(short);
//            FileSystem.Input(SRC.SaveDataFileNumber, ref num);
//            foreach (Unit u in colUnits)
//                u.RestoreParameter();
//        }


//        // ユニットリストをクリア
//        public void Clear()
//        {
//            short i;
//            var loopTo = Count();
//            for (i = 1; i <= loopTo; i++)
//            {
//                object argIndex1 = 1;
//                Delete(ref argIndex1);
//            }
//        }

//        // ユニットリストに登録されたユニットのビットマップIDをクリア
//        public void ClearUnitBitmap()
//        {

//            // UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
//            {
//                var withBlock = GUI.MainForm.picUnitBitmap;
//                // UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
//                if (withBlock.Width == 32)
//                {
//                    // 既にクリアされていればそのまま終了
//                    return;
//                }

//                // 画像をクリア
//                // UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
//                withBlock.Picture = Image.FromFile("");
//                // UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
//                withBlock.Move(0, 0, 32, 96);
//            }

//            // BitmapIDをクリア
//            foreach (Unit u in colUnits)
//                u.BitmapID = 0;
//        }


//        // ハイパーモードの自動発動チェック
//        public void CheckAutoHyperMode()
//        {
//            foreach (Unit u in colUnits)
//                u.CheckAutoHyperMode();
//        }

//        // ノーマルモードの自動発動チェック
//        public void CheckAutoNormalMode()
//        {
//            var is_redraw_necessary = default(bool);
//            foreach (Unit u in colUnits)
//            {
//                if (u.CheckAutoNormalMode(true))
//                {
//                    is_redraw_necessary = true;
//                }
//            }

//            // 画面の再描画が必要？
//            if (is_redraw_necessary)
//            {
//                GUI.RedrawScreen();
//            }
//        }

//        // 破棄されたユニットを削除
//        public void Clean()
//        {
//            Unit u;
//            short i;
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
//                            Unit localOtherForm() { object argIndex1 = i; var ret = u.OtherForm(ref argIndex1); return ret; }

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
//                        Pilot localPilot() { object argIndex1 = i; var ret = u.Pilot(ref argIndex1); return ret; }

//                        localPilot().Alive = false;
//                    }

//                    var loopTo2 = u.CountSupport();
//                    for (i = 1; i <= loopTo2; i++)
//                    {
//                        Pilot localSupport() { object argIndex1 = i; var ret = u.Support(ref argIndex1); return ret; }

//                        localSupport().Alive = false;
//                    }

//                    // ユニットが装備しているアイテムも破棄
//                    var loopTo3 = u.CountItem();
//                    for (i = 1; i <= loopTo3; i++)
//                    {
//                        Item localItem() { object argIndex1 = i; var ret = u.Item(ref argIndex1); return ret; }

//                        localItem().Exist = false;
//                    }

//                    object argIndex1 = u.ID;
//                    Delete(ref argIndex1);
//                }
//            }
//        }
    }
}