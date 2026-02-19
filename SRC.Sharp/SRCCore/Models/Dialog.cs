// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Units;
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
    public class Dialog : ISituationItem
    {
        public string Situation { get; set; }
        private IList<DialogItem> dialogs = new List<DialogItem>();
        private SRC SRC;

        public Dialog(SRC src)
        {
            SRC = src;
        }

        public IList<DialogItem> Items => dialogs;

        // ダイアログにメッセージを追加
        public void AddMessage(string Name, string Message)
        {
            dialogs.Add(new DialogItem(Name, Message));
        }

        // ダイアログに含まれるメッセージ数
        public int Count => dialogs.Count;

        // ダイアログに使われているパイロットが全て利用可能か判定
        public bool IsAvailable(Unit u, bool ignore_condition)
        {
            if (u == null)
            {
                return true;
            }

            string mpname = u.MainPilot().Name;
            string mpnickname = u.MainPilot().get_Nickname(false);
            string mtype = u.MainPilot().MessageType;

            foreach (var item in dialogs)
            {
                string pname = item.strName;
                string pname2 = null;

                // 合体技のパートナーが指定されている場合
                if (pname.Length > 0 && pname[0] == '@')
                {
                    pname = pname.Substring(1);
                    bool partnerFound = false;
                    foreach (var partner in SRC.Commands.SelectedPartners)
                    {
                        if (partner.CountPilot() > 0)
                        {
                            var mp = partner.MainPilot();
                            if ((pname ?? "") != (mp.Name ?? "")
                                && !pname.StartsWith(mp.Name + "(")
                                && (pname ?? "") != (mp.get_Nickname(false) ?? "")
                                && !pname.StartsWith(mp.get_Nickname(false) + "("))
                            {
                                continue;
                            }

                            // 喋れるかどうかチェック
                            if (!ignore_condition)
                            {
                                if (partner.IsConditionSatisfied("睡眠") || partner.IsConditionSatisfied("麻痺")
                                    || partner.IsConditionSatisfied("石化") || partner.IsConditionSatisfied("恐怖")
                                    || partner.IsConditionSatisfied("沈黙") || partner.IsConditionSatisfied("混乱"))
                                {
                                    return false;
                                }
                            }

                            partnerFound = true;
                            break;
                        }
                    }

                    if (!partnerFound)
                    {
                        return false;
                    }

                    continue;
                }

                // 表情パターンが指定されている？
                if (pname.IndexOf('(') >= 0)
                {
                    if (!SRC.PDList.IsDefined2(pname) && SRC.NPDList.IsDefined2(pname))
                    {
                        // 括弧部分を削除して基本名を取得
                        for (int j = 2; j <= pname.Length; j++)
                        {
                            if (pname[pname.Length - j] == '(')
                            {
                                pname2 = pname.Substring(0, pname.Length - j - 1);
                                break;
                            }
                        }

                        // 表情パターンかどうか判定
                        if (pname2 != null && (SRC.PDList.IsDefined2(pname2) || SRC.NPDList.IsDefined2(pname2)))
                        {
                            // 表情パターンとみなす
                            pname = pname2;
                        }
                    }
                }

                // メインパイロットは常に存在
                if ((pname ?? "") == (mpname ?? ""))
                {
                    continue;
                }

                if ((pname ?? "") == (mpnickname ?? ""))
                {
                    continue;
                }

                if ((pname ?? "") == (mtype ?? ""))
                {
                    continue;
                }

                // ノンパイロットはLeaveしていない限り常に存在
                if (SRC.NPDList.IsDefined(pname))
                {
                    if (SRC.Expression.IsGlobalVariableDefined("IsAway(" + pname + ")"))
                    {
                        return false;
                    }

                    continue;
                }

                if (SRC.PDList.IsDefined(pname))
                {
                    // パイロットの場合

                    // パイロットが作成されていない？
                    if (!SRC.PList.IsDefined(pname))
                    {
                        return false;
                    }

                    var pilot = SRC.PList.Item(pname);
                    // Leave中？
                    if (pilot.Away)
                    {
                        return false;
                    }

                    // 喋れるかどうかチェック
                    if (!ignore_condition && pilot.Unit is object)
                    {
                        var pu = pilot.Unit;
                        if (pu.IsConditionSatisfied("睡眠") || pu.IsConditionSatisfied("麻痺")
                            || pu.IsConditionSatisfied("石化") || pu.IsConditionSatisfied("恐怖")
                            || pu.IsConditionSatisfied("沈黙") || pu.IsConditionSatisfied("混乱"))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

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
