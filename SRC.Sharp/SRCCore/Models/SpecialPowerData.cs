using System;
using System.Drawing;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    internal class SpecialPowerData
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // スペシャルパワーデータのクラス

        // スペシャルパワー名
        public string Name;
        // スペシャルパワー名の読み仮名
        public string KanaName;
        // スペシャルパワー名の短縮形
        public string ShortName;
        // 消費ＳＰ
        public short SPConsumption;
        // 対象
        public string TargetType;
        // 効果時間
        public string Duration;
        // 適用条件
        public string NecessaryCondition;
        // アニメ
        public string Animation;

        // 効果名
        private string[] strEffectType;
        // 効果レベル
        private double[] dblEffectLevel;
        // 効果データ
        private string[] strEffectData;

        // 解説
        public string Comment;


        // クラスの初期化
        // UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        private void Class_Initialize_Renamed()
        {
            strEffectType = new string[1];
            dblEffectLevel = new double[1];
            strEffectData = new string[1];
        }

        public SpecialPowerData() : base()
        {
            Class_Initialize_Renamed();
        }


        // スペシャルパワーに効果を追加
        public void SetEffect(ref string elist)
        {
            short j, i, k;
            string buf;
            string elevel, etype, edata;
            strEffectType = new string[(GeneralLib.ListLength(ref elist) + 1)];
            dblEffectLevel = new double[(GeneralLib.ListLength(ref elist) + 1)];
            strEffectData = new string[(GeneralLib.ListLength(ref elist) + 1)];
            GeneralLib.TrimString(ref elist);
            var loopTo = GeneralLib.ListLength(ref elist);
            for (i = 1; i <= loopTo; i++)
            {
                buf = GeneralLib.ListIndex(ref elist, i);
                j = (short)Strings.InStr(buf, "Lv");
                k = (short)Strings.InStr(buf, "=");
                if (j > 0 & (k == 0 | j < k))
                {
                    // レベル指定のある効果(データ指定を伴うものを含む)
                    strEffectType[i] = Strings.Left(buf, j - 1);
                    if (k > 0)
                    {
                        // データ指定を伴うもの
                        dblEffectLevel[i] = Conversions.ToDouble(Strings.Mid(buf, j + 2, k - (j + 2)));
                        buf = Strings.Mid(buf, k + 1);
                        if (Strings.Left(buf, 1) == "\"")
                        {
                            buf = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
                        }

                        j = (short)Strings.InStr(buf, "Lv");
                        k = (short)Strings.InStr(buf, "=");
                        if (j > 0 & (k == 0 | j < k))
                        {
                            // データ指定中にレベル指定があるもの
                            etype = Strings.Left(buf, j - 1);
                            if (k > 0)
                            {
                                elevel = Strings.Mid(buf, j + 2, k - (j + 2));
                                edata = Strings.Mid(buf, k + 1);
                            }
                            else
                            {
                                elevel = Strings.Mid(buf, j + 2);
                                edata = "";
                            }
                        }
                        else if (k > 0)
                        {
                            // データ指定中にデータ指定があるもの
                            etype = Strings.Left(buf, k - 1);
                            elevel = "";
                            edata = Strings.Mid(buf, k + 1);
                        }
                        else
                        {
                            // データ指定のみ
                            etype = buf;
                            elevel = "";
                            edata = "";
                        }

                        if (Name == "付加" & string.IsNullOrEmpty(elevel))
                        {
                            elevel = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(SRC.DEFAULT_LEVEL);
                        }

                        strEffectData[i] = Strings.Trim(etype + " " + elevel + " " + edata);
                    }
                    else
                    {
                        // データ指定を伴わないもの
                        dblEffectLevel[i] = Conversions.ToDouble(Strings.Mid(buf, j + 2));
                    }
                }
                else if (k > 0)
                {
                    // データ指定を伴う効果
                    strEffectType[i] = Strings.Left(buf, k - 1);
                    buf = Strings.Mid(buf, k + 1);
                    if (Strings.Asc(buf) == 34) // "
                    {
                        buf = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
                    }

                    j = (short)Strings.InStr(buf, "Lv");
                    k = (short)Strings.InStr(buf, "=");
                    if (j > 0)
                    {
                        // データ指定中にレベル指定があるもの
                        etype = Strings.Left(buf, j - 1);
                        if (k > 0)
                        {
                            elevel = Strings.Mid(buf, j + 2, k - (j + 2));
                            edata = Strings.Mid(buf, k + 1);
                        }
                        else
                        {
                            elevel = Strings.Mid(buf, j + 2);
                            edata = "";
                        }
                    }
                    else if (k > 0)
                    {
                        // データ指定中にデータ指定があるもの
                        etype = Strings.Left(buf, k - 1);
                        elevel = "";
                        edata = Strings.Mid(buf, k + 1);
                    }
                    else
                    {
                        // データ指定のみ
                        etype = buf;
                        elevel = "";
                        edata = "";
                    }

                    if (Name == "付加" & string.IsNullOrEmpty(elevel))
                    {
                        elevel = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(SRC.DEFAULT_LEVEL);
                    }

                    strEffectData[i] = Strings.Trim(etype + " " + elevel + " " + edata);
                }
                else
                {
                    // 効果名のみ
                    strEffectType[i] = buf;
                }
            }
        }


        // 特殊効果の個数
        public short CountEffect()
        {
            short CountEffectRet = default;
            CountEffectRet = (short)Information.UBound(strEffectType);
            return CountEffectRet;
        }

        // idx番目の特殊効果タイプ
        public string EffectType(short idx)
        {
            string EffectTypeRet = default;
            EffectTypeRet = strEffectType[idx];
            return EffectTypeRet;
        }

        // idx番目の特殊効果レベル
        public double EffectLevel(short idx)
        {
            double EffectLevelRet = default;
            EffectLevelRet = dblEffectLevel[idx];
            return EffectLevelRet;
        }

        // idx番目の特殊効果データ
        public string EffectData(short idx)
        {
            string EffectDataRet = default;
            EffectDataRet = strEffectData[idx];
            return EffectDataRet;
        }

        // 特殊効果 ename を持っているか
        public object IsEffectAvailable(ref string ename)
        {
            object IsEffectAvailableRet = default;
            short i;
            var loopTo = CountEffect();
            for (i = 1; i <= loopTo; i++)
            {
                if ((ename ?? "") == (EffectType(i) ?? ""))
                {
                    // UPGRADE_WARNING: オブジェクト IsEffectAvailable の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    IsEffectAvailableRet = true;
                    return IsEffectAvailableRet;
                }

                if (EffectType(i) == "スペシャルパワー")
                {
                    // UPGRADE_WARNING: オブジェクト SPDList.Item(EffectData(i)).IsEffectAvailable(ename) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    SpecialPowerData localItem() { object argIndex1 = EffectData(i); var ret = SRC.SPDList.Item(ref argIndex1); return ret; }

                    if (Conversions.ToBoolean(localItem().IsEffectAvailable(ref ename)))
                    {
                        // UPGRADE_WARNING: オブジェクト IsEffectAvailable の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        IsEffectAvailableRet = true;
                        return IsEffectAvailableRet;
                    }
                }
            }

            return IsEffectAvailableRet;
        }


        // スペシャルパワーがその時点で役に立つかどうか
        // (パイロット p が使用した場合)
        public bool Useful(ref Pilot p)
        {
            bool UsefulRet = default;
            Unit u;
            short i;
            switch (TargetType ?? "")
            {
                case "自分":
                    {
                        UsefulRet = Effective(ref p, ref p.Unit_Renamed);
                        return UsefulRet;
                    }

                case "味方":
                case "全味方":
                    {
                        foreach (Unit currentU in SRC.UList)
                        {
                            u = currentU;
                            {
                                var withBlock = u;
                                // 出撃している？
                                if (withBlock.Status_Renamed != "出撃")
                                {
                                    goto NextUnit1;
                                }

                                // 味方ユニット？
                                switch (p.Party ?? "")
                                {
                                    case "味方":
                                    case "ＮＰＣ":
                                        {
                                            if (withBlock.Party != "味方" & withBlock.Party0 != "味方" & withBlock.Party != "ＮＰＣ" & withBlock.Party0 != "ＮＰＣ")
                                            {
                                                goto NextUnit1;
                                            }

                                            break;
                                        }

                                    default:
                                        {
                                            if ((p.Party ?? "") != (withBlock.Party ?? ""))
                                            {
                                                goto NextUnit1;
                                            }

                                            break;
                                        }
                                }

                                // 効果がある？
                                if (Effective(ref p, ref u))
                                {
                                    UsefulRet = true;
                                    return UsefulRet;
                                }
                            }

                            NextUnit1:
                            ;
                        }

                        break;
                    }

                case "破壊味方":
                    {
                        foreach (Unit currentU1 in SRC.UList)
                        {
                            u = currentU1;
                            {
                                var withBlock1 = u;
                                // 破壊されている？
                                if (withBlock1.Status_Renamed != "破壊")
                                {
                                    goto NextUnit2;
                                }

                                // 味方ユニット？
                                if ((p.Party ?? "") != (withBlock1.Party0 ?? ""))
                                {
                                    goto NextUnit2;
                                }

                                // 効果がある？
                                if (Effective(ref p, ref u))
                                {
                                    UsefulRet = true;
                                    return UsefulRet;
                                }
                            }

                            NextUnit2:
                            ;
                        }

                        break;
                    }

                case "敵":
                case "全敵":
                    {
                        foreach (Unit currentU2 in SRC.UList)
                        {
                            u = currentU2;
                            {
                                var withBlock2 = u;
                                // 出撃している？
                                if (withBlock2.Status_Renamed != "出撃")
                                {
                                    goto NextUnit3;
                                }

                                // 敵ユニット？
                                switch (p.Party ?? "")
                                {
                                    case "味方":
                                    case "ＮＰＣ":
                                        {
                                            if (withBlock2.Party == "味方" & withBlock2.Party0 == "味方" | withBlock2.Party == "ＮＰＣ" & withBlock2.Party0 == "ＮＰＣ")
                                            {
                                                goto NextUnit3;
                                            }

                                            break;
                                        }

                                    default:
                                        {
                                            if ((p.Party ?? "") == (withBlock2.Party ?? ""))
                                            {
                                                goto NextUnit3;
                                            }

                                            break;
                                        }
                                }

                                // 効果がある？
                                if (Effective(ref p, ref u))
                                {
                                    UsefulRet = true;
                                    return UsefulRet;
                                }
                            }

                            NextUnit3:
                            ;
                        }

                        break;
                    }

                case "任意":
                case "全":
                    {
                        foreach (Unit currentU3 in SRC.UList)
                        {
                            u = currentU3;
                            // 出撃している？
                            if (u.Status_Renamed == "出撃")
                            {
                                // 効果がある？
                                if (Effective(ref p, ref u))
                                {
                                    UsefulRet = true;
                                    return UsefulRet;
                                }
                            }
                        }

                        break;
                    }
            }

            return UsefulRet;
        }

        // スペシャルパワーがユニット t に対して効果があるかどうか
        // (パイロット p が使用した場合)
        public bool Effective(ref Pilot p, ref Unit t)
        {
            bool EffectiveRet = default;
            short i, j;
            string ncond;
            Unit my_unit;

            // 同じ追加パイロットを持つユニットが複数いる場合、パイロットのUnitが
            // 変化してしまうことがあるため、元のUnitを記録しておく
            my_unit = p.Unit_Renamed;
            // 適用条件を満たしている？
            var loopTo = GeneralLib.LLength(ref NecessaryCondition);
            for (i = 1; i <= loopTo; i++)
            {
                ncond = GeneralLib.LIndex(ref NecessaryCondition, i);
                switch (ncond ?? "")
                {
                    case "技量":
                        {
                            if (p.Technique < t.MainPilot().Technique)
                            {
                                goto ExitFunc;
                            }

                            break;
                        }

                    case "非ボス":
                        {
                            if (t.BossRank >= 0)
                            {
                                goto ExitFunc;
                            }

                            break;
                        }

                    case "支援":
                        {
                            if (ReferenceEquals(my_unit, t))
                            {
                                goto ExitFunc;
                            }

                            break;
                        }

                    case "隣接":
                        {
                            if (Math.Abs((short)(my_unit.x - t.x)) + Math.Abs((short)(my_unit.y - t.y)) != 1)
                            {
                                goto ExitFunc;
                            }

                            break;
                        }

                    default:
                        {
                            if (Strings.InStr(ncond, "射程Lv") == 1)
                            {
                                int localStrToLng() { string argexpr = Strings.Mid(ncond, 5); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

                                if (Math.Abs((short)(my_unit.x - t.x)) + Math.Abs((short)(my_unit.y - t.y)) > localStrToLng())
                                {
                                    goto ExitFunc;
                                }
                            }

                            break;
                        }
                }

                // Unitが変化してしまった場合は元に戻しておく
                if (!ReferenceEquals(my_unit, p.Unit_Renamed))
                {
                    my_unit.MainPilot();
                }
            }

            // 無効化されている？
            switch (TargetType ?? "")
            {
                case "敵":
                case "全敵":
                case "任意":
                case "全":
                    {
                        object argIndex1 = "スペシャルパワー無効化";
                        object argIndex2 = "精神コマンド無効化";
                        if (t.IsConditionSatisfied(ref argIndex1) | t.IsConditionSatisfied(ref argIndex2))
                        {
                            goto ExitFunc;
                        }

                        break;
                    }
            }

            // 持続効果があるものは同じスペシャルパワーが既に適用されていなければ
            // 効果があるとみなす
            if (Duration != "即効")
            {
                if (!t.IsSpecialPowerInEffect(ref Name))
                {
                    EffectiveRet = true;
                }

                // ただしみがわりは自分自身には使えないのでチェックしておく
                if (EffectType(1) == "みがわり")
                {
                    if (ReferenceEquals(my_unit, t))
                    {
                        EffectiveRet = false;
                        goto ExitFunc;
                    }
                }

                goto ExitFunc;
            }

            // 個々の効果に関して有効かどうか判定
            var loopTo1 = CountEffect();
            for (i = 1; i <= loopTo1; i++)
            {
                switch (EffectType(i) ?? "")
                {
                    case "ＨＰ回復":
                    case "ＨＰ増加":
                        {
                            if (EffectLevel(i) < 0d)
                            {
                                EffectiveRet = true;
                                goto ExitFunc;
                            }

                            object argIndex3 = "ゾンビ";
                            if (t.IsConditionSatisfied(ref argIndex3))
                            {
                                goto NextEffect;
                            }

                            if (t.HP < t.MaxHP)
                            {
                                EffectiveRet = true;
                                goto ExitFunc;
                            }

                            break;
                        }

                    case "ＥＮ回復":
                    case "ＥＮ増加":
                        {
                            if (EffectLevel(i) < 0d)
                            {
                                EffectiveRet = true;
                                goto ExitFunc;
                            }

                            object argIndex4 = "ゾンビ";
                            if (t.IsConditionSatisfied(ref argIndex4))
                            {
                                goto NextEffect;
                            }

                            if (t.EN < t.MaxEN)
                            {
                                EffectiveRet = true;
                                goto ExitFunc;
                            }

                            break;
                        }

                    case "霊力回復":
                    case "霊力増加":
                        {
                            if (EffectLevel(i) < 0d)
                            {
                                EffectiveRet = true;
                                goto ExitFunc;
                            }

                            object argIndex5 = "ゾンビ";
                            if (t.IsConditionSatisfied(ref argIndex5))
                            {
                                goto NextEffect;
                            }

                            if (t.MainPilot().Plana < t.MainPilot().MaxPlana())
                            {
                                EffectiveRet = true;
                                goto ExitFunc;
                            }

                            break;
                        }

                    case "ＳＰ回復":
                        {
                            if (EffectLevel(i) < 0d)
                            {
                                EffectiveRet = true;
                                goto ExitFunc;
                            }

                            object argIndex6 = "ゾンビ";
                            if (t.IsConditionSatisfied(ref argIndex6))
                            {
                                goto NextEffect;
                            }

                            if (t.MainPilot().SP < t.MainPilot().MaxSP)
                            {
                                EffectiveRet = true;
                                goto ExitFunc;
                            }

                            var loopTo2 = t.CountPilot();
                            for (j = 2; j <= loopTo2; j++)
                            {
                                Pilot localPilot() { object argIndex1 = j; var ret = t.Pilot(ref argIndex1); return ret; }

                                Pilot localPilot1() { object argIndex1 = j; var ret = t.Pilot(ref argIndex1); return ret; }

                                if (localPilot().SP < localPilot1().MaxSP)
                                {
                                    EffectiveRet = true;
                                    goto ExitFunc;
                                }
                            }

                            var loopTo3 = t.CountSupport();
                            for (j = 1; j <= loopTo3; j++)
                            {
                                Pilot localSupport() { object argIndex1 = j; var ret = t.Support(ref argIndex1); return ret; }

                                Pilot localSupport1() { object argIndex1 = j; var ret = t.Support(ref argIndex1); return ret; }

                                if (localSupport().SP < localSupport1().MaxSP)
                                {
                                    EffectiveRet = true;
                                    goto ExitFunc;
                                }
                            }

                            string argfname = "追加サポート";
                            if (t.IsFeatureAvailable(ref argfname))
                            {
                                if (t.AdditionalSupport().SP < t.AdditionalSupport().MaxSP)
                                {
                                    EffectiveRet = true;
                                    goto ExitFunc;
                                }
                            }

                            break;
                        }

                    case "状態回復":
                        {
                            object argIndex8 = "攻撃不能";
                            object argIndex9 = "移動不能";
                            object argIndex10 = "装甲劣化";
                            object argIndex11 = "混乱";
                            object argIndex12 = "魅了";
                            object argIndex13 = "憑依";
                            object argIndex14 = "石化";
                            object argIndex15 = "凍結";
                            object argIndex16 = "麻痺";
                            object argIndex17 = "睡眠";
                            object argIndex18 = "毒";
                            object argIndex19 = "盲目";
                            object argIndex20 = "撹乱";
                            object argIndex21 = "恐怖";
                            object argIndex22 = "沈黙";
                            object argIndex23 = "ゾンビ";
                            object argIndex24 = "回復不能";
                            object argIndex25 = "オーラ使用不能";
                            object argIndex26 = "超能力使用不能";
                            object argIndex27 = "同調率使用不能";
                            object argIndex28 = "超感覚使用不能";
                            object argIndex29 = "知覚強化使用不能";
                            object argIndex30 = "霊力使用不能";
                            object argIndex31 = "術使用不能";
                            object argIndex32 = "技使用不能";
                            if (t.ConditionLifetime(ref argIndex8) > 0 | t.ConditionLifetime(ref argIndex9) > 0 | t.ConditionLifetime(ref argIndex10) > 0 | t.ConditionLifetime(ref argIndex11) > 0 | t.ConditionLifetime(ref argIndex12) > 0 | t.ConditionLifetime(ref argIndex13) > 0 | t.ConditionLifetime(ref argIndex14) > 0 | t.ConditionLifetime(ref argIndex15) > 0 | t.ConditionLifetime(ref argIndex16) > 0 | t.ConditionLifetime(ref argIndex17) > 0 | t.ConditionLifetime(ref argIndex18) > 0 | t.ConditionLifetime(ref argIndex19) > 0 | t.ConditionLifetime(ref argIndex20) > 0 | t.ConditionLifetime(ref argIndex21) > 0 | t.ConditionLifetime(ref argIndex22) > 0 | t.ConditionLifetime(ref argIndex23) > 0 | t.ConditionLifetime(ref argIndex24) > 0 | t.ConditionLifetime(ref argIndex25) > 0 | t.ConditionLifetime(ref argIndex26) > 0 | t.ConditionLifetime(ref argIndex27) > 0 | t.ConditionLifetime(ref argIndex28) > 0 | t.ConditionLifetime(ref argIndex29) > 0 | t.ConditionLifetime(ref argIndex30) > 0 | t.ConditionLifetime(ref argIndex31) > 0 | t.ConditionLifetime(ref argIndex32) > 0)
                            {
                                EffectiveRet = true;
                                goto ExitFunc;
                            }
                            else
                            {
                                var loopTo4 = t.CountCondition();
                                for (j = 1; j <= loopTo4; j++)
                                {
                                    string localCondition2() { object argIndex1 = j; var ret = t.Condition(ref argIndex1); return ret; }

                                    if (Strings.Len(localCondition2()) > 6)
                                    {
                                        string localCondition1() { object argIndex1 = j; var ret = t.Condition(ref argIndex1); return ret; }

                                        if (Strings.Right(localCondition1(), 6) == "属性使用不能")
                                        {
                                            string localCondition() { object argIndex1 = j; var ret = t.Condition(ref argIndex1); return ret; }

                                            object argIndex7 = localCondition();
                                            if (t.ConditionLifetime(ref argIndex7) > 0)
                                            {
                                                EffectiveRet = true;
                                                goto ExitFunc;
                                            }
                                        }
                                    }
                                }
                            }

                            break;
                        }

                    case "装填":
                        {
                            var loopTo5 = t.CountWeapon();
                            for (j = 1; j <= loopTo5; j++)
                            {
                                if (t.Bullet(j) < t.MaxBullet(j))
                                {
                                    EffectiveRet = true;
                                    goto ExitFunc;
                                }
                            }

                            break;
                        }

                    case "行動数回復":
                        {
                            if (t.Action == 0 & t.MaxAction() > 0)
                            {
                                EffectiveRet = true;
                                goto ExitFunc;
                            }

                            break;
                        }

                    case "行動数増加":
                        {
                            if (EffectLevel(i) < 0d)
                            {
                                EffectiveRet = true;
                                goto ExitFunc;
                            }

                            if (t.Action < 3 & t.MaxAction() > 0)
                            {
                                EffectiveRet = true;
                                goto ExitFunc;
                            }

                            break;
                        }

                    case "スペシャルパワー":
                    case "精神コマンド":
                        {
                            bool localIsSpecialPowerInEffect() { string argsname = EffectData(i); var ret = t.IsSpecialPowerInEffect(ref argsname); return ret; }

                            if (!localIsSpecialPowerInEffect())
                            {
                                EffectiveRet = true;
                                goto ExitFunc;
                            }

                            break;
                        }

                    case "気力増加":
                        {
                            if (t.MainPilot().Personality != "機械" & t.MainPilot().Morale < t.MainPilot().MaxMorale)
                            {
                                EffectiveRet = true;
                                goto ExitFunc;
                            }

                            var loopTo6 = t.CountPilot();
                            for (j = 2; j <= loopTo6; j++)
                            {
                                Pilot localPilot2() { object argIndex1 = j; var ret = t.Pilot(ref argIndex1); return ret; }

                                Pilot localPilot3() { object argIndex1 = j; var ret = t.Pilot(ref argIndex1); return ret; }

                                if (localPilot2().Personality != "機械" & localPilot3().Morale < t.MainPilot().MaxMorale)
                                {
                                    EffectiveRet = true;
                                    goto ExitFunc;
                                }
                            }

                            break;
                        }

                    case "気力低下":
                        {
                            if (t.MainPilot().Personality == "機械")
                            {
                                goto NextEffect;
                            }

                            if (t.MainPilot().Morale > t.MainPilot().MinMorale)
                            {
                                EffectiveRet = true;
                                goto ExitFunc;
                            }

                            break;
                        }

                    case "ランダムダメージ":
                    case "ＨＰ減少":
                    case "ＥＮ減少":
                        {
                            object argIndex33 = "無敵";
                            if (!t.IsConditionSatisfied(ref argIndex33))
                            {
                                EffectiveRet = true;
                                goto ExitFunc;
                            }

                            break;
                        }

                    case var @case when @case == "気力増加":
                    case "自爆":
                    case "復活":
                    case "偵察":
                    case "味方スペシャルパワー実行":
                    case "イベント":
                        {
                            EffectiveRet = true;
                            goto ExitFunc;
                            break;
                        }
                }

                NextEffect:
                ;
            }

            ExitFunc:
            ;


            // Unitが変化してしまった場合は元に戻しておく
            if (!ReferenceEquals(my_unit, p.Unit_Renamed))
            {
                my_unit.MainPilot();
            }

            return EffectiveRet;
        }


        // スペシャルパワーを使用する
        // (パイロット p が使用した場合)
        public void Execute(ref Pilot p, bool is_event = false)
        {
            Unit u;
            short i, j;
            switch (TargetType ?? "")
            {
                case "自分":
                    {
                        if (Apply(ref p, p.Unit_Renamed, is_event) & !is_event)
                        {
                            GUI.Sleep(300);
                        }

                        break;
                    }

                case "全味方":
                    {
                        var loopTo = Map.MapWidth;
                        for (i = 1; i <= loopTo; i++)
                        {
                            var loopTo1 = Map.MapHeight;
                            for (j = 1; j <= loopTo1; j++)
                            {
                                u = Map.MapDataForUnit[i, j];
                                if (u is null)
                                {
                                    goto NextUnit1;
                                }

                                {
                                    var withBlock = u;
                                    // 味方ユニット？
                                    switch (p.Party ?? "")
                                    {
                                        case "味方":
                                        case "ＮＰＣ":
                                            {
                                                if (withBlock.Party != "味方" & withBlock.Party0 != "味方" & withBlock.Party != "ＮＰＣ" & withBlock.Party0 != "ＮＰＣ")
                                                {
                                                    goto NextUnit1;
                                                }

                                                break;
                                            }

                                        default:
                                            {
                                                if ((p.Party ?? "") != (withBlock.Party ?? ""))
                                                {
                                                    goto NextUnit1;
                                                }

                                                break;
                                            }
                                    }

                                    Apply(ref p, u, is_event);
                                }

                                NextUnit1:
                                ;
                            }
                        }

                        if (!is_event)
                        {
                            GUI.Sleep(300);
                        }

                        break;
                    }

                case "全敵":
                    {
                        var loopTo2 = Map.MapWidth;
                        for (i = 1; i <= loopTo2; i++)
                        {
                            var loopTo3 = Map.MapHeight;
                            for (j = 1; j <= loopTo3; j++)
                            {
                                u = Map.MapDataForUnit[i, j];
                                if (u is null)
                                {
                                    goto NextUnit2;
                                }

                                {
                                    var withBlock1 = u;
                                    // 敵ユニット？
                                    switch (p.Party ?? "")
                                    {
                                        case "味方":
                                        case "ＮＰＣ":
                                            {
                                                if (withBlock1.Party == "味方" | withBlock1.Party == "ＮＰＣ")
                                                {
                                                    goto NextUnit2;
                                                }

                                                break;
                                            }

                                        default:
                                            {
                                                if ((p.Party ?? "") == (withBlock1.Party ?? ""))
                                                {
                                                    goto NextUnit2;
                                                }

                                                break;
                                            }
                                    }

                                    Apply(ref p, u, is_event);
                                }

                                NextUnit2:
                                ;
                            }
                        }

                        if (!is_event)
                        {
                            GUI.Sleep(300);
                        }

                        break;
                    }

                case "全":
                    {
                        var loopTo4 = Map.MapWidth;
                        for (i = 1; i <= loopTo4; i++)
                        {
                            var loopTo5 = Map.MapHeight;
                            for (j = 1; j <= loopTo5; j++)
                            {
                                u = Map.MapDataForUnit[i, j];
                                if (u is object)
                                {
                                    Apply(ref p, u, is_event);
                                }
                            }
                        }

                        if (!is_event)
                        {
                            GUI.Sleep(300);
                        }

                        break;
                    }

                default:
                    {
                        if (Apply(ref p, Commands.SelectedTarget, is_event) & !is_event)
                        {
                            GUI.Sleep(300);
                        }

                        break;
                    }
            }

            if (!is_event)
            {
                GUI.CloseMessageForm();
                GUI.RedrawScreen();
            }
        }

        // スペシャルパワーをユニット t に対して適用
        // (パイロット p が使用)
        // 実行後にウェイトが必要かどうかを返す
        public bool Apply(ref Pilot p, Unit t, bool is_event = false, bool as_instant = false)
        {
            bool ApplyRet = default;
            short j, i, n;
            int tmp;
            bool need_update = default, is_invalid = default, displayed_string = default;
            string msg = default, ncond;
            Unit my_unit;

            // 同じ追加パイロットを持つユニットが複数いる場合、パイロットのUnitが
            // 変化してしまうことがあるため、元のUnitを記録しておく
            my_unit = p.Unit_Renamed;
            {
                var withBlock = t;
                // 適用条件を満たしている？
                var loopTo = GeneralLib.LLength(ref NecessaryCondition);
                for (i = 1; i <= loopTo; i++)
                {
                    ncond = GeneralLib.LIndex(ref NecessaryCondition, i);
                    switch (ncond ?? "")
                    {
                        case "技量":
                            {
                                if (p.Technique < withBlock.MainPilot().Technique)
                                {
                                    is_invalid = true;
                                }

                                break;
                            }

                        case "非ボス":
                            {
                                if (withBlock.BossRank >= 0)
                                {
                                    is_invalid = true;
                                }

                                break;
                            }

                        case "支援":
                            {
                                if (ReferenceEquals(my_unit, t))
                                {
                                    is_invalid = true;
                                }

                                break;
                            }

                        case "隣接":
                            {
                                if (Math.Abs((short)(my_unit.x - t.x)) + Math.Abs((short)(my_unit.y - t.y)) != 1)
                                {
                                    is_invalid = true;
                                }

                                break;
                            }

                        default:
                            {
                                if (Strings.InStr(ncond, "射程Lv") == 1)
                                {
                                    int localStrToLng() { string argexpr = Strings.Mid(ncond, 5); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

                                    if (Math.Abs((short)(my_unit.x - t.x)) + Math.Abs((short)(my_unit.y - t.y)) > localStrToLng())
                                    {
                                        is_invalid = true;
                                    }
                                }

                                break;
                            }
                    }

                    // Unitが変化してしまった場合は元に戻しておく
                    if (!ReferenceEquals(my_unit, p.Unit_Renamed))
                    {
                        my_unit.CurrentForm().MainPilot();
                    }
                }

                // 無効化されている？
                switch (TargetType ?? "")
                {
                    case "敵":
                    case "全敵":
                        {
                            object argIndex1 = "スペシャルパワー無効";
                            if (withBlock.IsConditionSatisfied(ref argIndex1))
                            {
                                is_invalid = true;
                            }

                            break;
                        }
                }

                // スペシャルパワーが適用可能？
                if (is_invalid)
                {
                    return ApplyRet;
                }

                // 持続効果がある場合は単にスペシャルパワーの効果を付加するだけでよい
                if (Duration != "即効" & !as_instant)
                {
                    withBlock.MakeSpecialPowerInEffect(ref Name, ref my_unit.MainPilot().ID);
                    return ApplyRet;
                }
            }

            // これ以降は持続効果が即効であるスペシャルパワーの処理

            // 個々の効果を適用
            var loopTo1 = CountEffect();
            for (i = 1; i <= loopTo1; i++)
            {
                {
                    var withBlock1 = t;
                    switch (EffectType(i) ?? "")
                    {
                        case "ＨＰ回復":
                        case "ＨＰ増加":
                            {
                                // 効果が適用可能かどうか判定
                                if (EffectLevel(i) > 0d)
                                {
                                    object argIndex2 = "ゾンビ";
                                    if (withBlock1.IsConditionSatisfied(ref argIndex2))
                                    {
                                        goto NextEffect;
                                    }

                                    if (withBlock1.HP == withBlock1.MaxHP)
                                    {
                                        goto NextEffect;
                                    }
                                }

                                if (!is_event)
                                {
                                    if (ReferenceEquals(t, Commands.SelectedUnit))
                                    {
                                        if (!My.MyProject.Forms.frmMessage.Visible)
                                        {
                                            Unit argu2 = null;
                                            GUI.OpenMessageForm(ref Commands.SelectedUnit, u2: ref argu2);
                                        }
                                        else
                                        {
                                            object argu21 = null;
                                            GUI.UpdateMessageForm(ref Commands.SelectedUnit, u2: ref argu21);
                                        }
                                    }
                                    else if (!My.MyProject.Forms.frmMessage.Visible)
                                    {
                                        GUI.OpenMessageForm(ref t, ref Commands.SelectedUnit);
                                    }
                                    else
                                    {
                                        object argu22 = Commands.SelectedUnit;
                                        GUI.UpdateMessageForm(ref t, ref argu22);
                                    }

                                    GUI.Sleep(150);
                                }

                                // ＨＰを回復させる
                                tmp = withBlock1.HP;
                                if (EffectType(i) == "ＨＰ増加")
                                {
                                    withBlock1.HP = (int)(withBlock1.HP + 1000d * EffectLevel(i));
                                }
                                else
                                {
                                    withBlock1.RecoverHP(10d * EffectLevel(i));
                                }

                                if (!is_event)
                                {
                                    if (!displayed_string)
                                    {
                                        if (EffectLevel(i) >= 0d)
                                        {
                                            string argmsg = "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.HP - tmp);
                                            GUI.DrawSysString(withBlock1.x, withBlock1.y, ref argmsg);
                                        }
                                        else
                                        {
                                            string argmsg1 = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.HP - tmp);
                                            GUI.DrawSysString(withBlock1.x, withBlock1.y, ref argmsg1);
                                        }
                                    }

                                    displayed_string = true;
                                    if (ReferenceEquals(t, Commands.SelectedUnit))
                                    {
                                        object argu23 = null;
                                        GUI.UpdateMessageForm(ref Commands.SelectedUnit, u2: ref argu23);
                                    }
                                    else
                                    {
                                        object argu24 = Commands.SelectedUnit;
                                        GUI.UpdateMessageForm(ref t, ref argu24);
                                    }

                                    if (EffectLevel(i) >= 0d)
                                    {
                                        string argtname = "ＨＰ";
                                        GUI.DisplaySysMessage(withBlock1.Nickname + "の" + Expression.Term(ref argtname, ref t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.HP - tmp) + "回復した。");
                                    }
                                    else
                                    {
                                        string argtname1 = "ＨＰ";
                                        GUI.DisplaySysMessage(withBlock1.Nickname + "の" + Expression.Term(ref argtname1, ref t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(tmp - withBlock1.HP) + "減少した。");
                                    }
                                }

                                need_update = true;
                                break;
                            }

                        case "ＥＮ回復":
                        case "ＥＮ増加":
                            {
                                // 効果が適用可能かどうか判定
                                if (EffectLevel(i) > 0d)
                                {
                                    object argIndex3 = "ゾンビ";
                                    if (withBlock1.IsConditionSatisfied(ref argIndex3))
                                    {
                                        goto NextEffect;
                                    }

                                    if (withBlock1.EN == withBlock1.MaxEN)
                                    {
                                        goto NextEffect;
                                    }
                                }

                                if (!is_event)
                                {
                                    if (ReferenceEquals(t, Commands.SelectedUnit))
                                    {
                                        if (!My.MyProject.Forms.frmMessage.Visible)
                                        {
                                            Unit argu25 = null;
                                            GUI.OpenMessageForm(ref Commands.SelectedUnit, u2: ref argu25);
                                        }
                                        else
                                        {
                                            object argu26 = null;
                                            GUI.UpdateMessageForm(ref Commands.SelectedUnit, u2: ref argu26);
                                        }
                                    }
                                    else if (!My.MyProject.Forms.frmMessage.Visible)
                                    {
                                        GUI.OpenMessageForm(ref t, ref Commands.SelectedUnit);
                                    }
                                    else
                                    {
                                        object argu27 = Commands.SelectedUnit;
                                        GUI.UpdateMessageForm(ref t, ref argu27);
                                    }

                                    GUI.Sleep(150);
                                }

                                // ＥＮを回復させる
                                tmp = withBlock1.EN;
                                if (EffectType(i) == "ＥＮ増加")
                                {
                                    withBlock1.EN = (int)(withBlock1.EN + 10d * EffectLevel(i));
                                }
                                else
                                {
                                    withBlock1.RecoverEN(10d * EffectLevel(i));
                                }

                                if (!is_event)
                                {
                                    if (!displayed_string)
                                    {
                                        if (EffectLevel(i) >= 0d)
                                        {
                                            string argmsg2 = "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.EN - tmp);
                                            GUI.DrawSysString(withBlock1.x, withBlock1.y, ref argmsg2);
                                        }
                                        else
                                        {
                                            string argmsg3 = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.EN - tmp);
                                            GUI.DrawSysString(withBlock1.x, withBlock1.y, ref argmsg3);
                                        }
                                    }

                                    displayed_string = true;
                                    if (ReferenceEquals(t, Commands.SelectedUnit))
                                    {
                                        object argu28 = null;
                                        GUI.UpdateMessageForm(ref Commands.SelectedUnit, u2: ref argu28);
                                    }
                                    else
                                    {
                                        object argu29 = Commands.SelectedUnit;
                                        GUI.UpdateMessageForm(ref t, ref argu29);
                                    }

                                    if (EffectLevel(i) >= 0d)
                                    {
                                        string argtname2 = "ＥＮ";
                                        GUI.DisplaySysMessage(withBlock1.Nickname + "の" + Expression.Term(ref argtname2, ref t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.EN - tmp) + "回復した。");
                                    }
                                    else
                                    {
                                        string argtname3 = "ＥＮ";
                                        GUI.DisplaySysMessage(withBlock1.Nickname + "の" + Expression.Term(ref argtname3, ref t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(tmp - withBlock1.EN) + "減少した。");
                                    }
                                }

                                need_update = true;
                                break;
                            }

                        case "霊力回復":
                        case "霊力増加":
                            {
                                // 効果が適用可能かどうか判定
                                if (EffectLevel(i) > 0d)
                                {
                                    object argIndex4 = "ゾンビ";
                                    if (withBlock1.IsConditionSatisfied(ref argIndex4))
                                    {
                                        goto NextEffect;
                                    }

                                    if (withBlock1.MainPilot().Plana == withBlock1.MainPilot().MaxPlana())
                                    {
                                        goto NextEffect;
                                    }
                                }

                                if (!is_event)
                                {
                                    if (ReferenceEquals(t, Commands.SelectedUnit))
                                    {
                                        if (!My.MyProject.Forms.frmMessage.Visible)
                                        {
                                            Unit argu210 = null;
                                            GUI.OpenMessageForm(ref Commands.SelectedUnit, u2: ref argu210);
                                        }
                                        else
                                        {
                                            object argu211 = null;
                                            GUI.UpdateMessageForm(ref Commands.SelectedUnit, u2: ref argu211);
                                        }
                                    }
                                    else if (!My.MyProject.Forms.frmMessage.Visible)
                                    {
                                        GUI.OpenMessageForm(ref t, ref Commands.SelectedUnit);
                                    }
                                    else
                                    {
                                        object argu212 = Commands.SelectedUnit;
                                        GUI.UpdateMessageForm(ref t, ref argu212);
                                    }

                                    GUI.Sleep(150);
                                }

                                // 霊力を回復させる
                                {
                                    var withBlock2 = withBlock1.MainPilot();
                                    tmp = withBlock2.Plana;
                                    if (EffectType(i) == "霊力増加")
                                    {
                                        withBlock2.Plana = (int)(withBlock2.Plana + 10d * EffectLevel(i));
                                    }
                                    else
                                    {
                                        withBlock2.Plana = (int)(withBlock2.Plana + (long)(withBlock2.MaxPlana() * EffectLevel(i)) / 10L);
                                    }
                                }

                                if (!is_event)
                                {
                                    if (!displayed_string)
                                    {
                                        if (EffectLevel(i) >= 0d)
                                        {
                                            string argmsg4 = "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MainPilot().Plana - tmp);
                                            GUI.DrawSysString(withBlock1.x, withBlock1.y, ref argmsg4);
                                        }
                                        else
                                        {
                                            string argmsg5 = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MainPilot().Plana - tmp);
                                            GUI.DrawSysString(withBlock1.x, withBlock1.y, ref argmsg5);
                                        }
                                    }

                                    displayed_string = true;
                                    if (ReferenceEquals(t, Commands.SelectedUnit))
                                    {
                                        object argu213 = null;
                                        GUI.UpdateMessageForm(ref Commands.SelectedUnit, u2: ref argu213);
                                    }
                                    else
                                    {
                                        object argu214 = Commands.SelectedUnit;
                                        GUI.UpdateMessageForm(ref t, ref argu214);
                                    }

                                    if (EffectLevel(i) >= 0d)
                                    {
                                        object argIndex5 = "霊力";
                                        GUI.DisplaySysMessage(withBlock1.Nickname + "の" + withBlock1.MainPilot().SkillName0(ref argIndex5) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MainPilot().Plana - tmp) + "回復した。");
                                    }
                                    else
                                    {
                                        object argIndex6 = "霊力";
                                        GUI.DisplaySysMessage(withBlock1.Nickname + "の" + withBlock1.MainPilot().SkillName0(ref argIndex6) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(tmp - withBlock1.MainPilot().Plana) + "減少した。");
                                    }
                                }

                                need_update = true;
                                break;
                            }

                        case "ＳＰ回復":
                            {
                                // 効果が適用可能かどうか判定
                                if (EffectLevel(i) > 0d)
                                {
                                    object argIndex7 = "ゾンビ";
                                    if (withBlock1.IsConditionSatisfied(ref argIndex7))
                                    {
                                        goto NextEffect;
                                    }
                                }

                                if (!is_event)
                                {
                                    if (ReferenceEquals(t, Commands.SelectedUnit))
                                    {
                                        if (!My.MyProject.Forms.frmMessage.Visible)
                                        {
                                            Unit argu215 = null;
                                            GUI.OpenMessageForm(ref Commands.SelectedUnit, u2: ref argu215);
                                        }
                                        else
                                        {
                                            object argu216 = null;
                                            GUI.UpdateMessageForm(ref Commands.SelectedUnit, u2: ref argu216);
                                        }
                                    }
                                    else if (!My.MyProject.Forms.frmMessage.Visible)
                                    {
                                        GUI.OpenMessageForm(ref t, ref Commands.SelectedUnit);
                                    }
                                    else
                                    {
                                        object argu217 = Commands.SelectedUnit;
                                        GUI.UpdateMessageForm(ref t, ref argu217);
                                    }

                                    GUI.Sleep(150);
                                }

                                // 回復対象となるパイロット数を算出
                                n = (short)(withBlock1.CountPilot() + withBlock1.CountSupport());
                                string argfname = "追加サポート";
                                if (withBlock1.IsFeatureAvailable(ref argfname))
                                {
                                    n = (short)(n + 1);
                                }

                                // ＳＰを回復
                                if (n == 1)
                                {
                                    // メインパイロットのみのＳＰを回復
                                    tmp = withBlock1.MainPilot().SP;
                                    withBlock1.MainPilot().SP = (int)(withBlock1.MainPilot().SP + 10d * EffectLevel(i));
                                    if (!is_event)
                                    {
                                        if (!displayed_string)
                                        {
                                            if (EffectLevel(i) >= 0d)
                                            {
                                                string argmsg6 = "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MainPilot().SP - tmp);
                                                GUI.DrawSysString(withBlock1.x, withBlock1.y, ref argmsg6);
                                            }
                                            else
                                            {
                                                string argmsg7 = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MainPilot().SP - tmp);
                                                GUI.DrawSysString(withBlock1.x, withBlock1.y, ref argmsg7);
                                            }
                                        }

                                        displayed_string = true;
                                        if (EffectLevel(i) >= 0d)
                                        {
                                            string argtname4 = "ＳＰ";
                                            GUI.DisplaySysMessage(withBlock1.MainPilot().get_Nickname(false) + "の" + Expression.Term(ref argtname4, ref t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MainPilot().SP - tmp) + "回復した。");
                                        }
                                        else
                                        {
                                            string argtname5 = "ＳＰ";
                                            GUI.DisplaySysMessage(withBlock1.MainPilot().get_Nickname(false) + "の" + Expression.Term(ref argtname5, ref t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(tmp - withBlock1.MainPilot().SP) + "減少した。");
                                        }
                                    }
                                }
                                else
                                {
                                    // メインパイロットのＳＰを回復
                                    tmp = withBlock1.MainPilot().SP;
                                    withBlock1.MainPilot().SP = (int)(withBlock1.MainPilot().SP + 2d * EffectLevel(i) + (long)(10d * EffectLevel(i)) / n);
                                    if (!is_event)
                                    {
                                        if (!displayed_string)
                                        {
                                            if (EffectLevel(i) >= 0d)
                                            {
                                                string argmsg8 = "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MainPilot().SP - tmp);
                                                GUI.DrawSysString(withBlock1.x, withBlock1.y, ref argmsg8);
                                            }
                                            else
                                            {
                                                string argmsg9 = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MainPilot().SP - tmp);
                                                GUI.DrawSysString(withBlock1.x, withBlock1.y, ref argmsg9);
                                            }
                                        }

                                        displayed_string = true;
                                        if (EffectLevel(i) >= 0d)
                                        {
                                            string argtname6 = "ＳＰ";
                                            GUI.DisplaySysMessage(withBlock1.MainPilot().get_Nickname(false) + "の" + Expression.Term(ref argtname6, ref t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MainPilot().SP - tmp) + "回復した。");
                                        }
                                        else
                                        {
                                            string argtname7 = "ＳＰ";
                                            GUI.DisplaySysMessage(withBlock1.MainPilot().get_Nickname(false) + "の" + Expression.Term(ref argtname7, ref t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(tmp - withBlock1.MainPilot().SP) + "減少した。");
                                        }
                                    }

                                    // サブパイロットのＳＰを回復
                                    var loopTo2 = withBlock1.CountPilot();
                                    for (j = 2; j <= loopTo2; j++)
                                    {
                                        object argIndex8 = j;
                                        {
                                            var withBlock3 = withBlock1.Pilot(ref argIndex8);
                                            tmp = withBlock3.SP;
                                            withBlock3.SP = (int)(withBlock3.SP + 2d * EffectLevel(i) + (long)(10d * EffectLevel(i)) / n);
                                            if (!is_event)
                                            {
                                                if (withBlock3.SP != tmp)
                                                {
                                                    if (EffectLevel(i) >= 0d)
                                                    {
                                                        string argtname8 = "ＳＰ";
                                                        GUI.DisplaySysMessage(withBlock3.get_Nickname(false) + "の" + Expression.Term(ref argtname8, ref t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock3.SP - tmp) + "回復した。");
                                                    }
                                                    else
                                                    {
                                                        string argtname9 = "ＳＰ";
                                                        GUI.DisplaySysMessage(withBlock3.get_Nickname(false) + "の" + Expression.Term(ref argtname9, ref t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(tmp - withBlock3.SP) + "減少した。");
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    // サポートパイロットのＳＰを回復
                                    var loopTo3 = withBlock1.CountSupport();
                                    for (j = 1; j <= loopTo3; j++)
                                    {
                                        object argIndex9 = j;
                                        {
                                            var withBlock4 = withBlock1.Support(ref argIndex9);
                                            tmp = withBlock4.SP;
                                            withBlock4.SP = (int)(withBlock4.SP + 2d * EffectLevel(i) + (long)(10d * EffectLevel(i)) / n);
                                            if (!is_event)
                                            {
                                                if (withBlock4.SP != tmp)
                                                {
                                                    if (EffectLevel(i) >= 0d)
                                                    {
                                                        string argtname10 = "ＳＰ";
                                                        GUI.DisplaySysMessage(withBlock4.get_Nickname(false) + "の" + Expression.Term(ref argtname10, ref t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.SP - tmp) + "回復した。");
                                                    }
                                                    else
                                                    {
                                                        string argtname11 = "ＳＰ";
                                                        GUI.DisplaySysMessage(withBlock4.get_Nickname(false) + "の" + Expression.Term(ref argtname11, ref t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(tmp - withBlock4.SP) + "減少した。");
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    // 追加サポートパイロットのＳＰを回復
                                    string argfname1 = "追加サポート";
                                    if (withBlock1.IsFeatureAvailable(ref argfname1))
                                    {
                                        {
                                            var withBlock5 = withBlock1.AdditionalSupport();
                                            tmp = withBlock5.SP;
                                            withBlock5.SP = (int)(withBlock5.SP + 2d * EffectLevel(i) + (long)(10d * EffectLevel(i)) / n);
                                            if (!is_event)
                                            {
                                                if (withBlock5.SP != tmp)
                                                {
                                                    if (EffectLevel(i) >= 0d)
                                                    {
                                                        string argtname12 = "ＳＰ";
                                                        GUI.DisplaySysMessage(withBlock5.get_Nickname(false) + "の" + Expression.Term(ref argtname12, ref t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock5.SP - tmp) + "回復した。");
                                                    }
                                                    else
                                                    {
                                                        string argtname13 = "ＳＰ";
                                                        GUI.DisplaySysMessage(withBlock5.get_Nickname(false) + "の" + Expression.Term(ref argtname13, ref t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(tmp - withBlock5.SP) + "減少した。");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                if (!is_event)
                                {
                                    if (TargetType == "全味方")
                                    {
                                        GUI.Sleep(150);
                                    }
                                }

                                break;
                            }

                        case "装填":
                            {
                                // 効果が適用可能かどうか判定
                                var loopTo4 = withBlock1.CountWeapon();
                                for (j = 1; j <= loopTo4; j++)
                                {
                                    if (withBlock1.Bullet(j) < withBlock1.MaxBullet(j))
                                    {
                                        break;
                                    }
                                }

                                if (j > withBlock1.CountWeapon())
                                {
                                    goto NextEffect;
                                }

                                if (!is_event)
                                {
                                    if (ReferenceEquals(t, Commands.SelectedUnit))
                                    {
                                        if (!My.MyProject.Forms.frmMessage.Visible)
                                        {
                                            Unit argu218 = null;
                                            GUI.OpenMessageForm(ref Commands.SelectedUnit, u2: ref argu218);
                                        }
                                        else
                                        {
                                            object argu219 = null;
                                            GUI.UpdateMessageForm(ref Commands.SelectedUnit, u2: ref argu219);
                                        }
                                    }
                                    else if (!My.MyProject.Forms.frmMessage.Visible)
                                    {
                                        GUI.OpenMessageForm(ref t, ref Commands.SelectedUnit);
                                    }
                                    else
                                    {
                                        object argu220 = Commands.SelectedUnit;
                                        GUI.UpdateMessageForm(ref t, ref argu220);
                                    }
                                }

                                // 弾薬を補給
                                withBlock1.BulletSupply();
                                if (!is_event)
                                {
                                    GUI.DisplaySysMessage(withBlock1.Nickname + "の弾数が全快した。");
                                }

                                break;
                            }

                        case "状態回復":
                            {
                                object argIndex11 = "攻撃不能";
                                object argIndex12 = "移動不能";
                                object argIndex13 = "装甲劣化";
                                object argIndex14 = "混乱";
                                object argIndex15 = "魅了";
                                object argIndex16 = "憑依";
                                object argIndex17 = "石化";
                                object argIndex18 = "凍結";
                                object argIndex19 = "麻痺";
                                object argIndex20 = "睡眠";
                                object argIndex21 = "毒";
                                object argIndex22 = "盲目";
                                object argIndex23 = "撹乱";
                                object argIndex24 = "恐怖";
                                object argIndex25 = "沈黙";
                                object argIndex26 = "ゾンビ";
                                object argIndex27 = "回復不能";
                                object argIndex28 = "オーラ使用不能";
                                object argIndex29 = "超能力使用不能";
                                object argIndex30 = "同調率使用不能";
                                object argIndex31 = "超感覚使用不能";
                                object argIndex32 = "知覚強化使用不能";
                                object argIndex33 = "霊力使用不能";
                                object argIndex34 = "術使用不能";
                                object argIndex35 = "技使用不能";
                                if (withBlock1.ConditionLifetime(ref argIndex11) <= 0 & withBlock1.ConditionLifetime(ref argIndex12) <= 0 & withBlock1.ConditionLifetime(ref argIndex13) <= 0 & withBlock1.ConditionLifetime(ref argIndex14) <= 0 & withBlock1.ConditionLifetime(ref argIndex15) <= 0 & withBlock1.ConditionLifetime(ref argIndex16) <= 0 & withBlock1.ConditionLifetime(ref argIndex17) <= 0 & withBlock1.ConditionLifetime(ref argIndex18) <= 0 & withBlock1.ConditionLifetime(ref argIndex19) <= 0 & withBlock1.ConditionLifetime(ref argIndex20) <= 0 & withBlock1.ConditionLifetime(ref argIndex21) <= 0 & withBlock1.ConditionLifetime(ref argIndex22) <= 0 & withBlock1.ConditionLifetime(ref argIndex23) <= 0 & withBlock1.ConditionLifetime(ref argIndex24) <= 0 & withBlock1.ConditionLifetime(ref argIndex25) <= 0 & withBlock1.ConditionLifetime(ref argIndex26) <= 0 & withBlock1.ConditionLifetime(ref argIndex27) <= 0 & withBlock1.ConditionLifetime(ref argIndex28) <= 0 & withBlock1.ConditionLifetime(ref argIndex29) <= 0 & withBlock1.ConditionLifetime(ref argIndex30) <= 0 & withBlock1.ConditionLifetime(ref argIndex31) <= 0 & withBlock1.ConditionLifetime(ref argIndex32) <= 0 & withBlock1.ConditionLifetime(ref argIndex33) <= 0 & withBlock1.ConditionLifetime(ref argIndex34) <= 0 & withBlock1.ConditionLifetime(ref argIndex35) <= 0)
                                {
                                    var loopTo5 = withBlock1.CountCondition();
                                    for (j = 1; j <= loopTo5; j++)
                                    {
                                        string localCondition2() { object argIndex1 = j; var ret = withBlock1.Condition(ref argIndex1); return ret; }

                                        if (Strings.Len(localCondition2()) > 6)
                                        {
                                            // 弱、効属性は状態回復から除外。
                                            string localCondition1() { object argIndex1 = j; var ret = withBlock1.Condition(ref argIndex1); return ret; }

                                            if (Strings.Right(localCondition1(), 6) == "属性使用不能")
                                            {
                                                string localCondition() { object argIndex1 = j; var ret = withBlock1.Condition(ref argIndex1); return ret; }

                                                object argIndex10 = localCondition();
                                                if (withBlock1.ConditionLifetime(ref argIndex10) > 0)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    if (j > withBlock1.CountCondition())
                                    {
                                        goto NextEffect;
                                    }
                                }

                                if (!is_event)
                                {
                                    if (ReferenceEquals(t, Commands.SelectedUnit))
                                    {
                                        if (!My.MyProject.Forms.frmMessage.Visible)
                                        {
                                            Unit argu221 = null;
                                            GUI.OpenMessageForm(ref Commands.SelectedUnit, u2: ref argu221);
                                        }
                                        else
                                        {
                                            object argu222 = null;
                                            GUI.UpdateMessageForm(ref Commands.SelectedUnit, u2: ref argu222);
                                        }
                                    }
                                    else if (!My.MyProject.Forms.frmMessage.Visible)
                                    {
                                        GUI.OpenMessageForm(ref t, ref Commands.SelectedUnit);
                                    }
                                    else
                                    {
                                        object argu223 = Commands.SelectedUnit;
                                        GUI.UpdateMessageForm(ref t, ref argu223);
                                    }
                                }

                                // 全てのステータス異常を回復
                                object argIndex37 = "攻撃不能";
                                if (withBlock1.ConditionLifetime(ref argIndex37) > 0)
                                {
                                    object argIndex36 = "攻撃不能";
                                    withBlock1.DeleteCondition(ref argIndex36);
                                }

                                object argIndex39 = "移動不能";
                                if (withBlock1.ConditionLifetime(ref argIndex39) > 0)
                                {
                                    object argIndex38 = "移動不能";
                                    withBlock1.DeleteCondition(ref argIndex38);
                                }

                                object argIndex41 = "装甲劣化";
                                if (withBlock1.ConditionLifetime(ref argIndex41) > 0)
                                {
                                    object argIndex40 = "装甲劣化";
                                    withBlock1.DeleteCondition(ref argIndex40);
                                }

                                object argIndex43 = "混乱";
                                if (withBlock1.ConditionLifetime(ref argIndex43) > 0)
                                {
                                    object argIndex42 = "混乱";
                                    withBlock1.DeleteCondition(ref argIndex42);
                                }

                                object argIndex45 = "魅了";
                                if (withBlock1.ConditionLifetime(ref argIndex45) > 0)
                                {
                                    object argIndex44 = "魅了";
                                    withBlock1.DeleteCondition(ref argIndex44);
                                }

                                object argIndex47 = "憑依";
                                if (withBlock1.ConditionLifetime(ref argIndex47) > 0)
                                {
                                    object argIndex46 = "憑依";
                                    withBlock1.DeleteCondition(ref argIndex46);
                                }

                                object argIndex49 = "石化";
                                if (withBlock1.ConditionLifetime(ref argIndex49) > 0)
                                {
                                    object argIndex48 = "石化";
                                    withBlock1.DeleteCondition(ref argIndex48);
                                }

                                object argIndex51 = "凍結";
                                if (withBlock1.ConditionLifetime(ref argIndex51) > 0)
                                {
                                    object argIndex50 = "凍結";
                                    withBlock1.DeleteCondition(ref argIndex50);
                                }

                                object argIndex53 = "麻痺";
                                if (withBlock1.ConditionLifetime(ref argIndex53) > 0)
                                {
                                    object argIndex52 = "麻痺";
                                    withBlock1.DeleteCondition(ref argIndex52);
                                }

                                object argIndex55 = "睡眠";
                                if (withBlock1.ConditionLifetime(ref argIndex55) > 0)
                                {
                                    object argIndex54 = "睡眠";
                                    withBlock1.DeleteCondition(ref argIndex54);
                                }

                                object argIndex57 = "毒";
                                if (withBlock1.ConditionLifetime(ref argIndex57) > 0)
                                {
                                    object argIndex56 = "毒";
                                    withBlock1.DeleteCondition(ref argIndex56);
                                }

                                object argIndex59 = "盲目";
                                if (withBlock1.ConditionLifetime(ref argIndex59) > 0)
                                {
                                    object argIndex58 = "盲目";
                                    withBlock1.DeleteCondition(ref argIndex58);
                                }

                                object argIndex61 = "撹乱";
                                if (withBlock1.ConditionLifetime(ref argIndex61) > 0)
                                {
                                    object argIndex60 = "撹乱";
                                    withBlock1.DeleteCondition(ref argIndex60);
                                }

                                object argIndex63 = "恐怖";
                                if (withBlock1.ConditionLifetime(ref argIndex63) > 0)
                                {
                                    object argIndex62 = "恐怖";
                                    withBlock1.DeleteCondition(ref argIndex62);
                                }

                                object argIndex65 = "沈黙";
                                if (withBlock1.ConditionLifetime(ref argIndex65) > 0)
                                {
                                    object argIndex64 = "沈黙";
                                    withBlock1.DeleteCondition(ref argIndex64);
                                }

                                object argIndex67 = "ゾンビ";
                                if (withBlock1.ConditionLifetime(ref argIndex67) > 0)
                                {
                                    object argIndex66 = "ゾンビ";
                                    withBlock1.DeleteCondition(ref argIndex66);
                                }

                                object argIndex69 = "回復不能";
                                if (withBlock1.ConditionLifetime(ref argIndex69) > 0)
                                {
                                    object argIndex68 = "回復不能";
                                    withBlock1.DeleteCondition(ref argIndex68);
                                }

                                object argIndex71 = "オーラ使用不能";
                                if (withBlock1.ConditionLifetime(ref argIndex71) > 0)
                                {
                                    object argIndex70 = "オーラ使用不能";
                                    withBlock1.DeleteCondition(ref argIndex70);
                                }

                                object argIndex73 = "超能力使用不能";
                                if (withBlock1.ConditionLifetime(ref argIndex73) > 0)
                                {
                                    object argIndex72 = "超能力使用不能";
                                    withBlock1.DeleteCondition(ref argIndex72);
                                }

                                object argIndex75 = "同調率使用不能";
                                if (withBlock1.ConditionLifetime(ref argIndex75) > 0)
                                {
                                    object argIndex74 = "同調率使用不能";
                                    withBlock1.DeleteCondition(ref argIndex74);
                                }

                                object argIndex77 = "超感覚使用不能";
                                if (withBlock1.ConditionLifetime(ref argIndex77) > 0)
                                {
                                    object argIndex76 = "超感覚使用不能";
                                    withBlock1.DeleteCondition(ref argIndex76);
                                }

                                object argIndex79 = "知覚強化使用不能";
                                if (withBlock1.ConditionLifetime(ref argIndex79) > 0)
                                {
                                    object argIndex78 = "知覚強化使用不能";
                                    withBlock1.DeleteCondition(ref argIndex78);
                                }

                                object argIndex81 = "霊力使用不能";
                                if (withBlock1.ConditionLifetime(ref argIndex81) > 0)
                                {
                                    object argIndex80 = "霊力使用不能";
                                    withBlock1.DeleteCondition(ref argIndex80);
                                }

                                object argIndex83 = "術使用不能";
                                if (withBlock1.ConditionLifetime(ref argIndex83) > 0)
                                {
                                    object argIndex82 = "術使用不能";
                                    withBlock1.DeleteCondition(ref argIndex82);
                                }

                                object argIndex85 = "技使用不能";
                                if (withBlock1.ConditionLifetime(ref argIndex85) > 0)
                                {
                                    object argIndex84 = "技使用不能";
                                    withBlock1.DeleteCondition(ref argIndex84);
                                }

                                var loopTo6 = withBlock1.CountCondition();
                                for (j = 1; j <= loopTo6; j++)
                                {
                                    string localCondition6() { object argIndex1 = j; var ret = withBlock1.Condition(ref argIndex1); return ret; }

                                    if (Strings.Len(localCondition6()) > 6)
                                    {
                                        // 弱、効属性は状態回復から除外。
                                        string localCondition5() { object argIndex1 = j; var ret = withBlock1.Condition(ref argIndex1); return ret; }

                                        if (Strings.Right(localCondition5(), 6) == "属性使用不能")
                                        {
                                            string localCondition4() { object argIndex1 = j; var ret = withBlock1.Condition(ref argIndex1); return ret; }

                                            object argIndex87 = localCondition4();
                                            if (withBlock1.ConditionLifetime(ref argIndex87) > 0)
                                            {
                                                string localCondition3() { object argIndex1 = j; var ret = withBlock1.Condition(ref argIndex1); return ret; }

                                                object argIndex86 = localCondition3();
                                                withBlock1.DeleteCondition(ref argIndex86);
                                            }
                                        }
                                    }
                                }

                                if (!is_event)
                                {
                                    GUI.DisplaySysMessage(withBlock1.Nickname + "の状態が回復した。");
                                }

                                break;
                            }

                        case "行動数回復":
                            {
                                // 効果が適用可能かどうか判定
                                if (withBlock1.Action > 0 | withBlock1.MaxAction() == 0)
                                {
                                    goto NextEffect;
                                }

                                // 行動数を回復させる
                                withBlock1.UsedAction = (short)(withBlock1.UsedAction - 1);

                                // 他の効果の表示のためにメッセージウィンドウが表示されているので
                                // なければ特にメッセージは表示しない (効果は見れば分かるので)
                                if (!is_event)
                                {
                                    if (My.MyProject.Forms.frmMessage.Visible)
                                    {
                                        if (ReferenceEquals(t, Commands.SelectedUnit))
                                        {
                                            object argu224 = null;
                                            GUI.UpdateMessageForm(ref Commands.SelectedUnit, u2: ref argu224);
                                        }
                                        else
                                        {
                                            object argu225 = Commands.SelectedUnit;
                                            GUI.UpdateMessageForm(ref t, ref argu225);
                                        }

                                        GUI.DisplaySysMessage(withBlock1.Nickname + "は行動可能になった。");
                                    }
                                }

                                break;
                            }

                        case "行動数増加":
                            {
                                // 効果が適用可能かどうか判定
                                if (withBlock1.Action > 3 | withBlock1.MaxAction() == 0)
                                {
                                    goto NextEffect;
                                }

                                // 行動数を増やす
                                withBlock1.UsedAction = (short)(withBlock1.UsedAction - 1);
                                if (!is_event)
                                {
                                    if (ReferenceEquals(t, Commands.SelectedUnit))
                                    {
                                        if (!My.MyProject.Forms.frmMessage.Visible)
                                        {
                                            Unit argu226 = null;
                                            GUI.OpenMessageForm(ref Commands.SelectedUnit, u2: ref argu226);
                                        }
                                        else
                                        {
                                            object argu227 = null;
                                            GUI.UpdateMessageForm(ref Commands.SelectedUnit, u2: ref argu227);
                                        }
                                    }
                                    else if (!My.MyProject.Forms.frmMessage.Visible)
                                    {
                                        GUI.OpenMessageForm(ref t, ref Commands.SelectedUnit);
                                    }
                                    else
                                    {
                                        object argu228 = Commands.SelectedUnit;
                                        GUI.UpdateMessageForm(ref t, ref argu228);
                                    }

                                    GUI.DisplaySysMessage(withBlock1.Nickname + "は" + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.Action), VbStrConv.Wide) + "回行動可能になった。");
                                }

                                break;
                            }

                        case "スペシャルパワー":
                        case "精神コマンド":
                            {
                                object argIndex88 = EffectData(i);
                                if (SRC.SPDList.IsDefined(ref argIndex88))
                                {
                                    string argsname = EffectData(i);
                                    withBlock1.MakeSpecialPowerInEffect(ref argsname, ref my_unit.MainPilot().ID);
                                }
                                else
                                {
                                    string argmsg10 = "スペシャルパワー「" + Name + "」で使われているスペシャルパワー「" + EffectData(i) + "」は定義されていません。";
                                    GUI.ErrorMessage(ref argmsg10);
                                }

                                break;
                            }

                        case "気力増加":
                            {
                                if (withBlock1.MainPilot().Personality == "機械")
                                {
                                    goto NextEffect;
                                }

                                if (withBlock1.MainPilot().Morale == withBlock1.MainPilot().MaxMorale)
                                {
                                    goto NextEffect;
                                }

                                // 気力を増加させる
                                tmp = withBlock1.MainPilot().Morale;
                                withBlock1.IncreaseMorale((short)(10d * EffectLevel(i)));
                                if (!is_event)
                                {
                                    if (!displayed_string)
                                    {
                                        string argmsg11 = "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MainPilot().Morale - tmp);
                                        GUI.DrawSysString(withBlock1.x, withBlock1.y, ref argmsg11);
                                    }

                                    displayed_string = true;
                                }

                                need_update = true;
                                break;
                            }

                        case "気力低下":
                            {
                                // 効果が適用可能かどうか判定
                                if (withBlock1.MainPilot().Personality == "機械")
                                {
                                    goto NextEffect;
                                }

                                if (withBlock1.MainPilot().Morale == withBlock1.MainPilot().MinMorale)
                                {
                                    goto NextEffect;
                                }

                                // 気力を低下させる
                                tmp = withBlock1.MainPilot().Morale;
                                withBlock1.IncreaseMorale((short)(-10 * EffectLevel(i)));
                                if (!is_event)
                                {
                                    if (TargetType == "敵" | TargetType == "全敵")
                                    {
                                        if (!displayed_string)
                                        {
                                            string argmsg12 = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MainPilot().Morale - tmp);
                                            GUI.DrawSysString(withBlock1.x, withBlock1.y, ref argmsg12);
                                            displayed_string = true;
                                        }
                                    }
                                }

                                need_update = true;
                                break;
                            }

                        case "ランダムダメージ":
                            {
                                // 効果が適用可能かどうか判定
                                object argIndex89 = "無敵";
                                if (withBlock1.IsConditionSatisfied(ref argIndex89))
                                {
                                    goto NextEffect;
                                }

                                if (!is_event)
                                {
                                    if (ReferenceEquals(t, Commands.SelectedUnit))
                                    {
                                        if (!My.MyProject.Forms.frmMessage.Visible)
                                        {
                                            Unit argu229 = null;
                                            GUI.OpenMessageForm(ref Commands.SelectedUnit, u2: ref argu229);
                                        }
                                        else
                                        {
                                            object argu230 = null;
                                            GUI.UpdateMessageForm(ref Commands.SelectedUnit, u2: ref argu230);
                                        }
                                    }
                                    else if (!My.MyProject.Forms.frmMessage.Visible)
                                    {
                                        GUI.OpenMessageForm(ref t, ref Commands.SelectedUnit);
                                    }
                                    else
                                    {
                                        object argu231 = Commands.SelectedUnit;
                                        GUI.UpdateMessageForm(ref t, ref argu231);
                                    }
                                }

                                // ダメージを与える
                                tmp = withBlock1.HP;
                                withBlock1.HP = GeneralLib.MaxLng(withBlock1.HP - 10 * GeneralLib.Dice((int)(10d * EffectLevel(i))), 10);
                                if (TargetType == "全敵")
                                {
                                    GUI.Sleep(150);
                                }

                                // 特殊能力「不安定」による暴走チェック
                                string argfname2 = "不安定";
                                if (withBlock1.IsFeatureAvailable(ref argfname2))
                                {
                                    object argIndex90 = "暴走";
                                    if (withBlock1.HP <= withBlock1.MaxHP / 4 & !withBlock1.IsConditionSatisfied(ref argIndex90))
                                    {
                                        string argcname = "暴走";
                                        string argcdata = "";
                                        withBlock1.AddCondition(ref argcname, -1, cdata: ref argcdata);
                                    }
                                }

                                if (!is_event)
                                {
                                    if (!displayed_string)
                                    {
                                        string argmsg13 = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(tmp - withBlock1.HP);
                                        GUI.DrawSysString(withBlock1.x, withBlock1.y, ref argmsg13);
                                    }

                                    displayed_string = true;
                                    if (ReferenceEquals(t, Commands.SelectedUnit))
                                    {
                                        object argu232 = null;
                                        GUI.UpdateMessageForm(ref Commands.SelectedUnit, u2: ref argu232);
                                    }
                                    else
                                    {
                                        object argu233 = Commands.SelectedUnit;
                                        GUI.UpdateMessageForm(ref t, ref argu233);
                                    }

                                    GUI.DisplaySysMessage(withBlock1.Nickname + "に" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(tmp - withBlock1.HP) + "のダメージを与えた。");
                                }

                                need_update = true;
                                break;
                            }

                        case "ＨＰ減少":
                            {
                                // 効果が適用可能かどうか判定
                                object argIndex91 = "無敵";
                                if (withBlock1.IsConditionSatisfied(ref argIndex91))
                                {
                                    goto NextEffect;
                                }

                                if (!is_event)
                                {
                                    if (ReferenceEquals(t, Commands.SelectedUnit))
                                    {
                                        if (!My.MyProject.Forms.frmMessage.Visible)
                                        {
                                            Unit argu234 = null;
                                            GUI.OpenMessageForm(ref Commands.SelectedUnit, u2: ref argu234);
                                        }
                                        else
                                        {
                                            object argu235 = null;
                                            GUI.UpdateMessageForm(ref Commands.SelectedUnit, u2: ref argu235);
                                        }
                                    }
                                    else if (!My.MyProject.Forms.frmMessage.Visible)
                                    {
                                        GUI.OpenMessageForm(ref t, ref Commands.SelectedUnit);
                                    }
                                    else
                                    {
                                        object argu236 = Commands.SelectedUnit;
                                        GUI.UpdateMessageForm(ref t, ref argu236);
                                    }
                                }

                                // ＨＰを減少させる
                                tmp = withBlock1.HP;
                                withBlock1.HP = (int)(withBlock1.HP - (long)(withBlock1.HP * EffectLevel(i)) / 10L);
                                if (TargetType == "全敵")
                                {
                                    GUI.Sleep(150);
                                }

                                // 特殊能力「不安定」による暴走チェック
                                string argfname3 = "不安定";
                                if (withBlock1.IsFeatureAvailable(ref argfname3))
                                {
                                    object argIndex92 = "暴走";
                                    if (withBlock1.HP <= withBlock1.MaxHP / 4 & !withBlock1.IsConditionSatisfied(ref argIndex92))
                                    {
                                        string argcname1 = "暴走";
                                        string argcdata1 = "";
                                        withBlock1.AddCondition(ref argcname1, -1, cdata: ref argcdata1);
                                    }
                                }

                                if (!is_event)
                                {
                                    if (!displayed_string)
                                    {
                                        string argmsg14 = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(tmp - withBlock1.HP);
                                        GUI.DrawSysString(withBlock1.x, withBlock1.y, ref argmsg14);
                                    }

                                    displayed_string = true;
                                    if (ReferenceEquals(t, Commands.SelectedUnit))
                                    {
                                        object argu237 = null;
                                        GUI.UpdateMessageForm(ref Commands.SelectedUnit, u2: ref argu237);
                                    }
                                    else
                                    {
                                        object argu238 = Commands.SelectedUnit;
                                        GUI.UpdateMessageForm(ref t, ref argu238);
                                    }

                                    if (ReferenceEquals(Commands.SelectedUnit, t))
                                    {
                                        string argtname14 = "ＨＰ";
                                        GUI.DisplaySysMessage(withBlock1.Nickname + "の" + Expression.Term(ref argtname14, ref t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(tmp - withBlock1.HP) + "減少した。");
                                    }
                                    else
                                    {
                                        string argtname15 = "ＨＰ";
                                        GUI.DisplaySysMessage(withBlock1.Nickname + "の" + Expression.Term(ref argtname15, ref t) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(tmp - withBlock1.HP) + "減少させた。");
                                    }
                                }

                                need_update = true;
                                break;
                            }

                        case "ＥＮ減少":
                            {
                                // 効果が適用可能かどうか判定
                                object argIndex93 = "無敵";
                                if (withBlock1.IsConditionSatisfied(ref argIndex93))
                                {
                                    goto NextEffect;
                                }

                                if (!is_event)
                                {
                                    if (ReferenceEquals(t, Commands.SelectedUnit))
                                    {
                                        if (!My.MyProject.Forms.frmMessage.Visible)
                                        {
                                            Unit argu239 = null;
                                            GUI.OpenMessageForm(ref Commands.SelectedUnit, u2: ref argu239);
                                        }
                                        else
                                        {
                                            object argu240 = null;
                                            GUI.UpdateMessageForm(ref Commands.SelectedUnit, u2: ref argu240);
                                        }
                                    }
                                    else if (!My.MyProject.Forms.frmMessage.Visible)
                                    {
                                        GUI.OpenMessageForm(ref t, ref Commands.SelectedUnit);
                                    }
                                    else
                                    {
                                        object argu241 = Commands.SelectedUnit;
                                        GUI.UpdateMessageForm(ref t, ref argu241);
                                    }
                                }

                                // ＥＮを減少させる
                                tmp = withBlock1.EN;
                                withBlock1.EN = (int)(withBlock1.EN - (long)(withBlock1.EN * EffectLevel(i)) / 10L);
                                if (TargetType == "全敵")
                                {
                                    GUI.Sleep(150);
                                }

                                if (!displayed_string)
                                {
                                    string argmsg15 = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(tmp - withBlock1.EN);
                                    GUI.DrawSysString(withBlock1.x, withBlock1.y, ref argmsg15);
                                }

                                displayed_string = true;
                                if (ReferenceEquals(t, Commands.SelectedUnit))
                                {
                                    object argu242 = null;
                                    GUI.UpdateMessageForm(ref Commands.SelectedUnit, u2: ref argu242);
                                }
                                else
                                {
                                    object argu243 = Commands.SelectedUnit;
                                    GUI.UpdateMessageForm(ref t, ref argu243);
                                }

                                if (ReferenceEquals(Commands.SelectedUnit, t))
                                {
                                    string argtname16 = "ＥＮ";
                                    GUI.DisplaySysMessage(withBlock1.Nickname + "の" + Expression.Term(ref argtname16, ref t) + "が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(tmp - withBlock1.EN) + "減少した。");
                                }
                                else
                                {
                                    string argtname17 = "ＥＮ";
                                    GUI.DisplaySysMessage(withBlock1.Nickname + "の" + Expression.Term(ref argtname17, ref t) + "を" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(tmp - withBlock1.EN) + "減少させた。");
                                }

                                need_update = true;
                                break;
                            }

                        case "偵察":
                            {
                                // 未識別のユニットは識別しておく
                                string argoname = "ユニット情報隠蔽";
                                if (Expression.IsOptionDefined(ref argoname))
                                {
                                    object argIndex94 = "識別済み";
                                    if (!withBlock1.IsConditionSatisfied(ref argIndex94))
                                    {
                                        string argcname2 = "識別済み";
                                        string argcdata2 = "非表示";
                                        withBlock1.AddCondition(ref argcname2, -1, 0d, ref argcdata2);
                                        Status.DisplayUnitStatus(ref t);
                                    }
                                }

                                object argIndex96 = "ユニット情報隠蔽";
                                if (withBlock1.IsConditionSatisfied(ref argIndex96))
                                {
                                    object argIndex95 = "ユニット情報隠蔽";
                                    withBlock1.DeleteCondition(ref argIndex95);
                                    Status.DisplayUnitStatus(ref t);
                                }

                                if (!My.MyProject.Forms.frmMessage.Visible)
                                {
                                    Unit argu1 = null;
                                    Unit argu244 = null;
                                    GUI.OpenMessageForm(u1: ref argu1, u2: ref argu244);
                                }

                                string argpname = "システム";
                                string argtname18 = "ＨＰ";
                                string argtname19 = "ＥＮ";
                                string argtname20 = "資金";
                                GUI.DisplayMessage(ref argpname, Expression.Term(ref argtname18, ref t, 6) + "：" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.HP) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MaxHP) + ";" + Expression.Term(ref argtname19, ref t, 6) + "：" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.EN) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MaxEN) + ";" + Expression.Term(ref argtname20, ref t, 6) + "：" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.Value / 2) + ";" + "経験値：" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.ExpValue + withBlock1.MainPilot().ExpValue));
                                string argfname4 = "アイテム所有";
                                if (withBlock1.IsFeatureAvailable(ref argfname4))
                                {
                                    object argIndex98 = "アイテム所有";
                                    object argIndex99 = withBlock1.FeatureData(ref argIndex98);
                                    if (SRC.IDList.IsDefined(ref argIndex99))
                                    {
                                        ItemData localItem() { object argIndex1 = "アイテム所有"; object argIndex2 = withBlock1.FeatureData(ref argIndex1); var ret = SRC.IDList.Item(ref argIndex2); return ret; }

                                        msg = localItem().Nickname + "を盗むことが出来る。;";
                                    }
                                    else
                                    {
                                        object argIndex97 = "アイテム所有";
                                        string argmsg16 = withBlock1.Name + "の所有アイテム「" + withBlock1.FeatureData(ref argIndex97) + "」のデータが見つかりません";
                                        GUI.ErrorMessage(ref argmsg16);
                                    }
                                }

                                string argfname5 = "レアアイテム所有";
                                if (withBlock1.IsFeatureAvailable(ref argfname5))
                                {
                                    object argIndex101 = "レアアイテム所有";
                                    object argIndex102 = withBlock1.FeatureData(ref argIndex101);
                                    if (SRC.IDList.IsDefined(ref argIndex102))
                                    {
                                        if (Strings.Len(msg) > 0)
                                        {
                                            msg = msg + "また、";
                                        }

                                        ItemData localItem1() { object argIndex1 = "レアアイテム所有"; object argIndex2 = withBlock1.FeatureData(ref argIndex1); var ret = SRC.IDList.Item(ref argIndex2); return ret; }

                                        msg = msg + "まれに" + localItem1().Nickname + "を盗むことが出来る。;";
                                    }
                                    else
                                    {
                                        object argIndex100 = "レアアイテム所有";
                                        string argmsg17 = withBlock1.Name + "の所有レアアイテム「" + withBlock1.FeatureData(ref argIndex100) + "」のデータが見つかりません";
                                        GUI.ErrorMessage(ref argmsg17);
                                    }
                                }

                                string argfname6 = "ラーニング可能技";
                                if (withBlock1.IsFeatureAvailable(ref argfname6))
                                {
                                    object argIndex103 = "ラーニング可能技";
                                    msg = msg + "「" + withBlock1.FeatureData(ref argIndex103) + "」をラーニング可能。";
                                }

                                if (Strings.Len(msg) > 0)
                                {
                                    string argpname1 = "システム";
                                    GUI.DisplayMessage(ref argpname1, msg);
                                }

                                break;
                            }

                        case "自爆":
                            {
                                Unit argu245 = null;
                                GUI.OpenMessageForm(ref t, u2: ref argu245);
                                withBlock1.SuicidalExplosion();
                                return ApplyRet;
                            }

                        case "復活":
                            {
                                if (Duration == "破壊")
                                {
                                    // 破壊直後に復活する場合
                                    withBlock1.HP = withBlock1.MaxHP;
                                }
                                else
                                {
                                    // 破壊後に他のパイロットの力で復活する場合

                                    // 復活時は通常形態に戻る
                                    string argfname7 = "ノーマルモード";
                                    if (withBlock1.IsFeatureAvailable(ref argfname7))
                                    {
                                        string localLIndex() { object argIndex1 = "ノーマルモード"; string arglist = withBlock1.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

                                        string argnew_form = localLIndex();
                                        withBlock1.Transform(ref argnew_form);
                                        t = withBlock1.CurrentForm();
                                        n = 0;
                                    }
                                    else
                                    {
                                        object argIndex104 = "残り時間";
                                        n = withBlock1.ConditionLifetime(ref argIndex104);

                                        // 後のRestで残り時間が0にならないように一旦時間を巻き戻す
                                        if (n > 0)
                                        {
                                            string argcname3 = "残り時間";
                                            string argcdata3 = "";
                                            withBlock1.AddCondition(ref argcname3, 10, cdata: ref argcdata3);
                                        }
                                    }

                                    // ユニットを復活させる
                                    t.FullRecover();
                                    t.UsedAction = 0;
                                    t.StandBy(my_unit.x, my_unit.y);
                                    t.Rest();

                                    // 残り時間を元に戻す
                                    if (n > 0)
                                    {
                                        object argIndex105 = "残り時間";
                                        t.DeleteCondition(ref argIndex105);
                                        string argcname4 = "残り時間";
                                        string argcdata4 = "";
                                        t.AddCondition(ref argcname4, n, cdata: ref argcdata4);
                                    }

                                    GUI.RedrawScreen();
                                }

                                if (!My.MyProject.Forms.frmMessage.Visible)
                                {
                                    Unit argu11 = null;
                                    Unit argu246 = null;
                                    GUI.OpenMessageForm(u1: ref argu11, u2: ref argu246);
                                }

                                string argmain_situation = "復活";
                                if (t.IsMessageDefined(ref argmain_situation))
                                {
                                    string argSituation = "復活";
                                    string argmsg_mode = "";
                                    t.PilotMessage(ref argSituation, msg_mode: ref argmsg_mode);
                                }

                                string argmain_situation3 = "復活";
                                string argsub_situation2 = "";
                                if (t.IsAnimationDefined(ref argmain_situation3, sub_situation: ref argsub_situation2))
                                {
                                    string argmain_situation1 = "復活";
                                    string argsub_situation = "";
                                    t.PlayAnimation(ref argmain_situation1, sub_situation: ref argsub_situation);
                                }
                                else
                                {
                                    string argmain_situation2 = "復活";
                                    string argsub_situation1 = "";
                                    t.SpecialEffect(ref argmain_situation2, sub_situation: ref argsub_situation1);
                                }

                                GUI.DisplaySysMessage(t.Nickname + "は復活した。");
                                break;
                            }

                        case "イベント":
                            {
                                // イベントコマンドで定義されたスペシャルパワー
                                // 対象ユニットＩＤ及び相手ユニットＩＤを設定
                                Event_Renamed.SelectedUnitForEvent = my_unit.CurrentForm();
                                Event_Renamed.SelectedTargetForEvent = withBlock1.CurrentForm();
                                // 指定されたサブルーチンを実行
                                string argexpr = "Call(" + EffectData(i) + ")";
                                Expression.GetValueAsString(ref argexpr);
                                break;
                            }
                    }
                }

                NextEffect:
                ;
            }

            // Unitが変化してしまった場合は元に戻しておく
            if (!ReferenceEquals(my_unit, p.Unit_Renamed))
            {
                my_unit.CurrentForm().MainPilot();
            }

            // ステータスの更新が必要？
            if (need_update)
            {
                t.CheckAutoHyperMode();
                t.CurrentForm().CheckAutoNormalMode();
                t.CurrentForm().Update();
                SRC.PList.UpdateSupportMod(t.CurrentForm());
            }

            ApplyRet = displayed_string;
            return ApplyRet;
        }


        // スペシャルパワーが有効なターゲットの総数を返す
        // (パイロット p が使用した場合)
        public short CountTarget(ref Pilot p)
        {
            short CountTargetRet = default;
            Unit u;
            short i;
            switch (TargetType ?? "")
            {
                case "自分":
                    {
                        if (Effective(ref p, ref p.Unit_Renamed))
                        {
                            CountTargetRet = 1;
                        }

                        break;
                    }

                case "味方":
                case "全味方":
                    {
                        foreach (Unit currentU in SRC.UList)
                        {
                            u = currentU;
                            {
                                var withBlock = u;
                                // 出撃している？
                                if (withBlock.Status_Renamed != "出撃")
                                {
                                    goto NextUnit1;
                                }

                                // 味方ユニット？
                                switch (p.Party ?? "")
                                {
                                    case "味方":
                                    case "ＮＰＣ":
                                        {
                                            if (withBlock.Party != "味方" & withBlock.Party0 != "味方" & withBlock.Party != "ＮＰＣ" & withBlock.Party0 != "ＮＰＣ")
                                            {
                                                goto NextUnit1;
                                            }

                                            break;
                                        }

                                    default:
                                        {
                                            if ((p.Party ?? "") != (withBlock.Party ?? ""))
                                            {
                                                goto NextUnit1;
                                            }

                                            break;
                                        }
                                }

                                // 効果がある？
                                if (Effective(ref p, ref u))
                                {
                                    CountTargetRet = (short)(CountTargetRet + 1);
                                }
                            }

                            NextUnit1:
                            ;
                        }

                        break;
                    }

                case "破壊味方":
                    {
                        foreach (Unit currentU1 in SRC.UList)
                        {
                            u = currentU1;
                            {
                                var withBlock1 = u;
                                // 破壊されている？
                                if (withBlock1.Status_Renamed != "破壊")
                                {
                                    goto NextUnit2;
                                }

                                // 味方ユニット？
                                if ((p.Party ?? "") != (withBlock1.Party0 ?? ""))
                                {
                                    goto NextUnit2;
                                }

                                // 効果がある？
                                if (Effective(ref p, ref u))
                                {
                                    CountTargetRet = (short)(CountTargetRet + 1);
                                }
                            }

                            NextUnit2:
                            ;
                        }

                        break;
                    }

                case "敵":
                case "全敵":
                    {
                        foreach (Unit currentU2 in SRC.UList)
                        {
                            u = currentU2;
                            {
                                var withBlock2 = u;
                                // 出撃している？
                                if (withBlock2.Status_Renamed != "出撃")
                                {
                                    goto NextUnit3;
                                }

                                // 敵ユニット？
                                switch (p.Party ?? "")
                                {
                                    case "味方":
                                    case "ＮＰＣ":
                                        {
                                            if (withBlock2.Party == "味方" & withBlock2.Party0 == "味方" | withBlock2.Party == "ＮＰＣ" & withBlock2.Party0 == "ＮＰＣ")
                                            {
                                                goto NextUnit3;
                                            }

                                            break;
                                        }

                                    default:
                                        {
                                            if ((p.Party ?? "") == (withBlock2.Party ?? ""))
                                            {
                                                goto NextUnit3;
                                            }

                                            break;
                                        }
                                }

                                // 効果がある？
                                if (Effective(ref p, ref u))
                                {
                                    CountTargetRet = (short)(CountTargetRet + 1);
                                }
                            }

                            NextUnit3:
                            ;
                        }

                        break;
                    }

                case "任意":
                case "全":
                    {
                        foreach (Unit currentU3 in SRC.UList)
                        {
                            u = currentU3;
                            // 出撃している？
                            if (u.Status_Renamed == "出撃")
                            {
                                // 効果がある？
                                if (Effective(ref p, ref u))
                                {
                                    CountTargetRet = (short)(CountTargetRet + 1);
                                }
                            }
                        }

                        break;
                    }
            }

            return CountTargetRet;
        }

        // スペシャルパワーのアニメーションを表示
        public bool PlayAnimation()
        {
            bool PlayAnimationRet = default;
            string anime;
            string[] animes;
            short anime_head;
            var buf = default(string);
            var ret = default(double);
            short i, j;
            string expr;
            var wait_time = default(int);
            int prev_obj_color;
            int prev_obj_fill_color;
            int prev_obj_fill_style;
            int prev_obj_draw_width;
            string prev_obj_draw_option;
            if (!SRC.SpecialPowerAnimation)
            {
                return PlayAnimationRet;
            }

            if (Animation == "-")
            {
                PlayAnimationRet = true;
                return PlayAnimationRet;
            }

            // アニメ指定がなされていない場合はアニメ表示用サブルーチンが見つらなければそのまま終了
            if ((Animation ?? "") == (Name ?? ""))
            {
                string arglname = "ＳＰアニメ_" + Animation;
                if (Event_Renamed.FindNormalLabel(ref arglname) == 0)
                {
                    if (Name != "自爆" & Name != "祈り")
                    {
                        object argIndex1 = "特殊効果 " + Name;
                        if (Event_Renamed.IsLabelDefined(ref argIndex1))
                        {
                            Event_Renamed.HandleEvent("特殊効果", Name);
                            PlayAnimationRet = true;
                        }
                    }

                    return PlayAnimationRet;
                }
            }

            // 右クリック中はアニメ表示をスキップ
            if (GUI.IsRButtonPressed())
            {
                PlayAnimationRet = true;
                return PlayAnimationRet;
            }

            // オブジェクト色等を記録しておく
            prev_obj_color = Event_Renamed.ObjColor;
            prev_obj_fill_color = Event_Renamed.ObjFillColor;
            prev_obj_fill_style = Event_Renamed.ObjFillStyle;
            prev_obj_draw_width = Event_Renamed.ObjDrawWidth;
            prev_obj_draw_option = Event_Renamed.ObjDrawOption;

            // オブジェクト色等をデフォルトに戻す
            Event_Renamed.ObjColor = ColorTranslator.ToOle(Color.White);
            Event_Renamed.ObjFillColor = ColorTranslator.ToOle(Color.White);
            // UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
            Event_Renamed.ObjFillStyle = vbFSTransparent;
            Event_Renamed.ObjDrawWidth = 1;
            Event_Renamed.ObjDrawOption = "";

            // アニメ指定を分割
            animes = new string[2];
            anime_head = 1;
            var loopTo = (short)Strings.Len(Animation);
            for (i = 1; i <= loopTo; i++)
            {
                if (Strings.Mid(Animation, i, 1) == ";")
                {
                    animes[Information.UBound(animes)] = Strings.Mid(Animation, anime_head, i - anime_head);
                    Array.Resize(ref animes, Information.UBound(animes) + 1 + 1);
                    anime_head = (short)(i + 1);
                }
            }

            animes[Information.UBound(animes)] = Strings.Mid(Animation, anime_head);
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 59356


            Input:

                    On Error GoTo ErrorHandler

             */
            var loopTo1 = (short)Information.UBound(animes);
            for (i = 1; i <= loopTo1; i++)
            {
                anime = animes[i];

                // 式評価
                Expression.FormatMessage(ref anime);

                // 画面クリア？
                if (Strings.LCase(anime) == "clear")
                {
                    GUI.ClearPicture();
                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    GUI.MainForm.picMain(0).Refresh();
                    goto NextAnime;
                }

                // 戦闘アニメ以外の特殊効果
                switch (Strings.LCase(Strings.Right(GeneralLib.LIndex(ref anime, 1), 4)) ?? "")
                {
                    case ".wav":
                    case ".mp3":
                        {
                            // 効果音
                            Sound.PlayWave(ref anime);
                            if (wait_time > 0)
                            {
                                GUI.Sleep(wait_time);
                                wait_time = 0;
                            }

                            goto NextAnime;
                            break;
                        }

                    case ".bmp":
                    case ".jpg":
                    case ".gif":
                    case ".png":
                        {
                            // カットインの表示
                            if (wait_time > 0)
                            {
                                anime = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(wait_time / 100d) + ";" + anime;
                                wait_time = 0;
                            }

                            string argpname = "";
                            string argmsg_mode = "";
                            GUI.DisplayBattleMessage(ref argpname, anime, msg_mode: ref argmsg_mode);
                            goto NextAnime;
                            break;
                        }
                }

                switch (Strings.LCase(GeneralLib.LIndex(ref anime, 1)) ?? "")
                {
                    case "line":
                    case "circle":
                    case "arc":
                    case "oval":
                    case "color":
                    case "fillcolor":
                    case "fillstyle":
                    case "drawwidth":
                        {
                            // 画面処理コマンド
                            if (wait_time > 0)
                            {
                                anime = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(wait_time / 100d) + ";" + anime;
                                wait_time = 0;
                            }

                            string argpname1 = "";
                            string argmsg_mode1 = "";
                            GUI.DisplayBattleMessage(ref argpname1, anime, msg_mode: ref argmsg_mode1);
                            goto NextAnime;
                            break;
                        }

                    case "center":
                        {
                            // 指定したユニットを中央表示
                            string argexpr = GeneralLib.ListIndex(ref anime, 2);
                            buf = Expression.GetValueAsString(ref argexpr);
                            object argIndex3 = buf;
                            if (SRC.UList.IsDefined(ref argIndex3))
                            {
                                object argIndex2 = buf;
                                {
                                    var withBlock = SRC.UList.Item(ref argIndex2);
                                    GUI.Center(withBlock.x, withBlock.y);
                                    GUI.RedrawScreen();
                                }
                            }

                            goto NextAnime;
                            break;
                        }
                }

                // ウェイト？
                if (Information.IsNumeric(anime))
                {
                    wait_time = (int)(100d * Conversions.ToDouble(anime));
                    goto NextAnime;
                }

                // サブルーチンの呼び出しが確定

                // 戦闘アニメ再生前にウェイトを入れる？
                if (wait_time > 0)
                {
                    GUI.Sleep(wait_time);
                    wait_time = 0;
                }

                // サブルーチン呼び出しのための式を作成
                if (Strings.Left(anime, 1) == "@")
                {
                    expr = Strings.Mid(GeneralLib.ListIndex(ref anime, 1), 2) + "(";
                    // 引数の構築
                    var loopTo2 = GeneralLib.ListLength(ref anime);
                    for (j = 2; j <= loopTo2; j++)
                    {
                        if (j > 2)
                        {
                            expr = expr + ",";
                        }

                        expr = expr + GeneralLib.ListIndex(ref anime, j);
                    }

                    expr = expr + ")";
                }
                else if (Commands.SelectedTarget is object)
                {
                    expr = "ＳＰアニメ_" + anime + "(" + Commands.SelectedUnit.ID + "," + Commands.SelectedTarget.ID + ")";
                }
                else
                {
                    expr = "ＳＰアニメ_" + anime + "(" + Commands.SelectedUnit.ID + ",-)";
                }

                // 画像描画が行われたかどうかの判定のためにフラグを初期化
                GUI.IsPictureDrawn = false;

                // アニメ再生
                Event_Renamed.SaveBasePoint();
                Expression.CallFunction(ref expr, ref Expression.ValueType.StringType, ref buf, ref ret);
                Event_Renamed.RestoreBasePoint();

                // 画像を消去しておく
                if (GUI.IsPictureDrawn & Strings.LCase(buf) != "keep")
                {
                    GUI.ClearPicture();
                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    GUI.MainForm.picMain(0).Refresh();
                }

                NextAnime:
                ;
            }

            // 戦闘アニメ再生後にウェイトを入れる？
            if (wait_time > 0)
            {
                GUI.Sleep(wait_time);
                wait_time = 0;
            }

            // メッセージウィンドウを閉じる
            GUI.CloseMessageForm();

            // オブジェクト色等を元に戻す
            Event_Renamed.ObjColor = prev_obj_color;
            Event_Renamed.ObjFillColor = prev_obj_fill_color;
            Event_Renamed.ObjFillStyle = prev_obj_fill_style;
            Event_Renamed.ObjDrawWidth = prev_obj_draw_width;
            Event_Renamed.ObjDrawOption = prev_obj_draw_option;
            PlayAnimationRet = true;
            return PlayAnimationRet;
            ErrorHandler:
            ;


            // アニメ再生中に発生したエラーの処理
            if (Strings.Len(Event_Renamed.EventErrorMessage) > 0)
            {
                Event_Renamed.DisplayEventErrorMessage(Event_Renamed.CurrentLineNum, Event_Renamed.EventErrorMessage);
                Event_Renamed.EventErrorMessage = "";
            }
            else
            {
                Event_Renamed.DisplayEventErrorMessage(Event_Renamed.CurrentLineNum, "");
            }
        }
    }
}