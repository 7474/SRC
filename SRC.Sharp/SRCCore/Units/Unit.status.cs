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

            //    // ＨＰとＥＮの値を記録
            //    hp_ratio = 100 * HP / (double)MaxHP;
            //    en_ratio = 100 * EN / (double)MaxEN;

            //    // ユニット用画像ファイル名を記録しておく
            //    ubitmap = get_Bitmap(false);

            //    // 非表示かどうか記録しておく
            //    is_invisible = IsFeatureAvailable(ref "非表示");

            //    // 制御不可がどうかを記録しておく
            //    is_uncontrollable = IsFeatureAvailable(ref "制御不可");

            //    // 不安定がどうかを記録しておく
            //    is_stable = IsFeatureAvailable(ref "不安定");
            //TryAgain:
            //    ;


            //    // アイテムが現在の形態で効力を発揮してくれるか判定
            //    foreach (Item currentItm in colItem)
            //    {
            //        itm = currentItm;
            //        itm.Activated = itm.IsAvailable(ref this);
            //    }

            //    // ランクアップによるデータ変更
            //    while (Data.IsFeatureAvailable(ref "ランクアップ"))
            //    {
            //        {
            //            var withBlock = Data;
            //            if (Rank < withBlock.FeatureLevel(ref "ランクアップ"))
            //            {
            //                break;
            //            }

            //            bool localIsNecessarySkillSatisfied() { object argIndex1 = "ランクアップ"; string argnabilities = withBlock.FeatureNecessarySkill(ref argIndex1); Pilot argp = null; var ret = IsNecessarySkillSatisfied(ref argnabilities, p: ref argp); return ret; }

            //            if (!localIsNecessarySkillSatisfied())
            //            {
            //                break;
            //            }

            //            fdata = withBlock.FeatureData(ref "ランクアップ");
            //        }

            //        {
            //            var withBlock1 = SRC.UDList;
            //            bool localIsDefined() { object argIndex1 = fdata; var ret = withBlock1.IsDefined(ref argIndex1); return ret; }

            //            if (!localIsDefined())
            //            {
            //                GUI.ErrorMessage(ref Name + "のランクアップ先ユニット「" + fdata + "」のデータが定義されていません");
            //                SRC.TerminateSRC();
            //            }

            //            Data = withBlock1.Item(ref fdata);
            //        }
            //    }

            // 特殊能力を更新

            // まず特殊能力リストをクリア
            colFeature.Clear();

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

            // ユニットデータで定義されている特殊能力
            AddFeatures(Data.Features);

            //    // アイテムで得られた特殊能力
            //    var loopTo = CountItem();
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        {
            //            var withBlock3 = Item(ref i);
            //            if (withBlock3.Activated)
            //            {
            //                AddFeatures(ref withBlock3.Data.colFeature, true);
            //            }
            //        }
            //    }

            //    // パイロットデータで定義されている特殊能力
            //    if (CountPilot() > 0)
            //    {
            //        if (IsFeatureAvailable(ref "追加パイロット"))
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

            //        if (IsFeatureAvailable(ref "追加サポート"))
            //        {
            //            // 特殊能力を付加する前に必要技能が満たされているかどうか判定
            //            UpdateFeatures("追加サポート");
            //            if (IsFeatureAvailable(ref "追加サポート"))
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
            //                    DeleteCondition(ref i);
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
            //                    if (!IsNecessarySkillSatisfied(ref fd.NecessarySkill, p: ref null))
            //                    {
            //                        found = true;
            //                        goto NextFeature;
            //                    }
            //                    // 必要条件を満たしている？
            //                    if (!IsNecessarySkillSatisfied(ref fd.NecessaryCondition, p: ref null))
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
            //                    if (SRC.ALDList.IsDefined(ref stype))
            //                    {
            //                        {
            //                            var withBlock4 = SRC.ALDList.Item(ref stype);
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
            //                                            sdata2 = sdata + " " + GeneralLib.ListTail(ref sdata2, (short)(GeneralLib.LLength(ref sdata) + 1));
            //                                        }
            //                                    }

            //                                    if (withBlock4.get_AliasLevelIsPlusMod(j) | withBlock4.get_AliasLevelIsMultMod(j))
            //                                    {
            //                                        sdata2 = GeneralLib.LIndex(ref sdata2, 1) + "Lv" + SrcFormatter.Format(slevel) + " " + GeneralLib.ListTail(ref sdata2, 2);
            //                                        sdata2 = Strings.Trim(sdata2);
            //                                    }
            //                                }

            //                                // 属性使用不能攻撃により使用不能になった技能を封印する。
            //                                if (ConditionLifetime(ref stype2 + "使用不能") > 0)
            //                                {
            //                                    goto NextFeature;
            //                                }

            //                                AddCondition(ref stype2 + "付加２", -1, slevel2, ref sdata2);
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        // 属性使用不能攻撃により使用不能になった技能を封印する。
            //                        if (ConditionLifetime(ref stype + "使用不能") > 0)
            //                        {
            //                            goto NextFeature;
            //                        }

            //                        AddCondition(ref stype + "付加２", -1, slevel, ref sdata);
            //                    }

            //                    break;
            //                }

            //            case "パイロット能力強化":
            //                {
            //                    // 必要技能を満たしている？
            //                    if (!IsNecessarySkillSatisfied(ref fd.NecessarySkill, p: ref null))
            //                    {
            //                        found = true;
            //                        goto NextFeature;
            //                    }
            //                    // 必要条件を満たしている？
            //                    if (!IsNecessarySkillSatisfied(ref fd.NecessaryCondition, p: ref null))
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
            //                    if (SRC.ALDList.IsDefined(ref stype))
            //                    {
            //                        {
            //                            var withBlock5 = SRC.ALDList.Item(ref stype);
            //                            var loopTo4 = withBlock5.Count;
            //                            for (j = 1; j <= loopTo4; j++)
            //                            {
            //                                // エリアスの定義に従って特殊能力定義を置き換える
            //                                stype2 = withBlock5.get_AliasType(j);

            //                                // 属性使用不能攻撃により使用不能になった技能を封印する。
            //                                if (ConditionLifetime(ref stype2 + "使用不能") > 0)
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

            //                                    slevel2 = Constants.DEFAULT_LEVEL;
            //                                    sdata2 = withBlock5.get_AliasData(j);
            //                                    // 属性使用不能攻撃により使用不能になった技能を封印する。
            //                                    if (ConditionLifetime(ref stype2 + "使用不能") > 0)
            //                                    {
            //                                        goto NextFeature;
            //                                    }

            //                                    AddCondition(ref stype2 + "付加２", -1, slevel2, ref sdata2);
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
            //                                            sdata2 = sdata + " " + GeneralLib.ListTail(ref sdata2, (short)(GeneralLib.LLength(ref sdata) + 1));
            //                                        }
            //                                    }

            //                                    // 強化するレベルは累積する
            //                                    if (IsConditionSatisfied(ref stype2 + "強化２"))
            //                                    {
            //                                        double localConditionLevel() { object argIndex1 = stype2 + "強化２"; var ret = ConditionLevel(ref argIndex1); return ret; }

            //                                        slevel2 = slevel2 + localConditionLevel();
            //                                        DeleteCondition(ref stype2 + "強化２");
            //                                    }

            //                                    AddCondition(ref stype2 + "強化２", -1, slevel2, ref sdata2);
            //                                }
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        // 強化するレベルは累積する
            //                        if (IsConditionSatisfied(ref stype + "強化２"))
            //                        {
            //                            double localConditionLevel1() { object argIndex1 = stype + "強化２"; var ret = ConditionLevel(ref argIndex1); return ret; }

            //                            slevel = slevel + localConditionLevel1();
            //                            DeleteCondition(ref stype + "強化２");
            //                        }

            //                        AddCondition(ref stype + "強化２", -1, slevel, ref sdata);
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
            //        if (itm.Activated != itm.IsAvailable(ref this))
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
            //        if (withBlock6.IsFeatureAvailable(ref "ランクアップ"))
            //        {
            //            if (Rank >= withBlock6.FeatureLevel(ref "ランクアップ"))
            //            {
            //                if (IsNecessarySkillSatisfied(ref withBlock6.FeatureNecessarySkill(ref argIndex18), p: ref null))
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
            //        Pilot(ref 1).Update();
            //        if (!ReferenceEquals(MainPilot(), Pilot(ref 1)))
            //        {
            //            MainPilot().Update();
            //        }
            //    }

            //    // ユニット画像用ファイル名に変化がある場合はユニット画像を更新
            //    if (BitmapID != 0)
            //    {
            //        if ((ubitmap ?? "") != (get_Bitmap(false) ?? ""))
            //        {
            //            BitmapID = GUI.MakeUnitBitmap(ref this);
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
            //                        GUI.PaintUnitBitmap(ref this);
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    // ユニットの表示、非表示が切り替わった場合
            //    if (is_invisible != IsFeatureAvailable(ref "非表示"))
            //    {
            //        if (Status_Renamed == "出撃")
            //        {
            //            if (!GUI.IsPictureVisible & !string.IsNullOrEmpty(Map.MapFileName))
            //            {
            //                BitmapID = GUI.MakeUnitBitmap(ref this);
            //                if (IsFeatureAvailable(ref "非表示"))
            //                {
            //                    GUI.EraseUnitBitmap(x, y, !without_refresh);
            //                }
            //                else if (!without_refresh)
            //                {
            //                    GUI.PaintUnitBitmap(ref this);
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
            //    if (IsHero() | Expression.IsOptionDefined(ref "等身大基準"))
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

            //        if (Expression.IsOptionDefined(ref "BossRank装甲修正低下"))
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
            //    if (Expression.IsOptionDefined(ref "ＨＰ成長"))
            //    {
            //        if (CountPilot() > 0)
            //        {
            //            lngMaxHP = GeneralLib.MinLng((int)(lngMaxHP / 100d * (100 + this.MainPilot().Level)), 9999999);
            //        }
            //    }

            //    // ＥＮ成長オプション
            //    if (Expression.IsOptionDefined(ref "ＥＮ成長"))
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
            //    if (IsConditionSatisfied(ref "Ｖ－ＵＰ"))
            //    {
            //        switch (FeatureData(ref "Ｖ－ＵＰ") ?? "")
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
            //        if (itm.IsFeatureAvailable(ref "Ｖ－ＵＰ"))
            //        {
            //            switch (itm.FeatureData(ref "Ｖ－ＵＰ") ?? "")
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
            //            if (withBlock8.IsFeatureAvailable(ref "Ｖ－ＵＰ"))
            //            {
            //                switch (withBlock8.FeatureData(ref "Ｖ－ＵＰ") ?? "")
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
            //    if (IsFeatureAvailable(ref "追加移動力"))
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
            //    if (IsFeatureAvailable(ref "空中移動"))
            //    {
            //        uadaption[1] = (short)GeneralLib.MaxLng(uadaption[1], 4);
            //    }

            //    if (IsFeatureAvailable(ref "陸上移動"))
            //    {
            //        uadaption[2] = (short)GeneralLib.MaxLng(uadaption[2], 4);
            //    }

            //    if (IsFeatureAvailable(ref "水中移動"))
            //    {
            //        uadaption[3] = (short)GeneralLib.MaxLng(uadaption[3], 4);
            //    }

            //    if (IsFeatureAvailable(ref "宇宙移動"))
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
            //                        num = (short)GeneralLib.StrToLng(ref GeneralLib.LIndex(ref fd.StrData, i));
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
            //                        num = (short)GeneralLib.StrToLng(ref GeneralLib.LIndex(ref fd.StrData, i));
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
            //    if (Status_Renamed == "出撃" & Area == "空中" & !IsTransAvailable(ref "空"))
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
            //                    if (IsTransAvailable(ref "水上"))
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
            //                GUI.PaintUnitBitmap(ref this);
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
            //        if (ConditionLifetime(ref i) != 0)
            //        {
            //            ch = Condition(ref i);
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
            //            WData[i] = withBlock10.Weapon(ref i);
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
            //                WData[Information.UBound(WData)] = withBlock11.Weapon(ref i);
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
            //                    WData[Information.UBound(WData)] = withBlock12.Weapon(ref j);
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
            //                    WData[Information.UBound(WData)] = withBlock13.Weapon(ref j);
            //                }
            //            }
            //        }

            //        if (IsFeatureAvailable(ref "追加サポート"))
            //        {
            //            {
            //                var withBlock14 = AdditionalSupport().Data;
            //                var loopTo22 = withBlock14.CountWeapon();
            //                for (i = 1; i <= loopTo22; i++)
            //                {
            //                    Array.Resize(ref WData, Information.UBound(WData) + 1 + 1);
            //                    WData[Information.UBound(WData)] = withBlock14.Weapon(ref i);
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
            //                WData[Information.UBound(WData)] = itm.Weapon(ref i);
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
            //    if (IsFeatureAvailable(ref "攻撃属性"))
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
            //            if (GeneralLib.InStrNotNest(ref wclass, ref "|") > 0)
            //            {
            //                strWeaponClass[i] = Strings.Left(wclass, GeneralLib.InStrNotNest(ref wclass, ref "|") - 1);
            //                hidden_attr = Strings.Mid(wclass, GeneralLib.InStrNotNest(ref wclass, ref "|") + 1);
            //            }
            //            else
            //            {
            //                hidden_attr = "";
            //            }

            //            var loopTo26 = CountFeature();
            //            for (j = 1; j <= loopTo26; j++)
            //            {
            //                if (Feature(ref j) == "攻撃属性")
            //                {
            //                    fdata = FeatureData(ref j);

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
            //                                    if (GeneralLib.InStrNotNest(ref wclass, ref "魔") == 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔武") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔突") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔接") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔銃") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔実") > 0)
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
            //                                        l = GeneralLib.InStrNotNest(ref strWeaponClass[i], ref "武");
            //                                        l = (short)GeneralLib.MaxLng(GeneralLib.InStrNotNest(ref strWeaponClass[i], ref "突"), l);
            //                                        l = (short)GeneralLib.MaxLng(GeneralLib.InStrNotNest(ref strWeaponClass[i], ref "接"), l);
            //                                        l = (short)GeneralLib.MaxLng(GeneralLib.InStrNotNest(ref strWeaponClass[i], ref "銃"), l);
            //                                        l = (short)GeneralLib.MaxLng(GeneralLib.InStrNotNest(ref strWeaponClass[i], ref "実"), l);
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
            //    if (IsConditionSatisfied(ref "Ｖ－ＵＰ"))
            //    {
            //        switch (FeatureData(ref "Ｖ－ＵＰ") ?? "")
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
            //            if (itm.IsFeatureAvailable(ref "Ｖ－ＵＰ"))
            //            {
            //                switch (itm.FeatureData(ref "Ｖ－ＵＰ") ?? "")
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
            //            if (withBlock16.IsFeatureAvailable(ref "Ｖ－ＵＰ"))
            //            {
            //                switch (withBlock16.FeatureData(ref "Ｖ－ＵＰ") ?? "")
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
            //        if (IsFeatureAvailable(ref "武器強化"))
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
            //                if (Feature(ref j) == "武器強化")
            //                {
            //                    fdata = FeatureData(ref j);

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
            //                        if (IsWeaponClassifiedAs(i, ref "固"))
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
            //                                        if (GeneralLib.InStrNotNest(ref wclass, ref "魔") == 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔武") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔突") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔接") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔銃") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔実") > 0)
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
            //        if (IsFeatureAvailable(ref "武器割合強化"))
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
            //                if (Feature(ref j) == "武器割合強化")
            //                {
            //                    fdata = FeatureData(ref j);

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
            //                        if (IsWeaponClassifiedAs(i, ref "固"))
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
            //                                        if (GeneralLib.InStrNotNest(ref wclass, ref "魔") == 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔武") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔突") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔接") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔銃") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔実") > 0)
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
            //        if (IsWeaponClassifiedAs(i, ref "固"))
            //        {
            //            goto NextWeapon;
            //        }

            //        if (IsWeaponClassifiedAs(i, ref "Ｒ"))
            //        {
            //            // 低成長型の攻撃
            //            if (IsWeaponLevelSpecified(i, ref "Ｒ"))
            //            {
            //                // レベル設定されている場合、増加量をレベル×１０×ランクにする
            //                lngWeaponPower[i] = (int)(lngWeaponPower[i] + 10d * WeaponLevel(i, ref "Ｒ") * (Rank + num));
            //                // オ・シ・超と併用した場合
            //                if (IsWeaponClassifiedAs(i, ref "オ") | IsWeaponClassifiedAs(i, ref "超") | IsWeaponClassifiedAs(i, ref "シ"))
            //                {
            //                    lngWeaponPower[i] = (int)(lngWeaponPower[i] + 10d * (10d - WeaponLevel(i, ref "Ｒ")) * (Rank + num));

            //                    // オーラ技
            //                    if (IsWeaponClassifiedAs(i, ref "オ"))
            //                    {
            //                        lngWeaponPower[i] = (int)(lngWeaponPower[i] + 10d * WeaponLevel(i, ref "Ｒ") * AuraLevel());
            //                    }

            //                    // サイキック攻撃
            //                    if (IsWeaponClassifiedAs(i, ref "超"))
            //                    {
            //                        lngWeaponPower[i] = (int)(lngWeaponPower[i] + 10d * WeaponLevel(i, ref "Ｒ") * PsychicLevel());
            //                    }

            //                    // 同調率対象攻撃
            //                    if (IsWeaponClassifiedAs(i, ref "シ"))
            //                    {
            //                        if (CountPilot() > 0)
            //                        {
            //                            if (MainPilot().SynchroRate() > 0)
            //                            {
            //                                lngWeaponPower[i] = (int)(lngWeaponPower[i] + (long)(15d * WeaponLevel(i, ref "Ｒ") * (SyncLevel() - 50d)) / 10L);
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
            //                if (IsWeaponClassifiedAs(i, ref "オ") | IsWeaponClassifiedAs(i, ref "超") | IsWeaponClassifiedAs(i, ref "シ"))
            //                {
            //                    lngWeaponPower[i] = lngWeaponPower[i] + 50 * (Rank + num);

            //                    // オーラ技
            //                    if (IsWeaponClassifiedAs(i, ref "オ"))
            //                    {
            //                        lngWeaponPower[i] = (int)(lngWeaponPower[i] + 50d * AuraLevel());
            //                    }

            //                    // サイキック攻撃
            //                    if (IsWeaponClassifiedAs(i, ref "超"))
            //                    {
            //                        lngWeaponPower[i] = (int)(lngWeaponPower[i] + 50d * PsychicLevel());
            //                    }

            //                    // 同調率対象攻撃
            //                    if (IsWeaponClassifiedAs(i, ref "シ"))
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
            //        else if (IsWeaponClassifiedAs(i, ref "改"))
            //        {
            //            // 改属性＝オ・超・シ属性を無視したＲ属性
            //            if (IsWeaponLevelSpecified(i, ref "改"))
            //            {
            //                // レベル設定されている場合、増加量をレベル×１０×ランクにする
            //                lngWeaponPower[i] = (int)(lngWeaponPower[i] + 10d * WeaponLevel(i, ref "改") * (Rank + num));
            //            }
            //            else
            //            {
            //                // レベル指定がない場合、増加量は５０×ランク
            //                lngWeaponPower[i] = lngWeaponPower[i] + 50 * (Rank + num);
            //            }

            //            // オーラ技
            //            if (IsWeaponClassifiedAs(i, ref "オ"))
            //            {
            //                lngWeaponPower[i] = (int)(lngWeaponPower[i] + 100d * AuraLevel());
            //            }

            //            // サイキック攻撃
            //            if (IsWeaponClassifiedAs(i, ref "超"))
            //            {
            //                lngWeaponPower[i] = (int)(lngWeaponPower[i] + 100d * PsychicLevel());
            //            }

            //            // 同調率対象攻撃
            //            if (IsWeaponClassifiedAs(i, ref "シ"))
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
            //            if (IsWeaponClassifiedAs(i, ref "オ"))
            //            {
            //                lngWeaponPower[i] = (int)(lngWeaponPower[i] + 100d * AuraLevel());
            //            }

            //            // サイキック攻撃
            //            if (IsWeaponClassifiedAs(i, ref "超"))
            //            {
            //                lngWeaponPower[i] = (int)(lngWeaponPower[i] + 100d * PsychicLevel());
            //            }

            //            // 同調率対象攻撃
            //            if (IsWeaponClassifiedAs(i, ref "シ"))
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
            //        if (GeneralLib.InStrNotNest(ref strWeaponClass[i], ref "サ") > 0)
            //        {
            //            if (CountPilot() > 0)
            //            {
            //                {
            //                    var withBlock19 = MainPilot();
            //                    intWeaponMaxRange[i] = (short)(intWeaponMaxRange[i] + (long)withBlock19.SkillLevel(ref "超感覚", ref_mode: ref "") / 4L + (long)withBlock19.SkillLevel(ref "知覚強化", ref_mode: ref "") / 4L);
            //                }
            //            }
            //        }

            //        // マップ攻撃には適用されない
            //        if (GeneralLib.InStrNotNest(ref strWeaponClass[i], ref "Ｍ") > 0)
            //        {
            //            goto NextWeapon2;
            //        }

            //        // 接近戦武器には適用されない
            //        if (GeneralLib.InStrNotNest(ref strWeaponClass[i], ref "武") > 0 | GeneralLib.InStrNotNest(ref strWeaponClass[i], ref "突") > 0 | GeneralLib.InStrNotNest(ref strWeaponClass[i], ref "接") > 0)
            //        {
            //            goto NextWeapon2;
            //        }

            //        // 有線式誘導攻撃には適用されない
            //        if (GeneralLib.InStrNotNest(ref strWeaponClass[i], ref "有") > 0)
            //        {
            //            goto NextWeapon2;
            //        }

            //        // 射程延長による修正
            //        if (IsFeatureAvailable(ref "射程延長"))
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
            //                if (Feature(ref j) == "射程延長")
            //                {
            //                    fdata = FeatureData(ref j);

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
            //                                    if (GeneralLib.InStrNotNest(ref wclass, ref "魔") == 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔武") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔突") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔接") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔銃") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔実") > 0)
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
            //        if (IsFeatureAvailable(ref "命中率強化"))
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
            //                if (Feature(ref j) == "命中率強化")
            //                {
            //                    fdata = FeatureData(ref j);

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
            //                                    if (GeneralLib.InStrNotNest(ref wclass, ref "魔") == 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔武") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔突") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔接") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔銃") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔実") > 0)
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
            //        if (IsFeatureAvailable(ref "ＣＴ率強化") & IsNormalWeapon(i))
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
            //                if (Feature(ref j) == "ＣＴ率強化")
            //                {
            //                    fdata = FeatureData(ref j);

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
            //                                    if (GeneralLib.InStrNotNest(ref wclass, ref "魔") == 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔武") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔突") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔接") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔銃") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔実") > 0)
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
            //        if (IsFeatureAvailable(ref "特殊効果発動率強化") & !IsNormalWeapon(i))
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
            //                if (Feature(ref j) == "特殊効果発動率強化")
            //                {
            //                    fdata = FeatureData(ref j);

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
            //                                    if (GeneralLib.InStrNotNest(ref wclass, ref "魔") == 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔武") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔突") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔接") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔銃") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔実") > 0)
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
            //        if (IsFeatureAvailable(ref "最大弾数増加"))
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
            //                if (Feature(ref j) == "最大弾数増加")
            //                {
            //                    fdata = FeatureData(ref j);

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
            //                                    if (GeneralLib.InStrNotNest(ref wclass, ref "魔") == 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔武") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔突") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔接") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔銃") > 0 | GeneralLib.InStrNotNest(ref wclass, ref "魔実") > 0)
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

            // アビリティデータを更新
            AData = Data.Abilities.Select(x => new UnitAbility(SRC, this, x)).ToList();
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
            //            adata[i] = withBlock25.Ability(ref i);
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
            //                adata[Information.UBound(adata)] = withBlock26.Ability(ref i);
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
            //                    adata[Information.UBound(adata)] = withBlock27.Ability(ref j);
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
            //                    adata[Information.UBound(adata)] = withBlock28.Ability(ref j);
            //                }
            //            }
            //        }

            //        if (IsFeatureAvailable(ref "追加サポート"))
            //        {
            //            {
            //                var withBlock29 = AdditionalSupport().Data;
            //                var loopTo64 = withBlock29.CountAbility();
            //                for (i = 1; i <= loopTo64; i++)
            //                {
            //                    Array.Resize(ref adata, Information.UBound(adata) + 1 + 1);
            //                    adata[Information.UBound(adata)] = withBlock29.Ability(ref i);
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
            //                adata[Information.UBound(adata)] = itm.Ability(ref i);
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
            //    if (IsFeatureAvailable(ref "制御不可"))
            //    {
            //        if (!is_uncontrollable)
            //        {
            //            AddCondition(ref "暴走", -1, cdata: ref "");
            //        }
            //    }
            //    else if (is_uncontrollable)
            //    {
            //        if (IsConditionSatisfied(ref "暴走"))
            //        {
            //            DeleteCondition(ref "暴走");
            //        }
            //    }

            //    // 不安定？
            //    if (IsFeatureAvailable(ref "不安定"))
            //    {
            //        if (!is_stable)
            //        {
            //            if (HP <= MaxHP / 4)
            //            {
            //                AddCondition(ref "暴走", -1, cdata: ref "");
            //            }
            //        }
            //    }
            //    else if (is_stable)
            //    {
            //        if (IsConditionSatisfied(ref "暴走"))
            //        {
            //            DeleteCondition(ref "暴走");
            //        }
            //    }
        }
    }
}
