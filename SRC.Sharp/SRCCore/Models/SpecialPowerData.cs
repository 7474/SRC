// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Lib;
using SRCCore.Pilots;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Models
{
    // スペシャルパワーデータのクラス
    public class SpecialPowerData
    {
        // スペシャルパワー名
        public string Name;
        // スペシャルパワー名の読み仮名
        public string KanaName;
        // スペシャルパワー名の短縮形
        public string ShortName;
        // 消費ＳＰ
        public int SPConsumption;
        // 対象
        public string TargetType;
        // 効果時間
        public string Duration;
        // 適用条件
        public string NecessaryCondition;
        // アニメ
        public string Animation;

        // 効果
        private IList<SpecialPowerEffect> effects = new List<SpecialPowerEffect>();
        public IList<SpecialPowerEffect> Effects => effects;

        // 解説
        public string Comment;

        private SRC SRC;
        public SpecialPowerData(SRC src)
        {
            SRC = src;
        }

        // スペシャルパワーに効果を追加
        public void SetEffect(string elist)
        {
            var list = GeneralLib.ToList(elist.Trim());
            foreach (var item in list)
            {
                var buf = item;
                string elevel, etype, edata;
                var j = Strings.InStr(buf, "Lv");
                var k = Strings.InStr(buf, "=");
                string strEffectType;
                double dblEffectLevel = Constants.DEFAULT_LEVEL;
                string strEffectData = null;
                if (j > 0 && (k == 0 | j < k))
                {
                    // レベル指定のある効果(データ指定を伴うものを含む)
                    strEffectType = Strings.Left(buf, j - 1);
                    if (k > 0)
                    {
                        // データ指定を伴うもの
                        dblEffectLevel = Conversions.ToDouble(Strings.Mid(buf, j + 2, k - (j + 2)));
                        buf = Strings.Mid(buf, k + 1);
                        if (Strings.Left(buf, 1) == "\"")
                        {
                            buf = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
                        }

                        j = Strings.InStr(buf, "Lv");
                        k = Strings.InStr(buf, "=");
                        if (j > 0 && (k == 0 | j < k))
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

                        if (Name == "付加" && string.IsNullOrEmpty(elevel))
                        {
                            elevel = SrcFormatter.Format(Constants.DEFAULT_LEVEL);
                        }

                        strEffectData = Strings.Trim(etype + " " + elevel + " " + edata);
                    }
                    else
                    {
                        // データ指定を伴わないもの
                        dblEffectLevel = Conversions.ToDouble(Strings.Mid(buf, j + 2));
                    }
                }
                else if (k > 0)
                {
                    // データ指定を伴う効果
                    strEffectType = Strings.Left(buf, k - 1);
                    buf = Strings.Mid(buf, k + 1);
                    if (Strings.Asc(buf) == 34) // "
                    {
                        buf = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
                    }

                    j = Strings.InStr(buf, "Lv");
                    k = Strings.InStr(buf, "=");
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

                    if (Name == "付加" && string.IsNullOrEmpty(elevel))
                    {
                        elevel = SrcFormatter.Format(Constants.DEFAULT_LEVEL);
                    }

                    strEffectData = Strings.Trim(etype + " " + elevel + " " + edata);
                }
                else
                {
                    // 効果名のみ
                    strEffectType = buf;
                }

                effects.Add(new SpecialPowerEffect
                {
                    strEffectType = strEffectType,
                    dblEffectLevel = dblEffectLevel,
                    strEffectData = strEffectData,
                });
            }
        }

        // 特殊効果の個数
        public int CountEffect()
        {
            return effects.Count;
        }

        //        // idx番目の特殊効果タイプ
        //        public string EffectType(int idx)
        //        {
        //            string EffectTypeRet = default;
        //            EffectTypeRet = strEffectType[idx];
        //            return EffectTypeRet;
        //        }

        //        // idx番目の特殊効果レベル
        //        public double EffectLevel(int idx)
        //        {
        //            double EffectLevelRet = default;
        //            EffectLevelRet = dblEffectLevel[idx];
        //            return EffectLevelRet;
        //        }

        //        // idx番目の特殊効果データ
        //        public string EffectData(int idx)
        //        {
        //            string EffectDataRet = default;
        //            EffectDataRet = strEffectData[idx];
        //            return EffectDataRet;
        //        }

        // 特殊効果 ename を持っているか
        public bool IsEffectAvailable(string ename)
        {
            return Effects.Any(x => x.strEffectType == ename
                || (x.strEffectType == "スペシャルパワー" && SRC.SPDList.Item(x.strEffectData).IsEffectAvailable(ename)));
        }

        // スペシャルパワーがその時点で役に立つかどうか
        // (パイロット p が使用した場合)
        public bool Useful(Pilot p)
        {
            return true;
            // TODO Impl
            //bool UsefulRet = default;
            //Unit u;
            //int i;
            //switch (TargetType ?? "")
            //{
            //    case "自分":
            //        {
            //            UsefulRet = Effective(p, p.Unit);
            //            return UsefulRet;
            //        }

            //    case "味方":
            //    case "全味方":
            //        {
            //            foreach (Unit currentU in SRC.UList)
            //            {
            //                u = currentU;
            //                {
            //                    var withBlock = u;
            //                    // 出撃している？
            //                    if (withBlock.Status != "出撃")
            //                    {
            //                        goto NextUnit1;
            //                    }

            //                    // 味方ユニット？
            //                    switch (p.Party ?? "")
            //                    {
            //                        case "味方":
            //                        case "ＮＰＣ":
            //                            {
            //                                if (withBlock.Party != "味方" && withBlock.Party0 != "味方" && withBlock.Party != "ＮＰＣ" && withBlock.Party0 != "ＮＰＣ")
            //                                {
            //                                    goto NextUnit1;
            //                                }

            //                                break;
            //                            }

            //                        default:
            //                            {
            //                                if ((p.Party ?? "") != (withBlock.Party ?? ""))
            //                                {
            //                                    goto NextUnit1;
            //                                }

            //                                break;
            //                            }
            //                    }

            //                    // 効果がある？
            //                    if (Effective(p, u))
            //                    {
            //                        UsefulRet = true;
            //                        return UsefulRet;
            //                    }
            //                }

            //            NextUnit1:
            //                ;
            //            }

            //            break;
            //        }

            //    case "破壊味方":
            //        {
            //            foreach (Unit currentU1 in SRC.UList)
            //            {
            //                u = currentU1;
            //                {
            //                    var withBlock1 = u;
            //                    // 破壊されている？
            //                    if (withBlock1.Status != "破壊")
            //                    {
            //                        goto NextUnit2;
            //                    }

            //                    // 味方ユニット？
            //                    if ((p.Party ?? "") != (withBlock1.Party0 ?? ""))
            //                    {
            //                        goto NextUnit2;
            //                    }

            //                    // 効果がある？
            //                    if (Effective(p, u))
            //                    {
            //                        UsefulRet = true;
            //                        return UsefulRet;
            //                    }
            //                }

            //            NextUnit2:
            //                ;
            //            }

            //            break;
            //        }

            //    case "敵":
            //    case "全敵":
            //        {
            //            foreach (Unit currentU2 in SRC.UList)
            //            {
            //                u = currentU2;
            //                {
            //                    var withBlock2 = u;
            //                    // 出撃している？
            //                    if (withBlock2.Status != "出撃")
            //                    {
            //                        goto NextUnit3;
            //                    }

            //                    // 敵ユニット？
            //                    switch (p.Party ?? "")
            //                    {
            //                        case "味方":
            //                        case "ＮＰＣ":
            //                            {
            //                                if (withBlock2.Party == "味方" && withBlock2.Party0 == "味方" | withBlock2.Party == "ＮＰＣ" && withBlock2.Party0 == "ＮＰＣ")
            //                                {
            //                                    goto NextUnit3;
            //                                }

            //                                break;
            //                            }

            //                        default:
            //                            {
            //                                if ((p.Party ?? "") == (withBlock2.Party ?? ""))
            //                                {
            //                                    goto NextUnit3;
            //                                }

            //                                break;
            //                            }
            //                    }

            //                    // 効果がある？
            //                    if (Effective(p, u))
            //                    {
            //                        UsefulRet = true;
            //                        return UsefulRet;
            //                    }
            //                }

            //            NextUnit3:
            //                ;
            //            }

            //            break;
            //        }

            //    case "任意":
            //    case "全":
            //        {
            //            foreach (Unit currentU3 in SRC.UList)
            //            {
            //                u = currentU3;
            //                // 出撃している？
            //                if (u.Status == "出撃")
            //                {
            //                    // 効果がある？
            //                    if (Effective(p, u))
            //                    {
            //                        UsefulRet = true;
            //                        return UsefulRet;
            //                    }
            //                }
            //            }

            //            break;
            //        }
            //}

            //return UsefulRet;
        }

        // スペシャルパワーがユニット t に対して効果があるかどうか
        // (パイロット p が使用した場合)
        public bool Effective(Pilot p, Unit t)
        {
            return true;
            // TODO Impl
            //    bool EffectiveRet = default;
            //    int i, j;
            //    string ncond;
            //    Unit my_unit;

            //    // 同じ追加パイロットを持つユニットが複数いる場合、パイロットのUnitが
            //    // 変化してしまうことがあるため、元のUnitを記録しておく
            //    my_unit = p.Unit;
            //    // 適用条件を満たしている？
            //    var loopTo = GeneralLib.LLength(NecessaryCondition);
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        ncond = GeneralLib.LIndex(NecessaryCondition, i);
            //        switch (ncond ?? "")
            //        {
            //            case "技量":
            //                {
            //                    if (p.Technique < t.MainPilot().Technique)
            //                    {
            //                        goto ExitFunc;
            //                    }

            //                    break;
            //                }

            //            case "非ボス":
            //                {
            //                    if (t.BossRank >= 0)
            //                    {
            //                        goto ExitFunc;
            //                    }

            //                    break;
            //                }

            //            case "支援":
            //                {
            //                    if (ReferenceEquals(my_unit, t))
            //                    {
            //                        goto ExitFunc;
            //                    }

            //                    break;
            //                }

            //            case "隣接":
            //                {
            //                    if (Math.Abs((my_unit.x - t.x)) + Math.Abs((my_unit.y - t.y)) != 1)
            //                    {
            //                        goto ExitFunc;
            //                    }

            //                    break;
            //                }

            //            default:
            //                {
            //                    if (Strings.InStr(ncond, "射程Lv") == 1)
            //                    {
            //                        int localStrToLng() { string argexpr = Strings.Mid(ncond, 5); var ret = GeneralLib.StrToLng(argexpr); return ret; }

            //                        if (Math.Abs((my_unit.x - t.x)) + Math.Abs((my_unit.y - t.y)) > localStrToLng())
            //                        {
            //                            goto ExitFunc;
            //                        }
            //                    }

            //                    break;
            //                }
            //        }

            //        // Unitが変化してしまった場合は元に戻しておく
            //        if (!ReferenceEquals(my_unit, p.Unit))
            //        {
            //            my_unit.MainPilot();
            //        }
            //    }

            //    // 無効化されている？
            //    switch (TargetType ?? "")
            //    {
            //        case "敵":
            //        case "全敵":
            //        case "任意":
            //        case "全":
            //            {
            //                if (t.IsConditionSatisfied("スペシャルパワー無効化") | t.IsConditionSatisfied("精神コマンド無効化"))
            //                {
            //                    goto ExitFunc;
            //                }

            //                break;
            //            }
            //    }

            //    // 持続効果があるものは同じスペシャルパワーが既に適用されていなければ
            //    // 効果があるとみなす
            //    if (Duration != "即効")
            //    {
            //        if (!t.IsSpecialPowerInEffect(Name))
            //        {
            //            EffectiveRet = true;
            //        }

            //        // ただしみがわりは自分自身には使えないのでチェックしておく
            //        if (EffectType(1) == "みがわり")
            //        {
            //            if (ReferenceEquals(my_unit, t))
            //            {
            //                EffectiveRet = false;
            //                goto ExitFunc;
            //            }
            //        }

            //        goto ExitFunc;
            //    }

            //    // 個々の効果に関して有効かどうか判定
            //    var loopTo1 = CountEffect();
            //    for (i = 1; i <= loopTo1; i++)
            //    {
            //        switch (EffectType(i) ?? "")
            //        {
            //            case "ＨＰ回復":
            //            case "ＨＰ増加":
            //                {
            //                    if (effect.dblEffectLevel < 0d)
            //                    {
            //                        EffectiveRet = true;
            //                        goto ExitFunc;
            //                    }

            //                    if (t.IsConditionSatisfied("ゾンビ"))
            //                    {
            //                        goto NextEffect;
            //                    }

            //                    if (t.HP < t.MaxHP)
            //                    {
            //                        EffectiveRet = true;
            //                        goto ExitFunc;
            //                    }

            //                    break;
            //                }

            //            case "ＥＮ回復":
            //            case "ＥＮ増加":
            //                {
            //                    if (effect.dblEffectLevel < 0d)
            //                    {
            //                        EffectiveRet = true;
            //                        goto ExitFunc;
            //                    }

            //                    if (t.IsConditionSatisfied("ゾンビ"))
            //                    {
            //                        goto NextEffect;
            //                    }

            //                    if (t.EN < t.MaxEN)
            //                    {
            //                        EffectiveRet = true;
            //                        goto ExitFunc;
            //                    }

            //                    break;
            //                }

            //            case "霊力回復":
            //            case "霊力増加":
            //                {
            //                    if (effect.dblEffectLevel < 0d)
            //                    {
            //                        EffectiveRet = true;
            //                        goto ExitFunc;
            //                    }

            //                    if (t.IsConditionSatisfied("ゾンビ"))
            //                    {
            //                        goto NextEffect;
            //                    }

            //                    if (t.MainPilot().Plana < t.MainPilot().MaxPlana())
            //                    {
            //                        EffectiveRet = true;
            //                        goto ExitFunc;
            //                    }

            //                    break;
            //                }

            //            case "ＳＰ回復":
            //                {
            //                    if (effect.dblEffectLevel < 0d)
            //                    {
            //                        EffectiveRet = true;
            //                        goto ExitFunc;
            //                    }

            //                    if (t.IsConditionSatisfied("ゾンビ"))
            //                    {
            //                        goto NextEffect;
            //                    }

            //                    if (t.MainPilot().SP < t.MainPilot().MaxSP)
            //                    {
            //                        EffectiveRet = true;
            //                        goto ExitFunc;
            //                    }

            //                    var loopTo2 = t.CountPilot();
            //                    for (j = 2; j <= loopTo2; j++)
            //                    {
            //                        Pilot localPilot() { object argIndex1 = j; var ret = t.Pilot(argIndex1); return ret; }

            //                        Pilot localPilot1() { object argIndex1 = j; var ret = t.Pilot(argIndex1); return ret; }

            //                        if (localPilot().SP < localPilot1().MaxSP)
            //                        {
            //                            EffectiveRet = true;
            //                            goto ExitFunc;
            //                        }
            //                    }

            //                    var loopTo3 = t.CountSupport();
            //                    for (j = 1; j <= loopTo3; j++)
            //                    {
            //                        Pilot localSupport() { object argIndex1 = j; var ret = t.Support(argIndex1); return ret; }

            //                        Pilot localSupport1() { object argIndex1 = j; var ret = t.Support(argIndex1); return ret; }

            //                        if (localSupport().SP < localSupport1().MaxSP)
            //                        {
            //                            EffectiveRet = true;
            //                            goto ExitFunc;
            //                        }
            //                    }

            //                    if (t.IsFeatureAvailable("追加サポート"))
            //                    {
            //                        if (t.AdditionalSupport().SP < t.AdditionalSupport().MaxSP)
            //                        {
            //                            EffectiveRet = true;
            //                            goto ExitFunc;
            //                        }
            //                    }

            //                    break;
            //                }

            //            case "状態回復":
            //                {
            //                    if (t.ConditionLifetime("攻撃不能") > 0 | t.ConditionLifetime("移動不能") > 0 | t.ConditionLifetime("装甲劣化") > 0 | t.ConditionLifetime("混乱") > 0 | t.ConditionLifetime("魅了") > 0 | t.ConditionLifetime("憑依") > 0 | t.ConditionLifetime("石化") > 0 | t.ConditionLifetime("凍結") > 0 | t.ConditionLifetime("麻痺") > 0 | t.ConditionLifetime("睡眠") > 0 | t.ConditionLifetime("毒") > 0 | t.ConditionLifetime("盲目") > 0 | t.ConditionLifetime("撹乱") > 0 | t.ConditionLifetime("恐怖") > 0 | t.ConditionLifetime("沈黙") > 0 | t.ConditionLifetime("ゾンビ") > 0 | t.ConditionLifetime("回復不能") > 0 | t.ConditionLifetime("オーラ使用不能") > 0 | t.ConditionLifetime("超能力使用不能") > 0 | t.ConditionLifetime("同調率使用不能") > 0 | t.ConditionLifetime("超感覚使用不能") > 0 | t.ConditionLifetime("知覚強化使用不能") > 0 | t.ConditionLifetime("霊力使用不能") > 0 | t.ConditionLifetime("術使用不能") > 0 | t.ConditionLifetime("技使用不能") > 0)
            //                    {
            //                        EffectiveRet = true;
            //                        goto ExitFunc;
            //                    }
            //                    else
            //                    {
            //                        var loopTo4 = t.CountCondition();
            //                        for (j = 1; j <= loopTo4; j++)
            //                        {
            //                            string localCondition2() { object argIndex1 = j; var ret = t.Condition(argIndex1); return ret; }

            //                            if (Strings.Len(localCondition2()) > 6)
            //                            {
            //                                string localCondition1() { object argIndex1 = j; var ret = t.Condition(argIndex1); return ret; }

            //                                if (Strings.Right(localCondition1(), 6) == "属性使用不能")
            //                                {
            //                                    string localCondition() { object argIndex1 = j; var ret = t.Condition(argIndex1); return ret; }

            //                                    if (t.ConditionLifetime(localCondition()) > 0)
            //                                    {
            //                                        EffectiveRet = true;
            //                                        goto ExitFunc;
            //                                    }
            //                                }
            //                            }
            //                        }
            //                    }

            //                    break;
            //                }

            //            case "装填":
            //                {
            //                    var loopTo5 = t.CountWeapon();
            //                    for (j = 1; j <= loopTo5; j++)
            //                    {
            //                        if (t.Bullet(j) < t.MaxBullet(j))
            //                        {
            //                            EffectiveRet = true;
            //                            goto ExitFunc;
            //                        }
            //                    }

            //                    break;
            //                }

            //            case "行動数回復":
            //                {
            //                    if (t.Action == 0 && t.MaxAction() > 0)
            //                    {
            //                        EffectiveRet = true;
            //                        goto ExitFunc;
            //                    }

            //                    break;
            //                }

            //            case "行動数増加":
            //                {
            //                    if (effect.dblEffectLevel < 0d)
            //                    {
            //                        EffectiveRet = true;
            //                        goto ExitFunc;
            //                    }

            //                    if (t.Action < 3 && t.MaxAction() > 0)
            //                    {
            //                        EffectiveRet = true;
            //                        goto ExitFunc;
            //                    }

            //                    break;
            //                }

            //            case "スペシャルパワー":
            //            case "精神コマンド":
            //                {
            //                    bool localIsSpecialPowerInEffect() { string argsname = EffectData(i); var ret = t.IsSpecialPowerInEffect(argsname); return ret; }

            //                    if (!localIsSpecialPowerInEffect())
            //                    {
            //                        EffectiveRet = true;
            //                        goto ExitFunc;
            //                    }

            //                    break;
            //                }

            //            case "気力増加":
            //                {
            //                    if (t.MainPilot().Personality != "機械" && t.MainPilot().Morale < t.MainPilot().MaxMorale)
            //                    {
            //                        EffectiveRet = true;
            //                        goto ExitFunc;
            //                    }

            //                    var loopTo6 = t.CountPilot();
            //                    for (j = 2; j <= loopTo6; j++)
            //                    {
            //                        Pilot localPilot2() { object argIndex1 = j; var ret = t.Pilot(argIndex1); return ret; }

            //                        Pilot localPilot3() { object argIndex1 = j; var ret = t.Pilot(argIndex1); return ret; }

            //                        if (localPilot2().Personality != "機械" && localPilot3().Morale < t.MainPilot().MaxMorale)
            //                        {
            //                            EffectiveRet = true;
            //                            goto ExitFunc;
            //                        }
            //                    }

            //                    break;
            //                }

            //            case "気力低下":
            //                {
            //                    if (t.MainPilot().Personality == "機械")
            //                    {
            //                        goto NextEffect;
            //                    }

            //                    if (t.MainPilot().Morale > t.MainPilot().MinMorale)
            //                    {
            //                        EffectiveRet = true;
            //                        goto ExitFunc;
            //                    }

            //                    break;
            //                }

            //            case "ランダムダメージ":
            //            case "ＨＰ減少":
            //            case "ＥＮ減少":
            //                {
            //                    if (!t.IsConditionSatisfied("無敵"))
            //                    {
            //                        EffectiveRet = true;
            //                        goto ExitFunc;
            //                    }

            //                    break;
            //                }

            //            case var @case when @case == "気力増加":
            //            case "自爆":
            //            case "復活":
            //            case "偵察":
            //            case "味方スペシャルパワー実行":
            //            case "イベント":
            //                {
            //                    EffectiveRet = true;
            //                    goto ExitFunc;
            //                    break;
            //                }
            //        }

            //    NextEffect:
            //        ;
            //    }

            //ExitFunc:
            //    ;


            //    // Unitが変化してしまった場合は元に戻しておく
            //    if (!ReferenceEquals(my_unit, p.Unit))
            //    {
            //        my_unit.MainPilot();
            //    }

            //    return EffectiveRet;
        }


        // スペシャルパワーを使用する
        // (パイロット p が使用した場合)
        public void Execute(Pilot p, bool is_event = false)
        {
            Unit u;
            int i, j;
            switch (TargetType ?? "")
            {
                case "自分":
                    if (Apply(p, p.Unit, is_event) && !is_event)
                    {
                        SRC.GUI.Sleep(300);
                    }
                    break;

                case "全味方":
                    for (i = 1; i <= SRC.Map.MapWidth; i++)
                    {
                        for (j = 1; j <= SRC.Map.MapHeight; j++)
                        {
                            u = SRC.Map.MapDataForUnit[i, j];
                            if (u is null)
                            {
                                continue;
                            }

                            // 味方ユニット？
                            switch (p.Party ?? "")
                            {
                                case "味方":
                                case "ＮＰＣ":
                                    if (u.Party != "味方" && u.Party0 != "味方" && u.Party != "ＮＰＣ" && u.Party0 != "ＮＰＣ")
                                    {
                                        continue;
                                    }
                                    break;

                                default:
                                    if ((p.Party ?? "") != (u.Party ?? ""))
                                    {
                                        continue;
                                    }
                                    break;
                            }
                            Apply(p, u, is_event);
                        }

                        if (!is_event)
                        {
                            SRC.GUI.Sleep(300);
                        }
                    }
                    break;

                case "全敵":
                    for (i = 1; i <= SRC.Map.MapWidth; i++)
                    {
                        for (j = 1; j <= SRC.Map.MapHeight; j++)
                        {
                            u = SRC.Map.MapDataForUnit[i, j];
                            if (u is null)
                            {
                                continue;
                            }

                            // 敵ユニット？
                            switch (p.Party ?? "")
                            {
                                case "味方":
                                case "ＮＰＣ":
                                    if (u.Party == "味方" | u.Party == "ＮＰＣ")
                                    {
                                        continue;
                                    }
                                    break;

                                default:
                                    if ((p.Party ?? "") == (u.Party ?? ""))
                                    {
                                        continue;
                                    }
                                    break;
                            }
                            Apply(p, u, is_event);
                        }
                    }
                    if (!is_event)
                    {
                        SRC.GUI.Sleep(300);
                    }
                    break;

                case "全":
                    for (i = 1; i <= SRC.Map.MapWidth; i++)
                    {
                        for (j = 1; j <= SRC.Map.MapHeight; j++)
                        {
                            u = SRC.Map.MapDataForUnit[i, j];
                            if (u is object)
                            {
                                Apply(p, u, is_event);
                            }
                        }
                    }
                    if (!is_event)
                    {
                        SRC.GUI.Sleep(300);
                    }
                    break;

                default:
                    if (Apply(p, SRC.Commands.SelectedTarget, is_event) && !is_event)
                    {
                        SRC.GUI.Sleep(300);
                    }
                    break;
            }

            if (!is_event)
            {
                SRC.GUI.CloseMessageForm();
                SRC.GUI.RedrawScreen();
            }
        }

        // スペシャルパワーをユニット t に対して適用
        // (パイロット p が使用)
        // 実行後にウェイトが必要かどうかを返す
        public bool Apply(Pilot p, Unit t, bool is_event = false, bool as_instant = false)
        {
            //            bool ApplyRet = default;
            //            int j, i, n;
            //            int tmp;
            //            bool need_update = default, is_invalid = default, displayed_string = default;
            //            string msg = default, ncond;
            //            Unit my_unit;

            var is_invalid = false;

            // 同じ追加パイロットを持つユニットが複数いる場合、パイロットのUnitが
            // 変化してしまうことがあるため、元のUnitを記録しておく
            var my_unit = p.Unit;
            {
                // 適用条件を満たしている？
                foreach (var ncond in GeneralLib.ToL(NecessaryCondition))
                {
                    switch (ncond ?? "")
                    {
                        case "技量":
                            if (p.Technique < t.MainPilot().Technique)
                            {
                                is_invalid = true;
                            }
                            break;

                        case "非ボス":
                            if (t.BossRank >= 0)
                            {
                                is_invalid = true;
                            }
                            break;

                        case "支援":
                            if (ReferenceEquals(my_unit, t))
                            {
                                is_invalid = true;
                            }
                            break;

                        case "隣接":
                            if (Math.Abs((my_unit.x - t.x)) + Math.Abs((my_unit.y - t.y)) != 1)
                            {
                                is_invalid = true;
                            }
                            break;

                        default:
                            if (Strings.InStr(ncond, "射程Lv") == 1)
                            {
                                int localStrToLng() { string argexpr = Strings.Mid(ncond, 5); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                                if (Math.Abs((my_unit.x - t.x)) + Math.Abs((my_unit.y - t.y)) > localStrToLng())
                                {
                                    is_invalid = true;
                                }
                            }
                            break;
                    }

                    // Unitが変化してしまった場合は元に戻しておく
                    if
                        (!ReferenceEquals(my_unit, p.Unit))
                    {
                        my_unit.CurrentForm().MainPilot();
                    }
                }

                // 無効化されている？
                switch (TargetType ?? "")
                {
                    case "敵":
                    case "全敵":
                        if (t.IsConditionSatisfied("スペシャルパワー無効"))
                        {
                            is_invalid = true;
                        }
                        break;
                }

                // スペシャルパワーが適用可能？
                if (is_invalid)
                {
                    return false;
                }

                // 持続効果がある場合は単にスペシャルパワーの効果を付加するだけでよい
                if (Duration != "即効" && !as_instant)
                {
                    t.MakeSpecialPowerInEffect(Name, my_unit.MainPilot().ID);
                    return false;
                }
            }

            // これ以降は持続効果が即効であるスペシャルパワーの処理

            // 個々の効果を適用
            //var loopTo1 = CountEffect();
            //for (i = 1; i <= loopTo1; i++)
            foreach (var effect in Effects)
            {
                switch (effect.strEffectType)
                {
                    case "ＨＰ回復":
                    case "ＨＰ増加":
                        {
                            // 効果が適用可能かどうか判定
                            if (effect.dblEffectLevel > 0d)
                            {
                                if (t.IsConditionSatisfied("ゾンビ"))
                                {
                                    goto NextEffect;
                                }

                                if (t.HP == t.MaxHP)
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
                                        GUI.OpenMessageForm(Commands.SelectedUnit, u2: null);
                                    }
                                    else
                                    {
                                        GUI.UpdateMessageForm(Commands.SelectedUnit, u2: null);
                                    }
                                }
                                else if (!My.MyProject.Forms.frmMessage.Visible)
                                {
                                    GUI.OpenMessageForm(t, Commands.SelectedUnit);
                                }
                                else
                                {
                                    GUI.UpdateMessageForm(t, Commands.SelectedUnit);
                                }

                                GUI.Sleep(150);
                            }

                            // ＨＰを回復させる
                            tmp = t.HP;
                            if (EffectType(i) == "ＨＰ増加")
                            {
                                t.HP = (t.HP + 1000d * effect.dblEffectLevel);
                            }
                            else
                            {
                                t.RecoverHP(10d * effect.dblEffectLevel);
                            }

                            if (!is_event)
                            {
                                if (!displayed_string)
                                {
                                    if (effect.dblEffectLevel >= 0d)
                                    {
                                        GUI.DrawSysString(t.x, t.y, "+" + SrcFormatter.Format(t.HP - tmp));
                                    }
                                    else
                                    {
                                        GUI.DrawSysString(t.x, t.y, SrcFormatter.Format(t.HP - tmp));
                                    }
                                }

                                displayed_string = true;
                                if (ReferenceEquals(t, Commands.SelectedUnit))
                                {
                                    GUI.UpdateMessageForm(Commands.SelectedUnit, u2: null);
                                }
                                else
                                {
                                    GUI.UpdateMessageForm(t, Commands.SelectedUnit);
                                }

                                if (effect.dblEffectLevel >= 0d)
                                {
                                    GUI.DisplaySysMessage(t.Nickname + "の" + Expression.Term("ＨＰ", t) + "が" + SrcFormatter.Format(t.HP - tmp) + "回復した。");
                                }
                                else
                                {
                                    GUI.DisplaySysMessage(t.Nickname + "の" + Expression.Term("ＨＰ", t) + "が" + SrcFormatter.Format(tmp - t.HP) + "減少した。");
                                }
                            }

                            need_update = true;
                            break;
                        }

                    case "ＥＮ回復":
                    case "ＥＮ増加":
                        {
                            // 効果が適用可能かどうか判定
                            if (effect.dblEffectLevel > 0d)
                            {
                                if (t.IsConditionSatisfied("ゾンビ"))
                                {
                                    goto NextEffect;
                                }

                                if (t.EN == t.MaxEN)
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
                                        GUI.OpenMessageForm(Commands.SelectedUnit, u2: null);
                                    }
                                    else
                                    {
                                        GUI.UpdateMessageForm(Commands.SelectedUnit, u2: null);
                                    }
                                }
                                else if (!My.MyProject.Forms.frmMessage.Visible)
                                {
                                    GUI.OpenMessageForm(t, Commands.SelectedUnit);
                                }
                                else
                                {
                                    GUI.UpdateMessageForm(t, Commands.SelectedUnit);
                                }

                                GUI.Sleep(150);
                            }

                            // ＥＮを回復させる
                            tmp = t.EN;
                            if (EffectType(i) == "ＥＮ増加")
                            {
                                t.EN = (t.EN + 10d * effect.dblEffectLevel);
                            }
                            else
                            {
                                t.RecoverEN(10d * effect.dblEffectLevel);
                            }

                            if (!is_event)
                            {
                                if (!displayed_string)
                                {
                                    if (effect.dblEffectLevel >= 0d)
                                    {
                                        GUI.DrawSysString(t.x, t.y, "+" + SrcFormatter.Format(t.EN - tmp));
                                    }
                                    else
                                    {
                                        GUI.DrawSysString(t.x, t.y, SrcFormatter.Format(t.EN - tmp));
                                    }
                                }

                                displayed_string = true;
                                if (ReferenceEquals(t, Commands.SelectedUnit))
                                {
                                    GUI.UpdateMessageForm(Commands.SelectedUnit, u2: null);
                                }
                                else
                                {
                                    GUI.UpdateMessageForm(t, Commands.SelectedUnit);
                                }

                                if (effect.dblEffectLevel >= 0d)
                                {
                                    GUI.DisplaySysMessage(t.Nickname + "の" + Expression.Term("ＥＮ", t) + "が" + SrcFormatter.Format(t.EN - tmp) + "回復した。");
                                }
                                else
                                {
                                    GUI.DisplaySysMessage(t.Nickname + "の" + Expression.Term("ＥＮ", t) + "が" + SrcFormatter.Format(tmp - t.EN) + "減少した。");
                                }
                            }

                            need_update = true;
                            break;
                        }

                    case "霊力回復":
                    case "霊力増加":
                        {
                            // 効果が適用可能かどうか判定
                            if (effect.dblEffectLevel > 0d)
                            {
                                if (t.IsConditionSatisfied("ゾンビ"))
                                {
                                    goto NextEffect;
                                }

                                if (t.MainPilot().Plana == t.MainPilot().MaxPlana())
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
                                        GUI.OpenMessageForm(Commands.SelectedUnit, u2: null);
                                    }
                                    else
                                    {
                                        GUI.UpdateMessageForm(Commands.SelectedUnit, u2: null);
                                    }
                                }
                                else if (!My.MyProject.Forms.frmMessage.Visible)
                                {
                                    GUI.OpenMessageForm(t, Commands.SelectedUnit);
                                }
                                else
                                {
                                    GUI.UpdateMessageForm(t, Commands.SelectedUnit);
                                }

                                GUI.Sleep(150);
                            }

                            // 霊力を回復させる
                            {
                                var withBlock2 = t.MainPilot();
                                tmp = withBlock2.Plana;
                                if (EffectType(i) == "霊力増加")
                                {
                                    withBlock2.Plana = (withBlock2.Plana + 10d * effect.dblEffectLevel);
                                }
                                else
                                {
                                    withBlock2.Plana = (withBlock2.Plana + (long)(withBlock2.MaxPlana() * effect.dblEffectLevel) / 10L);
                                }
                            }

                            if (!is_event)
                            {
                                if (!displayed_string)
                                {
                                    if (effect.dblEffectLevel >= 0d)
                                    {
                                        GUI.DrawSysString(t.x, t.y, "+" + SrcFormatter.Format(t.MainPilot().Plana - tmp));
                                    }
                                    else
                                    {
                                        GUI.DrawSysString(t.x, t.y, SrcFormatter.Format(t.MainPilot().Plana - tmp));
                                    }
                                }

                                displayed_string = true;
                                if (ReferenceEquals(t, Commands.SelectedUnit))
                                {
                                    GUI.UpdateMessageForm(Commands.SelectedUnit, u2: null);
                                }
                                else
                                {
                                    GUI.UpdateMessageForm(t, Commands.SelectedUnit);
                                }

                                if (effect.dblEffectLevel >= 0d)
                                {
                                    GUI.DisplaySysMessage(t.Nickname + "の" + t.MainPilot().SkillName0("霊力") + "が" + SrcFormatter.Format(t.MainPilot().Plana - tmp) + "回復した。");
                                }
                                else
                                {
                                    GUI.DisplaySysMessage(t.Nickname + "の" + t.MainPilot().SkillName0("霊力") + "が" + SrcFormatter.Format(tmp - t.MainPilot().Plana) + "減少した。");
                                }
                            }

                            need_update = true;
                            break;
                        }

                    case "ＳＰ回復":
                        {
                            // 効果が適用可能かどうか判定
                            if (effect.dblEffectLevel > 0d)
                            {
                                if (t.IsConditionSatisfied("ゾンビ"))
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
                                        GUI.OpenMessageForm(Commands.SelectedUnit, u2: null);
                                    }
                                    else
                                    {
                                        GUI.UpdateMessageForm(Commands.SelectedUnit, u2: null);
                                    }
                                }
                                else if (!My.MyProject.Forms.frmMessage.Visible)
                                {
                                    GUI.OpenMessageForm(t, Commands.SelectedUnit);
                                }
                                else
                                {
                                    GUI.UpdateMessageForm(t, Commands.SelectedUnit);
                                }

                                GUI.Sleep(150);
                            }

                            // 回復対象となるパイロット数を算出
                            n = (t.CountPilot() + t.CountSupport());
                            if (t.IsFeatureAvailable("追加サポート"))
                            {
                                n = (n + 1);
                            }

                            // ＳＰを回復
                            if (n == 1)
                            {
                                // メインパイロットのみのＳＰを回復
                                tmp = t.MainPilot().SP;
                                t.MainPilot().SP = (t.MainPilot().SP + 10d * effect.dblEffectLevel);
                                if (!is_event)
                                {
                                    if (!displayed_string)
                                    {
                                        if (effect.dblEffectLevel >= 0d)
                                        {
                                            GUI.DrawSysString(t.x, t.y, "+" + SrcFormatter.Format(t.MainPilot().SP - tmp));
                                        }
                                        else
                                        {
                                            GUI.DrawSysString(t.x, t.y, SrcFormatter.Format(t.MainPilot().SP - tmp));
                                        }
                                    }

                                    displayed_string = true;
                                    if (effect.dblEffectLevel >= 0d)
                                    {
                                        GUI.DisplaySysMessage(t.MainPilot().get_Nickname(false) + "の" + Expression.Term("ＳＰ", t) + "が" + SrcFormatter.Format(t.MainPilot().SP - tmp) + "回復した。");
                                    }
                                    else
                                    {
                                        GUI.DisplaySysMessage(t.MainPilot().get_Nickname(false) + "の" + Expression.Term("ＳＰ", t) + "が" + SrcFormatter.Format(tmp - t.MainPilot().SP) + "減少した。");
                                    }
                                }
                            }
                            else
                            {
                                // メインパイロットのＳＰを回復
                                tmp = t.MainPilot().SP;
                                t.MainPilot().SP = (t.MainPilot().SP + 2d * effect.dblEffectLevel + (long)(10d * effect.dblEffectLevel) / n);
                                if (!is_event)
                                {
                                    if (!displayed_string)
                                    {
                                        if (effect.dblEffectLevel >= 0d)
                                        {
                                            GUI.DrawSysString(t.x, t.y, "+" + SrcFormatter.Format(t.MainPilot().SP - tmp));
                                        }
                                        else
                                        {
                                            GUI.DrawSysString(t.x, t.y, SrcFormatter.Format(t.MainPilot().SP - tmp));
                                        }
                                    }

                                    displayed_string = true;
                                    if (effect.dblEffectLevel >= 0d)
                                    {
                                        GUI.DisplaySysMessage(t.MainPilot().get_Nickname(false) + "の" + Expression.Term("ＳＰ", t) + "が" + SrcFormatter.Format(t.MainPilot().SP - tmp) + "回復した。");
                                    }
                                    else
                                    {
                                        GUI.DisplaySysMessage(t.MainPilot().get_Nickname(false) + "の" + Expression.Term("ＳＰ", t) + "が" + SrcFormatter.Format(tmp - t.MainPilot().SP) + "減少した。");
                                    }
                                }

                                // サブパイロットのＳＰを回復
                                var loopTo2 = t.CountPilot();
                                for (j = 2; j <= loopTo2; j++)
                                {
                                    {
                                        var withBlock3 = t.Pilot(j);
                                        tmp = withBlock3.SP;
                                        withBlock3.SP = (withBlock3.SP + 2d * effect.dblEffectLevel + (long)(10d * effect.dblEffectLevel) / n);
                                        if (!is_event)
                                        {
                                            if (withBlock3.SP != tmp)
                                            {
                                                if (effect.dblEffectLevel >= 0d)
                                                {
                                                    GUI.DisplaySysMessage(withBlock3.get_Nickname(false) + "の" + Expression.Term("ＳＰ", t) + "が" + SrcFormatter.Format(withBlock3.SP - tmp) + "回復した。");
                                                }
                                                else
                                                {
                                                    GUI.DisplaySysMessage(withBlock3.get_Nickname(false) + "の" + Expression.Term("ＳＰ", t) + "が" + SrcFormatter.Format(tmp - withBlock3.SP) + "減少した。");
                                                }
                                            }
                                        }
                                    }
                                }

                                // サポートパイロットのＳＰを回復
                                var loopTo3 = t.CountSupport();
                                for (j = 1; j <= loopTo3; j++)
                                {
                                    {
                                        var withBlock4 = t.Support(j);
                                        tmp = withBlock4.SP;
                                        withBlock4.SP = (withBlock4.SP + 2d * effect.dblEffectLevel + (long)(10d * effect.dblEffectLevel) / n);
                                        if (!is_event)
                                        {
                                            if (withBlock4.SP != tmp)
                                            {
                                                if (effect.dblEffectLevel >= 0d)
                                                {
                                                    GUI.DisplaySysMessage(withBlock4.get_Nickname(false) + "の" + Expression.Term("ＳＰ", t) + "が" + SrcFormatter.Format(withBlock4.SP - tmp) + "回復した。");
                                                }
                                                else
                                                {
                                                    GUI.DisplaySysMessage(withBlock4.get_Nickname(false) + "の" + Expression.Term("ＳＰ", t) + "が" + SrcFormatter.Format(tmp - withBlock4.SP) + "減少した。");
                                                }
                                            }
                                        }
                                    }
                                }

                                // 追加サポートパイロットのＳＰを回復
                                if (t.IsFeatureAvailable("追加サポート"))
                                {
                                    {
                                        var withBlock5 = t.AdditionalSupport();
                                        tmp = withBlock5.SP;
                                        withBlock5.SP = (withBlock5.SP + 2d * effect.dblEffectLevel + (long)(10d * effect.dblEffectLevel) / n);
                                        if (!is_event)
                                        {
                                            if (withBlock5.SP != tmp)
                                            {
                                                if (effect.dblEffectLevel >= 0d)
                                                {
                                                    GUI.DisplaySysMessage(withBlock5.get_Nickname(false) + "の" + Expression.Term("ＳＰ", t) + "が" + SrcFormatter.Format(withBlock5.SP - tmp) + "回復した。");
                                                }
                                                else
                                                {
                                                    GUI.DisplaySysMessage(withBlock5.get_Nickname(false) + "の" + Expression.Term("ＳＰ", t) + "が" + SrcFormatter.Format(tmp - withBlock5.SP) + "減少した。");
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
                            var loopTo4 = t.CountWeapon();
                            for (j = 1; j <= loopTo4; j++)
                            {
                                if (t.Bullet(j) < t.MaxBullet(j))
                                {
                                    break;
                                }
                            }

                            if (j > t.CountWeapon())
                            {
                                goto NextEffect;
                            }

                            if (!is_event)
                            {
                                if (ReferenceEquals(t, Commands.SelectedUnit))
                                {
                                    if (!My.MyProject.Forms.frmMessage.Visible)
                                    {
                                        GUI.OpenMessageForm(Commands.SelectedUnit, u2: null);
                                    }
                                    else
                                    {
                                        GUI.UpdateMessageForm(Commands.SelectedUnit, u2: null);
                                    }
                                }
                                else if (!My.MyProject.Forms.frmMessage.Visible)
                                {
                                    GUI.OpenMessageForm(t, Commands.SelectedUnit);
                                }
                                else
                                {
                                    GUI.UpdateMessageForm(t, Commands.SelectedUnit);
                                }
                            }

                            // 弾薬を補給
                            t.BulletSupply();
                            if (!is_event)
                            {
                                GUI.DisplaySysMessage(t.Nickname + "の弾数が全快した。");
                            }

                            break;
                        }

                    case "状態回復":
                        {
                            if (t.ConditionLifetime("攻撃不能") <= 0 && t.ConditionLifetime("移動不能") <= 0 && t.ConditionLifetime("装甲劣化") <= 0 && t.ConditionLifetime("混乱") <= 0 && t.ConditionLifetime("魅了") <= 0 && t.ConditionLifetime("憑依") <= 0 && t.ConditionLifetime("石化") <= 0 && t.ConditionLifetime("凍結") <= 0 && t.ConditionLifetime("麻痺") <= 0 && t.ConditionLifetime("睡眠") <= 0 && t.ConditionLifetime("毒") <= 0 && t.ConditionLifetime("盲目") <= 0 && t.ConditionLifetime("撹乱") <= 0 && t.ConditionLifetime("恐怖") <= 0 && t.ConditionLifetime("沈黙") <= 0 && t.ConditionLifetime("ゾンビ") <= 0 && t.ConditionLifetime("回復不能") <= 0 && t.ConditionLifetime("オーラ使用不能") <= 0 && t.ConditionLifetime("超能力使用不能") <= 0 && t.ConditionLifetime("同調率使用不能") <= 0 && t.ConditionLifetime("超感覚使用不能") <= 0 && t.ConditionLifetime("知覚強化使用不能") <= 0 && t.ConditionLifetime("霊力使用不能") <= 0 && t.ConditionLifetime("術使用不能") <= 0 && t.ConditionLifetime("技使用不能") <= 0)
                            {
                                var loopTo5 = t.CountCondition();
                                for (j = 1; j <= loopTo5; j++)
                                {
                                    string localCondition2() { object argIndex1 = j; var ret = t.Condition(argIndex1); return ret; }

                                    if (Strings.Len(localCondition2()) > 6)
                                    {
                                        // 弱、効属性は状態回復から除外。
                                        string localCondition1() { object argIndex1 = j; var ret = t.Condition(argIndex1); return ret; }

                                        if (Strings.Right(localCondition1(), 6) == "属性使用不能")
                                        {
                                            string localCondition() { object argIndex1 = j; var ret = t.Condition(argIndex1); return ret; }

                                            if (t.ConditionLifetime(localCondition()) > 0)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }

                                if (j > t.CountCondition())
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
                                        GUI.OpenMessageForm(Commands.SelectedUnit, u2: null);
                                    }
                                    else
                                    {
                                        GUI.UpdateMessageForm(Commands.SelectedUnit, u2: null);
                                    }
                                }
                                else if (!My.MyProject.Forms.frmMessage.Visible)
                                {
                                    GUI.OpenMessageForm(t, Commands.SelectedUnit);
                                }
                                else
                                {
                                    GUI.UpdateMessageForm(t, Commands.SelectedUnit);
                                }
                            }

                            // 全てのステータス異常を回復
                            if (t.ConditionLifetime("攻撃不能") > 0)
                            {
                                t.DeleteCondition("攻撃不能");
                            }

                            if (t.ConditionLifetime("移動不能") > 0)
                            {
                                t.DeleteCondition("移動不能");
                            }

                            if (t.ConditionLifetime("装甲劣化") > 0)
                            {
                                t.DeleteCondition("装甲劣化");
                            }

                            if (t.ConditionLifetime("混乱") > 0)
                            {
                                t.DeleteCondition("混乱");
                            }

                            if (t.ConditionLifetime("魅了") > 0)
                            {
                                t.DeleteCondition("魅了");
                            }

                            if (t.ConditionLifetime("憑依") > 0)
                            {
                                t.DeleteCondition("憑依");
                            }

                            if (t.ConditionLifetime("石化") > 0)
                            {
                                t.DeleteCondition("石化");
                            }

                            if (t.ConditionLifetime("凍結") > 0)
                            {
                                t.DeleteCondition("凍結");
                            }

                            if (t.ConditionLifetime("麻痺") > 0)
                            {
                                t.DeleteCondition("麻痺");
                            }

                            if (t.ConditionLifetime("睡眠") > 0)
                            {
                                t.DeleteCondition("睡眠");
                            }

                            if (t.ConditionLifetime("毒") > 0)
                            {
                                t.DeleteCondition("毒");
                            }

                            if (t.ConditionLifetime("盲目") > 0)
                            {
                                t.DeleteCondition("盲目");
                            }

                            if (t.ConditionLifetime("撹乱") > 0)
                            {
                                t.DeleteCondition("撹乱");
                            }

                            if (t.ConditionLifetime("恐怖") > 0)
                            {
                                t.DeleteCondition("恐怖");
                            }

                            if (t.ConditionLifetime("沈黙") > 0)
                            {
                                t.DeleteCondition("沈黙");
                            }

                            if (t.ConditionLifetime("ゾンビ") > 0)
                            {
                                t.DeleteCondition("ゾンビ");
                            }

                            if (t.ConditionLifetime("回復不能") > 0)
                            {
                                t.DeleteCondition("回復不能");
                            }

                            if (t.ConditionLifetime("オーラ使用不能") > 0)
                            {
                                t.DeleteCondition("オーラ使用不能");
                            }

                            if (t.ConditionLifetime("超能力使用不能") > 0)
                            {
                                t.DeleteCondition("超能力使用不能");
                            }

                            if (t.ConditionLifetime("同調率使用不能") > 0)
                            {
                                t.DeleteCondition("同調率使用不能");
                            }

                            if (t.ConditionLifetime("超感覚使用不能") > 0)
                            {
                                t.DeleteCondition("超感覚使用不能");
                            }

                            if (t.ConditionLifetime("知覚強化使用不能") > 0)
                            {
                                t.DeleteCondition("知覚強化使用不能");
                            }

                            if (t.ConditionLifetime("霊力使用不能") > 0)
                            {
                                t.DeleteCondition("霊力使用不能");
                            }

                            if (t.ConditionLifetime("術使用不能") > 0)
                            {
                                t.DeleteCondition("術使用不能");
                            }

                            if (t.ConditionLifetime("技使用不能") > 0)
                            {
                                t.DeleteCondition("技使用不能");
                            }

                            var loopTo6 = t.CountCondition();
                            for (j = 1; j <= loopTo6; j++)
                            {
                                string localCondition6() { object argIndex1 = j; var ret = t.Condition(argIndex1); return ret; }

                                if (Strings.Len(localCondition6()) > 6)
                                {
                                    // 弱、効属性は状態回復から除外。
                                    string localCondition5() { object argIndex1 = j; var ret = t.Condition(argIndex1); return ret; }

                                    if (Strings.Right(localCondition5(), 6) == "属性使用不能")
                                    {
                                        string localCondition4() { object argIndex1 = j; var ret = t.Condition(argIndex1); return ret; }

                                        if (t.ConditionLifetime(localCondition4()) > 0)
                                        {
                                            string localCondition3() { object argIndex1 = j; var ret = t.Condition(argIndex1); return ret; }

                                            t.DeleteCondition(localCondition3());
                                        }
                                    }
                                }
                            }

                            if (!is_event)
                            {
                                GUI.DisplaySysMessage(t.Nickname + "の状態が回復した。");
                            }

                            break;
                        }

                    case "行動数回復":
                        {
                            // 効果が適用可能かどうか判定
                            if (t.Action > 0 | t.MaxAction() == 0)
                            {
                                goto NextEffect;
                            }

                            // 行動数を回復させる
                            t.UsedAction = (t.UsedAction - 1);

                            // 他の効果の表示のためにメッセージウィンドウが表示されているので
                            // なければ特にメッセージは表示しない (効果は見れば分かるので)
                            if (!is_event)
                            {
                                if (My.MyProject.Forms.frmMessage.Visible)
                                {
                                    if (ReferenceEquals(t, Commands.SelectedUnit))
                                    {
                                        GUI.UpdateMessageForm(Commands.SelectedUnit, u2: null);
                                    }
                                    else
                                    {
                                        GUI.UpdateMessageForm(t, Commands.SelectedUnit);
                                    }

                                    GUI.DisplaySysMessage(t.Nickname + "は行動可能になった。");
                                }
                            }

                            break;
                        }

                    case "行動数増加":
                        {
                            // 効果が適用可能かどうか判定
                            if (t.Action > 3 | t.MaxAction() == 0)
                            {
                                goto NextEffect;
                            }

                            // 行動数を増やす
                            t.UsedAction = (t.UsedAction - 1);
                            if (!is_event)
                            {
                                if (ReferenceEquals(t, Commands.SelectedUnit))
                                {
                                    if (!My.MyProject.Forms.frmMessage.Visible)
                                    {
                                        GUI.OpenMessageForm(Commands.SelectedUnit, u2: null);
                                    }
                                    else
                                    {
                                        GUI.UpdateMessageForm(Commands.SelectedUnit, u2: null);
                                    }
                                }
                                else if (!My.MyProject.Forms.frmMessage.Visible)
                                {
                                    GUI.OpenMessageForm(t, Commands.SelectedUnit);
                                }
                                else
                                {
                                    GUI.UpdateMessageForm(t, Commands.SelectedUnit);
                                }

                                GUI.DisplaySysMessage((string)(t.Nickname + "は" + Strings.StrConv(SrcFormatter.Format((object)t.Action), VbStrConv.Wide) + "回行動可能になった。"));
                            }

                            break;
                        }

                    case "スペシャルパワー":
                    case "精神コマンド":
                        {
                            if (SRC.SPDList.IsDefined(EffectData(i)))
                            {
                                t.MakeSpecialPowerInEffect(EffectData(i), my_unit.MainPilot().ID);
                            }
                            else
                            {
                                GUI.ErrorMessage("スペシャルパワー「" + Name + "」で使われているスペシャルパワー「" + EffectData(i) + "」は定義されていません。");
                            }

                            break;
                        }

                    case "気力増加":
                        {
                            if (t.MainPilot().Personality == "機械")
                            {
                                goto NextEffect;
                            }

                            if (t.MainPilot().Morale == t.MainPilot().MaxMorale)
                            {
                                goto NextEffect;
                            }

                            // 気力を増加させる
                            tmp = t.MainPilot().Morale;
                            t.IncreaseMorale((10d * effect.dblEffectLevel));
                            if (!is_event)
                            {
                                if (!displayed_string)
                                {
                                    GUI.DrawSysString(t.x, t.y, "+" + SrcFormatter.Format(t.MainPilot().Morale - tmp));
                                }

                                displayed_string = true;
                            }

                            need_update = true;
                            break;
                        }

                    case "気力低下":
                        {
                            // 効果が適用可能かどうか判定
                            if (t.MainPilot().Personality == "機械")
                            {
                                goto NextEffect;
                            }

                            if (t.MainPilot().Morale == t.MainPilot().MinMorale)
                            {
                                goto NextEffect;
                            }

                            // 気力を低下させる
                            tmp = t.MainPilot().Morale;
                            t.IncreaseMorale((-10 * effect.dblEffectLevel));
                            if (!is_event)
                            {
                                if (TargetType == "敵" | TargetType == "全敵")
                                {
                                    if (!displayed_string)
                                    {
                                        GUI.DrawSysString(t.x, t.y, SrcFormatter.Format(t.MainPilot().Morale - tmp));
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
                            if (t.IsConditionSatisfied("無敵"))
                            {
                                goto NextEffect;
                            }

                            if (!is_event)
                            {
                                if (ReferenceEquals(t, Commands.SelectedUnit))
                                {
                                    if (!My.MyProject.Forms.frmMessage.Visible)
                                    {
                                        GUI.OpenMessageForm(Commands.SelectedUnit, u2: null);
                                    }
                                    else
                                    {
                                        GUI.UpdateMessageForm(Commands.SelectedUnit, u2: null);
                                    }
                                }
                                else if (!My.MyProject.Forms.frmMessage.Visible)
                                {
                                    GUI.OpenMessageForm(t, Commands.SelectedUnit);
                                }
                                else
                                {
                                    GUI.UpdateMessageForm(t, Commands.SelectedUnit);
                                }
                            }

                            // ダメージを与える
                            tmp = t.HP;
                            t.HP = GeneralLib.MaxLng(t.HP - 10 * GeneralLib.Dice((10d * effect.dblEffectLevel)), 10);
                            if (TargetType == "全敵")
                            {
                                GUI.Sleep(150);
                            }

                            // 特殊能力「不安定」による暴走チェック
                            if (t.IsFeatureAvailable("不安定"))
                            {
                                if (t.HP <= t.MaxHP / 4 && !t.IsConditionSatisfied("暴走"))
                                {
                                    t.AddCondition("暴走", -1, cdata: "");
                                }
                            }

                            if (!is_event)
                            {
                                if (!displayed_string)
                                {
                                    GUI.DrawSysString(t.x, t.y, SrcFormatter.Format(tmp - t.HP));
                                }

                                displayed_string = true;
                                if (ReferenceEquals(t, Commands.SelectedUnit))
                                {
                                    GUI.UpdateMessageForm(Commands.SelectedUnit, u2: null);
                                }
                                else
                                {
                                    GUI.UpdateMessageForm(t, Commands.SelectedUnit);
                                }

                                GUI.DisplaySysMessage(t.Nickname + "に" + SrcFormatter.Format(tmp - t.HP) + "のダメージを与えた。");
                            }

                            need_update = true;
                            break;
                        }

                    case "ＨＰ減少":
                        {
                            // 効果が適用可能かどうか判定
                            if (t.IsConditionSatisfied("無敵"))
                            {
                                goto NextEffect;
                            }

                            if (!is_event)
                            {
                                if (ReferenceEquals(t, Commands.SelectedUnit))
                                {
                                    if (!My.MyProject.Forms.frmMessage.Visible)
                                    {
                                        GUI.OpenMessageForm(Commands.SelectedUnit, u2: null);
                                    }
                                    else
                                    {
                                        GUI.UpdateMessageForm(Commands.SelectedUnit, u2: null);
                                    }
                                }
                                else if (!My.MyProject.Forms.frmMessage.Visible)
                                {
                                    GUI.OpenMessageForm(t, Commands.SelectedUnit);
                                }
                                else
                                {
                                    GUI.UpdateMessageForm(t, Commands.SelectedUnit);
                                }
                            }

                            // ＨＰを減少させる
                            tmp = t.HP;
                            t.HP = (t.HP - (long)(t.HP * effect.dblEffectLevel) / 10L);
                            if (TargetType == "全敵")
                            {
                                GUI.Sleep(150);
                            }

                            // 特殊能力「不安定」による暴走チェック
                            if (t.IsFeatureAvailable("不安定"))
                            {
                                if (t.HP <= t.MaxHP / 4 && !t.IsConditionSatisfied("暴走"))
                                {
                                    t.AddCondition("暴走", -1, cdata: "");
                                }
                            }

                            if (!is_event)
                            {
                                if (!displayed_string)
                                {
                                    GUI.DrawSysString(t.x, t.y, SrcFormatter.Format(tmp - t.HP));
                                }

                                displayed_string = true;
                                if (ReferenceEquals(t, Commands.SelectedUnit))
                                {
                                    GUI.UpdateMessageForm(Commands.SelectedUnit, u2: null);
                                }
                                else
                                {
                                    GUI.UpdateMessageForm(t, Commands.SelectedUnit);
                                }

                                if (ReferenceEquals(Commands.SelectedUnit, t))
                                {
                                    GUI.DisplaySysMessage(t.Nickname + "の" + Expression.Term("ＨＰ", t) + "が" + SrcFormatter.Format(tmp - t.HP) + "減少した。");
                                }
                                else
                                {
                                    GUI.DisplaySysMessage(t.Nickname + "の" + Expression.Term("ＨＰ", t) + "を" + SrcFormatter.Format(tmp - t.HP) + "減少させた。");
                                }
                            }

                            need_update = true;
                            break;
                        }

                    case "ＥＮ減少":
                        {
                            // 効果が適用可能かどうか判定
                            if (t.IsConditionSatisfied("無敵"))
                            {
                                goto NextEffect;
                            }

                            if (!is_event)
                            {
                                if (ReferenceEquals(t, Commands.SelectedUnit))
                                {
                                    if (!My.MyProject.Forms.frmMessage.Visible)
                                    {
                                        GUI.OpenMessageForm(Commands.SelectedUnit, u2: null);
                                    }
                                    else
                                    {
                                        GUI.UpdateMessageForm(Commands.SelectedUnit, u2: null);
                                    }
                                }
                                else if (!My.MyProject.Forms.frmMessage.Visible)
                                {
                                    GUI.OpenMessageForm(t, Commands.SelectedUnit);
                                }
                                else
                                {
                                    GUI.UpdateMessageForm(t, Commands.SelectedUnit);
                                }
                            }

                            // ＥＮを減少させる
                            tmp = t.EN;
                            t.EN = (t.EN - (long)(t.EN * effect.dblEffectLevel) / 10L);
                            if (TargetType == "全敵")
                            {
                                GUI.Sleep(150);
                            }

                            if (!displayed_string)
                            {
                                GUI.DrawSysString(t.x, t.y, SrcFormatter.Format(tmp - t.EN));
                            }

                            displayed_string = true;
                            if (ReferenceEquals(t, Commands.SelectedUnit))
                            {
                                GUI.UpdateMessageForm(Commands.SelectedUnit, u2: null);
                            }
                            else
                            {
                                GUI.UpdateMessageForm(t, Commands.SelectedUnit);
                            }

                            if (ReferenceEquals(Commands.SelectedUnit, t))
                            {
                                GUI.DisplaySysMessage(t.Nickname + "の" + Expression.Term("ＥＮ", t) + "が" + SrcFormatter.Format(tmp - t.EN) + "減少した。");
                            }
                            else
                            {
                                GUI.DisplaySysMessage(t.Nickname + "の" + Expression.Term("ＥＮ", t) + "を" + SrcFormatter.Format(tmp - t.EN) + "減少させた。");
                            }

                            need_update = true;
                            break;
                        }

                    case "偵察":
                        {
                            // 未識別のユニットは識別しておく
                            if (Expression.IsOptionDefined("ユニット情報隠蔽"))
                            {
                                if (!t.IsConditionSatisfied("識別済み"))
                                {
                                    t.AddCondition("識別済み", -1, 0d, "非表示");
                                    Status.DisplayUnitStatus(t);
                                }
                            }

                            if (t.IsConditionSatisfied("ユニット情報隠蔽"))
                            {
                                t.DeleteCondition("ユニット情報隠蔽");
                                Status.DisplayUnitStatus(t);
                            }

                            if (!My.MyProject.Forms.frmMessage.Visible)
                            {
                                GUI.OpenMessageForm(u1: null, u2: null);
                            }

                            GUI.DisplayMessage("システム", Expression.Term("ＨＰ", t, 6) + "：" + SrcFormatter.Format(t.HP) + "/" + SrcFormatter.Format(t.MaxHP) + ";" + Expression.Term("ＥＮ", t, 6) + "：" + SrcFormatter.Format(t.EN) + "/" + SrcFormatter.Format(t.MaxEN) + ";" + Expression.Term("資金", t, 6) + "：" + SrcFormatter.Format(t.Value / 2) + ";" + "経験値：" + SrcFormatter.Format(t.ExpValue + t.MainPilot().ExpValue));
                            if (t.IsFeatureAvailable("アイテム所有"))
                            {
                                if (SRC.IDList.IsDefined(t.FeatureData("アイテム所有")))
                                {
                                    ItemData localItem() { object argIndex1 = "アイテム所有"; object argIndex2 = t.FeatureData(argIndex1); var ret = SRC.IDList.Item(argIndex2); return ret; }

                                    msg = localItem().Nickname + "を盗むことが出来る。;";
                                }
                                else
                                {
                                    GUI.ErrorMessage(t.Name + "の所有アイテム「" + t.FeatureData("アイテム所有") + "」のデータが見つかりません");
                                }
                            }

                            if (t.IsFeatureAvailable("レアアイテム所有"))
                            {
                                if (SRC.IDList.IsDefined(t.FeatureData("レアアイテム所有")))
                                {
                                    if (Strings.Len(msg) > 0)
                                    {
                                        msg = msg + "また、";
                                    }

                                    ItemData localItem1() { object argIndex1 = "レアアイテム所有"; object argIndex2 = t.FeatureData(argIndex1); var ret = SRC.IDList.Item(argIndex2); return ret; }

                                    msg = msg + "まれに" + localItem1().Nickname + "を盗むことが出来る。;";
                                }
                                else
                                {
                                    GUI.ErrorMessage(t.Name + "の所有レアアイテム「" + t.FeatureData("レアアイテム所有") + "」のデータが見つかりません");
                                }
                            }

                            if (t.IsFeatureAvailable("ラーニング可能技"))
                            {
                                msg = msg + "「" + t.FeatureData("ラーニング可能技") + "」をラーニング可能。";
                            }

                            if (Strings.Len(msg) > 0)
                            {
                                GUI.DisplayMessage("システム", msg);
                            }

                            break;
                        }

                    case "自爆":
                        {
                            GUI.OpenMessageForm(t, u2: null);
                            t.SuicidalExplosion();
                            return ApplyRet;
                        }

                    case "復活":
                        {
                            if (Duration == "破壊")
                            {
                                // 破壊直後に復活する場合
                                t.HP = t.MaxHP;
                            }
                            else
                            {
                                // 破壊後に他のパイロットの力で復活する場合

                                // 復活時は通常形態に戻る
                                if (t.IsFeatureAvailable("ノーマルモード"))
                                {
                                    string localLIndex() { object argIndex1 = "ノーマルモード"; string arglist = t.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

                                    t.Transform(localLIndex());
                                    t = t.CurrentForm();
                                    n = 0;
                                }
                                else
                                {
                                    n = t.ConditionLifetime("残り時間");

                                    // 後のRestで残り時間が0にならないように一旦時間を巻き戻す
                                    if (n > 0)
                                    {
                                        t.AddCondition("残り時間", 10, cdata: "");
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
                                    t.DeleteCondition("残り時間");
                                    t.AddCondition("残り時間", n, cdata: "");
                                }

                                GUI.RedrawScreen();
                            }

                            if (!My.MyProject.Forms.frmMessage.Visible)
                            {
                                GUI.OpenMessageForm(u1: null, u2: null);
                            }

                            if (t.IsMessageDefined("復活"))
                            {
                                t.PilotMessage("復活", msg_mode: "");
                            }

                            if (t.IsAnimationDefined("復活", sub_situation: ""))
                            {
                                t.PlayAnimation("復活", sub_situation: "");
                            }
                            else
                            {
                                t.SpecialEffect("復活", sub_situation: "");
                            }

                            GUI.DisplaySysMessage(t.Nickname + "は復活した。");
                            break;
                        }

                    case "イベント":
                        {
                            // イベントコマンドで定義されたスペシャルパワー
                            // 対象ユニットＩＤ及び相手ユニットＩＤを設定
                            Event.SelectedUnitForEvent = my_unit.CurrentForm();
                            Event.SelectedTargetForEvent = t.CurrentForm();
                            // 指定されたサブルーチンを実行
                            Expression.GetValueAsString("Call(" + EffectData(i) + ")");
                            break;
                        }
                }

            NextEffect:
                ;
            }

            // Unitが変化してしまった場合は元に戻しておく
            if (!ReferenceEquals(my_unit, p.Unit))
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


        //        // スペシャルパワーが有効なターゲットの総数を返す
        //        // (パイロット p が使用した場合)
        //        public int CountTarget(Pilot p)
        //        {
        //            int CountTargetRet = default;
        //            Unit u;
        //            int i;
        //            switch (TargetType ?? "")
        //            {
        //                case "自分":
        //                    {
        //                        if (Effective(p, p.Unit))
        //                        {
        //                            CountTargetRet = 1;
        //                        }

        //                        break;
        //                    }

        //                case "味方":
        //                case "全味方":
        //                    {
        //                        foreach (Unit currentU in SRC.UList)
        //                        {
        //                            u = currentU;
        //                            {
        //                                var withBlock = u;
        //                                // 出撃している？
        //                                if (withBlock.Status != "出撃")
        //                                {
        //                                    goto NextUnit1;
        //                                }

        //                                // 味方ユニット？
        //                                switch (p.Party ?? "")
        //                                {
        //                                    case "味方":
        //                                    case "ＮＰＣ":
        //                                        {
        //                                            if (withBlock.Party != "味方" && withBlock.Party0 != "味方" && withBlock.Party != "ＮＰＣ" && withBlock.Party0 != "ＮＰＣ")
        //                                            {
        //                                                goto NextUnit1;
        //                                            }

        //                                            break;
        //                                        }

        //                                    default:
        //                                        {
        //                                            if ((p.Party ?? "") != (withBlock.Party ?? ""))
        //                                            {
        //                                                goto NextUnit1;
        //                                            }

        //                                            break;
        //                                        }
        //                                }

        //                                // 効果がある？
        //                                if (Effective(p, u))
        //                                {
        //                                    CountTargetRet = (CountTargetRet + 1);
        //                                }
        //                            }

        //                        NextUnit1:
        //                            ;
        //                        }

        //                        break;
        //                    }

        //                case "破壊味方":
        //                    {
        //                        foreach (Unit currentU1 in SRC.UList)
        //                        {
        //                            u = currentU1;
        //                            {
        //                                var withBlock1 = u;
        //                                // 破壊されている？
        //                                if (withBlock1.Status != "破壊")
        //                                {
        //                                    goto NextUnit2;
        //                                }

        //                                // 味方ユニット？
        //                                if ((p.Party ?? "") != (withBlock1.Party0 ?? ""))
        //                                {
        //                                    goto NextUnit2;
        //                                }

        //                                // 効果がある？
        //                                if (Effective(p, u))
        //                                {
        //                                    CountTargetRet = (CountTargetRet + 1);
        //                                }
        //                            }

        //                        NextUnit2:
        //                            ;
        //                        }

        //                        break;
        //                    }

        //                case "敵":
        //                case "全敵":
        //                    {
        //                        foreach (Unit currentU2 in SRC.UList)
        //                        {
        //                            u = currentU2;
        //                            {
        //                                var withBlock2 = u;
        //                                // 出撃している？
        //                                if (withBlock2.Status != "出撃")
        //                                {
        //                                    goto NextUnit3;
        //                                }

        //                                // 敵ユニット？
        //                                switch (p.Party ?? "")
        //                                {
        //                                    case "味方":
        //                                    case "ＮＰＣ":
        //                                        {
        //                                            if (withBlock2.Party == "味方" && withBlock2.Party0 == "味方" | withBlock2.Party == "ＮＰＣ" && withBlock2.Party0 == "ＮＰＣ")
        //                                            {
        //                                                goto NextUnit3;
        //                                            }

        //                                            break;
        //                                        }

        //                                    default:
        //                                        {
        //                                            if ((p.Party ?? "") == (withBlock2.Party ?? ""))
        //                                            {
        //                                                goto NextUnit3;
        //                                            }

        //                                            break;
        //                                        }
        //                                }

        //                                // 効果がある？
        //                                if (Effective(p, u))
        //                                {
        //                                    CountTargetRet = (CountTargetRet + 1);
        //                                }
        //                            }

        //                        NextUnit3:
        //                            ;
        //                        }

        //                        break;
        //                    }

        //                case "任意":
        //                case "全":
        //                    {
        //                        foreach (Unit currentU3 in SRC.UList)
        //                        {
        //                            u = currentU3;
        //                            // 出撃している？
        //                            if (u.Status == "出撃")
        //                            {
        //                                // 効果がある？
        //                                if (Effective(p, u))
        //                                {
        //                                    CountTargetRet = (CountTargetRet + 1);
        //                                }
        //                            }
        //                        }

        //                        break;
        //                    }
        //            }

        //            return CountTargetRet;
        //        }

        //        // スペシャルパワーのアニメーションを表示
        //        public bool PlayAnimation()
        //        {
        //            bool PlayAnimationRet = default;
        //            string anime;
        //            string[] animes;
        //            int anime_head;
        //            var buf = default(string);
        //            var ret = default(double);
        //            int i, j;
        //            string expr;
        //            var wait_time = default;
        //            int prev_obj_color;
        //            int prev_obj_fill_color;
        //            int prev_obj_fill_style;
        //            int prev_obj_draw_width;
        //            string prev_obj_draw_option;
        //            if (!SRC.SpecialPowerAnimation)
        //            {
        //                return PlayAnimationRet;
        //            }

        //            if (Animation == "-")
        //            {
        //                PlayAnimationRet = true;
        //                return PlayAnimationRet;
        //            }

        //            // アニメ指定がなされていない場合はアニメ表示用サブルーチンが見つらなければそのまま終了
        //            if ((Animation ?? "") == (Name ?? ""))
        //            {
        //                if (Event.FindNormalLabel("ＳＰアニメ_" + Animation) == 0)
        //                {
        //                    if (Name != "自爆" && Name != "祈り")
        //                    {
        //                        if (Event.IsLabelDefined("特殊効果 " + Name))
        //                        {
        //                            Event.HandleEvent("特殊効果", Name);
        //                            PlayAnimationRet = true;
        //                        }
        //                    }

        //                    return PlayAnimationRet;
        //                }
        //            }

        //            // 右クリック中はアニメ表示をスキップ
        //            if (GUI.IsRButtonPressed())
        //            {
        //                PlayAnimationRet = true;
        //                return PlayAnimationRet;
        //            }

        //            // オブジェクト色等を記録しておく
        //            prev_obj_color = Event.ObjColor;
        //            prev_obj_fill_color = Event.ObjFillColor;
        //            prev_obj_fill_style = Event.ObjFillStyle;
        //            prev_obj_draw_width = Event.ObjDrawWidth;
        //            prev_obj_draw_option = Event.ObjDrawOption;

        //            // オブジェクト色等をデフォルトに戻す
        //            Event.ObjColor = ColorTranslator.ToOle(Color.White);
        //            Event.ObjFillColor = ColorTranslator.ToOle(Color.White);
        //            Event.ObjFillStyle = vbFSTransparent;
        //            Event.ObjDrawWidth = 1;
        //            Event.ObjDrawOption = "";

        //            // アニメ指定を分割
        //            animes = new string[2];
        //            anime_head = 1;
        //            var loopTo = Strings.Len(Animation);
        //            for (i = 1; i <= loopTo; i++)
        //            {
        //                if (Strings.Mid(Animation, i, 1) == ";")
        //                {
        //                    animes[Information.UBound(animes)] = Strings.Mid(Animation, anime_head, i - anime_head);
        //                    Array.Resize(animes, Information.UBound(animes) + 1 + 1);
        //                    anime_head = (i + 1);
        //                }
        //            }

        //            animes[Information.UBound(animes)] = Strings.Mid(Animation, anime_head);
        //            ;
        //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
        //            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 59356


        //            Input:

        //                    On Error GoTo ErrorHandler

        //             */
        //            var loopTo1 = Information.UBound(animes);
        //            for (i = 1; i <= loopTo1; i++)
        //            {
        //                anime = animes[i];

        //                // 式評価
        //                Expression.FormatMessage(anime);

        //                // 画面クリア？
        //                if (Strings.LCase(anime) == "clear")
        //                {
        //                    GUI.ClearPicture();
        //                    GUI.MainForm.picMain(0).Refresh();
        //                    goto NextAnime;
        //                }

        //                // 戦闘アニメ以外の特殊効果
        //                switch (Strings.LCase(Strings.Right(GeneralLib.LIndex(anime, 1), 4)) ?? "")
        //                {
        //                    case ".wav":
        //                    case ".mp3":
        //                        {
        //                            // 効果音
        //                            Sound.PlayWave(anime);
        //                            if (wait_time > 0)
        //                            {
        //                                GUI.Sleep(wait_time);
        //                                wait_time = 0;
        //                            }

        //                            goto NextAnime;
        //                            break;
        //                        }

        //                    case ".bmp":
        //                    case ".jpg":
        //                    case ".gif":
        //                    case ".png":
        //                        {
        //                            // カットインの表示
        //                            if (wait_time > 0)
        //                            {
        //                                anime = SrcFormatter.Format(wait_time / 100d) + ";" + anime;
        //                                wait_time = 0;
        //                            }

        //                            GUI.DisplayBattleMessage("", anime, msg_mode: "");
        //                            goto NextAnime;
        //                            break;
        //                        }
        //                }

        //                switch (Strings.LCase(GeneralLib.LIndex(anime, 1)) ?? "")
        //                {
        //                    case "line":
        //                    case "circle":
        //                    case "arc":
        //                    case "oval":
        //                    case "color":
        //                    case "fillcolor":
        //                    case "fillstyle":
        //                    case "drawwidth":
        //                        {
        //                            // 画面処理コマンド
        //                            if (wait_time > 0)
        //                            {
        //                                anime = SrcFormatter.Format(wait_time / 100d) + ";" + anime;
        //                                wait_time = 0;
        //                            }

        //                            GUI.DisplayBattleMessage("", anime, msg_mode: "");
        //                            goto NextAnime;
        //                            break;
        //                        }

        //                    case "center":
        //                        {
        //                            // 指定したユニットを中央表示
        //                            buf = Expression.GetValueAsString(GeneralLib.ListIndex(anime, 2));
        //                            if (SRC.UList.IsDefined(buf))
        //                            {
        //                                {
        //                                    var withBlock = SRC.UList.Item(buf);
        //                                    GUI.Center(withBlock.x, withBlock.y);
        //                                    GUI.RedrawScreen();
        //                                }
        //                            }

        //                            goto NextAnime;
        //                            break;
        //                        }
        //                }

        //                // ウェイト？
        //                if (Information.IsNumeric(anime))
        //                {
        //                    wait_time = (100d * Conversions.ToDouble(anime));
        //                    goto NextAnime;
        //                }

        //                // サブルーチンの呼び出しが確定

        //                // 戦闘アニメ再生前にウェイトを入れる？
        //                if (wait_time > 0)
        //                {
        //                    GUI.Sleep(wait_time);
        //                    wait_time = 0;
        //                }

        //                // サブルーチン呼び出しのための式を作成
        //                if (Strings.Left(anime, 1) == "@")
        //                {
        //                    expr = Strings.Mid(GeneralLib.ListIndex(anime, 1), 2) + "(";
        //                    // 引数の構築
        //                    var loopTo2 = GeneralLib.ListLength(anime);
        //                    for (j = 2; j <= loopTo2; j++)
        //                    {
        //                        if (j > 2)
        //                        {
        //                            expr = expr + ",";
        //                        }

        //                        expr = expr + GeneralLib.ListIndex(anime, j);
        //                    }

        //                    expr = expr + ")";
        //                }
        //                else if (Commands.SelectedTarget is object)
        //                {
        //                    expr = "ＳＰアニメ_" + anime + "(" + Commands.SelectedUnit.ID + "," + Commands.SelectedTarget.ID + ")";
        //                }
        //                else
        //                {
        //                    expr = "ＳＰアニメ_" + anime + "(" + Commands.SelectedUnit.ID + ",-)";
        //                }

        //                // 画像描画が行われたかどうかの判定のためにフラグを初期化
        //                GUI.IsPictureDrawn = false;

        //                // アニメ再生
        //                Event.SaveBasePoint();
        //                Expression.CallFunction(expr, Expression.ValueType.StringType, buf, ret);
        //                Event.RestoreBasePoint();

        //                // 画像を消去しておく
        //                if (GUI.IsPictureDrawn && Strings.LCase(buf) != "keep")
        //                {
        //                    GUI.ClearPicture();
        //                    GUI.MainForm.picMain(0).Refresh();
        //                }

        //            NextAnime:
        //                ;
        //            }

        //            // 戦闘アニメ再生後にウェイトを入れる？
        //            if (wait_time > 0)
        //            {
        //                GUI.Sleep(wait_time);
        //                wait_time = 0;
        //            }

        //            // メッセージウィンドウを閉じる
        //            GUI.CloseMessageForm();

        //            // オブジェクト色等を元に戻す
        //            Event.ObjColor = prev_obj_color;
        //            Event.ObjFillColor = prev_obj_fill_color;
        //            Event.ObjFillStyle = prev_obj_fill_style;
        //            Event.ObjDrawWidth = prev_obj_draw_width;
        //            Event.ObjDrawOption = prev_obj_draw_option;
        //            PlayAnimationRet = true;
        //            return PlayAnimationRet;
        //        ErrorHandler:
        //            ;


        //            // アニメ再生中に発生したエラーの処理
        //            if (Strings.Len(Event.EventErrorMessage) > 0)
        //            {
        //                Event.DisplayEventErrorMessage(Event.CurrentLineNum, Event.EventErrorMessage);
        //                Event.EventErrorMessage = "";
        //            }
        //            else
        //            {
        //                Event.DisplayEventErrorMessage(Event.CurrentLineNum, "");
        //            }
        //}
    }
}
