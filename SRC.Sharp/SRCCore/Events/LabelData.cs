// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.VB;

namespace SRCCore.Events
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

        protected SRC SRC { get; }
        private Expressions.Expression Expression => SRC.Expression;

        public LabelData(SRC src)
        {
            SRC = src;
        }

        public override string ToString()
        {
            return $"{EventDataId}: {StrData}";
        }

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
                    ParaRet = Expression.GetValueAsString(strParas[idx], true);
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
                // ラベル全体
                StrData = value;

                //        TODO Impl
                int i;
                string lname;

                // ラベル名
                lname = GeneralLib.ListIndex(value, 1);
                // 「*」は省く
                switch (Strings.Asc(lname))
                {
                    case 42: // *
                        {
                            lname = Strings.Mid(lname, 2);
                            switch (Strings.Asc(lname))
                            {
                                case 42: // *
                                    {
                                        lname = Strings.Mid(lname, 2);
                                        AsterNum = 3;
                                        break;
                                    }

                                case 45: // -
                                    {
                                        lname = Strings.Mid(lname, 2);
                                        AsterNum = 2;
                                        break;
                                    }

                                default:
                                    {
                                        AsterNum = 2;
                                        break;
                                    }
                            }

                            break;
                        }

                    case 45: // -
                        {
                            lname = Strings.Mid(lname, 2);
                            switch (Strings.Asc(lname))
                            {
                                case 42: // *
                                    {
                                        lname = Strings.Mid(lname, 2);
                                        AsterNum = 1;
                                        break;
                                    }

                                case 45: // -
                                    {
                                        lname = Strings.Mid(lname, 2);
                                        break;
                                    }
                            }

                            break;
                        }
                }

                switch (lname ?? "")
                {
                    case "プロローグ":
                        {
                            Name = Events.LabelType.PrologueEventLabel;
                            break;
                        }

                    case "スタート":
                        {
                            Name = Events.LabelType.StartEventLabel;
                            break;
                        }

                    case "エピローグ":
                        {
                            Name = Events.LabelType.EpilogueEventLabel;
                            break;
                        }

                    case "ターン":
                        {
                            Name = Events.LabelType.TurnEventLabel;
                            break;
                        }

                    case "損傷率":
                        {
                            Name = Events.LabelType.DamageEventLabel;
                            break;
                        }

                    case "破壊":
                        {
                            Name = Events.LabelType.DestructionEventLabel;
                            break;
                        }

                    case "全滅":
                        {
                            Name = Events.LabelType.TotalDestructionEventLabel;
                            break;
                        }

                    case "攻撃":
                        {
                            Name = Events.LabelType.AttackEventLabel;
                            break;
                        }

                    case "攻撃後":
                        {
                            Name = Events.LabelType.AfterAttackEventLabel;
                            break;
                        }

                    case "会話":
                        {
                            Name = Events.LabelType.TalkEventLabel;
                            break;
                        }

                    case "接触":
                        {
                            Name = Events.LabelType.ContactEventLabel;
                            break;
                        }

                    case "進入":
                        {
                            Name = Events.LabelType.EnterEventLabel;
                            break;
                        }

                    case "脱出":
                        {
                            Name = Events.LabelType.EscapeEventLabel;
                            break;
                        }

                    case "収納":
                        {
                            Name = Events.LabelType.LandEventLabel;
                            break;
                        }

                    case "使用":
                        {
                            Name = Events.LabelType.UseEventLabel;
                            break;
                        }

                    case "使用後":
                        {
                            Name = Events.LabelType.AfterUseEventLabel;
                            break;
                        }

                    case "変形":
                        {
                            Name = Events.LabelType.TransformEventLabel;
                            break;
                        }

                    case "合体":
                        {
                            Name = Events.LabelType.CombineEventLabel;
                            break;
                        }

                    case "分離":
                        {
                            Name = Events.LabelType.SplitEventLabel;
                            break;
                        }

                    case "行動終了":
                        {
                            Name = Events.LabelType.FinishEventLabel;
                            break;
                        }

                    case "レベルアップ":
                        {
                            Name = Events.LabelType.LevelUpEventLabel;
                            break;
                        }

                    case "勝利条件":
                        {
                            Name = Events.LabelType.RequirementEventLabel;
                            break;
                        }

                    case "再開":
                        {
                            Name = Events.LabelType.ResumeEventLabel;
                            break;
                        }

                    case "マップコマンド":
                        {
                            Name = Events.LabelType.MapCommandEventLabel;
                            break;
                        }

                    case "ユニットコマンド":
                        {
                            Name = Events.LabelType.UnitCommandEventLabel;
                            break;
                        }

                    case "特殊効果":
                        {
                            Name = Events.LabelType.EffectEventLabel;
                            break;
                        }

                    default:
                        {
                            Name = Events.LabelType.NormalLabel;
                            break;
                        }
                }

                // パラメータ
                intParaNum = GeneralLib.ListLength(value);
                if (intParaNum == -1)
                {
                    // TODO Impl
                    throw new TerminateException("ラベルの引数の括弧の対応が取れていません");
                    //Event.DisplayEventErrorMessage(Event.CurrentLineNum, "ラベルの引数の括弧の対応が取れていません");
                    //return;
                }

                strParas = new string[(intParaNum + 1)];
                blnConst = new bool[(intParaNum + 1)];
                var loopTo = intParaNum;
                for (i = 2; i <= loopTo; i++)
                {
                    strParas[i] = GeneralLib.ListIndex(value, i);
                    // パラメータが固定値かどうか判定
                    if (Information.IsNumeric(strParas[i]))
                    {
                        blnConst[i] = true;
                    }
                    else if (SRC.PDList.IsDefined(strParas[i]))
                    {
                        if (Strings.InStr(strParas[i], "主人公") != 1 && Strings.InStr(strParas[i], "ヒロイン") != 1)
                        {
                            blnConst[i] = true;
                        }
                    }
                    else if (SRC.UDList.IsDefined(strParas[i]))
                    {
                        blnConst[i] = true;
                    }
                    else if (SRC.IDList.IsDefined(strParas[i]))
                    {
                        blnConst[i] = true;
                    }
                    else
                    {
                        switch (strParas[i] ?? "")
                        {
                            case "味方":
                            case "ＮＰＣ":
                            case "敵":
                            case "中立":
                            case "全":
                                blnConst[i] = true;
                                break;

                            case "N":
                            case "W":
                            case "S":
                            case "E":
                                if (Name == Events.LabelType.EscapeEventLabel)
                                {
                                    blnConst[i] = true;
                                }

                                break;

                            default:
                                if (Strings.Left(strParas[i], 1) == "\"" && Strings.Right(strParas[i], 1) == "\"")
                                {
                                    if (Strings.InStr(strParas[i], "$(") == 0)
                                    {
                                        strParas[i] = Strings.Mid(strParas[i], 2, Strings.Len(strParas[i]) - 2);
                                        blnConst[i] = true;
                                    }
                                }

                                break;
                        }
                    }
                }
            }
        }
    }
}
