using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRCCore.Units
{
    // === ユニットステータスの更新処理 ===
    public partial class Unit
    {
        // ユニットの各種パラメータを更新するサブルーチン
        // パラメータや武器、アビリティデータ等が変化する際には必ず呼び出す必要がある。
        public void Update(bool without_refresh = false)
        {
            // TODO Impl
            //    WeaponData[] prev_wdata;
            //    double[] prev_wbullets;
            //    AbilityData[] prev_adata;
            //    double[] prev_astocks;
            //    short l, j, i, k, num;
            //    string ch, buf;
            //    FeatureData fd;
            //    Item itm;
            //    string stype, sdata;
            //    double slevel;
            //    string stype2, sdata2;
            //    double slevel2;
            //    string wname, wnickname;
            //    string wnskill, wclass, wtype, sname;
            //    string fdata;
            //    short flen;
            //    bool found, flag;
            //    bool[] flags;
            //    bool with_not;
            //    short false_count;
            //    var uadaption = new short[5];
            //    double hp_ratio, en_ratio;
            //    string ubitmap;
            //    short pmorale;
            //    bool is_stable, is_uncontrollable, is_invisible;

            //    // ＨＰとＥＮの値を記録
            //    hp_ratio = 100 * HP / (double)MaxHP;
            //    en_ratio = 100 * EN / (double)MaxEN;

            //    // ユニット用画像ファイル名を記録しておく
            //    ubitmap = get_Bitmap(false);

            //    // 非表示かどうか記録しておく
            //    string argfname = "非表示";
            //    is_invisible = IsFeatureAvailable(ref argfname);

            //    // 制御不可がどうかを記録しておく
            //    string argfname1 = "制御不可";
            //    is_uncontrollable = IsFeatureAvailable(ref argfname1);

            //    // 不安定がどうかを記録しておく
            //    string argfname2 = "不安定";
            //    is_stable = IsFeatureAvailable(ref argfname2);
            //TryAgain:
            //    ;


            //    // アイテムが現在の形態で効力を発揮してくれるか判定
            //    foreach (Item currentItm in colItem)
            //    {
            //        itm = currentItm;
            //        var argu = this;
            //        itm.Activated = itm.IsAvailable(ref argu);
            //    }

            //    // ランクアップによるデータ変更
            //    string argfname3 = "ランクアップ";
            //    while (Data.IsFeatureAvailable(ref argfname3))
            //    {
            //        {
            //            var withBlock = Data;
            //            object argIndex1 = "ランクアップ";
            //            if (Rank < withBlock.FeatureLevel(ref argIndex1))
            //            {
            //                break;
            //            }

            //            bool localIsNecessarySkillSatisfied() { object argIndex1 = "ランクアップ"; string argnabilities = withBlock.FeatureNecessarySkill(ref argIndex1); Pilot argp = null; var ret = IsNecessarySkillSatisfied(ref argnabilities, p: ref argp); return ret; }

            //            if (!localIsNecessarySkillSatisfied())
            //            {
            //                break;
            //            }

            //            object argIndex2 = "ランクアップ";
            //            fdata = withBlock.FeatureData(ref argIndex2);
            //        }

            //        {
            //            var withBlock1 = SRC.UDList;
            //            bool localIsDefined() { object argIndex1 = fdata; var ret = withBlock1.IsDefined(ref argIndex1); return ret; }

            //            if (!localIsDefined())
            //            {
            //                string argmsg = Name + "のランクアップ先ユニット「" + fdata + "」のデータが定義されていません";
            //                GUI.ErrorMessage(ref argmsg);
            //                SRC.TerminateSRC();
            //            }

            //            object argIndex3 = fdata;
            //            Data = withBlock1.Item(ref argIndex3);
            //        }
            //    }

            //    // 特殊能力を更新

            //    // まず特殊能力リストをクリア
            //    {
            //        var withBlock2 = colFeature;
            //        foreach (FeatureData currentFd in colFeature)
            //        {
            //            fd = currentFd;
            //            withBlock2.Remove(1);
            //        }
            //    }

            //    // 付加された特殊能力
            //    foreach (Condition cnd in colCondition)
            //    {
            //        if (cnd.Lifetime != 0)
            //        {
            //            if (Strings.Right(cnd.Name, 2) == "付加")
            //            {
            //                fd = new FeatureData();
            //                fd.Name = Strings.Left(cnd.Name, Strings.Len(cnd.Name) - 2);
            //                fd.Level = cnd.Level;
            //                fd.StrData = cnd.StrData;
            //                colFeature.Add(fd, fd.Name);
            //            }
            //        }
            //    }

            //    AdditionalFeaturesNum = (short)colFeature.Count;

            //    // ユニットデータで定義されている特殊能力
            //    AddFeatures(ref Data.colFeature);

            //    // アイテムで得られた特殊能力
            //    var loopTo = CountItem();
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        object argIndex4 = i;
            //        {
            //            var withBlock3 = Item(ref argIndex4);
            //            if (withBlock3.Activated)
            //            {
            //                AddFeatures(ref withBlock3.Data.colFeature, true);
            //            }
            //        }
            //    }

            //    // パイロットデータで定義されている特殊能力
            //    if (CountPilot() > 0)
            //    {
            //        string argfname4 = "追加パイロット";
            //        if (IsFeatureAvailable(ref argfname4))
            //        {
            //            // 特殊能力を付加する前に必要技能が満たされているかどうか判定
            //            UpdateFeatures("追加パイロット");
            //        }

            //        AddFeatures(ref MainPilot().Data.colFeature);
            //        var loopTo1 = CountPilot();
            //        for (i = 2; i <= loopTo1; i++)
            //        {
            //            Pilot localPilot() { object argIndex1 = i; var ret = Pilot(ref argIndex1); return ret; }

            //            AddFeatures(ref localPilot().Data.colFeature);
            //        }

            //        var loopTo2 = CountSupport();
            //        for (i = 1; i <= loopTo2; i++)
            //        {
            //            Pilot localSupport() { object argIndex1 = i; var ret = Support(ref argIndex1); return ret; }

            //            AddFeatures(ref localSupport().Data.colFeature);
            //        }

            //        string argfname6 = "追加サポート";
            //        if (IsFeatureAvailable(ref argfname6))
            //        {
            //            // 特殊能力を付加する前に必要技能が満たされているかどうか判定
            //            UpdateFeatures("追加サポート");
            //            string argfname5 = "追加サポート";
            //            if (IsFeatureAvailable(ref argfname5))
            //            {
            //                AddFeatures(ref AdditionalSupport().Data.colFeature);
            //            }
            //        }
            //    }

            //    // パイロット能力付加＆強化の効果をクリア
            //    i = 1;
            //    while (i <= CountCondition())
            //    {
            //        string localCondition() { object argIndex1 = i; var ret = Condition(ref argIndex1); return ret; }

            //        switch (Strings.Right(localCondition(), 3) ?? "")
            //        {
            //            case "付加２":
            //            case "強化２":
            //                {
            //                    object argIndex5 = i;
            //                    DeleteCondition(ref argIndex5);
            //                    break;
            //                }

            //            default:
            //                {
            //                    i = (short)(i + 1);
            //                    break;
            //                }
            //        }
            //    }

            //    // パイロット能力付加
            //    found = false;
            //    flag = false;
            //    flags = new bool[colFeature.Count + 1];
            //AddSkills:
            //    ;
            //    i = 1;
            //    foreach (FeatureData currentFd1 in colFeature)
            //    {
            //        fd = currentFd1;
            //        if (flags[i])
            //        {
            //            goto NextFeature;
            //        }

            //        switch (fd.Name ?? "")
            //        {
            //            case "パイロット能力付加":
            //                {
            //                    // 必要技能を満たしている？
            //                    Pilot argp = null;
            //                    if (!IsNecessarySkillSatisfied(ref fd.NecessarySkill, p: ref argp))
            //                    {
            //                        found = true;
            //                        goto NextFeature;
            //                    }
            //                    // 必要条件を満たしている？
            //                    Pilot argp1 = null;
            //                    if (!IsNecessarySkillSatisfied(ref fd.NecessaryCondition, p: ref argp1))
            //                    {
            //                        found = true;
            //                        goto NextFeature;
            //                    }

            //                    flags[i] = true;

            //                    // 能力指定が「"」で囲まれている場合は「"」を削除
            //                    if (Strings.Asc(fd.StrData) == 34) // "
            //                    {
            //                        buf = Strings.Mid(fd.StrData, 2, Strings.Len(fd.StrData) - 2);
            //                    }
            //                    else
            //                    {
            //                        buf = fd.StrData;
            //                    }

            //                    // 付加する特殊能力の種類、レベル、データを解析
            //                    if (Strings.InStr(buf, "=") > 0)
            //                    {
            //                        sdata = Strings.Mid(buf, Strings.InStr(buf, "=") + 1);
            //                        buf = Strings.Left(buf, Strings.InStr(buf, "=") - 1);
            //                        if (Strings.InStr(buf, "Lv") > 0)
            //                        {
            //                            stype = Strings.Left(buf, Strings.InStr(buf, "Lv") - 1);
            //                            if (Information.IsNumeric(Strings.Mid(buf, Strings.InStr(buf, "Lv") + 2)))
            //                            {
            //                                slevel = Conversions.ToDouble(Strings.Mid(buf, Strings.InStr(buf, "Lv") + 2));
            //                            }
            //                            else
            //                            {
            //                                slevel = 1d;
            //                            }
            //                        }
            //                        else
            //                        {
            //                            stype = buf;
            //                            slevel = SRC.DEFAULT_LEVEL;
            //                        }
            //                    }
            //                    else
            //                    {
            //                        sdata = "";
            //                        if (Strings.InStr(buf, "Lv") > 0)
            //                        {
            //                            stype = Strings.Left(buf, Strings.InStr(buf, "Lv") - 1);
            //                            if (Information.IsNumeric(Strings.Mid(buf, Strings.InStr(buf, "Lv") + 2)))
            //                            {
            //                                slevel = Conversions.ToDouble(Strings.Mid(buf, Strings.InStr(buf, "Lv") + 2));
            //                            }
            //                            else
            //                            {
            //                                slevel = 1d;
            //                            }
            //                        }
            //                        else
            //                        {
            //                            stype = buf;
            //                            slevel = SRC.DEFAULT_LEVEL;
            //                        }
            //                    }

            //                    // エリアスが定義されている？
            //                    object argIndex9 = stype;
            //                    if (SRC.ALDList.IsDefined(ref argIndex9))
            //                    {
            //                        object argIndex7 = stype;
            //                        {
            //                            var withBlock4 = SRC.ALDList.Item(ref argIndex7);
            //                            var loopTo3 = withBlock4.Count;
            //                            for (j = 1; j <= loopTo3; j++)
            //                            {
            //                                // エリアスの定義に従って特殊能力定義を置き換える
            //                                stype2 = withBlock4.get_AliasType(j);
            //                                string localLIndex() { string arglist = withBlock4.get_AliasData(j); var ret = GeneralLib.LIndex(ref arglist, 1); withBlock4.get_AliasData(j) = arglist; return ret; }

            //                                if (localLIndex() == "解説")
            //                                {
            //                                    // 特殊能力の解説
            //                                    if (!string.IsNullOrEmpty(sdata))
            //                                    {
            //                                        stype2 = GeneralLib.LIndex(ref sdata, 1);
            //                                    }

            //                                    slevel2 = SRC.DEFAULT_LEVEL;
            //                                    sdata2 = withBlock4.get_AliasData(j);
            //                                }
            //                                else
            //                                {
            //                                    // 通常の能力
            //                                    if (withBlock4.get_AliasLevelIsPlusMod(j))
            //                                    {
            //                                        if (slevel == SRC.DEFAULT_LEVEL)
            //                                        {
            //                                            slevel = 1d;
            //                                        }

            //                                        slevel2 = slevel + withBlock4.get_AliasLevel(j);
            //                                    }
            //                                    else if (withBlock4.get_AliasLevelIsMultMod(j))
            //                                    {
            //                                        if (slevel == SRC.DEFAULT_LEVEL)
            //                                        {
            //                                            slevel = 1d;
            //                                        }

            //                                        slevel2 = slevel * withBlock4.get_AliasLevel(j);
            //                                    }
            //                                    else if (slevel != SRC.DEFAULT_LEVEL)
            //                                    {
            //                                        slevel2 = slevel;
            //                                    }
            //                                    else
            //                                    {
            //                                        slevel2 = withBlock4.get_AliasLevel(j);
            //                                    }

            //                                    sdata2 = withBlock4.get_AliasData(j);
            //                                    if (!string.IsNullOrEmpty(sdata))
            //                                    {
            //                                        if (Strings.InStr(sdata2, "非表示") != 1)
            //                                        {
            //                                            sdata2 = sdata + " " + GeneralLib.ListTail(ref sdata2, (short)(GeneralLib.LLength(ref sdata) + 1));
            //                                        }
            //                                    }

            //                                    if (withBlock4.get_AliasLevelIsPlusMod(j) | withBlock4.get_AliasLevelIsMultMod(j))
            //                                    {
            //                                        sdata2 = GeneralLib.LIndex(ref sdata2, 1) + "Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(slevel) + " " + GeneralLib.ListTail(ref sdata2, 2);
            //                                        sdata2 = Strings.Trim(sdata2);
            //                                    }
            //                                }

            //                                // 属性使用不能攻撃により使用不能になった技能を封印する。
            //                                object argIndex6 = stype2 + "使用不能";
            //                                if (ConditionLifetime(ref argIndex6) > 0)
            //                                {
            //                                    goto NextFeature;
            //                                }

            //                                string argcname = stype2 + "付加２";
            //                                AddCondition(ref argcname, -1, slevel2, ref sdata2);
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        // 属性使用不能攻撃により使用不能になった技能を封印する。
            //                        object argIndex8 = stype + "使用不能";
            //                        if (ConditionLifetime(ref argIndex8) > 0)
            //                        {
            //                            goto NextFeature;
            //                        }

            //                        string argcname1 = stype + "付加２";
            //                        AddCondition(ref argcname1, -1, slevel, ref sdata);
            //                    }

            //                    break;
            //                }

            //            case "パイロット能力強化":
            //                {
            //                    // 必要技能を満たしている？
            //                    Pilot argp2 = null;
            //                    if (!IsNecessarySkillSatisfied(ref fd.NecessarySkill, p: ref argp2))
            //                    {
            //                        found = true;
            //                        goto NextFeature;
            //                    }
            //                    // 必要条件を満たしている？
            //                    Pilot argp3 = null;
            //                    if (!IsNecessarySkillSatisfied(ref fd.NecessaryCondition, p: ref argp3))
            //                    {
            //                        found = true;
            //                        goto NextFeature;
            //                    }

            //                    flags[i] = true;

            //                    // 能力指定が「"」で囲まれている場合は「"」を削除
            //                    if (Strings.Asc(fd.StrData) == 34) // "
            //                    {
            //                        buf = Strings.Mid(fd.StrData, 2, Strings.Len(fd.StrData) - 2);
            //                    }
            //                    else
            //                    {
            //                        buf = fd.StrData;
            //                    }

            //                    // 強化する特殊能力の種類、レベル、データを解析
            //                    if (Strings.InStr(buf, "=") > 0)
            //                    {
            //                        sdata = Strings.Mid(buf, Strings.InStr(buf, "=") + 1);
            //                        buf = Strings.Left(buf, Strings.InStr(buf, "=") - 1);
            //                        if (Strings.InStr(buf, "Lv") > 0)
            //                        {
            //                            stype = Strings.Left(buf, Strings.InStr(buf, "Lv") - 1);
            //                            if (Information.IsNumeric(Strings.Mid(buf, Strings.InStr(buf, "Lv") + 2)))
            //                            {
            //                                slevel = Conversions.ToDouble(Strings.Mid(buf, Strings.InStr(buf, "Lv") + 2));
            //                            }
            //                            else
            //                            {
            //                                slevel = 1d;
            //                            }
            //                        }
            //                        else
            //                        {
            //                            stype = buf;
            //                            slevel = 1d;
            //                        }
            //                    }
            //                    else
            //                    {
            //                        sdata = "";
            //                        if (Strings.InStr(buf, "Lv") > 0)
            //                        {
            //                            stype = Strings.Left(buf, Strings.InStr(buf, "Lv") - 1);
            //                            if (Information.IsNumeric(Strings.Mid(buf, Strings.InStr(buf, "Lv") + 2)))
            //                            {
            //                                slevel = Conversions.ToDouble(Strings.Mid(buf, Strings.InStr(buf, "Lv") + 2));
            //                            }
            //                            else
            //                            {
            //                                slevel = 1d;
            //                            }
            //                        }
            //                        else
            //                        {
            //                            stype = buf;
            //                            slevel = 1d;
            //                        }
            //                    }

            //                    // エリアスが定義されている？
            //                    object argIndex17 = stype;
            //                    if (SRC.ALDList.IsDefined(ref argIndex17))
            //                    {
            //                        object argIndex14 = stype;
            //                        {
            //                            var withBlock5 = SRC.ALDList.Item(ref argIndex14);
            //                            var loopTo4 = withBlock5.Count;
            //                            for (j = 1; j <= loopTo4; j++)
            //                            {
            //                                // エリアスの定義に従って特殊能力定義を置き換える
            //                                stype2 = withBlock5.get_AliasType(j);

            //                                // 属性使用不能攻撃により使用不能になった技能を封印する。
            //                                object argIndex10 = stype2 + "使用不能";
            //                                if (ConditionLifetime(ref argIndex10) > 0)
            //                                {
            //                                    goto NextFeature;
            //                                }

            //                                string localLIndex1() { string arglist = withBlock5.get_AliasData(j); var ret = GeneralLib.LIndex(ref arglist, 1); withBlock5.get_AliasData(j) = arglist; return ret; }

            //                                if (localLIndex1() == "解説")
            //                                {
            //                                    // 特殊能力の解説
            //                                    if (!string.IsNullOrEmpty(sdata))
            //                                    {
            //                                        stype2 = GeneralLib.LIndex(ref sdata, 1);
            //                                    }

            //                                    slevel2 = SRC.DEFAULT_LEVEL;
            //                                    sdata2 = withBlock5.get_AliasData(j);
            //                                    // 属性使用不能攻撃により使用不能になった技能を封印する。
            //                                    object argIndex11 = stype2 + "使用不能";
            //                                    if (ConditionLifetime(ref argIndex11) > 0)
            //                                    {
            //                                        goto NextFeature;
            //                                    }

            //                                    string argcname2 = stype2 + "付加２";
            //                                    AddCondition(ref argcname2, -1, slevel2, ref sdata2);
            //                                }
            //                                else
            //                                {
            //                                    // 通常の能力
            //                                    if (withBlock5.get_AliasLevelIsMultMod(j))
            //                                    {
            //                                        if (slevel == SRC.DEFAULT_LEVEL)
            //                                        {
            //                                            slevel = 1d;
            //                                        }

            //                                        slevel2 = slevel * withBlock5.get_AliasLevel(j);
            //                                    }
            //                                    else if (slevel != SRC.DEFAULT_LEVEL)
            //                                    {
            //                                        slevel2 = slevel;
            //                                    }
            //                                    else
            //                                    {
            //                                        slevel2 = withBlock5.get_AliasLevel(j);
            //                                    }

            //                                    sdata2 = withBlock5.get_AliasData(j);
            //                                    if (!string.IsNullOrEmpty(sdata))
            //                                    {
            //                                        if (Strings.InStr(sdata2, "非表示") != 1)
            //                                        {
            //                                            sdata2 = sdata + " " + GeneralLib.ListTail(ref sdata2, (short)(GeneralLib.LLength(ref sdata) + 1));
            //                                        }
            //                                    }

            //                                    // 強化するレベルは累積する
            //                                    object argIndex13 = stype2 + "強化２";
            //                                    if (IsConditionSatisfied(ref argIndex13))
            //                                    {
            //                                        double localConditionLevel() { object argIndex1 = stype2 + "強化２"; var ret = ConditionLevel(ref argIndex1); return ret; }

            //                                        slevel2 = slevel2 + localConditionLevel();
            //                                        object argIndex12 = stype2 + "強化２";
            //                                        DeleteCondition(ref argIndex12);
            //                                    }

            //                                    string argcname3 = stype2 + "強化２";
            //                                    AddCondition(ref argcname3, -1, slevel2, ref sdata2);
            //                                }
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        // 強化するレベルは累積する
            //                        object argIndex16 = stype + "強化２";
            //                        if (IsConditionSatisfied(ref argIndex16))
            //                        {
            //                            double localConditionLevel1() { object argIndex1 = stype + "強化２"; var ret = ConditionLevel(ref argIndex1); return ret; }

            //                            slevel = slevel + localConditionLevel1();
            //                            object argIndex15 = stype + "強化２";
            //                            DeleteCondition(ref argIndex15);
            //                        }

            //                        string argcname4 = stype + "強化２";
            //                        AddCondition(ref argcname4, -1, slevel, ref sdata);
            //                    }

            //                    break;
            //                }
            //        }

            //    NextFeature:
            //        ;
            //        i = (short)(i + 1);
            //    }
            //    // 必要技能＆必要条件付きのパイロット能力付加＆強化がある場合は付加や強化の結果、
            //    // 必要技能＆必要条件が満たされることがあるので一度だけやり直す
            //    if (!flag & found)
            //    {
            //        flag = true;
            //        goto AddSkills;
            //    }

            //    // パイロット用特殊能力の付加＆強化が完了したので必要技能の判定が可能になった。
            //    UpdateFeatures();

            //    // アイテムが必要技能を満たすか再度チェック。
            //    found = false;
            //    foreach (Item currentItm1 in colItem)
            //    {
            //        itm = currentItm1;
            //        var argu1 = this;
            //        if (itm.Activated != itm.IsAvailable(ref argu1))
            //        {
            //            found = true;
            //            break;
            //        }
            //    }

            //    if (found)
            //    {
            //        // アイテムの使用可否が変化したので最初からやり直す
            //        goto TryAgain;
            //    }

            //    // ランクアップするか再度チェック。
            //    {
            //        var withBlock6 = Data;
            //        string argfname7 = "ランクアップ";
            //        if (withBlock6.IsFeatureAvailable(ref argfname7))
            //        {
            //            object argIndex19 = "ランクアップ";
            //            if (Rank >= withBlock6.FeatureLevel(ref argIndex19))
            //            {
            //                object argIndex18 = "ランクアップ";
            //                string argnabilities = withBlock6.FeatureNecessarySkill(ref argIndex18);
            //                Pilot argp4 = null;
            //                if (IsNecessarySkillSatisfied(ref argnabilities, p: ref argp4))
            //                {
            //                    // ランクアップが可能になったので最初からやり直す
            //                    goto TryAgain;
            //                }
            //            }
            //        }
            //    }

            //    if (CountPilot() > 0)
            //    {
            //        // パイロット能力をアップデート
            //        var loopTo5 = CountPilot();
            //        for (i = 2; i <= loopTo5; i++)
            //        {
            //            Pilot localPilot1() { object argIndex1 = i; var ret = Pilot(ref argIndex1); return ret; }

            //            localPilot1().Update();
            //        }

            //        var loopTo6 = CountSupport();
            //        for (i = 1; i <= loopTo6; i++)
            //        {
            //            Pilot localSupport1() { object argIndex1 = i; var ret = Support(ref argIndex1); return ret; }

            //            localSupport1().Update();
            //        }

            //        // メインパイロットは他のパイロットのサポートを受ける関係上
            //        // 最後にアップデートする
            //        object argIndex20 = 1;
            //        Pilot(ref argIndex20).Update();
            //        object argIndex21 = 1;
            //        object argIndex22 = 1;
            //        if (!ReferenceEquals(MainPilot(), Pilot(ref argIndex22)))
            //        {
            //            MainPilot().Update();
            //        }
            //    }

            //    // ユニット画像用ファイル名に変化がある場合はユニット画像を更新
            //    if (BitmapID != 0)
            //    {
            //        if ((ubitmap ?? "") != (get_Bitmap(false) ?? ""))
            //        {
            //            var argu2 = this;
            //            BitmapID = GUI.MakeUnitBitmap(ref argu2);
            //            var loopTo7 = CountOtherForm();
            //            for (i = 1; i <= loopTo7; i++)
            //            {
            //                Unit localOtherForm() { object argIndex1 = i; var ret = OtherForm(ref argIndex1); return ret; }

            //                localOtherForm().BitmapID = 0;
            //            }

            //            if (!without_refresh)
            //            {
            //                if (Status_Renamed == "出撃")
            //                {
            //                    if (!GUI.IsPictureVisible & !string.IsNullOrEmpty(Map.MapFileName))
            //                    {
            //                        var argu3 = this;
            //                        GUI.PaintUnitBitmap(ref argu3);
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // ユニットの表示、非表示が切り替わった場合
            //    string argfname9 = "非表示";
            //    if (is_invisible != IsFeatureAvailable(ref argfname9))
            //    {
            //        if (Status_Renamed == "出撃")
            //        {
            //            if (!GUI.IsPictureVisible & !string.IsNullOrEmpty(Map.MapFileName))
            //            {
            //                var argu4 = this;
            //                BitmapID = GUI.MakeUnitBitmap(ref argu4);
            //                string argfname8 = "非表示";
            //                if (IsFeatureAvailable(ref argfname8))
            //                {
            //                    GUI.EraseUnitBitmap(x, y, !without_refresh);
            //                }
            //                else if (!without_refresh)
            //                {
            //                    var argu5 = this;
            //                    GUI.PaintUnitBitmap(ref argu5);
            //                }
            //            }
            //        }
            //    }

            // 各種パラメータ
            lngMaxHP = Data.HP + 200 * Rank;
            intMaxEN = Data.EN + 10 * Rank;
            lngArmor = Data.Armor + 100 * Rank;
            intMobility = Data.Mobility + 5 * Rank;
            intSpeed = Data.Speed;

            //    // ボスランクによる修正
            //    string argoname1 = "等身大基準";
            //    if (IsHero() | Expression.IsOptionDefined(ref argoname1))
            //    {
            //        switch (BossRank)
            //        {
            //            case 1:
            //                {
            //                    lngMaxHP = lngMaxHP + Data.HP;
            //                    break;
            //                }

            //            case 2:
            //                {
            //                    lngMaxHP = lngMaxHP + Data.HP + 10000;
            //                    break;
            //                }

            //            case 3:
            //                {
            //                    lngMaxHP = lngMaxHP + Data.HP + 20000;
            //                    break;
            //                }

            //            case 4:
            //                {
            //                    lngMaxHP = lngMaxHP + Data.HP + 40000;
            //                    break;
            //                }

            //            case 5:
            //                {
            //                    lngMaxHP = lngMaxHP + Data.HP + 80000;
            //                    break;
            //                }
            //        }

            //        if (BossRank > 0)
            //        {
            //            lngArmor = lngArmor + 200 * BossRank;
            //        }
            //    }
            //    else
            //    {
            //        switch (BossRank)
            //        {
            //            case 1:
            //                {
            //                    lngMaxHP = (int)(lngMaxHP + 0.5d * Data.HP);
            //                    break;
            //                }

            //            case 2:
            //                {
            //                    lngMaxHP = lngMaxHP + Data.HP;
            //                    break;
            //                }

            //            case 3:
            //                {
            //                    lngMaxHP = lngMaxHP + Data.HP + 10000;
            //                    break;
            //                }

            //            case 4:
            //                {
            //                    lngMaxHP = lngMaxHP + Data.HP + 20000;
            //                    break;
            //                }

            //            case 5:
            //                {
            //                    lngMaxHP = lngMaxHP + Data.HP + 40000;
            //                    break;
            //                }
            //        }

            //        string argoname = "BossRank装甲修正低下";
            //        if (Expression.IsOptionDefined(ref argoname))
            //        {
            //            if (BossRank > 0)
            //            {
            //                lngArmor = lngArmor + 300 * BossRank;
            //            }
            //        }
            //        else
            //        {
            //            switch (BossRank)
            //            {
            //                case 1:
            //                    {
            //                        lngArmor = lngArmor + 300;
            //                        break;
            //                    }

            //                case 2:
            //                    {
            //                        lngArmor = lngArmor + 600;
            //                        break;
            //                    }

            //                case 3:
            //                    {
            //                        lngArmor = lngArmor + 1000;
            //                        break;
            //                    }

            //                case 4:
            //                    {
            //                        lngArmor = lngArmor + 1500;
            //                        break;
            //                    }

            //                case 5:
            //                    {
            //                        lngArmor = lngArmor + 2500;
            //                        break;
            //                    }
            //            }
            //        }
            //    }

            //    if (BossRank > 0)
            //    {
            //        intMaxEN = (short)(intMaxEN + 20 * BossRank);
            //        intMobility = (short)(intMobility + 5 * BossRank);
            //    }

            //    // ＨＰ成長オプション
            //    string argoname2 = "ＨＰ成長";
            //    if (Expression.IsOptionDefined(ref argoname2))
            //    {
            //        if (CountPilot() > 0)
            //        {
            //            lngMaxHP = GeneralLib.MinLng((int)(lngMaxHP / 100d * (100 + this.MainPilot().Level)), 9999999);
            //        }
            //    }

            //    // ＥＮ成長オプション
            //    string argoname3 = "ＥＮ成長";
            //    if (Expression.IsOptionDefined(ref argoname3))
            //    {
            //        if (CountPilot() > 0)
            //        {
            //            intMaxEN = (short)GeneralLib.MinLng((int)(intMaxEN / 100d * (100 + this.MainPilot().Level)), 9999);
            //        }
            //    }

            //    // 特殊能力による修正
            //    if (CountPilot() > 0)
            //    {
            //        pmorale = MainPilot().Morale;
            //    }
            //    else
            //    {
            //        pmorale = 100;
            //    }

            //    foreach (FeatureData currentFd2 in colFeature)
            //    {
            //        fd = currentFd2;
            //        switch (fd.Name ?? "")
            //        {
            //            // 固定値による強化
            //            case "ＨＰ強化":
            //                {
            //                    int localStrToLng() { string argexpr = GeneralLib.LIndex(ref fd.StrData, 2); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

            //                    if (pmorale >= localStrToLng())
            //                    {
            //                        lngMaxHP = (int)(lngMaxHP + 200d * fd.Level);
            //                    }

            //                    break;
            //                }

            //            case "ＥＮ強化":
            //                {
            //                    int localStrToLng1() { string argexpr = GeneralLib.LIndex(ref fd.StrData, 2); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

            //                    if (pmorale >= localStrToLng1())
            //                    {
            //                        intMaxEN = (short)(intMaxEN + 10d * fd.Level);
            //                    }

            //                    break;
            //                }

            //            case "装甲強化":
            //                {
            //                    int localStrToLng2() { string argexpr = GeneralLib.LIndex(ref fd.StrData, 2); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

            //                    if (pmorale >= localStrToLng2())
            //                    {
            //                        lngArmor = (int)(lngArmor + 100d * fd.Level);
            //                    }

            //                    break;
            //                }

            //            case "運動性強化":
            //                {
            //                    int localStrToLng3() { string argexpr = GeneralLib.LIndex(ref fd.StrData, 2); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

            //                    if (pmorale >= localStrToLng3())
            //                    {
            //                        intMobility = (short)(intMobility + 5d * fd.Level);
            //                    }

            //                    break;
            //                }

            //            case "移動力強化":
            //                {
            //                    int localStrToLng4() { string argexpr = GeneralLib.LIndex(ref fd.StrData, 2); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

            //                    if (pmorale >= localStrToLng4())
            //                    {
            //                        intSpeed = (short)(intSpeed + fd.Level);
            //                    }

            //                    break;
            //                }
            //            // 割合による強化
            //            case "ＨＰ割合強化":
            //                {
            //                    int localStrToLng5() { string argexpr = GeneralLib.LIndex(ref fd.StrData, 2); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

            //                    if (pmorale >= localStrToLng5())
            //                    {
            //                        lngMaxHP = (int)(lngMaxHP + (long)(Data.HP * fd.Level) / 20L);
            //                    }

            //                    break;
            //                }

            //            case "ＥＮ割合強化":
            //                {
            //                    int localStrToLng6() { string argexpr = GeneralLib.LIndex(ref fd.StrData, 2); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

            //                    if (pmorale >= localStrToLng6())
            //                    {
            //                        intMaxEN = (short)(intMaxEN + (long)(Data.EN * fd.Level) / 20L);
            //                    }

            //                    break;
            //                }

            //            case "装甲割合強化":
            //                {
            //                    int localStrToLng7() { string argexpr = GeneralLib.LIndex(ref fd.StrData, 2); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

            //                    if (pmorale >= localStrToLng7())
            //                    {
            //                        lngArmor = (int)(lngArmor + (long)(Data.Armor * fd.Level) / 20L);
            //                    }

            //                    break;
            //                }

            //            case "運動性割合強化":
            //                {
            //                    int localStrToLng8() { string argexpr = GeneralLib.LIndex(ref fd.StrData, 2); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

            //                    if (pmorale >= localStrToLng8())
            //                    {
            //                        intMobility = (short)(intMobility + (long)(Data.Mobility * fd.Level) / 20L);
            //                    }

            //                    break;
            //                }
            //        }
            //    }

            //    // アイテムによる修正
            //    foreach (Item currentItm2 in colItem)
            //    {
            //        itm = currentItm2;
            //        if (itm.Activated)
            //        {
            //            lngMaxHP = lngMaxHP + itm.HP();
            //            intMaxEN = (short)(intMaxEN + itm.EN());
            //            lngArmor = lngArmor + itm.Armor();
            //            intMobility = (short)(intMobility + itm.Mobility());
            //            intSpeed = (short)(intSpeed + itm.Speed());
            //        }
            //    }

            //    // 装備している「Ｖ－ＵＰ=ユニット」アイテムによる修正
            //    num = 0;
            //    object argIndex24 = "Ｖ－ＵＰ";
            //    if (IsConditionSatisfied(ref argIndex24))
            //    {
            //        object argIndex23 = "Ｖ－ＵＰ";
            //        switch (FeatureData(ref argIndex23) ?? "")
            //        {
            //            case "全":
            //            case "ユニット":
            //                {
            //                    num = (short)(num + 1);
            //                    break;
            //                }
            //        }
            //    }

            //    foreach (Item currentItm3 in colItem)
            //    {
            //        itm = currentItm3;
            //        string argfname10 = "Ｖ－ＵＰ";
            //        if (itm.IsFeatureAvailable(ref argfname10))
            //        {
            //            object argIndex25 = "Ｖ－ＵＰ";
            //            switch (itm.FeatureData(ref argIndex25) ?? "")
            //            {
            //                case "全":
            //                case "ユニット":
            //                    {
            //                        num = (short)(num + 1);
            //                        break;
            //                    }
            //            }
            //        }
            //    }

            //    if (CountPilot() > 0)
            //    {
            //        {
            //            var withBlock8 = MainPilot().Data;
            //            string argfname11 = "Ｖ－ＵＰ";
            //            if (withBlock8.IsFeatureAvailable(ref argfname11))
            //            {
            //                object argIndex26 = "Ｖ－ＵＰ";
            //                switch (withBlock8.FeatureData(ref argIndex26) ?? "")
            //                {
            //                    case "全":
            //                    case "ユニット":
            //                        {
            //                            num = (short)(num + 1);
            //                            break;
            //                        }
            //                }
            //            }
            //        }
            //    }

            //    if (num > 0)
            //    {
            //        {
            //            var withBlock9 = Data;
            //            lngMaxHP = lngMaxHP + 100 * num * (withBlock9.ItemNum + 1);
            //            intMaxEN = (short)(intMaxEN + 20 * num * withBlock9.ItemNum);
            //            lngArmor = lngArmor + 50 * num * withBlock9.ItemNum;
            //            intMobility = (short)(intMobility + 5 * num * withBlock9.ItemNum);
            //        }
            //    }

            //    // 追加移動力
            //    string argfname12 = "追加移動力";
            //    if (IsFeatureAvailable(ref argfname12))
            //    {
            //        foreach (FeatureData currentFd3 in colFeature)
            //        {
            //            fd = currentFd3;
            //            if (fd.Name == "追加移動力")
            //            {
            //                if ((Area ?? "") == (GeneralLib.LIndex(ref fd.StrData, 2) ?? ""))
            //                {
            //                    intSpeed = (short)(intSpeed + fd.Level);
            //                }
            //            }
            //        }

            //        intSpeed = (short)GeneralLib.MaxLng(intSpeed, 0);
            //    }

            //    // 上限値を超えないように
            //    lngMaxHP = GeneralLib.MinLng(lngMaxHP, 9999999);
            //    intMaxEN = (short)GeneralLib.MinLng(intMaxEN, 9999);
            //    lngArmor = GeneralLib.MinLng(lngArmor, 99999);
            //    intMobility = (short)GeneralLib.MinLng(intMobility, 9999);
            //    intSpeed = (short)GeneralLib.MinLng(intSpeed, 99);

            //    // ＨＰ、ＥＮの最大値の変動に対応
            //    HP = (int)(MaxHP * hp_ratio / 100d);
            //    EN = (int)(MaxEN * en_ratio / 100d);

            //    // 切り下げの結果ＨＰが0になることを防ぐ
            //    if (hp_ratio > 0d)
            //    {
            //        if (HP == 0)
            //        {
            //            HP = 1;
            //        }
            //    }

            //    // 地形適応
            //    for (i = 1; i <= 4; i++)
            //    {
            //        switch (Strings.Mid(Data.Adaption, i, 1) ?? "")
            //        {
            //            case "S":
            //                {
            //                    uadaption[i] = 5;
            //                    break;
            //                }

            //            case "A":
            //                {
            //                    uadaption[i] = 4;
            //                    break;
            //                }

            //            case "B":
            //                {
            //                    uadaption[i] = 3;
            //                    break;
            //                }

            //            case "C":
            //                {
            //                    uadaption[i] = 2;
            //                    break;
            //                }

            //            case "D":
            //                {
            //                    uadaption[i] = 1;
            //                    break;
            //                }

            //            case "E":
            //            case "-":
            //                {
            //                    uadaption[i] = 0;
            //                    break;
            //                }
            //        }
            //    }

            //    // 移動タイプ追加による地形適応修正
            //    string argfname13 = "空中移動";
            //    if (IsFeatureAvailable(ref argfname13))
            //    {
            //        uadaption[1] = (short)GeneralLib.MaxLng(uadaption[1], 4);
            //    }

            //    string argfname14 = "陸上移動";
            //    if (IsFeatureAvailable(ref argfname14))
            //    {
            //        uadaption[2] = (short)GeneralLib.MaxLng(uadaption[2], 4);
            //    }

            //    string argfname15 = "水中移動";
            //    if (IsFeatureAvailable(ref argfname15))
            //    {
            //        uadaption[3] = (short)GeneralLib.MaxLng(uadaption[3], 4);
            //    }

            //    string argfname16 = "宇宙移動";
            //    if (IsFeatureAvailable(ref argfname16))
            //    {
            //        uadaption[4] = (short)GeneralLib.MaxLng(uadaption[4], 4);
            //    }

            //    // 地形適応変更能力による修正
            //    foreach (FeatureData currentFd4 in colFeature)
            //    {
            //        fd = currentFd4;
            //        switch (fd.Name ?? "")
            //        {
            //            case "地形適応変更":
            //                {
            //                    for (i = 1; i <= 4; i++)
            //                    {
            //                        string argexpr = GeneralLib.LIndex(ref fd.StrData, i);
            //                        num = (short)GeneralLib.StrToLng(ref argexpr);
            //                        if (num > 0)
            //                        {
            //                            if (uadaption[i] < 4)
            //                            {
            //                                uadaption[i] = (short)(uadaption[i] + num);
            //                                // 地形適応はAより高くはならない
            //                                if (uadaption[i] > 4)
            //                                {
            //                                    uadaption[i] = 4;
            //                                }
            //                            }
            //                        }
            //                        else
            //                        {
            //                            uadaption[i] = (short)(uadaption[i] + num);
            //                        }
            //                    }

            //                    break;
            //                }

            //            case "地形適応固定変更":
            //                {
            //                    for (i = 1; i <= 4; i++)
            //                    {
            //                        string argexpr1 = GeneralLib.LIndex(ref fd.StrData, i);
            //                        num = (short)GeneralLib.StrToLng(ref argexpr1);
            //                        if (GeneralLib.LIndex(ref fd.StrData, 5) == "強制")
            //                        {
            //                            // 強制変更の場合
            //                            if (num >= 0 & num <= 5)
            //                            {
            //                                uadaption[i] = num;
            //                            }
            //                        }
            //                        // 高いほうを優先する場合
            //                        else if (num > uadaption[i] & num <= 5)
            //                        {
            //                            uadaption[i] = num;
            //                        }
            //                    }

            //                    break;
            //                }
            //        }
            //    }

            //    strAdaption = "";
            //    for (i = 1; i <= 4; i++)
            //    {
            //        switch (uadaption[i])
            //        {
            //            case var @case when @case >= 5:
            //                {
            //                    strAdaption = strAdaption + "S";
            //                    break;
            //                }

            //            case 4:
            //                {
            //                    strAdaption = strAdaption + "A";
            //                    break;
            //                }

            //            case 3:
            //                {
            //                    strAdaption = strAdaption + "B";
            //                    break;
            //                }

            //            case 2:
            //                {
            //                    strAdaption = strAdaption + "C";
            //                    break;
            //                }

            //            case 1:
            //                {
            //                    strAdaption = strAdaption + "D";
            //                    break;
            //                }

            //            case var case1 when case1 <= 0:
            //                {
            //                    strAdaption = strAdaption + "-";
            //                    break;
            //                }
            //        }
            //    }

            //    // 空中に留まることが出来るかチェック
            //    string argarea_name1 = "空";
            //    if (Status_Renamed == "出撃" & Area == "空中" & !IsTransAvailable(ref argarea_name1))
            //    {
            //        // 地上(水中)に戻す
            //        switch (Map.TerrainClass(x, y) ?? "")
            //        {
            //            case "陸":
            //            case "屋内":
            //                {
            //                    Area = "地上";
            //                    break;
            //                }

            //            case "水":
            //            case "深水":
            //                {
            //                    string argarea_name = "水上";
            //                    if (IsTransAvailable(ref argarea_name))
            //                    {
            //                        Area = "水上";
            //                    }
            //                    else
            //                    {
            //                        Area = "水中";
            //                    }

            //                    break;
            //                }
            //        }

            //        if (!without_refresh)
            //        {
            //            if (!GUI.IsPictureVisible & !string.IsNullOrEmpty(Map.MapFileName))
            //            {
            //                var argu6 = this;
            //                GUI.PaintUnitBitmap(ref argu6);
            //            }
            //        }
            //    }

            //    // 攻撃への耐性を更新
            //    strAbsorb = "";
            //    strImmune = "";
            //    strResist = "";
            //    strWeakness = "";
            //    strEffective = "";
            //    strSpecialEffectImmune = "";
            //    // 特殊能力によって得られた耐性
            //    foreach (FeatureData currentFd5 in colFeature)
            //    {
            //        fd = currentFd5;
            //        switch (fd.Name ?? "")
            //        {
            //            case "吸収":
            //                {
            //                    strAbsorb = strAbsorb + fd.StrData;
            //                    break;
            //                }

            //            case "無効化":
            //                {
            //                    strImmune = strImmune + fd.StrData;
            //                    break;
            //                }

            //            case "耐性":
            //                {
            //                    strResist = strResist + fd.StrData;
            //                    break;
            //                }

            //            case "弱点":
            //                {
            //                    strWeakness = strWeakness + fd.StrData;
            //                    break;
            //                }

            //            case "有効":
            //                {
            //                    strEffective = strEffective + fd.StrData;
            //                    break;
            //                }

            //            case "特殊効果無効化":
            //                {
            //                    strSpecialEffectImmune = strSpecialEffectImmune + fd.StrData;
            //                    break;
            //                }
            //        }
            //    }
            //    // 弱点、有効付加属性攻撃による弱点、有効の付加
            //    var loopTo8 = CountCondition();
            //    for (i = 1; i <= loopTo8; i++)
            //    {
            //        object argIndex28 = i;
            //        if (ConditionLifetime(ref argIndex28) != 0)
            //        {
            //            object argIndex27 = i;
            //            ch = Condition(ref argIndex27);
            //            switch (Strings.Right(ch, 6) ?? "")
            //            {
            //                case "属性弱点付加":
            //                    {
            //                        strWeakness = strWeakness + Strings.Left(ch, Strings.Len(ch) - 6);
            //                        break;
            //                    }

            //                case "属性有効付加":
            //                    {
            //                        strEffective = strEffective + Strings.Left(ch, Strings.Len(ch) - 6);
            //                        break;
            //                    }
            //            }
            //        }
            //    }
            //    // 属性のダブりをなくす
            //    buf = "";
            //    var loopTo9 = (short)Strings.Len(strAbsorb);
            //    for (i = 1; i <= loopTo9; i++)
            //    {
            //        ch = GeneralLib.GetClassBundle(ref strAbsorb, ref i);
            //        if (GeneralLib.InStrNotNest(ref buf, ref ch) == 0)
            //        {
            //            buf = buf + ch;
            //        }
            //    }

            //    strAbsorb = buf;
            //    buf = "";
            //    var loopTo10 = (short)Strings.Len(strImmune);
            //    for (i = 1; i <= loopTo10; i++)
            //    {
            //        ch = GeneralLib.GetClassBundle(ref strImmune, ref i);
            //        if (GeneralLib.InStrNotNest(ref buf, ref ch) == 0)
            //        {
            //            buf = buf + ch;
            //        }
            //    }

            //    strImmune = buf;
            //    buf = "";
            //    var loopTo11 = (short)Strings.Len(strResist);
            //    for (i = 1; i <= loopTo11; i++)
            //    {
            //        ch = GeneralLib.GetClassBundle(ref strResist, ref i);
            //        if (GeneralLib.InStrNotNest(ref buf, ref ch) == 0)
            //        {
            //            buf = buf + ch;
            //        }
            //    }

            //    strResist = buf;
            //    buf = "";
            //    var loopTo12 = (short)Strings.Len(strWeakness);
            //    for (i = 1; i <= loopTo12; i++)
            //    {
            //        ch = GeneralLib.GetClassBundle(ref strWeakness, ref i);
            //        if (GeneralLib.InStrNotNest(ref buf, ref ch) == 0)
            //        {
            //            buf = buf + ch;
            //        }
            //    }

            //    strWeakness = buf;
            //    buf = "";
            //    var loopTo13 = (short)Strings.Len(strEffective);
            //    for (i = 1; i <= loopTo13; i++)
            //    {
            //        ch = GeneralLib.GetClassBundle(ref strEffective, ref i);
            //        if (GeneralLib.InStrNotNest(ref buf, ref ch) == 0)
            //        {
            //            buf = buf + ch;
            //        }
            //    }

            //    strEffective = buf;
            //    buf = "";
            //    var loopTo14 = (short)Strings.Len(strSpecialEffectImmune);
            //    for (i = 1; i <= loopTo14; i++)
            //    {
            //        ch = GeneralLib.GetClassBundle(ref strSpecialEffectImmune, ref i);
            //        if (GeneralLib.InStrNotNest(ref buf, ref ch) == 0)
            //        {
            //            buf = buf + ch;
            //        }
            //    }

            //    strSpecialEffectImmune = buf;

            // 武器データを更新
            // XXX
            WData = Data.Weapons.Select(x => new UnitWeapon(SRC, this, x)).ToList();
            //    prev_wdata = new WeaponData[Information.UBound(WData) + 1];
            //    prev_wbullets = new double[Information.UBound(WData) + 1];
            //    var loopTo15 = (short)Information.UBound(WData);
            //    for (i = 1; i <= loopTo15; i++)
            //    {
            //        prev_wdata[i] = WData[i];
            //        prev_wbullets[i] = dblBullet[i];
            //    }

            //    {
            //        var withBlock10 = Data;
            //        WData = new WeaponData[(withBlock10.CountWeapon() + 1)];
            //        var loopTo16 = withBlock10.CountWeapon();
            //        for (i = 1; i <= loopTo16; i++)
            //        {
            //            object argIndex29 = i;
            //            WData[i] = withBlock10.Weapon(ref argIndex29);
            //        }
            //    }

            //    if (CountPilot() > 0)
            //    {
            //        {
            //            var withBlock11 = MainPilot().Data;
            //            var loopTo17 = withBlock11.CountWeapon();
            //            for (i = 1; i <= loopTo17; i++)
            //            {
            //                Array.Resize(ref WData, Information.UBound(WData) + 1 + 1);
            //                object argIndex30 = i;
            //                WData[Information.UBound(WData)] = withBlock11.Weapon(ref argIndex30);
            //            }
            //        }

            //        var loopTo18 = CountPilot();
            //        for (i = 2; i <= loopTo18; i++)
            //        {
            //            Pilot localPilot2() { object argIndex1 = i; var ret = Pilot(ref argIndex1); return ret; }

            //            {
            //                var withBlock12 = localPilot2().Data;
            //                var loopTo19 = withBlock12.CountWeapon();
            //                for (j = 1; j <= loopTo19; j++)
            //                {
            //                    Array.Resize(ref WData, Information.UBound(WData) + 1 + 1);
            //                    object argIndex31 = j;
            //                    WData[Information.UBound(WData)] = withBlock12.Weapon(ref argIndex31);
            //                }
            //            }
            //        }

            //        var loopTo20 = CountSupport();
            //        for (i = 1; i <= loopTo20; i++)
            //        {
            //            Pilot localSupport2() { object argIndex1 = i; var ret = Support(ref argIndex1); return ret; }

            //            {
            //                var withBlock13 = localSupport2().Data;
            //                var loopTo21 = withBlock13.CountWeapon();
            //                for (j = 1; j <= loopTo21; j++)
            //                {
            //                    Array.Resize(ref WData, Information.UBound(WData) + 1 + 1);
            //                    object argIndex32 = j;
            //                    WData[Information.UBound(WData)] = withBlock13.Weapon(ref argIndex32);
            //                }
            //            }
            //        }

            //        string argfname17 = "追加サポート";
            //        if (IsFeatureAvailable(ref argfname17))
            //        {
            //            {
            //                var withBlock14 = AdditionalSupport().Data;
            //                var loopTo22 = withBlock14.CountWeapon();
            //                for (i = 1; i <= loopTo22; i++)
            //                {
            //                    Array.Resize(ref WData, Information.UBound(WData) + 1 + 1);
            //                    object argIndex33 = i;
            //                    WData[Information.UBound(WData)] = withBlock14.Weapon(ref argIndex33);
            //                }
            //            }
            //        }
            //    }

            //    foreach (Item currentItm4 in colItem)
            //    {
            //        itm = currentItm4;
            //        if (itm.Activated)
            //        {
            //            var loopTo23 = itm.CountWeapon();
            //            for (i = 1; i <= loopTo23; i++)
            //            {
            //                Array.Resize(ref WData, Information.UBound(WData) + 1 + 1);
            //                object argIndex34 = i;
            //                WData[Information.UBound(WData)] = itm.Weapon(ref argIndex34);
            //            }
            //        }
            //    }

            //    // 武器属性を更新
            //    strWeaponClass = new string[(CountWeapon() + 1)];
            //    var loopTo24 = CountWeapon();
            //    for (i = 1; i <= loopTo24; i++)
            //        strWeaponClass[i] = Weapon(i).Class_Renamed;
            //    string hidden_attr;
            //    bool skipped;
            //    string argfname18 = "攻撃属性";
            //    if (IsFeatureAvailable(ref argfname18))
            //    {
            //        var loopTo25 = CountWeapon();
            //        for (i = 1; i <= loopTo25; i++)
            //        {
            //            {
            //                var withBlock15 = Weapon(i);
            //                wname = withBlock15.Name;
            //                wnickname = WeaponNickname(i);
            //                wnskill = withBlock15.NecessarySkill;
            //            }

            //            wclass = strWeaponClass[i];

            //            // 非表示の属性がある場合は一旦抜き出す
            //            string argstring22 = "|";
            //            if (GeneralLib.InStrNotNest(ref wclass, ref argstring22) > 0)
            //            {
            //                string argstring2 = "|";
            //                strWeaponClass[i] = Strings.Left(wclass, GeneralLib.InStrNotNest(ref wclass, ref argstring2) - 1);
            //                string argstring21 = "|";
            //                hidden_attr = Strings.Mid(wclass, GeneralLib.InStrNotNest(ref wclass, ref argstring21) + 1);
            //            }
            //            else
            //            {
            //                hidden_attr = "";
            //            }

            //            var loopTo26 = CountFeature();
            //            for (j = 1; j <= loopTo26; j++)
            //            {
            //                object argIndex36 = j;
            //                if (Feature(ref argIndex36) == "攻撃属性")
            //                {
            //                    object argIndex35 = j;
            //                    fdata = FeatureData(ref argIndex35);

            //                    // 「"」を除去
            //                    if (Strings.Left(fdata, 1) == "\"")
            //                    {
            //                        fdata = Strings.Mid(fdata, 2, Strings.Len(fdata) - 2);
            //                    }

            //                    flen = GeneralLib.LLength(ref fdata);
            //                    if (flen == 1)
            //                    {
            //                        // 武器指定がない場合はすべての武器に属性を付加
            //                        flag = true;
            //                        k = 2;
            //                    }
            //                    else if (GeneralLib.LIndex(ref fdata, 1) == "非表示")
            //                    {
            //                        // 非表示指定がある場合 (武器指定がある場合を含む)
            //                        if (flen == 2)
            //                        {
            //                            // 武器指定無し
            //                            flag = true;
            //                        }
            //                        else
            //                        {
            //                            // 武器指定あり
            //                            flag = false;
            //                        }

            //                        k = 3;
            //                    }
            //                    else
            //                    {
            //                        // 武器指定がある場合
            //                        flag = false;
            //                        k = 2;
            //                    }

            //                    // 武器指定がある場合はそれぞれの指定をチェック
            //                    false_count = 0;
            //                    while (k <= flen)
            //                    {
            //                        wtype = GeneralLib.LIndex(ref fdata, k);
            //                        if (Strings.Left(wtype, 1) == "!")
            //                        {
            //                            wtype = Strings.Mid(wtype, 2);
            //                            with_not = true;
            //                        }
            //                        else
            //                        {
            //                            with_not = false;
            //                        }

            //                        found = false;
            //                        switch (wtype ?? "")
            //                        {
            //                            case "全":
            //                                {
            //                                    found = true;
            //                                    break;
            //                                }

            //                            case "物":
            //                                {
            //                                    string argstring23 = "魔";
            //                                    string argstring24 = "魔武";
            //                                    string argstring25 = "魔突";
            //                                    string argstring26 = "魔接";
            //                                    string argstring27 = "魔銃";
            //                                    string argstring28 = "魔実";
            //                                    if (GeneralLib.InStrNotNest(ref wclass, ref argstring23) == 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring24) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring25) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring26) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring27) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring28) > 0)
            //                                    {
            //                                        found = true;
            //                                    }

            //                                    break;
            //                                }

            //                            default:
            //                                {
            //                                    if (GeneralLib.InStrNotNest(ref wclass, ref wtype) > 0 | (wname ?? "") == (wtype ?? "") | (wnickname ?? "") == (wtype ?? ""))
            //                                    {
            //                                        found = true;
            //                                    }
            //                                    else
            //                                    {
            //                                        var loopTo27 = GeneralLib.LLength(ref wnskill);
            //                                        for (l = 1; l <= loopTo27; l++)
            //                                        {
            //                                            sname = GeneralLib.LIndex(ref wnskill, l);
            //                                            if (Strings.InStr(sname, "Lv") > 0)
            //                                            {
            //                                                sname = Strings.Left(sname, Strings.InStr(sname, "Lv") - 1);
            //                                            }

            //                                            if ((sname ?? "") == (wtype ?? ""))
            //                                            {
            //                                                found = true;
            //                                                break;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (with_not)
            //                        {
            //                            // !指定あり
            //                            if (found)
            //                            {
            //                                // 条件を満たした場合は適用しない
            //                                flag = false;
            //                                false_count = (short)(false_count + 1);
            //                            }
            //                        }
            //                        else if (found)
            //                        {
            //                            // !指定無しの条件を満たした
            //                            flag = true;
            //                        }
            //                        else
            //                        {
            //                            // !指定無しの条件を満たさず
            //                            false_count = (short)(false_count + 1);
            //                        }

            //                        k = (short)(k + 1);
            //                    }

            //                    // 属性を追加
            //                    if (flag | false_count == 0)
            //                    {
            //                        buf = GeneralLib.LIndex(ref fdata, 1);
            //                        if (buf == "非表示")
            //                        {
            //                            // 非表示の属性の場合
            //                            hidden_attr = hidden_attr + GeneralLib.LIndex(ref fdata, 2);
            //                        }
            //                        else
            //                        {
            //                            // 属性が重複しないように付加
            //                            skipped = false;
            //                            var loopTo28 = (short)Strings.Len(buf);
            //                            for (k = 1; k <= loopTo28; k++)
            //                            {
            //                                ch = GeneralLib.GetClassBundle(ref buf, ref k);
            //                                if (!Information.IsNumeric(ch) & ch != "L" & ch != ".")
            //                                {
            //                                    skipped = false;
            //                                }

            //                                if ((GeneralLib.InStrNotNest(ref strWeaponClass[i], ref ch) == 0 | Information.IsNumeric(ch) | ch == "L" | ch == ".") & !skipped)
            //                                {
            //                                    if (ch == "魔")
            //                                    {
            //                                        // 魔属性を付加する場合は武器を魔法武器化する
            //                                        string argstring29 = "武";
            //                                        l = GeneralLib.InStrNotNest(ref strWeaponClass[i], ref argstring29);
            //                                        string argstring210 = "突";
            //                                        l = (short)GeneralLib.MaxLng(GeneralLib.InStrNotNest(ref strWeaponClass[i], ref argstring210), l);
            //                                        string argstring211 = "接";
            //                                        l = (short)GeneralLib.MaxLng(GeneralLib.InStrNotNest(ref strWeaponClass[i], ref argstring211), l);
            //                                        string argstring212 = "銃";
            //                                        l = (short)GeneralLib.MaxLng(GeneralLib.InStrNotNest(ref strWeaponClass[i], ref argstring212), l);
            //                                        string argstring213 = "実";
            //                                        l = (short)GeneralLib.MaxLng(GeneralLib.InStrNotNest(ref strWeaponClass[i], ref argstring213), l);
            //                                        if (l > 0)
            //                                        {
            //                                            strWeaponClass[i] = Strings.Left(strWeaponClass[i], l - 1) + ch + Strings.Mid(strWeaponClass[i], l);
            //                                        }
            //                                        else
            //                                        {
            //                                            strWeaponClass[i] = strWeaponClass[i] + ch;
            //                                        }
            //                                    }
            //                                    else
            //                                    {
            //                                        strWeaponClass[i] = strWeaponClass[i] + ch;
            //                                    }
            //                                }
            //                                else
            //                                {
            //                                    skipped = true;
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }

            //            // 非表示の属性を追加
            //            if (Strings.Len(hidden_attr) > 0)
            //            {
            //                strWeaponClass[i] = strWeaponClass[i] + "|" + hidden_attr;
            //            }
            //        }
            //    }

            //    // 武器攻撃力を更新
            //    lngWeaponPower = new int[(CountWeapon() + 1)];

            //    // 装備している「Ｖ－ＵＰ=武器」アイテムの個数をカウントしておく
            //    num = 0;
            //    object argIndex38 = "Ｖ－ＵＰ";
            //    if (IsConditionSatisfied(ref argIndex38))
            //    {
            //        object argIndex37 = "Ｖ－ＵＰ";
            //        switch (FeatureData(ref argIndex37) ?? "")
            //        {
            //            case "全":
            //            case "武器":
            //                {
            //                    num = (short)(num + 1);
            //                    break;
            //                }
            //        }
            //    }

            //    foreach (Item currentItm5 in colItem)
            //    {
            //        itm = currentItm5;
            //        if (itm.Activated)
            //        {
            //            string argfname19 = "Ｖ－ＵＰ";
            //            if (itm.IsFeatureAvailable(ref argfname19))
            //            {
            //                object argIndex39 = "Ｖ－ＵＰ";
            //                switch (itm.FeatureData(ref argIndex39) ?? "")
            //                {
            //                    case "全":
            //                    case "武器":
            //                        {
            //                            num = (short)(num + 1);
            //                            break;
            //                        }
            //                }
            //            }
            //        }
            //    }

            //    if (CountPilot() > 0)
            //    {
            //        {
            //            var withBlock16 = MainPilot().Data;
            //            string argfname20 = "Ｖ－ＵＰ";
            //            if (withBlock16.IsFeatureAvailable(ref argfname20))
            //            {
            //                object argIndex40 = "Ｖ－ＵＰ";
            //                switch (withBlock16.FeatureData(ref argIndex40) ?? "")
            //                {
            //                    case "全":
            //                    case "武器":
            //                        {
            //                            num = (short)(num + 1);
            //                            break;
            //                        }
            //                }
            //            }
            //        }
            //    }

            //    num = (short)(num * Data.ItemNum);
            //    var loopTo29 = CountWeapon();
            //    for (i = 1; i <= loopTo29; i++)
            //    {
            //        lngWeaponPower[i] = Weapon(i).Power;

            //        // もともと攻撃力が0の武器は0に固定
            //        if (lngWeaponPower[i] == 0)
            //        {
            //            goto NextWeapon;
            //        }

            //        // 武器強化による修正
            //        string argfname21 = "武器強化";
            //        if (IsFeatureAvailable(ref argfname21))
            //        {
            //            {
            //                var withBlock17 = Weapon(i);
            //                wname = withBlock17.Name;
            //                wnickname = WeaponNickname(i);
            //                wnskill = withBlock17.NecessarySkill;
            //            }

            //            wclass = strWeaponClass[i];
            //            var loopTo30 = CountFeature();
            //            for (j = 1; j <= loopTo30; j++)
            //            {
            //                object argIndex42 = j;
            //                if (Feature(ref argIndex42) == "武器強化")
            //                {
            //                    object argIndex41 = j;
            //                    fdata = FeatureData(ref argIndex41);

            //                    // 「"」を除去
            //                    if (Strings.Left(fdata, 1) == "\"")
            //                    {
            //                        fdata = Strings.Mid(fdata, 2, Strings.Len(fdata) - 2);
            //                    }

            //                    flen = GeneralLib.LLength(ref fdata);
            //                    flag = false;

            //                    // 武器指定がない場合はすべての武器を強化
            //                    if (flen == 0)
            //                    {
            //                        flag = true;
            //                    }

            //                    // 武器指定がある場合はそれぞれの指定をチェック
            //                    false_count = 0;
            //                    var loopTo31 = flen;
            //                    for (k = 1; k <= loopTo31; k++)
            //                    {
            //                        wtype = GeneralLib.LIndex(ref fdata, k);
            //                        if (Strings.Left(wtype, 1) == "!")
            //                        {
            //                            wtype = Strings.Mid(wtype, 2);
            //                            with_not = true;
            //                        }
            //                        else
            //                        {
            //                            with_not = false;
            //                        }

            //                        found = false;
            //                        string argattr = "固";
            //                        if (IsWeaponClassifiedAs(i, ref argattr))
            //                        {
            //                            // ダメージ固定武器は武器指定が武器名、武器表示名、「固」の
            //                            // いずれかで行われた場合にのみ強化
            //                            if (wtype == "固" | (wname ?? "") == (wtype ?? "") | (wnickname ?? "") == (wtype ?? ""))
            //                            {
            //                                found = true;
            //                            }
            //                        }
            //                        else
            //                        {
            //                            switch (wtype ?? "")
            //                            {
            //                                case "全":
            //                                    {
            //                                        found = true;
            //                                        break;
            //                                    }

            //                                case "物":
            //                                    {
            //                                        string argstring214 = "魔";
            //                                        string argstring215 = "魔武";
            //                                        string argstring216 = "魔突";
            //                                        string argstring217 = "魔接";
            //                                        string argstring218 = "魔銃";
            //                                        string argstring219 = "魔実";
            //                                        if (GeneralLib.InStrNotNest(ref wclass, ref argstring214) == 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring215) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring216) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring217) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring218) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring219) > 0)
            //                                        {
            //                                            found = true;
            //                                        }

            //                                        break;
            //                                    }

            //                                default:
            //                                    {
            //                                        if (GeneralLib.InStrNotNest(ref wclass, ref wtype) > 0 | (wname ?? "") == (wtype ?? "") | (wnickname ?? "") == (wtype ?? ""))
            //                                        {
            //                                            found = true;
            //                                        }
            //                                        else
            //                                        {
            //                                            // 必要技能による指定
            //                                            var loopTo32 = GeneralLib.LLength(ref wnskill);
            //                                            for (l = 1; l <= loopTo32; l++)
            //                                            {
            //                                                sname = GeneralLib.LIndex(ref wnskill, l);
            //                                                if (Strings.InStr(sname, "Lv") > 0)
            //                                                {
            //                                                    sname = Strings.Left(sname, Strings.InStr(sname, "Lv") - 1);
            //                                                }

            //                                                if ((sname ?? "") == (wtype ?? ""))
            //                                                {
            //                                                    found = true;
            //                                                    break;
            //                                                }
            //                                            }
            //                                        }

            //                                        break;
            //                                    }
            //                            }
            //                        }

            //                        if (with_not)
            //                        {
            //                            // !指定あり
            //                            if (found)
            //                            {
            //                                // 条件を満たした場合は適用しない
            //                                flag = false;
            //                                false_count = (short)(false_count + 1);
            //                            }
            //                        }
            //                        else if (found)
            //                        {
            //                            // !指定無しの条件を満たした
            //                            flag = true;
            //                        }
            //                        else
            //                        {
            //                            // !指定無しの条件を満たさず
            //                            false_count = (short)(false_count + 1);
            //                        }
            //                    }

            //                    if (flag | false_count == 0)
            //                    {
            //                        double localFeatureLevel() { object argIndex1 = j; var ret = FeatureLevel(ref argIndex1); return ret; }

            //                        lngWeaponPower[i] = (int)(lngWeaponPower[i] + 100d * localFeatureLevel());
            //                    }
            //                }
            //            }
            //        }

            //        // ADD START MARGE
            //        // 武器割合強化による修正
            //        string argfname22 = "武器割合強化";
            //        if (IsFeatureAvailable(ref argfname22))
            //        {
            //            {
            //                var withBlock18 = Weapon(i);
            //                wname = withBlock18.Name;
            //                wnickname = WeaponNickname(i);
            //                wnskill = withBlock18.NecessarySkill;
            //            }

            //            wclass = strWeaponClass[i];
            //            var loopTo33 = CountFeature();
            //            for (j = 1; j <= loopTo33; j++)
            //            {
            //                object argIndex44 = j;
            //                if (Feature(ref argIndex44) == "武器割合強化")
            //                {
            //                    object argIndex43 = j;
            //                    fdata = FeatureData(ref argIndex43);

            //                    // 「"」を除去
            //                    if (Strings.Left(fdata, 1) == "\"")
            //                    {
            //                        fdata = Strings.Mid(fdata, 2, Strings.Len(fdata) - 2);
            //                    }

            //                    flen = GeneralLib.LLength(ref fdata);
            //                    flag = false;

            //                    // 武器指定がない場合はすべての武器を強化
            //                    if (flen == 0)
            //                    {
            //                        flag = true;
            //                    }

            //                    // 武器指定がある場合はそれぞれの指定をチェック
            //                    false_count = 0;
            //                    var loopTo34 = flen;
            //                    for (k = 1; k <= loopTo34; k++)
            //                    {
            //                        wtype = GeneralLib.LIndex(ref fdata, k);
            //                        if (Strings.Left(wtype, 1) == "!")
            //                        {
            //                            wtype = Strings.Mid(wtype, 2);
            //                            with_not = true;
            //                        }
            //                        else
            //                        {
            //                            with_not = false;
            //                        }

            //                        found = false;
            //                        string argattr1 = "固";
            //                        if (IsWeaponClassifiedAs(i, ref argattr1))
            //                        {
            //                            // ダメージ固定武器は武器指定が武器名、武器表示名、「固」の
            //                            // いずれかで行われた場合にのみ強化
            //                            if (wtype == "固" | (wname ?? "") == (wtype ?? "") | (wnickname ?? "") == (wtype ?? ""))
            //                            {
            //                                found = true;
            //                            }
            //                        }
            //                        else
            //                        {
            //                            switch (wtype ?? "")
            //                            {
            //                                case "全":
            //                                    {
            //                                        found = true;
            //                                        break;
            //                                    }

            //                                case "物":
            //                                    {
            //                                        string argstring220 = "魔";
            //                                        string argstring221 = "魔武";
            //                                        string argstring222 = "魔突";
            //                                        string argstring223 = "魔接";
            //                                        string argstring224 = "魔銃";
            //                                        string argstring225 = "魔実";
            //                                        if (GeneralLib.InStrNotNest(ref wclass, ref argstring220) == 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring221) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring222) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring223) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring224) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring225) > 0)
            //                                        {
            //                                            found = true;
            //                                        }

            //                                        break;
            //                                    }

            //                                default:
            //                                    {
            //                                        if (GeneralLib.InStrNotNest(ref wclass, ref wtype) > 0 | (wname ?? "") == (wtype ?? "") | (wnickname ?? "") == (wtype ?? ""))
            //                                        {
            //                                            found = true;
            //                                        }
            //                                        else
            //                                        {
            //                                            // 必要技能による指定
            //                                            var loopTo35 = GeneralLib.LLength(ref wnskill);
            //                                            for (l = 1; l <= loopTo35; l++)
            //                                            {
            //                                                sname = GeneralLib.LIndex(ref wnskill, l);
            //                                                if (Strings.InStr(sname, "Lv") > 0)
            //                                                {
            //                                                    sname = Strings.Left(sname, Strings.InStr(sname, "Lv") - 1);
            //                                                }

            //                                                if ((sname ?? "") == (wtype ?? ""))
            //                                                {
            //                                                    found = true;
            //                                                    break;
            //                                                }
            //                                            }
            //                                        }

            //                                        break;
            //                                    }
            //                            }
            //                        }

            //                        if (with_not)
            //                        {
            //                            // !指定あり
            //                            if (found)
            //                            {
            //                                // 条件を満たした場合は適用しない
            //                                flag = false;
            //                                false_count = (short)(false_count + 1);
            //                            }
            //                        }
            //                        else if (found)
            //                        {
            //                            // !指定無しの条件を満たした
            //                            flag = true;
            //                        }
            //                        else
            //                        {
            //                            // !指定無しの条件を満たさず
            //                            false_count = (short)(false_count + 1);
            //                        }
            //                    }

            //                    if (flag | false_count == 0)
            //                    {
            //                        double localFeatureLevel1() { object argIndex1 = j; var ret = FeatureLevel(ref argIndex1); return ret; }

            //                        lngWeaponPower[i] = (int)(lngWeaponPower[i] + (long)(this.Weapon(i).Power * localFeatureLevel1()) / 20L);
            //                    }
            //                }
            //            }
            //        }
            //        // ADD END MARGE

            //        // ダメージ固定武器
            //        string argattr2 = "固";
            //        if (IsWeaponClassifiedAs(i, ref argattr2))
            //        {
            //            goto NextWeapon;
            //        }

            //        string argattr29 = "Ｒ";
            //        string argattr30 = "改";
            //        if (IsWeaponClassifiedAs(i, ref argattr29))
            //        {
            //            // 低成長型の攻撃
            //            string argattr20 = "Ｒ";
            //            if (IsWeaponLevelSpecified(i, ref argattr20))
            //            {
            //                // レベル設定されている場合、増加量をレベル×１０×ランクにする
            //                string argattr3 = "Ｒ";
            //                lngWeaponPower[i] = (int)(lngWeaponPower[i] + 10d * WeaponLevel(i, ref argattr3) * (Rank + num));
            //                // オ・シ・超と併用した場合
            //                string argattr11 = "オ";
            //                string argattr12 = "超";
            //                string argattr13 = "シ";
            //                if (IsWeaponClassifiedAs(i, ref argattr11) | IsWeaponClassifiedAs(i, ref argattr12) | IsWeaponClassifiedAs(i, ref argattr13))
            //                {
            //                    string argattr4 = "Ｒ";
            //                    lngWeaponPower[i] = (int)(lngWeaponPower[i] + 10d * (10d - WeaponLevel(i, ref argattr4)) * (Rank + num));

            //                    // オーラ技
            //                    string argattr6 = "オ";
            //                    if (IsWeaponClassifiedAs(i, ref argattr6))
            //                    {
            //                        string argattr5 = "Ｒ";
            //                        lngWeaponPower[i] = (int)(lngWeaponPower[i] + 10d * WeaponLevel(i, ref argattr5) * AuraLevel());
            //                    }

            //                    // サイキック攻撃
            //                    string argattr8 = "超";
            //                    if (IsWeaponClassifiedAs(i, ref argattr8))
            //                    {
            //                        string argattr7 = "Ｒ";
            //                        lngWeaponPower[i] = (int)(lngWeaponPower[i] + 10d * WeaponLevel(i, ref argattr7) * PsychicLevel());
            //                    }

            //                    // 同調率対象攻撃
            //                    string argattr10 = "シ";
            //                    if (IsWeaponClassifiedAs(i, ref argattr10))
            //                    {
            //                        if (CountPilot() > 0)
            //                        {
            //                            if (MainPilot().SynchroRate() > 0)
            //                            {
            //                                string argattr9 = "Ｒ";
            //                                lngWeaponPower[i] = (int)(lngWeaponPower[i] + (long)(15d * WeaponLevel(i, ref argattr9) * (SyncLevel() - 50d)) / 10L);
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                // レベル指定されていない場合は今までどおりランク×５０
            //                lngWeaponPower[i] = lngWeaponPower[i] + 50 * (Rank + num);

            //                // オ・シ・超と併用した場合
            //                string argattr17 = "オ";
            //                string argattr18 = "超";
            //                string argattr19 = "シ";
            //                if (IsWeaponClassifiedAs(i, ref argattr17) | IsWeaponClassifiedAs(i, ref argattr18) | IsWeaponClassifiedAs(i, ref argattr19))
            //                {
            //                    lngWeaponPower[i] = lngWeaponPower[i] + 50 * (Rank + num);

            //                    // オーラ技
            //                    string argattr14 = "オ";
            //                    if (IsWeaponClassifiedAs(i, ref argattr14))
            //                    {
            //                        lngWeaponPower[i] = (int)(lngWeaponPower[i] + 50d * AuraLevel());
            //                    }

            //                    // サイキック攻撃
            //                    string argattr15 = "超";
            //                    if (IsWeaponClassifiedAs(i, ref argattr15))
            //                    {
            //                        lngWeaponPower[i] = (int)(lngWeaponPower[i] + 50d * PsychicLevel());
            //                    }

            //                    // 同調率対象攻撃
            //                    string argattr16 = "シ";
            //                    if (IsWeaponClassifiedAs(i, ref argattr16))
            //                    {
            //                        if (CountPilot() > 0)
            //                        {
            //                            if (MainPilot().SynchroRate() > 0)
            //                            {
            //                                lngWeaponPower[i] = (int)(lngWeaponPower[i] + (long)(15d * (SyncLevel() - 50d)) / 2L);
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        else if (IsWeaponClassifiedAs(i, ref argattr30))
            //        {
            //            // 改属性＝オ・超・シ属性を無視したＲ属性
            //            string argattr25 = "改";
            //            if (IsWeaponLevelSpecified(i, ref argattr25))
            //            {
            //                // レベル設定されている場合、増加量をレベル×１０×ランクにする
            //                string argattr24 = "改";
            //                lngWeaponPower[i] = (int)(lngWeaponPower[i] + 10d * WeaponLevel(i, ref argattr24) * (Rank + num));
            //            }
            //            else
            //            {
            //                // レベル指定がない場合、増加量は５０×ランク
            //                lngWeaponPower[i] = lngWeaponPower[i] + 50 * (Rank + num);
            //            }

            //            // オーラ技
            //            string argattr26 = "オ";
            //            if (IsWeaponClassifiedAs(i, ref argattr26))
            //            {
            //                lngWeaponPower[i] = (int)(lngWeaponPower[i] + 100d * AuraLevel());
            //            }

            //            // サイキック攻撃
            //            string argattr27 = "超";
            //            if (IsWeaponClassifiedAs(i, ref argattr27))
            //            {
            //                lngWeaponPower[i] = (int)(lngWeaponPower[i] + 100d * PsychicLevel());
            //            }

            //            // 同調率対象攻撃
            //            string argattr28 = "シ";
            //            if (IsWeaponClassifiedAs(i, ref argattr28))
            //            {
            //                if (CountPilot() > 0)
            //                {
            //                    if (MainPilot().SynchroRate() > 0)
            //                    {
            //                        lngWeaponPower[i] = (int)(lngWeaponPower[i] + 15d * (SyncLevel() - 50d));
            //                    }
            //                }
            //            }
            //        }
            //        else
            //        {
            //            // Ｒ、改属性が両方ともない場合
            //            lngWeaponPower[i] = lngWeaponPower[i] + 100 * (Rank + num);

            //            // オーラ技
            //            string argattr21 = "オ";
            //            if (IsWeaponClassifiedAs(i, ref argattr21))
            //            {
            //                lngWeaponPower[i] = (int)(lngWeaponPower[i] + 100d * AuraLevel());
            //            }

            //            // サイキック攻撃
            //            string argattr22 = "超";
            //            if (IsWeaponClassifiedAs(i, ref argattr22))
            //            {
            //                lngWeaponPower[i] = (int)(lngWeaponPower[i] + 100d * PsychicLevel());
            //            }

            //            // 同調率対象攻撃
            //            string argattr23 = "シ";
            //            if (IsWeaponClassifiedAs(i, ref argattr23))
            //            {
            //                if (CountPilot() > 0)
            //                {
            //                    if (MainPilot().SynchroRate() > 0)
            //                    {
            //                        lngWeaponPower[i] = (int)(lngWeaponPower[i] + 15d * (SyncLevel() - 50d));
            //                    }
            //                }
            //            }
            //        }

            //        // ボスランクによる修正
            //        if (BossRank > 0)
            //        {
            //            lngWeaponPower[i] = lngWeaponPower[i] + GeneralLib.MinLng(100 * BossRank, 300);
            //        }

            //        // 攻撃力の最高値は99999
            //        if (lngWeaponPower[i] > 99999)
            //        {
            //            lngWeaponPower[i] = 99999;
            //        }

            //        // 最低値は1
            //        if (lngWeaponPower[i] <= 0)
            //        {
            //            lngWeaponPower[i] = 1;
            //        }

            //    NextWeapon:
            //        ;
            //    }

            //    // 武器射程を更新
            //    intWeaponMaxRange = new short[(CountWeapon() + 1)];
            //    var loopTo36 = CountWeapon();
            //    for (i = 1; i <= loopTo36; i++)
            //    {
            //        intWeaponMaxRange[i] = Weapon(i).MaxRange;

            //        // 最大射程がもともと１ならそれ以上変化しない
            //        if (intWeaponMaxRange[i] == 1)
            //        {
            //            goto NextWeapon2;
            //        }

            //        // 思念誘導攻撃のＮＴ能力による射程延長
            //        string argstring226 = "サ";
            //        if (GeneralLib.InStrNotNest(ref strWeaponClass[i], ref argstring226) > 0)
            //        {
            //            if (CountPilot() > 0)
            //            {
            //                {
            //                    var withBlock19 = MainPilot();
            //                    object argIndex45 = "超感覚";
            //                    string argref_mode = "";
            //                    object argIndex46 = "知覚強化";
            //                    string argref_mode1 = "";
            //                    intWeaponMaxRange[i] = (short)(intWeaponMaxRange[i] + (long)withBlock19.SkillLevel(ref argIndex45, ref_mode: ref argref_mode) / 4L + (long)withBlock19.SkillLevel(ref argIndex46, ref_mode: ref argref_mode1) / 4L);
            //                }
            //            }
            //        }

            //        // マップ攻撃には適用されない
            //        string argstring227 = "Ｍ";
            //        if (GeneralLib.InStrNotNest(ref strWeaponClass[i], ref argstring227) > 0)
            //        {
            //            goto NextWeapon2;
            //        }

            //        // 接近戦武器には適用されない
            //        string argstring228 = "武";
            //        string argstring229 = "突";
            //        string argstring230 = "接";
            //        if (GeneralLib.InStrNotNest(ref strWeaponClass[i], ref argstring228) > 0 | GeneralLib.InStrNotNest(ref strWeaponClass[i], ref argstring229) > 0 | GeneralLib.InStrNotNest(ref strWeaponClass[i], ref argstring230) > 0)
            //        {
            //            goto NextWeapon2;
            //        }

            //        // 有線式誘導攻撃には適用されない
            //        string argstring231 = "有";
            //        if (GeneralLib.InStrNotNest(ref strWeaponClass[i], ref argstring231) > 0)
            //        {
            //            goto NextWeapon2;
            //        }

            //        // 射程延長による修正
            //        string argfname23 = "射程延長";
            //        if (IsFeatureAvailable(ref argfname23))
            //        {
            //            {
            //                var withBlock20 = Weapon(i);
            //                wname = withBlock20.Name;
            //                wnickname = WeaponNickname(i);
            //                wnskill = withBlock20.NecessarySkill;
            //            }

            //            wclass = strWeaponClass[i];
            //            var loopTo37 = CountFeature();
            //            for (j = 1; j <= loopTo37; j++)
            //            {
            //                object argIndex48 = j;
            //                if (Feature(ref argIndex48) == "射程延長")
            //                {
            //                    object argIndex47 = j;
            //                    fdata = FeatureData(ref argIndex47);

            //                    // 「"」を除去
            //                    if (Strings.Left(fdata, 1) == "\"")
            //                    {
            //                        fdata = Strings.Mid(fdata, 2, Strings.Len(fdata) - 2);
            //                    }

            //                    flen = GeneralLib.LLength(ref fdata);
            //                    flag = false;

            //                    // 武器指定がない場合はすべての武器を強化
            //                    if (flen == 0)
            //                    {
            //                        flag = true;
            //                    }

            //                    // 武器指定がある場合はそれぞれの指定をチェック
            //                    false_count = 0;
            //                    var loopTo38 = flen;
            //                    for (k = 1; k <= loopTo38; k++)
            //                    {
            //                        wtype = GeneralLib.LIndex(ref fdata, k);
            //                        if (Strings.Left(wtype, 1) == "!")
            //                        {
            //                            wtype = Strings.Mid(wtype, 2);
            //                            with_not = true;
            //                        }
            //                        else
            //                        {
            //                            with_not = false;
            //                        }

            //                        found = false;
            //                        switch (wtype ?? "")
            //                        {
            //                            case "全":
            //                                {
            //                                    found = true;
            //                                    break;
            //                                }

            //                            case "物":
            //                                {
            //                                    string argstring232 = "魔";
            //                                    string argstring233 = "魔武";
            //                                    string argstring234 = "魔突";
            //                                    string argstring235 = "魔接";
            //                                    string argstring236 = "魔銃";
            //                                    string argstring237 = "魔実";
            //                                    if (GeneralLib.InStrNotNest(ref wclass, ref argstring232) == 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring233) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring234) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring235) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring236) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring237) > 0)
            //                                    {
            //                                        found = true;
            //                                    }

            //                                    break;
            //                                }

            //                            default:
            //                                {
            //                                    if (GeneralLib.InStrNotNest(ref wclass, ref wtype) > 0 | (wname ?? "") == (wtype ?? "") | (wnickname ?? "") == (wtype ?? ""))
            //                                    {
            //                                        found = true;
            //                                    }
            //                                    else
            //                                    {
            //                                        var loopTo39 = GeneralLib.LLength(ref wnskill);
            //                                        for (l = 1; l <= loopTo39; l++)
            //                                        {
            //                                            sname = GeneralLib.LIndex(ref wnskill, l);
            //                                            if (Strings.InStr(sname, "Lv") > 0)
            //                                            {
            //                                                sname = Strings.Left(sname, Strings.InStr(sname, "Lv") - 1);
            //                                            }

            //                                            if ((sname ?? "") == (wtype ?? ""))
            //                                            {
            //                                                found = true;
            //                                                break;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (with_not)
            //                        {
            //                            // !指定あり
            //                            if (found)
            //                            {
            //                                // 条件を満たした場合は適用しない
            //                                flag = false;
            //                                false_count = (short)(false_count + 1);
            //                            }
            //                        }
            //                        else if (found)
            //                        {
            //                            // !指定無しの条件を満たした
            //                            flag = true;
            //                        }
            //                        else
            //                        {
            //                            // !指定無しの条件を満たさず
            //                            false_count = (short)(false_count + 1);
            //                        }
            //                    }

            //                    if (flag | false_count == 0)
            //                    {
            //                        double localFeatureLevel2() { object argIndex1 = j; var ret = FeatureLevel(ref argIndex1); return ret; }

            //                        intWeaponMaxRange[i] = (short)(intWeaponMaxRange[i] + localFeatureLevel2());
            //                    }
            //                }
            //            }
            //        }

            //        // 最低値は1
            //        if (intWeaponMaxRange[i] <= 0)
            //        {
            //            intWeaponMaxRange[i] = 1;
            //        }

            //    NextWeapon2:
            //        ;
            //    }

            //    // 武器命中率を更新
            //    intWeaponPrecision = new short[(CountWeapon() + 1)];
            //    var loopTo40 = CountWeapon();
            //    for (i = 1; i <= loopTo40; i++)
            //    {
            //        intWeaponPrecision[i] = Weapon(i).Precision;

            //        // 武器強化による修正
            //        string argfname24 = "命中率強化";
            //        if (IsFeatureAvailable(ref argfname24))
            //        {
            //            {
            //                var withBlock21 = Weapon(i);
            //                wname = withBlock21.Name;
            //                wnickname = WeaponNickname(i);
            //                wnskill = withBlock21.NecessarySkill;
            //            }

            //            wclass = strWeaponClass[i];
            //            var loopTo41 = CountFeature();
            //            for (j = 1; j <= loopTo41; j++)
            //            {
            //                object argIndex50 = j;
            //                if (Feature(ref argIndex50) == "命中率強化")
            //                {
            //                    object argIndex49 = j;
            //                    fdata = FeatureData(ref argIndex49);

            //                    // 「"」を除去
            //                    if (Strings.Left(fdata, 1) == "\"")
            //                    {
            //                        fdata = Strings.Mid(fdata, 2, Strings.Len(fdata) - 2);
            //                    }

            //                    flen = GeneralLib.LLength(ref fdata);
            //                    flag = false;

            //                    // 武器指定がない場合はすべての武器を強化
            //                    if (flen == 0)
            //                    {
            //                        flag = true;
            //                    }

            //                    // 武器指定がある場合はそれぞれの指定をチェック
            //                    false_count = 0;
            //                    var loopTo42 = flen;
            //                    for (k = 1; k <= loopTo42; k++)
            //                    {
            //                        wtype = GeneralLib.LIndex(ref fdata, k);
            //                        if (Strings.Left(wtype, 1) == "!")
            //                        {
            //                            wtype = Strings.Mid(wtype, 2);
            //                            with_not = true;
            //                        }
            //                        else
            //                        {
            //                            with_not = false;
            //                        }

            //                        found = false;
            //                        switch (wtype ?? "")
            //                        {
            //                            case "全":
            //                                {
            //                                    found = true;
            //                                    break;
            //                                }

            //                            case "物":
            //                                {
            //                                    string argstring238 = "魔";
            //                                    string argstring239 = "魔武";
            //                                    string argstring240 = "魔突";
            //                                    string argstring241 = "魔接";
            //                                    string argstring242 = "魔銃";
            //                                    string argstring243 = "魔実";
            //                                    if (GeneralLib.InStrNotNest(ref wclass, ref argstring238) == 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring239) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring240) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring241) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring242) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring243) > 0)
            //                                    {
            //                                        found = true;
            //                                    }

            //                                    break;
            //                                }

            //                            default:
            //                                {
            //                                    if (GeneralLib.InStrNotNest(ref wclass, ref wtype) > 0 | (wname ?? "") == (wtype ?? "") | (wnickname ?? "") == (wtype ?? ""))
            //                                    {
            //                                        found = true;
            //                                    }
            //                                    else
            //                                    {
            //                                        var loopTo43 = GeneralLib.LLength(ref wnskill);
            //                                        for (l = 1; l <= loopTo43; l++)
            //                                        {
            //                                            sname = GeneralLib.LIndex(ref wnskill, l);
            //                                            if (Strings.InStr(sname, "Lv") > 0)
            //                                            {
            //                                                sname = Strings.Left(sname, Strings.InStr(sname, "Lv") - 1);
            //                                            }

            //                                            if ((sname ?? "") == (wtype ?? ""))
            //                                            {
            //                                                found = true;
            //                                                break;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (with_not)
            //                        {
            //                            // !指定あり
            //                            if (found)
            //                            {
            //                                // 条件を満たした場合は適用しない
            //                                flag = false;
            //                                false_count = (short)(false_count + 1);
            //                            }
            //                        }
            //                        else if (found)
            //                        {
            //                            // !指定無しの条件を満たした
            //                            flag = true;
            //                        }
            //                        else
            //                        {
            //                            // !指定無しの条件を満たさず
            //                            false_count = (short)(false_count + 1);
            //                        }
            //                    }

            //                    if (flag | false_count == 0)
            //                    {
            //                        double localFeatureLevel3() { object argIndex1 = j; var ret = FeatureLevel(ref argIndex1); return ret; }

            //                        intWeaponPrecision[i] = (short)(intWeaponPrecision[i] + 5d * localFeatureLevel3());
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // 武器のＣＴ率を更新
            //    intWeaponCritical = new short[(CountWeapon() + 1)];
            //    var loopTo44 = CountWeapon();
            //    for (i = 1; i <= loopTo44; i++)
            //    {
            //        intWeaponCritical[i] = Weapon(i).Critical;

            //        // ＣＴ率強化による修正
            //        string argfname25 = "ＣＴ率強化";
            //        if (IsFeatureAvailable(ref argfname25) & IsNormalWeapon(i))
            //        {
            //            {
            //                var withBlock22 = Weapon(i);
            //                wname = withBlock22.Name;
            //                wnickname = WeaponNickname(i);
            //                wnskill = withBlock22.NecessarySkill;
            //            }

            //            wclass = strWeaponClass[i];
            //            var loopTo45 = CountFeature();
            //            for (j = 1; j <= loopTo45; j++)
            //            {
            //                object argIndex52 = j;
            //                if (Feature(ref argIndex52) == "ＣＴ率強化")
            //                {
            //                    object argIndex51 = j;
            //                    fdata = FeatureData(ref argIndex51);

            //                    // 「"」を除去
            //                    if (Strings.Left(fdata, 1) == "\"")
            //                    {
            //                        fdata = Strings.Mid(fdata, 2, Strings.Len(fdata) - 2);
            //                    }

            //                    flen = GeneralLib.LLength(ref fdata);
            //                    flag = false;

            //                    // 武器指定がない場合はすべての武器を強化
            //                    if (flen == 0)
            //                    {
            //                        flag = true;
            //                    }

            //                    // 武器指定がある場合はそれぞれの指定をチェック
            //                    false_count = 0;
            //                    var loopTo46 = flen;
            //                    for (k = 1; k <= loopTo46; k++)
            //                    {
            //                        wtype = GeneralLib.LIndex(ref fdata, k);
            //                        if (Strings.Left(wtype, 1) == "!")
            //                        {
            //                            wtype = Strings.Mid(wtype, 2);
            //                            with_not = true;
            //                        }
            //                        else
            //                        {
            //                            with_not = false;
            //                        }

            //                        found = false;
            //                        switch (wtype ?? "")
            //                        {
            //                            case "全":
            //                                {
            //                                    found = true;
            //                                    break;
            //                                }

            //                            case "物":
            //                                {
            //                                    string argstring244 = "魔";
            //                                    string argstring245 = "魔武";
            //                                    string argstring246 = "魔突";
            //                                    string argstring247 = "魔接";
            //                                    string argstring248 = "魔銃";
            //                                    string argstring249 = "魔実";
            //                                    if (GeneralLib.InStrNotNest(ref wclass, ref argstring244) == 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring245) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring246) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring247) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring248) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring249) > 0)
            //                                    {
            //                                        found = !with_not;
            //                                    }

            //                                    break;
            //                                }

            //                            default:
            //                                {
            //                                    if (GeneralLib.InStrNotNest(ref wclass, ref wtype) > 0 | (wname ?? "") == (wtype ?? "") | (wnickname ?? "") == (wtype ?? ""))
            //                                    {
            //                                        found = true;
            //                                    }
            //                                    else
            //                                    {
            //                                        var loopTo47 = GeneralLib.LLength(ref wnskill);
            //                                        for (l = 1; l <= loopTo47; l++)
            //                                        {
            //                                            sname = GeneralLib.LIndex(ref wnskill, l);
            //                                            if (Strings.InStr(sname, "Lv") > 0)
            //                                            {
            //                                                sname = Strings.Left(sname, Strings.InStr(sname, "Lv") - 1);
            //                                            }

            //                                            if ((sname ?? "") == (wtype ?? ""))
            //                                            {
            //                                                found = true;
            //                                                break;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (with_not)
            //                        {
            //                            // !指定あり
            //                            if (found)
            //                            {
            //                                // 条件を満たした場合は適用しない
            //                                flag = false;
            //                                false_count = (short)(false_count + 1);
            //                            }
            //                        }
            //                        else if (found)
            //                        {
            //                            // !指定無しの条件を満たした
            //                            flag = true;
            //                        }
            //                        else
            //                        {
            //                            // !指定無しの条件を満たさず
            //                            false_count = (short)(false_count + 1);
            //                        }
            //                    }

            //                    if (flag | false_count == 0)
            //                    {
            //                        double localFeatureLevel4() { object argIndex1 = j; var ret = FeatureLevel(ref argIndex1); return ret; }

            //                        intWeaponCritical[i] = (short)(intWeaponCritical[i] + 5d * localFeatureLevel4());
            //                    }
            //                }
            //            }
            //        }

            //        // 特殊効果発動率強化による修正
            //        string argfname26 = "特殊効果発動率強化";
            //        if (IsFeatureAvailable(ref argfname26) & !IsNormalWeapon(i))
            //        {
            //            {
            //                var withBlock23 = Weapon(i);
            //                wname = withBlock23.Name;
            //                wnickname = WeaponNickname(i);
            //                wnskill = withBlock23.NecessarySkill;
            //            }

            //            wclass = strWeaponClass[i];
            //            var loopTo48 = CountFeature();
            //            for (j = 1; j <= loopTo48; j++)
            //            {
            //                object argIndex54 = j;
            //                if (Feature(ref argIndex54) == "特殊効果発動率強化")
            //                {
            //                    object argIndex53 = j;
            //                    fdata = FeatureData(ref argIndex53);

            //                    // 「"」を除去
            //                    if (Strings.Left(fdata, 1) == "\"")
            //                    {
            //                        fdata = Strings.Mid(fdata, 2, Strings.Len(fdata) - 2);
            //                    }

            //                    flen = GeneralLib.LLength(ref fdata);
            //                    flag = false;

            //                    // 武器指定がない場合はすべての武器を強化
            //                    if (flen == 0)
            //                    {
            //                        flag = true;
            //                    }

            //                    // 武器指定がある場合はそれぞれの指定をチェック
            //                    false_count = 0;
            //                    var loopTo49 = flen;
            //                    for (k = 1; k <= loopTo49; k++)
            //                    {
            //                        wtype = GeneralLib.LIndex(ref fdata, k);
            //                        if (Strings.Left(wtype, 1) == "!")
            //                        {
            //                            wtype = Strings.Mid(wtype, 2);
            //                            with_not = true;
            //                        }
            //                        else
            //                        {
            //                            with_not = false;
            //                        }

            //                        found = false;
            //                        switch (wtype ?? "")
            //                        {
            //                            case "全":
            //                                {
            //                                    found = true;
            //                                    break;
            //                                }

            //                            case "物":
            //                                {
            //                                    string argstring250 = "魔";
            //                                    string argstring251 = "魔武";
            //                                    string argstring252 = "魔突";
            //                                    string argstring253 = "魔接";
            //                                    string argstring254 = "魔銃";
            //                                    string argstring255 = "魔実";
            //                                    if (GeneralLib.InStrNotNest(ref wclass, ref argstring250) == 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring251) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring252) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring253) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring254) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring255) > 0)
            //                                    {
            //                                        found = true;
            //                                    }

            //                                    break;
            //                                }

            //                            default:
            //                                {
            //                                    if (GeneralLib.InStrNotNest(ref wclass, ref wtype) > 0 | (wname ?? "") == (wtype ?? "") | (wnickname ?? "") == (wtype ?? ""))
            //                                    {
            //                                        found = true;
            //                                    }
            //                                    else
            //                                    {
            //                                        var loopTo50 = GeneralLib.LLength(ref wnskill);
            //                                        for (l = 1; l <= loopTo50; l++)
            //                                        {
            //                                            buf = GeneralLib.LIndex(ref wnskill, l);
            //                                            if (Strings.InStr(buf, "Lv") > 0)
            //                                            {
            //                                                buf = Strings.Left(buf, Strings.InStr(buf, "Lv") - 1);
            //                                            }

            //                                            if ((buf ?? "") == (wtype ?? ""))
            //                                            {
            //                                                found = true;
            //                                                break;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (with_not)
            //                        {
            //                            // !指定あり
            //                            if (found)
            //                            {
            //                                // 条件を満たした場合は適用しない
            //                                flag = false;
            //                                false_count = (short)(false_count + 1);
            //                            }
            //                        }
            //                        else if (found)
            //                        {
            //                            // !指定無しの条件を満たした
            //                            flag = true;
            //                        }
            //                        else
            //                        {
            //                            // !指定無しの条件を満たさず
            //                            false_count = (short)(false_count + 1);
            //                        }
            //                    }

            //                    if (flag | false_count == 0)
            //                    {
            //                        double localFeatureLevel5() { object argIndex1 = j; var ret = FeatureLevel(ref argIndex1); return ret; }

            //                        intWeaponCritical[i] = (short)(intWeaponCritical[i] + 5d * localFeatureLevel5());
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // 最大弾数を更新
            //    intMaxBullet = new short[(CountWeapon() + 1)];
            //    // UPGRADE_NOTE: rate は rate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
            //    double rate_Renamed;
            //    var loopTo51 = CountWeapon();
            //    for (i = 1; i <= loopTo51; i++)
            //    {
            //        intMaxBullet[i] = Weapon(i).Bullet;

            //        // 最大弾数の増加率
            //        rate_Renamed = 0d;

            //        // ボスランクによる修正
            //        if (intBossRank > 0)
            //        {
            //            rate_Renamed = 0.2d * BossRank;
            //        }

            //        // 最大弾数増加による修正
            //        string argfname27 = "最大弾数増加";
            //        if (IsFeatureAvailable(ref argfname27))
            //        {
            //            {
            //                var withBlock24 = Weapon(i);
            //                wname = withBlock24.Name;
            //                wnickname = WeaponNickname(i);
            //                wnskill = withBlock24.NecessarySkill;
            //            }

            //            wclass = strWeaponClass[i];
            //            var loopTo52 = CountFeature();
            //            for (j = 1; j <= loopTo52; j++)
            //            {
            //                object argIndex56 = j;
            //                if (Feature(ref argIndex56) == "最大弾数増加")
            //                {
            //                    object argIndex55 = j;
            //                    fdata = FeatureData(ref argIndex55);

            //                    // 「"」を除去
            //                    if (Strings.Left(fdata, 1) == "\"")
            //                    {
            //                        fdata = Strings.Mid(fdata, 2, Strings.Len(fdata) - 2);
            //                    }

            //                    flen = GeneralLib.LLength(ref fdata);
            //                    flag = false;

            //                    // 武器指定がない場合はすべての武器の弾数を増加
            //                    if (flen == 0)
            //                    {
            //                        flag = true;
            //                    }

            //                    // 武器指定がある場合はそれぞれの指定をチェック
            //                    false_count = 0;
            //                    var loopTo53 = flen;
            //                    for (k = 1; k <= loopTo53; k++)
            //                    {
            //                        wtype = GeneralLib.LIndex(ref fdata, k);
            //                        if (Strings.Left(wtype, 1) == "!")
            //                        {
            //                            wtype = Strings.Mid(wtype, 2);
            //                            with_not = true;
            //                        }
            //                        else
            //                        {
            //                            with_not = false;
            //                        }

            //                        found = false;
            //                        switch (wtype ?? "")
            //                        {
            //                            case "全":
            //                                {
            //                                    found = true;
            //                                    break;
            //                                }

            //                            case "物":
            //                                {
            //                                    string argstring256 = "魔";
            //                                    string argstring257 = "魔武";
            //                                    string argstring258 = "魔突";
            //                                    string argstring259 = "魔接";
            //                                    string argstring260 = "魔銃";
            //                                    string argstring261 = "魔実";
            //                                    if (GeneralLib.InStrNotNest(ref wclass, ref argstring256) == 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring257) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring258) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring259) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring260) > 0 | GeneralLib.InStrNotNest(ref wclass, ref argstring261) > 0)
            //                                    {
            //                                        found = true;
            //                                    }

            //                                    break;
            //                                }

            //                            default:
            //                                {
            //                                    if (GeneralLib.InStrNotNest(ref wclass, ref wtype) > 0 | (wname ?? "") == (wtype ?? "") | (wnickname ?? "") == (wtype ?? ""))
            //                                    {
            //                                        found = true;
            //                                    }
            //                                    else
            //                                    {
            //                                        var loopTo54 = GeneralLib.LLength(ref wnskill);
            //                                        for (l = 1; l <= loopTo54; l++)
            //                                        {
            //                                            sname = GeneralLib.LIndex(ref wnskill, l);
            //                                            if (Strings.InStr(sname, "Lv") > 0)
            //                                            {
            //                                                sname = Strings.Left(sname, Strings.InStr(sname, "Lv") - 1);
            //                                            }

            //                                            if ((sname ?? "") == (wtype ?? ""))
            //                                            {
            //                                                found = true;
            //                                                break;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (with_not)
            //                        {
            //                            // !指定あり
            //                            if (found)
            //                            {
            //                                // 条件を満たした場合は適用しない
            //                                flag = false;
            //                                false_count = (short)(false_count + 1);
            //                            }
            //                        }
            //                        else if (found)
            //                        {
            //                            // !指定無しの条件を満たした
            //                            flag = true;
            //                        }
            //                        else
            //                        {
            //                            // !指定無しの条件を満たさず
            //                            false_count = (short)(false_count + 1);
            //                        }
            //                    }

            //                    if (flag | false_count == 0)
            //                    {
            //                        double localFeatureLevel6() { object argIndex1 = j; var ret = FeatureLevel(ref argIndex1); return ret; }

            //                        rate_Renamed = rate_Renamed + 0.5d * localFeatureLevel6();
            //                    }
            //                }
            //            }
            //        }

            //        // 増加率に合わせて弾数を修正
            //        intMaxBullet[i] = (short)((1d + rate_Renamed) * intMaxBullet[i]);

            //        // 最大値は99
            //        if (intMaxBullet[i] > 99)
            //        {
            //            intMaxBullet[i] = 99;
            //        }
            //        // 最低値は0
            //        if (intMaxBullet[i] < 0)
            //        {
            //            intMaxBullet[i] = 0;
            //        }
            //    }

            //    // 弾数を更新
            //    Array.Resize(ref dblBullet, CountWeapon() + 1);
            //    flags = new bool[Information.UBound(prev_wdata) + 1];
            //    var loopTo55 = CountWeapon();
            //    for (i = 1; i <= loopTo55; i++)
            //    {
            //        dblBullet[i] = 1d;
            //        var loopTo56 = (short)Information.UBound(prev_wdata);
            //        for (j = 1; j <= loopTo56; j++)
            //        {
            //            if (ReferenceEquals(WData[i], prev_wdata[j]) & !flags[j])
            //            {
            //                dblBullet[i] = prev_wbullets[j];
            //                flags[j] = true;
            //                break;
            //            }
            //        }
            //    }

            //    // アビリティデータを更新
            //    prev_adata = new AbilityData[Information.UBound(adata) + 1];
            //    prev_astocks = new double[Information.UBound(adata) + 1];
            //    var loopTo57 = (short)Information.UBound(adata);
            //    for (i = 1; i <= loopTo57; i++)
            //    {
            //        prev_adata[i] = adata[i];
            //        prev_astocks[i] = dblStock[i];
            //    }

            //    {
            //        var withBlock25 = Data;
            //        adata = new AbilityData[(withBlock25.CountAbility() + 1)];
            //        var loopTo58 = withBlock25.CountAbility();
            //        for (i = 1; i <= loopTo58; i++)
            //        {
            //            object argIndex57 = i;
            //            adata[i] = withBlock25.Ability(ref argIndex57);
            //        }
            //    }

            //    if (CountPilot() > 0)
            //    {
            //        {
            //            var withBlock26 = MainPilot().Data;
            //            var loopTo59 = withBlock26.CountAbility();
            //            for (i = 1; i <= loopTo59; i++)
            //            {
            //                Array.Resize(ref adata, Information.UBound(adata) + 1 + 1);
            //                object argIndex58 = i;
            //                adata[Information.UBound(adata)] = withBlock26.Ability(ref argIndex58);
            //            }
            //        }

            //        var loopTo60 = CountPilot();
            //        for (i = 2; i <= loopTo60; i++)
            //        {
            //            Pilot localPilot3() { object argIndex1 = i; var ret = Pilot(ref argIndex1); return ret; }

            //            {
            //                var withBlock27 = localPilot3().Data;
            //                var loopTo61 = withBlock27.CountAbility();
            //                for (j = 1; j <= loopTo61; j++)
            //                {
            //                    Array.Resize(ref adata, Information.UBound(adata) + 1 + 1);
            //                    object argIndex59 = j;
            //                    adata[Information.UBound(adata)] = withBlock27.Ability(ref argIndex59);
            //                }
            //            }
            //        }

            //        var loopTo62 = CountSupport();
            //        for (i = 1; i <= loopTo62; i++)
            //        {
            //            Pilot localSupport3() { object argIndex1 = i; var ret = Support(ref argIndex1); return ret; }

            //            {
            //                var withBlock28 = localSupport3().Data;
            //                var loopTo63 = withBlock28.CountAbility();
            //                for (j = 1; j <= loopTo63; j++)
            //                {
            //                    Array.Resize(ref adata, Information.UBound(adata) + 1 + 1);
            //                    object argIndex60 = j;
            //                    adata[Information.UBound(adata)] = withBlock28.Ability(ref argIndex60);
            //                }
            //            }
            //        }

            //        string argfname28 = "追加サポート";
            //        if (IsFeatureAvailable(ref argfname28))
            //        {
            //            {
            //                var withBlock29 = AdditionalSupport().Data;
            //                var loopTo64 = withBlock29.CountAbility();
            //                for (i = 1; i <= loopTo64; i++)
            //                {
            //                    Array.Resize(ref adata, Information.UBound(adata) + 1 + 1);
            //                    object argIndex61 = i;
            //                    adata[Information.UBound(adata)] = withBlock29.Ability(ref argIndex61);
            //                }
            //            }
            //        }
            //    }

            //    foreach (Item currentItm6 in colItem)
            //    {
            //        itm = currentItm6;
            //        if (itm.Activated)
            //        {
            //            var loopTo65 = itm.CountAbility();
            //            for (i = 1; i <= loopTo65; i++)
            //            {
            //                Array.Resize(ref adata, Information.UBound(adata) + 1 + 1);
            //                object argIndex62 = i;
            //                adata[Information.UBound(adata)] = itm.Ability(ref argIndex62);
            //            }
            //        }
            //    }

            //    // 使用回数を更新
            //    Array.Resize(ref dblStock, CountAbility() + 1);
            //    flags = new bool[Information.UBound(prev_adata) + 1];
            //    var loopTo66 = CountAbility();
            //    for (i = 1; i <= loopTo66; i++)
            //    {
            //        dblStock[i] = 1d;
            //        var loopTo67 = (short)Information.UBound(prev_adata);
            //        for (j = 1; j <= loopTo67; j++)
            //        {
            //            if (ReferenceEquals(adata[i], prev_adata[j]) & !flags[j])
            //            {
            //                dblStock[i] = prev_astocks[j];
            //                flags[j] = true;
            //                break;
            //            }
            //        }
            //    }

            //    if (Status_Renamed != "出撃")
            //    {
            //        return;
            //    }

            //    // 制御不能？
            //    string argfname29 = "制御不可";
            //    if (IsFeatureAvailable(ref argfname29))
            //    {
            //        if (!is_uncontrollable)
            //        {
            //            string argcname5 = "暴走";
            //            string argcdata = "";
            //            AddCondition(ref argcname5, -1, cdata: ref argcdata);
            //        }
            //    }
            //    else if (is_uncontrollable)
            //    {
            //        object argIndex64 = "暴走";
            //        if (IsConditionSatisfied(ref argIndex64))
            //        {
            //            object argIndex63 = "暴走";
            //            DeleteCondition(ref argIndex63);
            //        }
            //    }

            //    // 不安定？
            //    string argfname30 = "不安定";
            //    if (IsFeatureAvailable(ref argfname30))
            //    {
            //        if (!is_stable)
            //        {
            //            if (HP <= MaxHP / 4)
            //            {
            //                string argcname6 = "暴走";
            //                string argcdata1 = "";
            //                AddCondition(ref argcname6, -1, cdata: ref argcdata1);
            //            }
            //        }
            //    }
            //    else if (is_stable)
            //    {
            //        object argIndex66 = "暴走";
            //        if (IsConditionSatisfied(ref argIndex66))
            //        {
            //            object argIndex65 = "暴走";
            //            DeleteCondition(ref argIndex65);
            //        }
            //    }
        }
    }
}
