// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using System.Collections.Generic;

namespace SRCCore.Models
{
    public class DialogItem
    {
        // メッセージの話者
        public string strName { get; }
        // メッセージ
        public string strMessage { get; }

        public DialogItem(string strName, string strMessage)
        {
            this.strName = strName;
            this.strMessage = strMessage;
        }
    }

    // ダイアログのクラス
    public class Dialog
    {
        public string Situation { get; set; }
        private IList<DialogItem> dialogs = new List<DialogItem>();

        public IList<DialogItem> Items => dialogs;

        // ダイアログにメッセージを追加
        public void AddMessage(string Name, string Message)
        {
            dialogs.Add(new DialogItem(Name, Message));
        }

        // ダイアログに含まれるメッセージ数
        public int Count => dialogs.Count;

        //// ダイアログに使われているパイロットが全て利用可能か判定
        //public bool get_IsAvailable(Unit u, bool ignore_condition)
        //{
        //    bool IsAvailableRet = default;
        //    string pname, pname2 = default;
        //    int i, j;
        //    string mpnickname, mpname, mtype;
        //    {
        //        var withBlock = u.MainPilot();
        //        mpname = withBlock.Name;
        //        mpnickname = withBlock.get_Nickname(false);
        //        mtype = withBlock.MessageType;
        //    }

        //    var loopTo = Count;
        //    for (i = 1; i <= loopTo; i++)
        //    {
        //        pname = strName[i];

        //        // 合体技のパートナーが指定されている場合
        //        if (Strings.Left(pname, 1) == "@")
        //        {
        //            pname = Strings.Mid(pname, 2);
        //            var loopTo1 = (int)Information.UBound(Commands.SelectedPartners);
        //            for (j = 1; j <= loopTo1; j++)
        //            {
        //                {
        //                    var withBlock1 = Commands.SelectedPartners[j];
        //                    if (withBlock1.CountPilot() > 0)
        //                    {
        //                        // パートナーの名前と一致する？
        //                        {
        //                            var withBlock2 = withBlock1.MainPilot();
        //                            if ((pname ?? "") != (withBlock2.Name ?? "") & Strings.InStr(pname, withBlock2.Name + "(") != 1 & (pname ?? "") != (withBlock2.get_Nickname(false) ?? "") & Strings.InStr(pname, withBlock2.get_Nickname(false) + "(") != 1)
        //                            {
        //                                goto NextPartner;
        //                            }
        //                        }

        //                        // 喋れるかどうかチェック
        //                        if (!ignore_condition)
        //                        {
        //                            object argIndex1 = "睡眠";
        //                            object argIndex2 = "麻痺";
        //                            object argIndex3 = "石化";
        //                            object argIndex4 = "恐怖";
        //                            object argIndex5 = "沈黙";
        //                            object argIndex6 = "混乱";
        //                            if (withBlock1.IsConditionSatisfied(argIndex1) | withBlock1.IsConditionSatisfied(argIndex2) | withBlock1.IsConditionSatisfied(argIndex3) | withBlock1.IsConditionSatisfied(argIndex4) | withBlock1.IsConditionSatisfied(argIndex5) | withBlock1.IsConditionSatisfied(argIndex6))
        //                            {
        //                                IsAvailableRet = false;
        //                                return default;
        //                            }
        //                        }

        //                        // メッセージは使用可能
        //                        goto NextMessage;
        //                    }
        //                }

        //            NextPartner:
        //                ;
        //            }

        //            IsAvailableRet = false;
        //            return default;
        //        }

        //        // 表情パターンが指定されている？
        //        if (Strings.InStr(pname, "(") > 0)
        //        {
        //            bool localIsDefined21() { object argIndex1 = pname; var ret = SRC.PDList.IsDefined2(argIndex1); return ret; }

        //            bool localIsDefined22() { object argIndex1 = pname; var ret = SRC.NPDList.IsDefined2(argIndex1); return ret; }

        //            if (!localIsDefined21() & localIsDefined22())
        //            {
        //                // 括弧部分を削除
        //                var loopTo2 = (int)Strings.Len(pname);
        //                for (j = 2; j <= loopTo2; j++)
        //                {
        //                    if (Strings.Mid(pname, Strings.Len(pname) - j, 1) == "(")
        //                    {
        //                        pname2 = Strings.Left(pname, Strings.Len(pname) - j - 1);
        //                        break;
        //                    }
        //                }

        //                // 表情パターンかどうか判定
        //                bool localIsDefined2() { object argIndex1 = pname2; var ret = SRC.NPDList.IsDefined2(argIndex1); return ret; }

        //                object argIndex7 = pname2;
        //                if (SRC.PDList.IsDefined2(argIndex7) | localIsDefined2())
        //                {
        //                    // 表情パターンとみなす
        //                    pname = pname2;
        //                }
        //            }
        //        }

        //        // メインパイロットは常に存在
        //        if ((pname ?? "") == (mpname ?? ""))
        //        {
        //            goto NextMessage;
        //        }

        //        if ((pname ?? "") == (mpnickname ?? ""))
        //        {
        //            goto NextMessage;
        //        }

        //        if ((pname ?? "") == (mtype ?? ""))
        //        {
        //            goto NextMessage;
        //        }

        //        // ノンパイロットはLeaveしていない限り常に存在
        //        object argIndex8 = pname;
        //        if (SRC.NPDList.IsDefined(argIndex8))
        //        {
        //            string argvname = "IsAway(" + pname + ")";
        //            if (Expression.IsGlobalVariableDefined(argvname))
        //            {
        //                IsAvailableRet = false;
        //                return default;
        //            }

        //            goto NextMessage;
        //        }

        //        object argIndex10 = pname;
        //        if (SRC.PDList.IsDefined(argIndex10))
        //        {
        //            // パイロットの場合

        //            // パイロットが作成されていない？
        //            bool localIsDefined() { object argIndex1 = pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

        //            if (!localIsDefined())
        //            {
        //                IsAvailableRet = false;
        //                return default;
        //            }

        //            object argIndex9 = pname;
        //            {
        //                var withBlock3 = SRC.PList.Item(argIndex9);
        //                // Leave中？
        //                if (withBlock3.Away)
        //                {
        //                    IsAvailableRet = false;
        //                    return default;
        //                }

        //                // 喋れるかどうかチェック
        //                if (!ignore_condition & withBlock3.Unit is object)
        //                {
        //                    {
        //                        var withBlock4 = withBlock3.Unit;
        //                        if (withBlock4.IsConditionSatisfied("睡眠") | withBlock4.IsConditionSatisfied("麻痺") | withBlock4.IsConditionSatisfied("石化") | withBlock4.IsConditionSatisfied("恐怖") | withBlock4.IsConditionSatisfied("沈黙") | withBlock4.IsConditionSatisfied("混乱"))
        //                        {
        //                            IsAvailableRet = false;
        //                            return default;
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //    NextMessage:
        //        ;
        //    }

        //    IsAvailableRet = true;
        //    return IsAvailableRet;
        //}

        //// idx番目のメッセージの話者
        //public string Name(int idx)
        //{
        //    string NameRet = default;
        //    NameRet = strName[idx];
        //    return NameRet;
        //}

        //// idx番目のメッセージ
        //public string Message(int idx)
        //{
        //    string MessageRet = default;
        //    MessageRet = strMessage[idx];
        //    return MessageRet;
        //}
    }
}