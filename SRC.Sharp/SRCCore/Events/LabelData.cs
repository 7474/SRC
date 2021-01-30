// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using System;

namespace SRC.Core.Events
{
    // イベントデータのラベルのクラス
    public class LabelData
    {
        // ラベル名
        public LabelType Name;
        // 行番号
        public int EventDataId;
        // ラベルが有効か？
        public bool Enable;
        // アスタリスクの指定状況
        public int AsterNum;

        // ラベル全体
        private string StrData;
        // ラベルの個数
        private int intParaNum;
        // ラベルの各パラメータ
        private string[] strParas;
        // パラメータが固定値？
        private bool[] blnConst;

        // パラメータの個数
        public int CountPara()
        {
            return intParaNum;
        }

        // ラベルの idx 番目のパラメータ
        public string Para(int idx)
        {
            string ParaRet = default;
            if (idx <= intParaNum)
            {
                if (blnConst[idx])
                {
                    ParaRet = strParas[idx];
                }
                else
                {
                    // TODO Impl
                    throw new NotImplementedException();
                    //ParaRet = Expression.GetValueAsString(ref strParas[idx], true);
                }
            }

            return ParaRet;
        }

        // ラベル全体を取り出す
        // ラベル全体を設定
        public string Data
        {
            get
            {
                return StrData;
            }
            set
            {
                StrData = value;
            }
        }
        // TODO Impl
        //    set
        //    {
        //        int i;
        //        string lname;

        //        // ラベル全体
        //        StrData = value;

        //        // ラベル名
        //        lname = GeneralLib.ListIndex(ref value, 1);
        //        // 「*」は省く
        //        switch (Strings.Asc(lname))
        //        {
        //            case 42: // *
        //                {
        //                    lname = Strings.Mid(lname, 2);
        //                    switch (Strings.Asc(lname))
        //                    {
        //                        case 42: // *
        //                            {
        //                                lname = Strings.Mid(lname, 2);
        //                                AsterNum = 3;
        //                                break;
        //                            }

        //                        case 45: // -
        //                            {
        //                                lname = Strings.Mid(lname, 2);
        //                                AsterNum = 2;
        //                                break;
        //                            }

        //                        default:
        //                            {
        //                                AsterNum = 2;
        //                                break;
        //                            }
        //                    }

        //                    break;
        //                }

        //            case 45: // -
        //                {
        //                    lname = Strings.Mid(lname, 2);
        //                    switch (Strings.Asc(lname))
        //                    {
        //                        case 42: // *
        //                            {
        //                                lname = Strings.Mid(lname, 2);
        //                                AsterNum = 1;
        //                                break;
        //                            }

        //                        case 45: // -
        //                            {
        //                                lname = Strings.Mid(lname, 2);
        //                                break;
        //                            }
        //                    }

        //                    break;
        //                }
        //        }

        //        switch (lname ?? "")
        //        {
        //            case "プロローグ":
        //                {
        //                    Name = Event_Renamed.LabelType.PrologueEventLabel;
        //                    break;
        //                }

        //            case "スタート":
        //                {
        //                    Name = Event_Renamed.LabelType.StartEventLabel;
        //                    break;
        //                }

        //            case "エピローグ":
        //                {
        //                    Name = Event_Renamed.LabelType.EpilogueEventLabel;
        //                    break;
        //                }

        //            case "ターン":
        //                {
        //                    Name = Event_Renamed.LabelType.TurnEventLabel;
        //                    break;
        //                }

        //            case "損傷率":
        //                {
        //                    Name = Event_Renamed.LabelType.DamageEventLabel;
        //                    break;
        //                }

        //            case "破壊":
        //                {
        //                    Name = Event_Renamed.LabelType.DestructionEventLabel;
        //                    break;
        //                }

        //            case "全滅":
        //                {
        //                    Name = Event_Renamed.LabelType.TotalDestructionEventLabel;
        //                    break;
        //                }

        //            case "攻撃":
        //                {
        //                    Name = Event_Renamed.LabelType.AttackEventLabel;
        //                    break;
        //                }

        //            case "攻撃後":
        //                {
        //                    Name = Event_Renamed.LabelType.AfterAttackEventLabel;
        //                    break;
        //                }

        //            case "会話":
        //                {
        //                    Name = Event_Renamed.LabelType.TalkEventLabel;
        //                    break;
        //                }

        //            case "接触":
        //                {
        //                    Name = Event_Renamed.LabelType.ContactEventLabel;
        //                    break;
        //                }

        //            case "進入":
        //                {
        //                    Name = Event_Renamed.LabelType.EnterEventLabel;
        //                    break;
        //                }

        //            case "脱出":
        //                {
        //                    Name = Event_Renamed.LabelType.EscapeEventLabel;
        //                    break;
        //                }

        //            case "収納":
        //                {
        //                    Name = Event_Renamed.LabelType.LandEventLabel;
        //                    break;
        //                }

        //            case "使用":
        //                {
        //                    Name = Event_Renamed.LabelType.UseEventLabel;
        //                    break;
        //                }

        //            case "使用後":
        //                {
        //                    Name = Event_Renamed.LabelType.AfterUseEventLabel;
        //                    break;
        //                }

        //            case "変形":
        //                {
        //                    Name = Event_Renamed.LabelType.TransformEventLabel;
        //                    break;
        //                }

        //            case "合体":
        //                {
        //                    Name = Event_Renamed.LabelType.CombineEventLabel;
        //                    break;
        //                }

        //            case "分離":
        //                {
        //                    Name = Event_Renamed.LabelType.SplitEventLabel;
        //                    break;
        //                }

        //            case "行動終了":
        //                {
        //                    Name = Event_Renamed.LabelType.FinishEventLabel;
        //                    break;
        //                }

        //            case "レベルアップ":
        //                {
        //                    Name = Event_Renamed.LabelType.LevelUpEventLabel;
        //                    break;
        //                }

        //            case "勝利条件":
        //                {
        //                    Name = Event_Renamed.LabelType.RequirementEventLabel;
        //                    break;
        //                }

        //            case "再開":
        //                {
        //                    Name = Event_Renamed.LabelType.ResumeEventLabel;
        //                    break;
        //                }

        //            case "マップコマンド":
        //                {
        //                    Name = Event_Renamed.LabelType.MapCommandEventLabel;
        //                    break;
        //                }

        //            case "ユニットコマンド":
        //                {
        //                    Name = Event_Renamed.LabelType.UnitCommandEventLabel;
        //                    break;
        //                }

        //            case "特殊効果":
        //                {
        //                    Name = Event_Renamed.LabelType.EffectEventLabel;
        //                    break;
        //                }

        //            default:
        //                {
        //                    Name = Event_Renamed.LabelType.NormalLabel;
        //                    break;
        //                }
        //        }

        //        // パラメータ
        //        intParaNum = GeneralLib.ListLength(ref value);
        //        if (intParaNum == -1)
        //        {
        //            Event_Renamed.DisplayEventErrorMessage(Event_Renamed.CurrentLineNum, "ラベルの引数の括弧の対応が取れていません");
        //            return;
        //        }

        //        strParas = new string[(intParaNum + 1)];
        //        blnConst = new bool[(intParaNum + 1)];
        //        var loopTo = intParaNum;
        //        for (i = 2; i <= loopTo; i++)
        //        {
        //            strParas[i] = GeneralLib.ListIndex(ref value, i);
        //            // パラメータが固定値かどうか判定
        //            bool localIsDefined() { var tmp = strParas; object argIndex1 = tmp[i]; var ret = SRC.PDList.IsDefined(ref argIndex1); return ret; }

        //            bool localIsDefined1() { var tmp = strParas; object argIndex1 = tmp[i]; var ret = SRC.UDList.IsDefined(ref argIndex1); return ret; }

        //            bool localIsDefined2() { var tmp = strParas; object argIndex1 = tmp[i]; var ret = SRC.IDList.IsDefined(ref argIndex1); return ret; }

        //            if (Information.IsNumeric(strParas[i]))
        //            {
        //                blnConst[i] = true;
        //            }
        //            else if (localIsDefined())
        //            {
        //                if (Strings.InStr(strParas[i], "主人公") != 1 & Strings.InStr(strParas[i], "ヒロイン") != 1)
        //                {
        //                    blnConst[i] = true;
        //                }
        //            }
        //            else if (localIsDefined1())
        //            {
        //                blnConst[i] = true;
        //            }
        //            else if (localIsDefined2())
        //            {
        //                blnConst[i] = true;
        //            }
        //            else
        //            {
        //                switch (strParas[i] ?? "")
        //                {
        //                    case "味方":
        //                    case "ＮＰＣ":
        //                    case "敵":
        //                    case "中立":
        //                    case "全":
        //                        {
        //                            blnConst[i] = true;
        //                            break;
        //                        }

        //                    case "N":
        //                    case "W":
        //                    case "S":
        //                    case "E":
        //                        {
        //                            if (Name == Event_Renamed.LabelType.EscapeEventLabel)
        //                            {
        //                                blnConst[i] = true;
        //                            }

        //                            break;
        //                        }

        //                    default:
        //                        {
        //                            if (Strings.Left(strParas[i], 1) == "\"" & Strings.Right(strParas[i], 1) == "\"")
        //                            {
        //                                if (Strings.InStr(strParas[i], "$(") == 0)
        //                                {
        //                                    strParas[i] = Strings.Mid(strParas[i], 2, Strings.Len(strParas[i]) - 2);
        //                                    blnConst[i] = true;
        //                                }
        //                            }

        //                            break;
        //                        }
        //                }
        //            }
        //        }
        //    }
        //}
    }
}