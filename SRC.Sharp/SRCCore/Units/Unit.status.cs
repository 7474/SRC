using SRCCore.Extensions;
using SRCCore.Items;
using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.VB;
using System.Collections.Generic;
using System.Linq;

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

            // ＨＰとＥＮの値を記録
            var hp_ratio = 100 * HP / (double)MaxHP;
            var en_ratio = 100 * EN / (double)MaxEN;

            //    // ユニット用画像ファイル名を記録しておく
            //    ubitmap = get_Bitmap(false);

            // 非表示かどうか記録しておく
            var is_invisible = IsFeatureAvailable("非表示");

            // 制御不可がどうかを記録しておく
            var is_uncontrollable = IsFeatureAvailable("制御不可");

            // 不安定がどうかを記録しておく
            var is_stable = IsFeatureAvailable("不安定");
        TryAgain:
            ;


            // アイテムが現在の形態で効力を発揮してくれるか判定
            foreach (Item currentItm in colItem)
            {
                currentItm.Activated = currentItm.IsAvailable(this);
            }

            // ランクアップによるデータ変更
            while (Data.IsFeatureAvailable("ランクアップ"))
            {
                var fd = Data.Feature("ランクアップ");
                if (Rank < fd.FeatureLevel)
                {
                    break;
                }

                if (!IsNecessarySkillSatisfied(fd.NecessarySkill))
                {
                    break;
                }

                if (!SRC.UDList.IsDefined(fd.Data))
                {
                    GUI.ErrorMessage(Name + "のランクアップ先ユニット「" + fd.Data + "」のデータが定義されていません");
                    SRC.TerminateSRC();
                }

                Data = SRC.UDList.Item(fd.Data);
            }

            // 特殊能力を更新

            // まず特殊能力リストをクリア
            colFeature.Clear();

            // 付加された特殊能力
            foreach (Condition cnd in colCondition)
            {
                if (cnd.Lifetime != 0)
                {
                    if (Strings.Right(cnd.Name, 2) == "付加")
                    {
                        var fd = new FeatureData();
                        fd.Name = Strings.Left(cnd.Name, Strings.Len(cnd.Name) - 2);
                        fd.Level = cnd.Level;
                        fd.StrData = cnd.StrData;
                        colFeature.Add(fd, fd.Name);
                    }
                }
            }

            AdditionalFeaturesNum = colFeature.Count;

            // ユニットデータで定義されている特殊能力
            AddFeatures(Data.Features);

            // アイテムで得られた特殊能力
            foreach (Item currentItm in colItem)
            {
                {
                    if (currentItm.Activated)
                    {
                        AddFeatures(currentItm.Data.Features, true);
                    }
                }
            }

            //    // パイロットデータで定義されている特殊能力
            //    if (CountPilot() > 0)
            //    {
            //        if (IsFeatureAvailable("追加パイロット"))
            //        {
            //            // 特殊能力を付加する前に必要技能が満たされているかどうか判定
            //            UpdateFeatures("追加パイロット");
            //        }

            //        AddFeatures(MainPilot().Data.colFeature);
            //        var loopTo1 = CountPilot();
            //        for (i = 2; i <= loopTo1; i++)
            //        {
            //            Pilot localPilot() { object argIndex1 = i; var ret = Pilot(argIndex1); return ret; }

            //            AddFeatures(localPilot().Data.colFeature);
            //        }

            //        var loopTo2 = CountSupport();
            //        for (i = 1; i <= loopTo2; i++)
            //        {
            //            Pilot localSupport() { object argIndex1 = i; var ret = Support(argIndex1); return ret; }

            //            AddFeatures(localSupport().Data.colFeature);
            //        }

            //        if (IsFeatureAvailable("追加サポート"))
            //        {
            //            // 特殊能力を付加する前に必要技能が満たされているかどうか判定
            //            UpdateFeatures("追加サポート");
            //            if (IsFeatureAvailable("追加サポート"))
            //            {
            //                AddFeatures(AdditionalSupport().Data.colFeature);
            //            }
            //        }
            //    }

            //    // パイロット能力付加＆強化の効果をクリア
            //    i = 1;
            //    while (i <= CountCondition())
            //    {
            //        string localCondition() { object argIndex1 = i; var ret = Condition(argIndex1); return ret; }

            //        switch (Strings.Right(localCondition(), 3) ?? "")
            //        {
            //            case "付加２":
            //            case "強化２":
            //                {
            //                    DeleteCondition(i);
            //                    break;
            //                }

            //            default:
            //                {
            //                    i = (i + 1);
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
            //                    if (!IsNecessarySkillSatisfied(fd.NecessarySkill, p: null))
            //                    {
            //                        found = true;
            //                        goto NextFeature;
            //                    }
            //                    // 必要条件を満たしている？
            //                    if (!IsNecessarySkillSatisfied(fd.NecessaryCondition, p: null))
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
            //                            slevel = Constants.DEFAULT_LEVEL;
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
            //                            slevel = Constants.DEFAULT_LEVEL;
            //                        }
            //                    }

            //                    // エリアスが定義されている？
            //                    if (SRC.ALDList.IsDefined(stype))
            //                    {
            //                        {
            //                            var withBlock4 = SRC.ALDList.Item(stype);
            //                            var loopTo3 = withBlock4.Count;
            //                            for (j = 1; j <= loopTo3; j++)
            //                            {
            //                                // エリアスの定義に従って特殊能力定義を置き換える
            //                                stype2 = withBlock4.get_AliasType(j);
            //                                string localLIndex() { string arglist = withBlock4.get_AliasData(j); var ret = GeneralLib.LIndex(arglist, 1); withBlock4.get_AliasData(j) = arglist; return ret; }

            //                                if (localLIndex() == "解説")
            //                                {
            //                                    // 特殊能力の解説
            //                                    if (!string.IsNullOrEmpty(sdata))
            //                                    {
            //                                        stype2 = GeneralLib.LIndex(sdata, 1);
            //                                    }

            //                                    slevel2 = Constants.DEFAULT_LEVEL;
            //                                    sdata2 = withBlock4.get_AliasData(j);
            //                                }
            //                                else
            //                                {
            //                                    // 通常の能力
            //                                    if (withBlock4.get_AliasLevelIsPlusMod(j))
            //                                    {
            //                                        if (slevel == Constants.DEFAULT_LEVEL)
            //                                        {
            //                                            slevel = 1d;
            //                                        }

            //                                        slevel2 = slevel + withBlock4.get_AliasLevel(j);
            //                                    }
            //                                    else if (withBlock4.get_AliasLevelIsMultMod(j))
            //                                    {
            //                                        if (slevel == Constants.DEFAULT_LEVEL)
            //                                        {
            //                                            slevel = 1d;
            //                                        }

            //                                        slevel2 = slevel * withBlock4.get_AliasLevel(j);
            //                                    }
            //                                    else if (slevel != Constants.DEFAULT_LEVEL)
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
            //                                            sdata2 = sdata + " " + GeneralLib.ListTail(sdata2, (GeneralLib.LLength(sdata) + 1));
            //                                        }
            //                                    }

            //                                    if (withBlock4.get_AliasLevelIsPlusMod(j) || withBlock4.get_AliasLevelIsMultMod(j))
            //                                    {
            //                                        sdata2 = GeneralLib.LIndex(sdata2, 1) + "Lv" + SrcFormatter.Format(slevel) + " " + GeneralLib.ListTail(sdata2, 2);
            //                                        sdata2 = Strings.Trim(sdata2);
            //                                    }
            //                                }

            //                                // 属性使用不能攻撃により使用不能になった技能を封印する。
            //                                if (ConditionLifetime(stype2 + "使用不能") > 0)
            //                                {
            //                                    goto NextFeature;
            //                                }

            //                                AddCondition(stype2 + "付加２", -1, slevel2, sdata2);
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        // 属性使用不能攻撃により使用不能になった技能を封印する。
            //                        if (ConditionLifetime(stype + "使用不能") > 0)
            //                        {
            //                            goto NextFeature;
            //                        }

            //                        AddCondition(stype + "付加２", -1, slevel, sdata);
            //                    }

            //                    break;
            //                }

            //            case "パイロット能力強化":
            //                {
            //                    // 必要技能を満たしている？
            //                    if (!IsNecessarySkillSatisfied(fd.NecessarySkill, p: null))
            //                    {
            //                        found = true;
            //                        goto NextFeature;
            //                    }
            //                    // 必要条件を満たしている？
            //                    if (!IsNecessarySkillSatisfied(fd.NecessaryCondition, p: null))
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
            //                    if (SRC.ALDList.IsDefined(stype))
            //                    {
            //                        {
            //                            var withBlock5 = SRC.ALDList.Item(stype);
            //                            var loopTo4 = withBlock5.Count;
            //                            for (j = 1; j <= loopTo4; j++)
            //                            {
            //                                // エリアスの定義に従って特殊能力定義を置き換える
            //                                stype2 = withBlock5.get_AliasType(j);

            //                                // 属性使用不能攻撃により使用不能になった技能を封印する。
            //                                if (ConditionLifetime(stype2 + "使用不能") > 0)
            //                                {
            //                                    goto NextFeature;
            //                                }

            //                                string localLIndex1() { string arglist = withBlock5.get_AliasData(j); var ret = GeneralLib.LIndex(arglist, 1); withBlock5.get_AliasData(j) = arglist; return ret; }

            //                                if (localLIndex1() == "解説")
            //                                {
            //                                    // 特殊能力の解説
            //                                    if (!string.IsNullOrEmpty(sdata))
            //                                    {
            //                                        stype2 = GeneralLib.LIndex(sdata, 1);
            //                                    }

            //                                    slevel2 = Constants.DEFAULT_LEVEL;
            //                                    sdata2 = withBlock5.get_AliasData(j);
            //                                    // 属性使用不能攻撃により使用不能になった技能を封印する。
            //                                    if (ConditionLifetime(stype2 + "使用不能") > 0)
            //                                    {
            //                                        goto NextFeature;
            //                                    }

            //                                    AddCondition(stype2 + "付加２", -1, slevel2, sdata2);
            //                                }
            //                                else
            //                                {
            //                                    // 通常の能力
            //                                    if (withBlock5.get_AliasLevelIsMultMod(j))
            //                                    {
            //                                        if (slevel == Constants.DEFAULT_LEVEL)
            //                                        {
            //                                            slevel = 1d;
            //                                        }

            //                                        slevel2 = slevel * withBlock5.get_AliasLevel(j);
            //                                    }
            //                                    else if (slevel != Constants.DEFAULT_LEVEL)
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
            //                                            sdata2 = sdata + " " + GeneralLib.ListTail(sdata2, (GeneralLib.LLength(sdata) + 1));
            //                                        }
            //                                    }

            //                                    // 強化するレベルは累積する
            //                                    if (IsConditionSatisfied(stype2 + "強化２"))
            //                                    {
            //                                        double localConditionLevel() { object argIndex1 = stype2 + "強化２"; var ret = ConditionLevel(argIndex1); return ret; }

            //                                        slevel2 = slevel2 + localConditionLevel();
            //                                        DeleteCondition(stype2 + "強化２");
            //                                    }

            //                                    AddCondition(stype2 + "強化２", -1, slevel2, sdata2);
            //                                }
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        // 強化するレベルは累積する
            //                        if (IsConditionSatisfied(stype + "強化２"))
            //                        {
            //                            double localConditionLevel1() { object argIndex1 = stype + "強化２"; var ret = ConditionLevel(argIndex1); return ret; }

            //                            slevel = slevel + localConditionLevel1();
            //                            DeleteCondition(stype + "強化２");
            //                        }

            //                        AddCondition(stype + "強化２", -1, slevel, sdata);
            //                    }

            //                    break;
            //                }
            //        }

            //    NextFeature:
            //        ;
            //        i = (i + 1);
            //    }
            //    // 必要技能＆必要条件付きのパイロット能力付加＆強化がある場合は付加や強化の結果、
            //    // 必要技能＆必要条件が満たされることがあるので一度だけやり直す
            //    if (!flag && found)
            //    {
            //        flag = true;
            //        goto AddSkills;
            //    }

            // パイロット用特殊能力の付加＆強化が完了したので必要技能の判定が可能になった。
            UpdateFeatures();

            //    // アイテムが必要技能を満たすか再度チェック。
            //    found = false;
            //    foreach (Item currentItm1 in colItem)
            //    {
            //        itm = currentItm1;
            //        if (itm.Activated != itm.IsAvailable(this))
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
            //        if (withBlock6.IsFeatureAvailable("ランクアップ"))
            //        {
            //            if (Rank >= withBlock6.FeatureLevel("ランクアップ"))
            //            {
            //                if (IsNecessarySkillSatisfied(withBlock6.FeatureNecessarySkill(argIndex18), p: null))
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
            //            Pilot localPilot1() { object argIndex1 = i; var ret = Pilot(argIndex1); return ret; }

            //            localPilot1().Update();
            //        }

            //        var loopTo6 = CountSupport();
            //        for (i = 1; i <= loopTo6; i++)
            //        {
            //            Pilot localSupport1() { object argIndex1 = i; var ret = Support(argIndex1); return ret; }

            //            localSupport1().Update();
            //        }

            //        // メインパイロットは他のパイロットのサポートを受ける関係上
            //        // 最後にアップデートする
            //        Pilot(1).Update();
            //        if (!ReferenceEquals(MainPilot(), Pilot(1)))
            //        {
            //            MainPilot().Update();
            //        }
            //    }

            //    // ユニット画像用ファイル名に変化がある場合はユニット画像を更新
            //    if (BitmapID != 0)
            //    {
            //        if ((ubitmap ?? "") != (get_Bitmap(false) ?? ""))
            //        {
            //            BitmapID = GUI.MakeUnitBitmap(this);
            //            var loopTo7 = CountOtherForm();
            //            for (i = 1; i <= loopTo7; i++)
            //            {
            //                Unit localOtherForm() { object argIndex1 = i; var ret = OtherForm(argIndex1); return ret; }

            //                localOtherForm().BitmapID = 0;
            //            }

            //            if (!without_refresh)
            //            {
            //                if (Status == "出撃")
            //                {
            //                    if (!GUI.IsPictureVisible && !string.IsNullOrEmpty(Map.MapFileName))
            //                    {
            //                        GUI.PaintUnitBitmap(this);
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // ユニットの表示、非表示が切り替わった場合
            //    if (is_invisible != IsFeatureAvailable("非表示"))
            //    {
            //        if (Status == "出撃")
            //        {
            //            if (!GUI.IsPictureVisible && !string.IsNullOrEmpty(Map.MapFileName))
            //            {
            //                BitmapID = GUI.MakeUnitBitmap(this);
            //                if (IsFeatureAvailable("非表示"))
            //                {
            //                    GUI.EraseUnitBitmap(x, y, !without_refresh);
            //                }
            //                else if (!without_refresh)
            //                {
            //                    GUI.PaintUnitBitmap(this);
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

            // ボスランクによる修正
            if (IsHero() || Expression.IsOptionDefined("等身大基準"))
            {
                switch (BossRank)
                {
                    case 1:
                        {
                            lngMaxHP = lngMaxHP + Data.HP;
                            break;
                        }

                    case 2:
                        {
                            lngMaxHP = lngMaxHP + Data.HP + 10000;
                            break;
                        }

                    case 3:
                        {
                            lngMaxHP = lngMaxHP + Data.HP + 20000;
                            break;
                        }

                    case 4:
                        {
                            lngMaxHP = lngMaxHP + Data.HP + 40000;
                            break;
                        }

                    case 5:
                        {
                            lngMaxHP = lngMaxHP + Data.HP + 80000;
                            break;
                        }
                }

                if (BossRank > 0)
                {
                    lngArmor = lngArmor + 200 * BossRank;
                }
            }
            else
            {
                switch (BossRank)
                {
                    case 1:
                        {
                            lngMaxHP = (int)(lngMaxHP + 0.5d * Data.HP);
                            break;
                        }

                    case 2:
                        {
                            lngMaxHP = lngMaxHP + Data.HP;
                            break;
                        }

                    case 3:
                        {
                            lngMaxHP = lngMaxHP + Data.HP + 10000;
                            break;
                        }

                    case 4:
                        {
                            lngMaxHP = lngMaxHP + Data.HP + 20000;
                            break;
                        }

                    case 5:
                        {
                            lngMaxHP = lngMaxHP + Data.HP + 40000;
                            break;
                        }
                }

                if (Expression.IsOptionDefined("BossRank装甲修正低下"))
                {
                    if (BossRank > 0)
                    {
                        lngArmor = lngArmor + 300 * BossRank;
                    }
                }
                else
                {
                    switch (BossRank)
                    {
                        case 1:
                            {
                                lngArmor = lngArmor + 300;
                                break;
                            }

                        case 2:
                            {
                                lngArmor = lngArmor + 600;
                                break;
                            }

                        case 3:
                            {
                                lngArmor = lngArmor + 1000;
                                break;
                            }

                        case 4:
                            {
                                lngArmor = lngArmor + 1500;
                                break;
                            }

                        case 5:
                            {
                                lngArmor = lngArmor + 2500;
                                break;
                            }
                    }
                }
            }

            if (BossRank > 0)
            {
                intMaxEN = (intMaxEN + 20 * BossRank);
                intMobility = (intMobility + 5 * BossRank);
            }

            // ＨＰ成長オプション
            if (Expression.IsOptionDefined("ＨＰ成長"))
            {
                if (CountPilot() > 0)
                {
                    lngMaxHP = GeneralLib.MinLng((int)(lngMaxHP / 100d * (100 + this.MainPilot().Level)), 9999999);
                }
            }

            // ＥＮ成長オプション
            if (Expression.IsOptionDefined("ＥＮ成長"))
            {
                if (CountPilot() > 0)
                {
                    intMaxEN = GeneralLib.MinLng((int)(intMaxEN / 100d * (100 + this.MainPilot().Level)), 9999);
                }
            }

            // 特殊能力による修正
            int pmorale;
            if (CountPilot() > 0)
            {
                pmorale = MainPilot().Morale;
            }
            else
            {
                pmorale = 100;
            }

            foreach (FeatureData fd in colFeature.List)
            {
                switch (fd.Name ?? "")
                {
                    // 固定値による強化
                    case "ＨＰ強化":
                        {
                            if (pmorale >= GeneralLib.StrToLng(GeneralLib.LIndex(fd.StrData, 2)))
                            {
                                lngMaxHP = (int)(lngMaxHP + 200d * fd.Level);
                            }

                            break;
                        }

                    case "ＥＮ強化":
                        {
                            if (pmorale >= GeneralLib.StrToLng(GeneralLib.LIndex(fd.StrData, 2)))
                            {
                                intMaxEN = (int)(intMaxEN + 10d * fd.Level);
                            }

                            break;
                        }

                    case "装甲強化":
                        {
                            if (pmorale >= GeneralLib.StrToLng(GeneralLib.LIndex(fd.StrData, 2)))
                            {
                                lngArmor = (int)(lngArmor + 100d * fd.Level);
                            }

                            break;
                        }

                    case "運動性強化":
                        {
                            if (pmorale >= GeneralLib.StrToLng(GeneralLib.LIndex(fd.StrData, 2)))
                            {
                                intMobility = (int)(intMobility + 5d * fd.Level);
                            }

                            break;
                        }

                    case "移動力強化":
                        {
                            if (pmorale >= GeneralLib.StrToLng(GeneralLib.LIndex(fd.StrData, 2)))
                            {
                                intSpeed = (int)(intSpeed + fd.Level);
                            }

                            break;
                        }
                    // 割合による強化
                    case "ＨＰ割合強化":
                        {
                            if (pmorale >= GeneralLib.StrToLng(GeneralLib.LIndex(fd.StrData, 2)))
                            {
                                lngMaxHP = (int)(lngMaxHP + (long)(Data.HP * fd.Level) / 20L);
                            }

                            break;
                        }

                    case "ＥＮ割合強化":
                        {
                            if (pmorale >= GeneralLib.StrToLng(GeneralLib.LIndex(fd.StrData, 2)))
                            {
                                intMaxEN = (int)(intMaxEN + (long)(Data.EN * fd.Level) / 20L);
                            }

                            break;
                        }

                    case "装甲割合強化":
                        {
                            if (pmorale >= GeneralLib.StrToLng(GeneralLib.LIndex(fd.StrData, 2)))
                            {
                                lngArmor = (int)(lngArmor + (long)(Data.Armor * fd.Level) / 20L);
                            }

                            break;
                        }

                    case "運動性割合強化":
                        {
                            if (pmorale >= GeneralLib.StrToLng(GeneralLib.LIndex(fd.StrData, 2)))
                            {
                                intMobility = (int)(intMobility + (long)(Data.Mobility * fd.Level) / 20L);
                            }

                            break;
                        }
                }
            }

            //    // アイテムによる修正
            //    foreach (Item currentItm2 in colItem)
            //    {
            //        itm = currentItm2;
            //        if (itm.Activated)
            //        {
            //            lngMaxHP = lngMaxHP + itm.HP();
            //            intMaxEN = (intMaxEN + itm.EN());
            //            lngArmor = lngArmor + itm.Armor();
            //            intMobility = (intMobility + itm.Mobility());
            //            intSpeed = (intSpeed + itm.Speed());
            //        }
            //    }

            //    // 装備している「Ｖ－ＵＰ=ユニット」アイテムによる修正
            //    num = 0;
            //    if (IsConditionSatisfied("Ｖ－ＵＰ"))
            //    {
            //        switch (FeatureData("Ｖ－ＵＰ") ?? "")
            //        {
            //            case "全":
            //            case "ユニット":
            //                {
            //                    num = (num + 1);
            //                    break;
            //                }
            //        }
            //    }

            //    foreach (Item currentItm3 in colItem)
            //    {
            //        itm = currentItm3;
            //        if (itm.IsFeatureAvailable("Ｖ－ＵＰ"))
            //        {
            //            switch (itm.FeatureData("Ｖ－ＵＰ") ?? "")
            //            {
            //                case "全":
            //                case "ユニット":
            //                    {
            //                        num = (num + 1);
            //                        break;
            //                    }
            //            }
            //        }
            //    }

            //    if (CountPilot() > 0)
            //    {
            //        {
            //            var withBlock8 = MainPilot().Data;
            //            if (withBlock8.IsFeatureAvailable("Ｖ－ＵＰ"))
            //            {
            //                switch (withBlock8.FeatureData("Ｖ－ＵＰ") ?? "")
            //                {
            //                    case "全":
            //                    case "ユニット":
            //                        {
            //                            num = (num + 1);
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
            //            intMaxEN = (intMaxEN + 20 * num * withBlock9.ItemNum);
            //            lngArmor = lngArmor + 50 * num * withBlock9.ItemNum;
            //            intMobility = (intMobility + 5 * num * withBlock9.ItemNum);
            //        }
            //    }

            //    // 追加移動力
            //    if (IsFeatureAvailable("追加移動力"))
            //    {
            //        foreach (FeatureData currentFd3 in colFeature)
            //        {
            //            fd = currentFd3;
            //            if (fd.Name == "追加移動力")
            //            {
            //                if ((Area ?? "") == (GeneralLib.LIndex(fd.StrData, 2) ?? ""))
            //                {
            //                    intSpeed = (intSpeed + fd.Level);
            //                }
            //            }
            //        }

            //        intSpeed = GeneralLib.MaxLng(intSpeed, 0);
            //    }

            //    // 上限値を超えないように
            //    lngMaxHP = GeneralLib.MinLng(lngMaxHP, 9999999);
            //    intMaxEN = GeneralLib.MinLng(intMaxEN, 9999);
            //    lngArmor = GeneralLib.MinLng(lngArmor, 99999);
            //    intMobility = GeneralLib.MinLng(intMobility, 9999);
            //    intSpeed = GeneralLib.MinLng(intSpeed, 99);

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

            // 地形適応
            var uadaption = new int[5];
            for (var i = 1; i <= 4; i++)
            {
                switch (Strings.Mid(Data.Adaption, i, 1) ?? "")
                {
                    case "S":
                        {
                            uadaption[i] = 5;
                            break;
                        }

                    case "A":
                        {
                            uadaption[i] = 4;
                            break;
                        }

                    case "B":
                        {
                            uadaption[i] = 3;
                            break;
                        }

                    case "C":
                        {
                            uadaption[i] = 2;
                            break;
                        }

                    case "D":
                        {
                            uadaption[i] = 1;
                            break;
                        }

                    case "E":
                    case "-":
                        {
                            uadaption[i] = 0;
                            break;
                        }
                }
            }

            // 移動タイプ追加による地形適応修正
            if (IsFeatureAvailable("空中移動"))
            {
                uadaption[1] = GeneralLib.MaxLng(uadaption[1], 4);
            }

            if (IsFeatureAvailable("陸上移動"))
            {
                uadaption[2] = GeneralLib.MaxLng(uadaption[2], 4);
            }

            if (IsFeatureAvailable("水中移動"))
            {
                uadaption[3] = GeneralLib.MaxLng(uadaption[3], 4);
            }

            if (IsFeatureAvailable("宇宙移動"))
            {
                uadaption[4] = GeneralLib.MaxLng(uadaption[4], 4);
            }

            // 地形適応変更能力による修正
            foreach (var fd in Features)
            {
                switch (fd.Name ?? "")
                {
                    case "地形適応変更":
                        {
                            for (var i = 1; i <= 4; i++)
                            {
                                var num = GeneralLib.StrToLng(GeneralLib.LIndex(fd.StrData, i));
                                if (num > 0)
                                {
                                    if (uadaption[i] < 4)
                                    {
                                        uadaption[i] = (uadaption[i] + num);
                                        // 地形適応はAより高くはならない
                                        if (uadaption[i] > 4)
                                        {
                                            uadaption[i] = 4;
                                        }
                                    }
                                }
                                else
                                {
                                    uadaption[i] = (uadaption[i] + num);
                                }
                            }

                            break;
                        }

                    case "地形適応固定変更":
                        {
                            for (var i = 1; i <= 4; i++)
                            {
                                var num = GeneralLib.StrToLng(GeneralLib.LIndex(fd.StrData, i));
                                if (GeneralLib.LIndex(fd.StrData, 5) == "強制")
                                {
                                    // 強制変更の場合
                                    if (num >= 0 && num <= 5)
                                    {
                                        uadaption[i] = num;
                                    }
                                }
                                // 高いほうを優先する場合
                                else if (num > uadaption[i] && num <= 5)
                                {
                                    uadaption[i] = num;
                                }
                            }

                            break;
                        }
                }
            }

            strAdaption = "";
            for (var i = 1; i <= 4; i++)
            {
                switch (uadaption[i])
                {
                    case var @case when @case >= 5:
                        {
                            strAdaption = strAdaption + "S";
                            break;
                        }

                    case 4:
                        {
                            strAdaption = strAdaption + "A";
                            break;
                        }

                    case 3:
                        {
                            strAdaption = strAdaption + "B";
                            break;
                        }

                    case 2:
                        {
                            strAdaption = strAdaption + "C";
                            break;
                        }

                    case 1:
                        {
                            strAdaption = strAdaption + "D";
                            break;
                        }

                    case var case1 when case1 <= 0:
                        {
                            strAdaption = strAdaption + "-";
                            break;
                        }
                }
            }

            //    // 空中に留まることが出来るかチェック
            //    if (Status == "出撃" && Area == "空中" && !IsTransAvailable("空"))
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
            //                    if (IsTransAvailable("水上"))
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
            //            if (!GUI.IsPictureVisible && !string.IsNullOrEmpty(Map.MapFileName))
            //            {
            //                GUI.PaintUnitBitmap(this);
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
            //        if (ConditionLifetime(i) != 0)
            //        {
            //            ch = Condition(i);
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
            //    var loopTo9 = Strings.Len(strAbsorb);
            //    for (i = 1; i <= loopTo9; i++)
            //    {
            //        ch = GeneralLib.GetClassBundle(strAbsorb, i);
            //        if (GeneralLib.InStrNotNest(buf, ch) == 0)
            //        {
            //            buf = buf + ch;
            //        }
            //    }

            //    strAbsorb = buf;
            //    buf = "";
            //    var loopTo10 = Strings.Len(strImmune);
            //    for (i = 1; i <= loopTo10; i++)
            //    {
            //        ch = GeneralLib.GetClassBundle(strImmune, i);
            //        if (GeneralLib.InStrNotNest(buf, ch) == 0)
            //        {
            //            buf = buf + ch;
            //        }
            //    }

            //    strImmune = buf;
            //    buf = "";
            //    var loopTo11 = Strings.Len(strResist);
            //    for (i = 1; i <= loopTo11; i++)
            //    {
            //        ch = GeneralLib.GetClassBundle(strResist, i);
            //        if (GeneralLib.InStrNotNest(buf, ch) == 0)
            //        {
            //            buf = buf + ch;
            //        }
            //    }

            //    strResist = buf;
            //    buf = "";
            //    var loopTo12 = Strings.Len(strWeakness);
            //    for (i = 1; i <= loopTo12; i++)
            //    {
            //        ch = GeneralLib.GetClassBundle(strWeakness, i);
            //        if (GeneralLib.InStrNotNest(buf, ch) == 0)
            //        {
            //            buf = buf + ch;
            //        }
            //    }

            //    strWeakness = buf;
            //    buf = "";
            //    var loopTo13 = Strings.Len(strEffective);
            //    for (i = 1; i <= loopTo13; i++)
            //    {
            //        ch = GeneralLib.GetClassBundle(strEffective, i);
            //        if (GeneralLib.InStrNotNest(buf, ch) == 0)
            //        {
            //            buf = buf + ch;
            //        }
            //    }

            //    strEffective = buf;
            //    buf = "";
            //    var loopTo14 = Strings.Len(strSpecialEffectImmune);
            //    for (i = 1; i <= loopTo14; i++)
            //    {
            //        ch = GeneralLib.GetClassBundle(strSpecialEffectImmune, i);
            //        if (GeneralLib.InStrNotNest(buf, ch) == 0)
            //        {
            //            buf = buf + ch;
            //        }
            //    }

            //    strSpecialEffectImmune = buf;

            // 武器データを更新
            {
                var prevWeapons = WData?.CloneList() ?? new List<UnitWeapon>();
                var newWeapons = new List<UnitWeapon>();
                // ユニット分
                newWeapons.AddRange(Data.Weapons.Select(x => UnitWeapon.NewOrRef(SRC, this, x, prevWeapons)));
                // メインパイロット、サブパイロット、サポート、追加サポート分
                if (CountPilot() > 0)
                {
                    newWeapons.AddRange(AllPilots.SelectMany(p => p.Data.Weapons.Select(x => UnitWeapon.NewOrRef(SRC, this, x, prevWeapons))));
                }
                // アイテム分
                newWeapons.AddRange(ItemList.SelectMany(itm => itm.Data.Weapons.Select(x => UnitWeapon.NewOrRef(SRC, this, x, prevWeapons))));
                WData = newWeapons;
            }

            // 武器属性を更新
            if (IsFeatureAvailable("攻撃属性"))
            {
                foreach (var w in Weapons)
                {
                    var wclass = w.WeaponData.Class;
                    string hidden_attr;
                    // 非表示の属性がある場合は一旦抜き出す
                    if (GeneralLib.InStrNotNest(wclass, "|") > 0)
                    {
                        wclass = Strings.Left(wclass, GeneralLib.InStrNotNest(wclass, "|") - 1);
                        hidden_attr = Strings.Mid(wclass, GeneralLib.InStrNotNest(wclass, "|") + 1);
                    }
                    else
                    {
                        hidden_attr = "";
                    }

                    foreach (var fd in Features.Where(x => x.Name == "攻撃属性"))
                    {
                        var fdata = fd.Data;
                        // 「"」を除去
                        if (Strings.Left(fdata, 1) == "\"")
                        {
                            fdata = Strings.Mid(fdata, 2, Strings.Len(fdata) - 2);
                        }

                        var flen = GeneralLib.LLength(fdata);
                        var skip = 0;
                        if (flen == 1)
                        {
                            // 武器指定がない場合はすべての武器に属性を付加
                            skip = 1;
                        }
                        else if (GeneralLib.LIndex(fdata, 1) == "非表示")
                        {
                            // 非表示指定がある場合 (武器指定がある場合を含む)
                            skip = 2;
                        }
                        else
                        {
                            // 武器指定がある場合
                            skip = 2;
                        }

                        if (w.IsMatchFeatureTarget(GeneralLib.ToL(fdata).Skip(skip).ToList()))
                        {
                            // 属性を追加
                            var buf = GeneralLib.LIndex(fdata, 1);
                            if (buf == "非表示")
                            {
                                // 非表示の属性の場合
                                hidden_attr = hidden_attr + GeneralLib.LIndex(fdata, 2);
                            }
                            else
                            {
                                // 属性が重複しないように付加
                                var skipped = false;
                                var loopTo28 = Strings.Len(buf);
                                for (var k = 1; k <= loopTo28; k++)
                                {
                                    var ch = GeneralLib.GetClassBundle(buf, ref k);
                                    if (!Information.IsNumeric(ch) && ch != "L" && ch != ".")
                                    {
                                        skipped = false;
                                    }

                                    if ((GeneralLib.InStrNotNest(wclass, ch) == 0 || Information.IsNumeric(ch) || ch == "L" || ch == ".") && !skipped)
                                    {
                                        if (ch == "魔")
                                        {
                                            // 魔属性を付加する場合は武器を魔法武器化する
                                            var l = GeneralLib.InStrNotNest(wclass, "武");
                                            l = GeneralLib.MaxLng(GeneralLib.InStrNotNest(wclass, "突"), l);
                                            l = GeneralLib.MaxLng(GeneralLib.InStrNotNest(wclass, "接"), l);
                                            l = GeneralLib.MaxLng(GeneralLib.InStrNotNest(wclass, "銃"), l);
                                            l = GeneralLib.MaxLng(GeneralLib.InStrNotNest(wclass, "実"), l);
                                            if (l > 0)
                                            {
                                                wclass = Strings.Left(wclass, l - 1) + ch + Strings.Mid(wclass, l);
                                            }
                                            else
                                            {
                                                wclass = wclass + ch;
                                            }
                                        }
                                        else
                                        {
                                            wclass = wclass + ch;
                                        }
                                    }
                                    else
                                    {
                                        skipped = true;
                                    }
                                }
                            }
                        }
                    }

                    w.SetWeaponClass(wclass + "|" + hidden_attr);
                }
            }

            // 武器攻撃力を更新
            // 装備している「Ｖ－ＵＰ=武器」アイテムの個数をカウントしておく
            var vupNum = 0;
            if (IsConditionSatisfied("Ｖ－ＵＰ"))
            {
                switch (FeatureData("Ｖ－ＵＰ") ?? "")
                {
                    case "全":
                    case "武器":
                        {
                            vupNum = (vupNum + 1);
                            break;
                        }
                }
            }

            foreach (var itm in colItem.List)
            {
                if (itm.Activated)
                {
                    if (itm.IsFeatureAvailable("Ｖ－ＵＰ"))
                    {
                        switch (itm.FeatureData("Ｖ－ＵＰ") ?? "")
                        {
                            case "全":
                            case "武器":
                                {
                                    vupNum = (vupNum + 1);
                                    break;
                                }
                        }
                    }
                }
            }

            if (CountPilot() > 0)
            {
                {
                    var withBlock16 = MainPilot().Data;
                    if (withBlock16.IsFeatureAvailable("Ｖ－ＵＰ"))
                    {
                        switch (withBlock16.FeatureData("Ｖ－ＵＰ") ?? "")
                        {
                            case "全":
                            case "武器":
                                {
                                    vupNum = (vupNum + 1);
                                    break;
                                }
                        }
                    }
                }
            }

            vupNum = (vupNum * Data.ItemNum);
            foreach (var w in Weapons)
            {
                var correction = 0;

                // もともと攻撃力が0の武器は0に固定
                if (w.WeaponData.Power == 0)
                {
                    continue;
                }

                // 武器強化による修正
                if (IsFeatureAvailable("武器強化"))
                {
                    foreach (var fd in Features.Where(x => x.Name == "武器強化"))
                    {
                        var fdata = fd.Data;
                        // 「"」を除去
                        if (Strings.Left(fdata, 1) == "\"")
                        {
                            fdata = Strings.Mid(fdata, 2, Strings.Len(fdata) - 2);
                        }

                        if (w.IsMatchFeatureTarget(GeneralLib.ToL(fdata)))
                        {
                            if (w.IsWeaponClassifiedAs("固"))
                            {
                                // ダメージ固定武器は武器指定が武器名、武器表示名、「固」の
                                // いずれかで行われた場合にのみ強化
                                if (GeneralLib.ToL(fdata).Any(x => x == "固" || x == w.Name || x == w.WeaponNickname()))
                                {
                                    correction = (int)(correction + 100d * fd.FeatureLevel);
                                }
                            }
                            else
                            {
                                correction = (int)(correction + 100d * fd.FeatureLevel);
                            }
                        }
                    }
                }

                // 武器割合強化による修正
                if (IsFeatureAvailable("武器割合強化"))
                {
                    foreach (var fd in Features.Where(x => x.Name == "武器割合強化"))
                    {
                        var fdata = fd.Data;
                        // 「"」を除去
                        if (Strings.Left(fdata, 1) == "\"")
                        {
                            fdata = Strings.Mid(fdata, 2, Strings.Len(fdata) - 2);
                        }

                        if (w.IsMatchFeatureTarget(GeneralLib.ToL(fdata)))
                        {
                            if (w.IsWeaponClassifiedAs("固"))
                            {
                                // ダメージ固定武器は武器指定が武器名、武器表示名、「固」の
                                // いずれかで行われた場合にのみ強化
                                if (GeneralLib.ToL(fdata).Any(x => x == "固" || x == w.Name || x == w.WeaponNickname()))
                                {
                                    correction = (int)(correction + w.WeaponData.Power * fd.FeatureLevel / 20);
                                }
                            }
                            else
                            {
                                correction = (int)(correction + w.WeaponData.Power * fd.FeatureLevel / 20);
                            }
                        }
                    }
                }

                // ダメージ固定武器
                if (w.IsWeaponClassifiedAs("固"))
                {
                    // 適用して次へ
                    w.SetPowerCorrection(correction);
                    continue;
                }

                if (w.IsWeaponClassifiedAs("Ｒ"))
                {
                    // 低成長型の攻撃
                    if (w.IsWeaponLevelSpecified("Ｒ"))
                    {
                        // レベル設定されている場合、増加量をレベル×１０×ランクにする
                        correction = (int)(correction + 10d * w.WeaponLevel("Ｒ") * (Rank + vupNum));
                        // オ・シ・超と併用した場合
                        if (w.IsWeaponClassifiedAs("オ") || w.IsWeaponClassifiedAs("超") || w.IsWeaponClassifiedAs("シ"))
                        {
                            correction = (int)(correction + 10d * (10d - w.WeaponLevel("Ｒ")) * (Rank + vupNum));

                            // オーラ技
                            if (w.IsWeaponClassifiedAs("オ"))
                            {
                                correction = (int)(correction + 10d * w.WeaponLevel("Ｒ") * AuraLevel());
                            }

                            // サイキック攻撃
                            if (w.IsWeaponClassifiedAs("超"))
                            {
                                correction = (int)(correction + 10d * w.WeaponLevel("Ｒ") * PsychicLevel());
                            }

                            // 同調率対象攻撃
                            if (w.IsWeaponClassifiedAs("シ"))
                            {
                                if (CountPilot() > 0)
                                {
                                    if (MainPilot().SynchroRate() > 0)
                                    {
                                        correction = (int)(correction + (long)(15d * w.WeaponLevel("Ｒ") * (SyncLevel() - 50d)) / 10L);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // レベル指定されていない場合は今までどおりランク×５０
                        correction = correction + 50 * (Rank + vupNum);

                        // オ・シ・超と併用した場合
                        if (w.IsWeaponClassifiedAs("オ") || w.IsWeaponClassifiedAs("超") || w.IsWeaponClassifiedAs("シ"))
                        {
                            correction = correction + 50 * (Rank + vupNum);

                            // オーラ技
                            if (w.IsWeaponClassifiedAs("オ"))
                            {
                                correction = (int)(correction + 50d * AuraLevel());
                            }

                            // サイキック攻撃
                            if (w.IsWeaponClassifiedAs("超"))
                            {
                                correction = (int)(correction + 50d * PsychicLevel());
                            }

                            // 同調率対象攻撃
                            if (w.IsWeaponClassifiedAs("シ"))
                            {
                                if (CountPilot() > 0)
                                {
                                    if (MainPilot().SynchroRate() > 0)
                                    {
                                        correction = (int)(correction + (long)(15d * (SyncLevel() - 50d)) / 2L);
                                    }
                                }
                            }
                        }
                    }
                }
                else if (w.IsWeaponClassifiedAs("改"))
                {
                    // 改属性＝オ・超・シ属性を無視したＲ属性
                    if (w.IsWeaponLevelSpecified("改"))
                    {
                        // レベル設定されている場合、増加量をレベル×１０×ランクにする
                        correction = (int)(correction + 10d * w.WeaponLevel("改") * (Rank + vupNum));
                    }
                    else
                    {
                        // レベル指定がない場合、増加量は５０×ランク
                        correction = correction + 50 * (Rank + vupNum);
                    }

                    // オーラ技
                    if (w.IsWeaponClassifiedAs("オ"))
                    {
                        correction = (int)(correction + 100d * AuraLevel());
                    }

                    // サイキック攻撃
                    if (w.IsWeaponClassifiedAs("超"))
                    {
                        correction = (int)(correction + 100d * PsychicLevel());
                    }

                    // 同調率対象攻撃
                    if (w.IsWeaponClassifiedAs("シ"))
                    {
                        if (CountPilot() > 0)
                        {
                            if (MainPilot().SynchroRate() > 0)
                            {
                                correction = (int)(correction + 15d * (SyncLevel() - 50d));
                            }
                        }
                    }
                }
                else
                {
                    // Ｒ、改属性が両方ともない場合
                    correction = correction + 100 * (Rank + vupNum);

                    // オーラ技
                    if (w.IsWeaponClassifiedAs("オ"))
                    {
                        correction = (int)(correction + 100d * AuraLevel());
                    }

                    // サイキック攻撃
                    if (w.IsWeaponClassifiedAs("超"))
                    {
                        correction = (int)(correction + 100d * PsychicLevel());
                    }

                    // 同調率対象攻撃
                    if (w.IsWeaponClassifiedAs("シ"))
                    {
                        if (CountPilot() > 0)
                        {
                            if (MainPilot().SynchroRate() > 0)
                            {
                                correction = (int)(correction + 15d * (SyncLevel() - 50d));
                            }
                        }
                    }
                }

                // ボスランクによる修正
                if (BossRank > 0)
                {
                    correction = correction + GeneralLib.MinLng(100 * BossRank, 300);
                }

                w.SetPowerCorrection(correction);
            }

            // 武器射程を更新
            foreach (var w in Weapons)
            {
                var correction = 0;

                // 最大射程がもともと１ならそれ以上変化しない
                if (w.WeaponData.MaxRange == 1)
                {
                    continue;
                }

                // 思念誘導攻撃のＮＴ能力による射程延長
                if (w.IsWeaponClassifiedAs("サ"))
                {
                    if (CountPilot() > 0)
                    {
                        var p = MainPilot();
                        correction = (int)(correction
                            + p.SkillLevel("超感覚", ref_mode: "") / 4
                            + p.SkillLevel("知覚強化", ref_mode: "") / 4);
                    }
                }

                // マップ攻撃には適用されない
                if (w.IsWeaponClassifiedAs("Ｍ"))
                {
                    continue;
                }

                // 接近戦武器には適用されない
                if (w.IsWeaponClassifiedAs("武")
                    || w.IsWeaponClassifiedAs("突")
                    || w.IsWeaponClassifiedAs("接"))
                {
                    continue;
                }

                // 有線式誘導攻撃には適用されない
                if (w.IsWeaponClassifiedAs("有"))
                {
                    continue;
                }

                // 射程延長による修正
                if (IsFeatureAvailable("射程延長"))
                {
                    foreach (var fd in Features.Where(x => x.Name == "射程延長"))
                    {
                        var fdata = fd.Data;
                        // 「"」を除去
                        if (Strings.Left(fdata, 1) == "\"")
                        {
                            fdata = Strings.Mid(fdata, 2, Strings.Len(fdata) - 2);
                        }

                        if (w.IsMatchFeatureTarget(GeneralLib.ToL(fdata)))
                        {
                            correction = (int)(correction + fd.FeatureLevel);
                        }
                    }
                }
                w.SetMaxRangeCorrection(correction);
            }

            // 武器命中率を更新
            foreach (var w in Weapons)
            {
                var correction = 0;

                // 武器強化による修正
                if (IsFeatureAvailable("命中率強化"))
                {
                    foreach (var fd in Features.Where(x => x.Name == "命中率強化"))
                    {
                        var fdata = fd.Data;
                        // 「"」を除去
                        if (Strings.Left(fdata, 1) == "\"")
                        {
                            fdata = Strings.Mid(fdata, 2, Strings.Len(fdata) - 2);
                        }

                        if (w.IsMatchFeatureTarget(GeneralLib.ToL(fdata)))
                        {
                            correction = (int)(correction + 5d * fd.FeatureLevel);
                        }
                    }
                }
                w.SetPrecisionCorrection(correction);
            }

            // 武器のＣＴ率を更新
            foreach (var w in Weapons)
            {
                var correction = 0;

                // ＣＴ率強化による修正
                if (IsFeatureAvailable("ＣＴ率強化") && w.IsNormalWeapon())
                {
                    foreach (var fd in Features.Where(x => x.Name == "ＣＴ率強化"))
                    {
                        var fdata = fd.Data;
                        // 「"」を除去
                        if (Strings.Left(fdata, 1) == "\"")
                        {
                            fdata = Strings.Mid(fdata, 2, Strings.Len(fdata) - 2);
                        }

                        if (w.IsMatchFeatureTarget(GeneralLib.ToL(fdata)))
                        {
                            correction = (int)(correction + 5d * fd.FeatureLevel);
                        }
                    }
                }

                // 特殊効果発動率強化による修正
                if (IsFeatureAvailable("特殊効果発動率強化") && !w.IsNormalWeapon())
                {
                    foreach (var fd in Features.Where(x => x.Name == "特殊効果発動率強化"))
                    {
                        var fdata = fd.Data;
                        // 「"」を除去
                        if (Strings.Left(fdata, 1) == "\"")
                        {
                            fdata = Strings.Mid(fdata, 2, Strings.Len(fdata) - 2);
                        }

                        if (w.IsMatchFeatureTarget(GeneralLib.ToL(fdata)))
                        {
                            correction = (int)(correction + 5d * fd.FeatureLevel);
                        }
                    }
                }
            }

            // 最大弾数を更新
            foreach (var w in Weapons)
            {
                //intMaxBullet[i] = Weapon(i).Bullet;

                // 最大弾数の増加率
                var maxBulletRate = 0d;

                // ボスランクによる修正
                if (intBossRank > 0)
                {
                    maxBulletRate = 0.2d * BossRank;
                }

                // 最大弾数増加による修正
                if (IsFeatureAvailable("最大弾数増加"))
                {
                    foreach (var fd in Features.Where(x => x.Name == "最大弾数増加"))
                    {
                        var fdata = fd.Data;
                        // 「"」を除去
                        if (Strings.Left(fdata, 1) == "\"")
                        {
                            fdata = Strings.Mid(fdata, 2, Strings.Len(fdata) - 2);
                        }

                        if (w.IsMatchFeatureTarget(GeneralLib.ToL(fdata)))
                        {
                            maxBulletRate = maxBulletRate + 0.5d * fd.FeatureLevel;
                        }
                    }
                }
                w.SetMaxBulletRate(1d + maxBulletRate);
            }

            // 弾数を更新
            // UnitWeapon の構築で引き継ぎしている
            //Array.Resize(dblBullet, CountWeapon() + 1);
            //flags = new bool[Information.UBound(prev_wdata) + 1];
            //var loopTo55 = CountWeapon();
            //for (i = 1; i <= loopTo55; i++)
            //{
            //    dblBullet[i] = 1d;
            //    var loopTo56 = Information.UBound(prev_wdata);
            //    for (j = 1; j <= loopTo56; j++)
            //    {
            //        if (ReferenceEquals(WData[i], prev_wdata[j]) && !flags[j])
            //        {
            //            dblBullet[i] = prev_wbullets[j];
            //            flags[j] = true;
            //            break;
            //        }
            //    }
            //}

            // アビリティデータを更新
            AData = Data.Abilities.Select(x => new UnitAbility(SRC, this, x)).ToList();
            //    prev_adata = new AbilityData[Information.UBound(adata) + 1];
            //    prev_astocks = new double[Information.UBound(adata) + 1];
            //    var loopTo57 = Information.UBound(adata);
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
            //            adata[i] = withBlock25.Ability(i);
            //        }
            //    }

            //    if (CountPilot() > 0)
            //    {
            //        {
            //            var withBlock26 = MainPilot().Data;
            //            var loopTo59 = withBlock26.CountAbility();
            //            for (i = 1; i <= loopTo59; i++)
            //            {
            //                Array.Resize(adata, Information.UBound(adata) + 1 + 1);
            //                adata[Information.UBound(adata)] = withBlock26.Ability(i);
            //            }
            //        }

            //        var loopTo60 = CountPilot();
            //        for (i = 2; i <= loopTo60; i++)
            //        {
            //            Pilot localPilot3() { object argIndex1 = i; var ret = Pilot(argIndex1); return ret; }

            //            {
            //                var withBlock27 = localPilot3().Data;
            //                var loopTo61 = withBlock27.CountAbility();
            //                for (j = 1; j <= loopTo61; j++)
            //                {
            //                    Array.Resize(adata, Information.UBound(adata) + 1 + 1);
            //                    adata[Information.UBound(adata)] = withBlock27.Ability(j);
            //                }
            //            }
            //        }

            //        var loopTo62 = CountSupport();
            //        for (i = 1; i <= loopTo62; i++)
            //        {
            //            Pilot localSupport3() { object argIndex1 = i; var ret = Support(argIndex1); return ret; }

            //            {
            //                var withBlock28 = localSupport3().Data;
            //                var loopTo63 = withBlock28.CountAbility();
            //                for (j = 1; j <= loopTo63; j++)
            //                {
            //                    Array.Resize(adata, Information.UBound(adata) + 1 + 1);
            //                    adata[Information.UBound(adata)] = withBlock28.Ability(j);
            //                }
            //            }
            //        }

            //        if (IsFeatureAvailable("追加サポート"))
            //        {
            //            {
            //                var withBlock29 = AdditionalSupport().Data;
            //                var loopTo64 = withBlock29.CountAbility();
            //                for (i = 1; i <= loopTo64; i++)
            //                {
            //                    Array.Resize(adata, Information.UBound(adata) + 1 + 1);
            //                    adata[Information.UBound(adata)] = withBlock29.Ability(i);
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
            //                Array.Resize(adata, Information.UBound(adata) + 1 + 1);
            //                adata[Information.UBound(adata)] = itm.Ability(i);
            //            }
            //        }
            //    }

            //    // 使用回数を更新
            //    Array.Resize(dblStock, CountAbility() + 1);
            //    flags = new bool[Information.UBound(prev_adata) + 1];
            //    var loopTo66 = CountAbility();
            //    for (i = 1; i <= loopTo66; i++)
            //    {
            //        dblStock[i] = 1d;
            //        var loopTo67 = Information.UBound(prev_adata);
            //        for (j = 1; j <= loopTo67; j++)
            //        {
            //            if (ReferenceEquals(adata[i], prev_adata[j]) && !flags[j])
            //            {
            //                dblStock[i] = prev_astocks[j];
            //                flags[j] = true;
            //                break;
            //            }
            //        }
            //    }

            //    if (Status != "出撃")
            //    {
            //        return;
            //    }

            //    // 制御不能？
            //    if (IsFeatureAvailable("制御不可"))
            //    {
            //        if (!is_uncontrollable)
            //        {
            //            AddCondition("暴走", -1, cdata: "");
            //        }
            //    }
            //    else if (is_uncontrollable)
            //    {
            //        if (IsConditionSatisfied("暴走"))
            //        {
            //            DeleteCondition("暴走");
            //        }
            //    }

            //    // 不安定？
            //    if (IsFeatureAvailable("不安定"))
            //    {
            //        if (!is_stable)
            //        {
            //            if (HP <= MaxHP / 4)
            //            {
            //                AddCondition("暴走", -1, cdata: "");
            //            }
            //        }
            //    }
            //    else if (is_stable)
            //    {
            //        if (IsConditionSatisfied("暴走"))
            //        {
            //            DeleteCondition("暴走");
            //        }
            //    }
        }
    }
}
