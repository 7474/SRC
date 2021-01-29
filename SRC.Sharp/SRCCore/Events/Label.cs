﻿using SRC.Core.Lib;
using SRC.Core.VB;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRC.Core.Events
{
    public partial class Event
    {
        // ラベルが定義されているか
        public bool IsLabelDefined(string Index)
        {
            try
            {
                return colEventLabelList.ContainsKey(Index);
            }
            catch
            {
                return false;
            }
        }

        // ラベルを追加
        public void AddLabel(string lname, int lnum)
        {
            SrcCollection<LabelData> normalLabelList = colNormalLabelList;
            SrcCollection<LabelData> eventLabelList = colEventLabelList;


            var new_label = new LabelData();
            string lname2;
            new_label.Data = lname;
            new_label.LineNum = lnum;
            new_label.Enable = true;
            if (new_label.Name == LabelType.NormalLabel)
            {
                // 通常ラベルを追加
                if (FindNormalLabel0(lname) == 0)
                {
                    colNormalLabelList.Add(new_label, lname);
                }
                // 通常ラベルが重複定義されている場合は無視
            }
            else
            {
                // イベントラベルを追加

                // パラメータ間の文字列の違いによる不一致をなくすため、
                // 文字列を半角スペース一文字に直しておく
                lname2 = string.Join(" ", GeneralLib.ToList(lname));

                if (!IsLabelDefined(lname2))
                {
                    colEventLabelList.Add(new_label, lname2);
                }
                else
                {
                    colEventLabelList.Add(new_label, lname2 + "(" + SrcFormatter.Format(lnum) + ")");
                }
            }

            return;
        }

        // システム側のラベルを追加
        public void AddSysLabel(string lname, int lnum)
        {
            var new_label = new LabelData();
            string lname2;
            int i;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 97526


            Input:

                    On Error GoTo ErrorHandler

             */
            new_label.Data = lname;
            new_label.LineNum = lnum;
            new_label.Enable = true;
            if (new_label.Name == LabelType.NormalLabel)
            {
                // 通常ラベルを追加
                if (FindSysNormalLabel(lname) == 0)
                {
                    colSysNormalLabelList.Add(new_label, lname);
                }
                else
                {
                    // UPGRADE_WARNING: オブジェクト colSysNormalLabelList.Item().LineNum の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    colSysNormalLabelList[lname].LineNum = lnum;
                }
            }
            else
            {
                // イベントラベルを追加

                // パラメータ間の文字列の違いによる不一致をなくすため、
                // 文字列を半角スペース一文字に直しておく
                lname2 = GeneralLib.ListIndex(lname, 1);
                var loopTo = GeneralLib.ListLength(lname);
                for (i = 2; i <= loopTo; i++)
                    lname2 = lname2 + " " + GeneralLib.ListIndex(lname, i);
                bool localIsLabelDefined() { object argIndex1 = lname2; var ret = IsLabelDefined(argIndex1); return ret; }

                if (!localIsLabelDefined())
                {
                    colEventLabelList.Add(new_label, lname2);
                }
                else
                {
                    colEventLabelList.Add(new_label, lname2 + "(" + SrcFormatter.Format(lnum) + ")");
                }
            }

            return;
        ErrorHandler:
            ;

            // 通常ラベルが重複定義されている場合は無視
        }

        // ラベルを消去
        public void ClearLabel(int lnum)
        {
            LabelData lab;
            int i;

            // 行番号lnumにあるラベルを探す
            foreach (LabelData currentLab in colEventLabelList)
            {
                lab = currentLab;
                if (lab.LineNum == lnum)
                {
                    lab.Enable = false;
                    return;
                }
            }

            // lnum行目になければその周りを探す
            for (i = 1; i <= 10; i++)
            {
                foreach (LabelData currentLab1 in colEventLabelList)
                {
                    lab = currentLab1;
                    if (lab.LineNum == lnum - i | lab.LineNum == lnum + i)
                    {
                        lab.Enable = false;
                        return;
                    }
                }
            }
        }

        // ラベルを復活
        public void RestoreLabel(string lname)
        {
            foreach (LabelData lab in colEventLabelList)
            {
                if ((lab.Data ?? "") == (lname ?? ""))
                {
                    lab.Enable = true;
                    return;
                }
            }
        }

        // ラベルを探す
        public int FindLabel(string lname)
        {
            int FindLabelRet = default;
            string lname2;
            int i;

            // 通常ラベルから検索
            FindLabelRet = FindNormalLabel(lname);
            if (FindLabelRet > 0)
            {
                return FindLabelRet;
            }

            // イベントラベルから検索
            FindLabelRet = FindEventLabel(lname);
            if (FindLabelRet > 0)
            {
                return FindLabelRet;
            }

            // パラメータ間の文字列の違いで一致しなかった可能性があるので
            // 文字列を半角スペース一文字のみにして検索してみる
            lname2 = GeneralLib.ListIndex(lname, 1);
            var loopTo = GeneralLib.ListLength(lname);
            for (i = 2; i <= loopTo; i++)
                lname2 = lname2 + " " + GeneralLib.ListIndex(lname, i);

            // イベントラベルから検索
            FindLabelRet = FindEventLabel(lname2);
            return FindLabelRet;
        }

        // イベントラベルを探す
        public int FindEventLabel(string lname)
        {
            int FindEventLabelRet = default;
            LabelData lab;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo NotFound' at character 100733


            Input:

                    On Error GoTo NotFound

             */
            lab = (LabelData)colEventLabelList[lname];
            FindEventLabelRet = lab.LineNum;
            return FindEventLabelRet;
        NotFound:
            ;
            FindEventLabelRet = 0;
        }

        // 通常ラベルを探す
        public int FindNormalLabel(string lname)
        {
            int FindNormalLabelRet = default;
            FindNormalLabelRet = FindNormalLabel0(lname);
            if (FindNormalLabelRet == 0)
            {
                FindNormalLabelRet = FindSysNormalLabel(lname);
            }

            return FindNormalLabelRet;
        }

        // シナリオ側の通常ラベルを探す
        private int FindNormalLabel0(string lname)
        {
            int FindNormalLabel0Ret = default;
            LabelData lab;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo NotFound' at character 101357


            Input:

                    On Error GoTo NotFound

             */
            lab = (LabelData)colNormalLabelList[lname];
            FindNormalLabel0Ret = lab.LineNum;
            return FindNormalLabel0Ret;
        NotFound:
            ;
            FindNormalLabel0Ret = 0;
        }

        // システム側の通常ラベルを探す
        private int FindSysNormalLabel(string lname)
        {
            int FindSysNormalLabelRet = default;
            LabelData lab;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo NotFound' at character 101696


            Input:

                    On Error GoTo NotFound

             */
            lab = (LabelData)colSysNormalLabelList[lname];
            FindSysNormalLabelRet = lab.LineNum;
            return FindSysNormalLabelRet;
        NotFound:
            ;
            FindSysNormalLabelRet = 0;
        }

    }
}
