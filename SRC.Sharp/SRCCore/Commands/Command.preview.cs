// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.VB;
using System;

namespace SRCCore.Commands
{
    public partial class Command
    {
        // 「特殊能力一覧」コマンド
        private void FeatureListCommand()
        {
            throw new NotImplementedException();
            //string[] list;
            //var id_list = default(string[]);
            //bool[] is_unit_feature;
            //int i, j;
            //var ret = default;
            //string fname0, fname, ftype;
            //GUI.LockGUI();

            //// 表示する特殊能力名一覧の作成
            //list = new string[1];
            //var id_ist = new object[1];
            //is_unit_feature = new bool[1];

            //// 武器・防具クラス
            //string argoname = "アイテム交換";
            //if (Expression.IsOptionDefined(argoname))
            //{
            //    {
            //        var withBlock = SelectedUnit;
            //        string argfname = "武器クラス";
            //        string argfname1 = "防具クラス";
            //        if (withBlock.IsFeatureAvailable(argfname) | withBlock.IsFeatureAvailable(argfname1))
            //        {
            //            Array.Resize(list, Information.UBound(list) + 1 + 1);
            //            Array.Resize(id_list, Information.UBound(list) + 1);
            //            Array.Resize(is_unit_feature, Information.UBound(list) + 1);
            //            list[Information.UBound(list)] = "武器・防具クラス";
            //            id_list[Information.UBound(list)] = "武器・防具クラス";
            //            is_unit_feature[Information.UBound(list)] = true;
            //        }
            //    }
            //}

            //{
            //    var withBlock1 = SelectedUnit.MainPilot();
            //    // パイロット特殊能力
            //    var loopTo = withBlock1.CountSkill();
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        object argIndex3 = i;
            //        switch (withBlock1.Skill(argIndex3) ?? "")
            //        {
            //            case "得意技":
            //            case "不得手":
            //                {
            //                    object argIndex1 = i;
            //                    fname = withBlock1.Skill(argIndex1);
            //                    break;
            //                }

            //            default:
            //                {
            //                    object argIndex2 = i;
            //                    fname = withBlock1.SkillName(argIndex2);
            //                    break;
            //                }
            //        }

            //        // 非表示の能力は除く
            //        if (Strings.InStr(fname, "非表示") > 0)
            //        {
            //            goto NextSkill;
            //        }

            //        // 既に表示されていればスキップ
            //        var loopTo1 = Information.UBound(list);
            //        for (j = 1; j <= loopTo1; j++)
            //        {
            //            if ((list[j] ?? "") == (fname ?? ""))
            //            {
            //                goto NextSkill;
            //            }
            //        }

            //        // リストに追加
            //        Array.Resize(list, Information.UBound(list) + 1 + 1);
            //        Array.Resize(id_list, Information.UBound(list) + 1);
            //        list[Information.UBound(list)] = fname;
            //        id_list[Information.UBound(list)] = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(i);
            //    NextSkill:
            //        ;
            //    }
            //}

            //{
            //    var withBlock2 = SelectedUnit;
            //    // 付加・強化されたパイロット用特殊能力
            //    var loopTo2 = withBlock2.CountCondition();
            //    for (i = 1; i <= loopTo2; i++)
            //    {
            //        // パイロット能力付加または強化？
            //        string localCondition() { object argIndex1 = i; var ret = withBlock2.Condition(argIndex1); return ret; }

            //        string localCondition1() { object argIndex1 = i; var ret = withBlock2.Condition(argIndex1); return ret; }

            //        if (Strings.Right(localCondition(), 3) != "付加２" & Strings.Right(localCondition1(), 3) != "強化２")
            //        {
            //            goto NextSkill2;
            //        }

            //        string localCondition2() { object argIndex1 = i; var ret = withBlock2.Condition(argIndex1); return ret; }

            //        string localCondition3() { object argIndex1 = i; var ret = withBlock2.Condition(argIndex1); return ret; }

            //        ftype = Strings.Left(localCondition2(), Strings.Len(localCondition3()) - 3);

            //        // 非表示の能力？
            //        string localConditionData() { object argIndex1 = i; var ret = withBlock2.ConditionData(argIndex1); return ret; }

            //        string arglist = localConditionData();
            //        switch (GeneralLib.LIndex(arglist, 1) ?? "")
            //        {
            //            case "非表示":
            //            case "解説":
            //                {
            //                    goto NextSkill2;
            //                    break;
            //                }
            //        }

            //        // 有効時間が残っている？
            //        object argIndex4 = i;
            //        if (withBlock2.ConditionLifetime(argIndex4) == 0)
            //        {
            //            goto NextSkill2;
            //        }

            //        // 表示名称
            //        object argIndex5 = ftype;
            //        fname = withBlock2.MainPilot().SkillName(argIndex5);
            //        if (Strings.InStr(fname, "非表示") > 0)
            //        {
            //            goto NextSkill2;
            //        }

            //        // 既に表示していればスキップ
            //        var loopTo3 = Information.UBound(list);
            //        for (j = 1; j <= loopTo3; j++)
            //        {
            //            if ((list[j] ?? "") == (fname ?? ""))
            //            {
            //                goto NextSkill2;
            //            }
            //        }

            //        // リストに追加
            //        Array.Resize(list, Information.UBound(list) + 1 + 1);
            //        Array.Resize(id_list, Information.UBound(list) + 1);
            //        list[Information.UBound(list)] = fname;
            //        id_list[Information.UBound(list)] = ftype;
            //    NextSkill2:
            //        ;
            //    }

            //    Array.Resize(is_unit_feature, Information.UBound(list) + 1);

            //    // ユニット用特殊能力
            //    // 付加された特殊能力より先に固有の特殊能力を表示
            //    if (withBlock2.CountAllFeature() > withBlock2.AdditionalFeaturesNum)
            //    {
            //        i = (withBlock2.AdditionalFeaturesNum + 1);
            //    }
            //    else
            //    {
            //        i = 1;
            //    }

            //    while (i <= withBlock2.CountAllFeature())
            //    {
            //        // 非表示の特殊能力を排除
            //        object argIndex6 = i;
            //        if (string.IsNullOrEmpty(withBlock2.AllFeatureName(argIndex6)))
            //        {
            //            goto NextFeature;
            //        }

            //        // 合体の場合は合体後の形態が作成されていなければならない
            //        string localAllFeature() { object argIndex1 = i; var ret = withBlock2.AllFeature(argIndex1); return ret; }

            //        string localAllFeatureData() { object argIndex1 = i; var ret = withBlock2.AllFeatureData(argIndex1); return ret; }

            //        string localLIndex() { string arglist = hs7afb75fef08b43c283a05523ef7388cb(); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //        bool localIsDefined() { object argIndex1 = (object)hse6256782c58b487b8147a3f247066e6f(); var ret = SRC.UList.IsDefined(argIndex1); return ret; }

            //        if (localAllFeature() == "合体" & !localIsDefined())
            //        {
            //            goto NextFeature;
            //        }

            //        // 既に表示していればスキップ
            //        var loopTo4 = Information.UBound(list);
            //        for (j = 1; j <= loopTo4; j++)
            //        {
            //            string localAllFeatureName() { object argIndex1 = i; var ret = withBlock2.AllFeatureName(argIndex1); return ret; }

            //            if ((list[j] ?? "") == (localAllFeatureName() ?? ""))
            //            {
            //                goto NextFeature;
            //            }
            //        }

            //        // リストに追加
            //        Array.Resize(list, Information.UBound(list) + 1 + 1);
            //        Array.Resize(id_list, Information.UBound(list) + 1);
            //        Array.Resize(is_unit_feature, Information.UBound(list) + 1);
            //        object argIndex7 = i;
            //        list[Information.UBound(list)] = withBlock2.AllFeatureName(argIndex7);
            //        id_list[Information.UBound(list)] = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(i);
            //        is_unit_feature[Information.UBound(list)] = true;
            //    NextFeature:
            //        ;
            //        if (i == withBlock2.AdditionalFeaturesNum)
            //        {
            //            break;
            //        }
            //        else if (i == withBlock2.CountFeature())
            //        {
            //            // 付加された特殊能力は後から表示
            //            if (withBlock2.AdditionalFeaturesNum > 0)
            //            {
            //                i = 0;
            //            }
            //        }

            //        i = (i + 1);
            //    }

            //    // アビリティで付加・強化されたパイロット用特殊能力
            //    var loopTo5 = withBlock2.CountCondition();
            //    for (i = 1; i <= loopTo5; i++)
            //    {
            //        // パイロット能力付加または強化？
            //        string localCondition4() { object argIndex1 = i; var ret = withBlock2.Condition(argIndex1); return ret; }

            //        string localCondition5() { object argIndex1 = i; var ret = withBlock2.Condition(argIndex1); return ret; }

            //        if (Strings.Right(localCondition4(), 2) != "付加" & Strings.Right(localCondition5(), 2) != "強化")
            //        {
            //            goto NextSkill3;
            //        }

            //        string localCondition6() { object argIndex1 = i; var ret = withBlock2.Condition(argIndex1); return ret; }

            //        string localCondition7() { object argIndex1 = i; var ret = withBlock2.Condition(argIndex1); return ret; }

            //        ftype = Strings.Left(localCondition6(), Strings.Len(localCondition7()) - 2);

            //        // 非表示の能力？
            //        if (ftype == "メッセージ")
            //        {
            //            goto NextSkill3;
            //        }

            //        string localConditionData1() { object argIndex1 = i; var ret = withBlock2.ConditionData(argIndex1); return ret; }

            //        string arglist1 = localConditionData1();
            //        switch (GeneralLib.LIndex(arglist1, 1) ?? "")
            //        {
            //            case "非表示":
            //            case "解説":
            //                {
            //                    goto NextSkill3;
            //                    break;
            //                }
            //        }

            //        // 有効時間が残っている？
            //        object argIndex8 = i;
            //        if (withBlock2.ConditionLifetime(argIndex8) == 0)
            //        {
            //            goto NextSkill3;
            //        }

            //        // 表示名称
            //        object argIndex9 = ftype;
            //        if (string.IsNullOrEmpty(withBlock2.FeatureName0(argIndex9)))
            //        {
            //            goto NextSkill3;
            //        }

            //        object argIndex10 = ftype;
            //        fname = withBlock2.MainPilot().SkillName0(argIndex10);
            //        if (Strings.InStr(fname, "非表示") > 0)
            //        {
            //            goto NextSkill3;
            //        }

            //        // 付加されたユニット用特殊能力として既に表示していればスキップ
            //        var loopTo6 = Information.UBound(list);
            //        for (j = 1; j <= loopTo6; j++)
            //        {
            //            if ((list[j] ?? "") == (fname ?? ""))
            //            {
            //                goto NextSkill3;
            //            }
            //        }

            //        object argIndex11 = ftype;
            //        fname = withBlock2.MainPilot().SkillName(argIndex11);
            //        if (Strings.InStr(fname, "Lv") > 0)
            //        {
            //            fname0 = Strings.Left(fname, Strings.InStr(fname, "Lv") - 1);
            //        }
            //        else
            //        {
            //            fname0 = fname;
            //        }

            //        // パイロット用特殊能力として既に表示していればスキップ
            //        var loopTo7 = Information.UBound(list);
            //        for (j = 1; j <= loopTo7; j++)
            //        {
            //            if ((list[j] ?? "") == (fname ?? "") | (list[j] ?? "") == (fname0 ?? ""))
            //            {
            //                goto NextSkill3;
            //            }
            //        }

            //        // リストに追加
            //        Array.Resize(list, Information.UBound(list) + 1 + 1);
            //        Array.Resize(id_list, Information.UBound(list) + 1);
            //        Array.Resize(is_unit_feature, Information.UBound(list) + 1);
            //        list[Information.UBound(list)] = fname;
            //        id_list[Information.UBound(list)] = ftype;
            //        is_unit_feature[Information.UBound(list)] = false;
            //    NextSkill3:
            //        ;
            //    }
            //}

            //GUI.ListItemFlag = new bool[Information.UBound(list) + 1];
            //switch (Information.UBound(list))
            //{
            //    case 0:
            //        {
            //            break;
            //        }

            //    case 1:
            //        {
            //            if (SRC.AutoMoveCursor)
            //            {
            //                GUI.SaveCursorPos();
            //            }

            //            if (id_list[ret] == "武器・防具クラス")
            //            {
            //                Help.FeatureHelp(SelectedUnit, id_list[1], false);
            //            }
            //            else if (is_unit_feature[1])
            //            {
            //                Help.FeatureHelp(SelectedUnit, id_list[1], GeneralLib.StrToLng(id_list[1]) <= SelectedUnit.AdditionalFeaturesNum);
            //            }
            //            else
            //            {
            //                Help.SkillHelp(SelectedUnit.MainPilot(), id_list[1]);
            //            }

            //            if (SRC.AutoMoveCursor)
            //            {
            //                GUI.RestoreCursorPos();
            //            }

            //            break;
            //        }

            //    default:
            //        {
            //            GUI.TopItem = 1;
            //            string arglb_caption = "特殊能力一覧";
            //            string arglb_info = "能力名";
            //            string arglb_mode = "表示のみ";
            //            ret = GUI.ListBox(arglb_caption, list, arglb_info, arglb_mode);
            //            if (SRC.AutoMoveCursor)
            //            {
            //                string argcursor_mode = "ダイアログ";
            //                GUI.MoveCursorPos(argcursor_mode);
            //            }

            //            while (true)
            //            {
            //                string arglb_caption1 = "特殊能力一覧";
            //                string arglb_info1 = "能力名";
            //                string arglb_mode1 = "連続表示";
            //                ret = GUI.ListBox(arglb_caption1, list, arglb_info1, arglb_mode1);
            //                // listが一定なので連続表示を流用
            //                My.MyProject.Forms.frmListBox.Hide();
            //                if (ret == 0)
            //                {
            //                    break;
            //                }

            //                if (id_list[ret] == "武器・防具クラス")
            //                {
            //                    Help.FeatureHelp(SelectedUnit, id_list[ret], false);
            //                }
            //                else if (is_unit_feature[ret])
            //                {
            //                    Help.FeatureHelp(SelectedUnit, id_list[ret], Conversions.ToDouble(id_list[ret]) <= SelectedUnit.AdditionalFeaturesNum);
            //                }
            //                else
            //                {
            //                    Help.SkillHelp(SelectedUnit.MainPilot(), id_list[ret]);
            //                }
            //            }

            //            if (SRC.AutoMoveCursor)
            //            {
            //                GUI.RestoreCursorPos();
            //            }

            //            break;
            //        }
            //}

            //CommandState = "ユニット選択";
            //GUI.UnlockGUI();
        }

        // 「武器一覧」コマンド
        private void WeaponListCommand()
        {
            throw new NotImplementedException();
            //string[] list;
            //int i;
            //string buf;
            //int w;
            //string wclass;
            //string atype, alevel;
            //string c;
            //GUI.LockGUI();
            //int min_range, max_range;
            //while (true)
            //{
            //    string argcaption_msg = "武装一覧";
            //    string arglb_mode = "一覧";
            //    string argBGM = "";
            //    w = GUI.WeaponListBox(SelectedUnit, argcaption_msg, arglb_mode, BGM: argBGM);
            //    SelectedWeapon = w;
            //    if (SelectedWeapon <= 0)
            //    {
            //        // キャンセル
            //        if (SRC.AutoMoveCursor)
            //        {
            //            GUI.RestoreCursorPos();
            //        }

            //        My.MyProject.Forms.frmListBox.Hide();
            //        GUI.UnlockGUI();
            //        CommandState = "ユニット選択";
            //        return;
            //    }

            //    // 指定された武器の属性一覧を作成
            //    list = new string[1];
            //    i = 0;
            //    {
            //        var withBlock = SelectedUnit;
            //        wclass = withBlock.WeaponClass(w);
            //        while (i <= Strings.Len(wclass))
            //        {
            //            i = (i + 1);
            //            buf = GeneralLib.GetClassBundle(wclass, i);
            //            atype = "";
            //            alevel = "";

            //            // 非表示？
            //            if (buf == "|")
            //            {
            //                break;
            //            }

            //            // Ｍ属性
            //            if (Strings.Mid(wclass, i, 1) == "Ｍ")
            //            {
            //                i = (i + 1);
            //                buf = buf + Strings.Mid(wclass, i, 1);
            //            }

            //            // レベル指定
            //            if (Strings.Mid(wclass, i + 1, 1) == "L")
            //            {
            //                i = (i + 2);
            //                c = Strings.Mid(wclass, i, 1);
            //                while (Information.IsNumeric(c) | c == "." | c == "-")
            //                {
            //                    alevel = alevel + c;
            //                    i = (i + 1);
            //                    c = Strings.Mid(wclass, i, 1);
            //                }

            //                i = (i - 1);
            //            }

            //            // 属性の名称
            //            atype = Help.AttributeName(SelectedUnit, buf);
            //            if (Strings.Len(atype) > 0)
            //            {
            //                Array.Resize(list, Information.UBound(list) + 1 + 1);
            //                if (Strings.Len(alevel) > 0)
            //                {
            //                    string localRightPaddedString() { string argbuf = buf + "L" + alevel; var ret = GeneralLib.RightPaddedString(argbuf, 8); return ret; }

            //                    list[Information.UBound(list)] = localRightPaddedString() + atype + "レベル" + Strings.StrConv(alevel, VbStrConv.Wide);
            //                }
            //                else
            //                {
            //                    list[Information.UBound(list)] = GeneralLib.RightPaddedString(buf, 8) + atype;
            //                }
            //            }
            //        }

            //        if (!string.IsNullOrEmpty(Map.MapFileName))
            //        {
            //            Array.Resize(list, Information.UBound(list) + 1 + 1);
            //            list[Information.UBound(list)] = "射程範囲";
            //        }

            //        if (Information.UBound(list) > 0)
            //        {
            //            GUI.TopItem = 1;
            //            while (true)
            //            {
            //                if (Information.UBound(list) == 1 & list[1] == "射程範囲")
            //                {
            //                    i = 1;
            //                }
            //                else
            //                {
            //                    GUI.ListItemFlag = new bool[Information.UBound(list) + 1];
            //                    string arglb_caption = "武器属性一覧";
            //                    string arglb_info = "属性    効果";
            //                    string arglb_mode1 = "連続表示";
            //                    i = GUI.ListBox(arglb_caption, list, arglb_info, arglb_mode1);
            //                }

            //                if (i == 0)
            //                {
            //                    // キャンセル
            //                    break;
            //                }
            //                else if (list[i] == "射程範囲")
            //                {
            //                    My.MyProject.Forms.frmListBox.Hide();

            //                    // 武器の射程を求めておく
            //                    min_range = withBlock.Weapon(w).MinRange;
            //                    max_range = withBlock.WeaponMaxRange(w);

            //                    // 射程範囲表示
            //                    string argattr3 = "Ｐ";
            //                    string argattr4 = "Ｑ";
            //                    string argattr5 = "Ｍ直";
            //                    string argattr6 = "Ｍ拡";
            //                    string argattr7 = "Ｍ扇";
            //                    string argattr8 = "Ｍ全";
            //                    string argattr9 = "Ｍ線";
            //                    string argattr10 = "Ｍ投";
            //                    string argattr11 = "Ｍ移";
            //                    if ((max_range == 1 | withBlock.IsWeaponClassifiedAs(w, argattr3)) & !withBlock.IsWeaponClassifiedAs(w, argattr4))
            //                    {
            //                        string arguparty = withBlock.Party + "の敵";
            //                        Map.AreaInReachable(SelectedUnit, max_range, arguparty);
            //                    }
            //                    else if (withBlock.IsWeaponClassifiedAs(w, argattr5))
            //                    {
            //                        Map.AreaInCross(withBlock.x, withBlock.y, min_range, max_range);
            //                    }
            //                    else if (withBlock.IsWeaponClassifiedAs(w, argattr6))
            //                    {
            //                        Map.AreaInWideCross(withBlock.x, withBlock.y, min_range, max_range);
            //                    }
            //                    else if (withBlock.IsWeaponClassifiedAs(w, argattr7))
            //                    {
            //                        string argattr = "Ｍ扇";
            //                        Map.AreaInSectorCross(withBlock.x, withBlock.y, min_range, max_range, withBlock.WeaponLevel(w, argattr));
            //                    }
            //                    else if (withBlock.IsWeaponClassifiedAs(w, argattr8) | withBlock.IsWeaponClassifiedAs(w, argattr9))
            //                    {
            //                        string arguparty2 = "すべて";
            //                        Map.AreaInRange(withBlock.x, withBlock.y, max_range, min_range, arguparty2);
            //                    }
            //                    else if (withBlock.IsWeaponClassifiedAs(w, argattr10))
            //                    {
            //                        string argattr1 = "Ｍ投";
            //                        max_range = (max_range + withBlock.WeaponLevel(w, argattr1));
            //                        string argattr2 = "Ｍ投";
            //                        min_range = (min_range - withBlock.WeaponLevel(w, argattr2));
            //                        min_range = GeneralLib.MaxLng(min_range, 1);
            //                        string arguparty3 = "すべて";
            //                        Map.AreaInRange(withBlock.x, withBlock.y, max_range, min_range, arguparty3);
            //                    }
            //                    else if (withBlock.IsWeaponClassifiedAs(w, argattr11))
            //                    {
            //                        Map.AreaInMoveAction(SelectedUnit, max_range);
            //                    }
            //                    else
            //                    {
            //                        string arguparty1 = withBlock.Party + "の敵";
            //                        Map.AreaInRange(withBlock.x, withBlock.y, max_range, min_range, arguparty1);
            //                    }

            //                    GUI.Center(withBlock.x, withBlock.y);
            //                    GUI.MaskScreen();

            //                    // 先行入力されていたクリックイベントを解消
            //                    Application.DoEvents();
            //                    WaitClickMode = true;
            //                    GUI.IsFormClicked = false;

            //                    // クリックされるまで待つ
            //                    while (!GUI.IsFormClicked)
            //                    {
            //                        GUI.Sleep(25);
            //                        Application.DoEvents();
            //                        if (GUI.IsRButtonPressed(true))
            //                        {
            //                            break;
            //                        }
            //                    }

            //                    GUI.RedrawScreen();
            //                    if (Information.UBound(list) == 1 & list[i] == "射程範囲")
            //                    {
            //                        break;
            //                    }
            //                }
            //                else
            //                {
            //                    // 指定された属性の解説を表示
            //                    My.MyProject.Forms.frmListBox.Hide();
            //                    string argatr = GeneralLib.LIndex(list[i], 1);
            //                    Help.AttributeHelp(SelectedUnit, argatr, w);
            //                }
            //            }
            //        }
            //    }
            //}
        }

        // 「アビリティ一覧」コマンド
        private void AbilityListCommand()
        {
            throw new NotImplementedException();
            //string[] list;
            //int i;
            //string buf;
            //int a;
            //string alevel, atype, aclass;
            //string c;
            //GUI.LockGUI();
            //int min_range, max_range;
            //while (true)
            //{
            //    string argtname = "アビリティ";
            //    string argcaption_msg = Expression.Term(argtname, SelectedUnit) + "一覧";
            //    string arglb_mode = "一覧";
            //    a = GUI.AbilityListBox(SelectedUnit, argcaption_msg, arglb_mode);
            //    SelectedAbility = a;
            //    if (SelectedAbility <= 0)
            //    {
            //        // キャンセル
            //        if (SRC.AutoMoveCursor)
            //        {
            //            GUI.RestoreCursorPos();
            //        }

            //        My.MyProject.Forms.frmListBox.Hide();
            //        GUI.UnlockGUI();
            //        CommandState = "ユニット選択";
            //        return;
            //    }

            //    // 指定されたアビリティの属性一覧を作成
            //    list = new string[1];
            //    i = 0;
            //    {
            //        var withBlock = SelectedUnit;
            //        aclass = withBlock.Ability(a).Class_Renamed;
            //        while (i <= Strings.Len(aclass))
            //        {
            //            i = (i + 1);
            //            buf = GeneralLib.GetClassBundle(aclass, i);
            //            atype = "";
            //            alevel = "";

            //            // 非表示？
            //            if (buf == "|")
            //            {
            //                break;
            //            }

            //            // Ｍ属性
            //            if (Strings.Mid(aclass, i, 1) == "Ｍ")
            //            {
            //                i = (i + 1);
            //                buf = buf + Strings.Mid(aclass, i, 1);
            //            }

            //            // レベル指定
            //            if (Strings.Mid(aclass, i + 1, 1) == "L")
            //            {
            //                i = (i + 2);
            //                c = Strings.Mid(aclass, i, 1);
            //                while (Information.IsNumeric(c) | c == "." | c == "-")
            //                {
            //                    alevel = alevel + c;
            //                    i = (i + 1);
            //                    c = Strings.Mid(aclass, i, 1);
            //                }

            //                i = (i - 1);
            //            }

            //            // 属性の名称
            //            atype = Help.AttributeName(SelectedUnit, buf, true);
            //            if (Strings.Len(atype) > 0)
            //            {
            //                Array.Resize(list, Information.UBound(list) + 1 + 1);
            //                if (Strings.Len(alevel) > 0)
            //                {
            //                    string localRightPaddedString() { string argbuf = buf + "L" + alevel; var ret = GeneralLib.RightPaddedString(argbuf, 8); return ret; }

            //                    list[Information.UBound(list)] = localRightPaddedString() + atype + "レベル" + Strings.StrConv(alevel, VbStrConv.Wide);
            //                }
            //                else
            //                {
            //                    list[Information.UBound(list)] = GeneralLib.RightPaddedString(buf, 8) + atype;
            //                }
            //            }
            //        }

            //        if (!string.IsNullOrEmpty(Map.MapFileName))
            //        {
            //            Array.Resize(list, Information.UBound(list) + 1 + 1);
            //            list[Information.UBound(list)] = "射程範囲";
            //        }

            //        if (Information.UBound(list) > 0)
            //        {
            //            GUI.TopItem = 1;
            //            while (true)
            //            {
            //                if (Information.UBound(list) == 1 & list[1] == "射程範囲")
            //                {
            //                    i = 1;
            //                }
            //                else
            //                {
            //                    GUI.ListItemFlag = new bool[Information.UBound(list) + 1];
            //                    string arglb_caption = "アビリティ属性一覧";
            //                    string arglb_info = "属性    効果";
            //                    string arglb_mode1 = "連続表示";
            //                    i = GUI.ListBox(arglb_caption, list, arglb_info, arglb_mode1);
            //                }

            //                if (i == 0)
            //                {
            //                    // キャンセル
            //                    break;
            //                }
            //                else if (list[i] == "射程範囲")
            //                {
            //                    My.MyProject.Forms.frmListBox.Hide();

            //                    // アビリティの射程を求めておく
            //                    min_range = withBlock.AbilityMinRange(a);
            //                    max_range = withBlock.AbilityMaxRange(a);

            //                    // 射程範囲表示
            //                    string argattr3 = "Ｐ";
            //                    string argattr4 = "Ｑ";
            //                    string argattr5 = "Ｍ直";
            //                    string argattr6 = "Ｍ拡";
            //                    string argattr7 = "Ｍ扇";
            //                    string argattr8 = "Ｍ投";
            //                    string argattr9 = "Ｍ移";
            //                    if ((max_range == 1 | withBlock.IsAbilityClassifiedAs(a, argattr3)) & !withBlock.IsAbilityClassifiedAs(a, argattr4))
            //                    {
            //                        string arguparty = "すべて";
            //                        Map.AreaInReachable(SelectedUnit, max_range, arguparty);
            //                    }
            //                    else if (withBlock.IsAbilityClassifiedAs(a, argattr5))
            //                    {
            //                        Map.AreaInCross(withBlock.x, withBlock.y, min_range, max_range);
            //                    }
            //                    else if (withBlock.IsAbilityClassifiedAs(a, argattr6))
            //                    {
            //                        Map.AreaInWideCross(withBlock.x, withBlock.y, min_range, max_range);
            //                    }
            //                    else if (withBlock.IsAbilityClassifiedAs(a, argattr7))
            //                    {
            //                        string argattr = "Ｍ扇";
            //                        Map.AreaInSectorCross(withBlock.x, withBlock.y, min_range, max_range, withBlock.AbilityLevel(a, argattr));
            //                    }
            //                    else if (withBlock.IsAbilityClassifiedAs(a, argattr8))
            //                    {
            //                        string argattr1 = "Ｍ投";
            //                        max_range = (max_range + withBlock.AbilityLevel(a, argattr1));
            //                        string argattr2 = "Ｍ投";
            //                        min_range = (min_range - withBlock.AbilityLevel(a, argattr2));
            //                        min_range = GeneralLib.MaxLng(min_range, 1);
            //                        string arguparty2 = "すべて";
            //                        Map.AreaInRange(withBlock.x, withBlock.y, max_range, min_range, arguparty2);
            //                    }
            //                    else if (withBlock.IsAbilityClassifiedAs(a, argattr9))
            //                    {
            //                        Map.AreaInMoveAction(SelectedUnit, max_range);
            //                    }
            //                    else
            //                    {
            //                        string arguparty1 = "すべて";
            //                        Map.AreaInRange(withBlock.x, withBlock.y, max_range, min_range, arguparty1);
            //                    }

            //                    GUI.Center(withBlock.x, withBlock.y);
            //                    GUI.MaskScreen();

            //                    // 先行入力されていたクリックイベントを解消
            //                    Application.DoEvents();
            //                    WaitClickMode = true;
            //                    GUI.IsFormClicked = false;

            //                    // クリックされるまで待つ
            //                    while (!GUI.IsFormClicked)
            //                    {
            //                        GUI.Sleep(25);
            //                        Application.DoEvents();
            //                        if (GUI.IsRButtonPressed(true))
            //                        {
            //                            break;
            //                        }
            //                    }

            //                    GUI.RedrawScreen();
            //                    if (Information.UBound(list) == 1 & list[i] == "射程範囲")
            //                    {
            //                        break;
            //                    }
            //                }
            //                else
            //                {
            //                    // 指定された属性の解説を表示
            //                    My.MyProject.Forms.frmListBox.Hide();
            //                    string argatr = GeneralLib.LIndex(list[i], 1);
            //                    Help.AttributeHelp(SelectedUnit, argatr, a, true);
            //                }
            //            }
            //        }
            //    }
            //}
        }

        // 「移動範囲」コマンド
        private void ShowAreaInSpeedCommand()
        {
            SelectedCommand = "移動範囲";
            //// If MainWidth <> 15 Then
            //if (GUI.NewGUIMode)
            //{
            //    // MOD END MARGE
            //    Status.ClearUnitStatus();
            //}

            Map.AreaInSpeed(SelectedUnit);
            GUI.Center(SelectedUnit.x, SelectedUnit.y);
            GUI.MaskScreen();
            CommandState = "ターゲット選択";
        }

        // 「射程範囲」コマンド
        private void ShowAreaInRangeCommand()
        {
            throw new NotImplementedException();
            //int w, i, max_range;
            //SelectedCommand = "射程範囲";

            //// MOD START MARGE
            //// If MainWidth <> 15 Then
            //if (GUI.NewGUIMode)
            //{
            //    // MOD END MARGE
            //    Status.ClearUnitStatus();
            //}

            //{
            //    var withBlock = SelectedUnit;
            //    // 最大の射程を持つ武器を探す
            //    w = 0;
            //    max_range = 0;
            //    var loopTo = withBlock.CountWeapon();
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        string argref_mode = "ステータス";
            //        string argattr = "Ｍ";
            //        if (withBlock.IsWeaponAvailable(i, argref_mode) & !withBlock.IsWeaponClassifiedAs(i, argattr))
            //        {
            //            if (withBlock.WeaponMaxRange(i) > max_range)
            //            {
            //                w = i;
            //                max_range = withBlock.WeaponMaxRange(i);
            //            }
            //        }
            //    }

            //    // 見つかった最大の射程を持つ武器の射程範囲を選択
            //    string arguparty = withBlock.Party + "の敵";
            //    Map.AreaInRange(withBlock.x, withBlock.y, max_range, 1, arguparty);

            //    // 射程範囲を表示
            //    GUI.Center(withBlock.x, withBlock.y);
            //    GUI.MaskScreen();
            //}

            //CommandState = "ターゲット選択";
        }

    }
}