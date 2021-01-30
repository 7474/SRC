using SRC.Core.Lib;
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
        public void AddLabel(string lname, int eventDataId)
        {
            SrcCollection<LabelData> normalLabelList = colNormalLabelList;
            SrcCollection<LabelData> eventLabelList = colEventLabelList;

            var new_label = new LabelData();
            string lname2;
            new_label.Data = lname;
            new_label.EventDataId = eventDataId;
            new_label.Enable = true;
            if (new_label.Name == LabelType.NormalLabel)
            {
                // 通常ラベルを追加
                if (FindNormalLabel0(lname) < 0)
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
                    colEventLabelList.Add(new_label, lname2 + "(" + SrcFormatter.Format(eventDataId) + ")");
                }
            }
        }

        // システム側のラベルを追加
        public void AddSysLabel(string lname, int eventDataId)
        {
            var new_label = new LabelData();
            string lname2;

            new_label.Data = lname;
            new_label.EventDataId = eventDataId;
            new_label.Enable = true;
            if (new_label.Name == LabelType.NormalLabel)
            {
                // 通常ラベルを追加
                if (FindSysNormalLabel(lname) < 0)
                {
                    colSysNormalLabelList.Add(new_label, lname);
                }
                else
                {
                    colSysNormalLabelList[lname].EventDataId = eventDataId;
                }
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
                    colEventLabelList.Add(new_label, lname2 + "(" + SrcFormatter.Format(eventDataId) + ")");
                }
            }
        }

        // ラベルを消去
        public void ClearLabel(int eventDataId)
        {
            LabelData lab;

            // 行番号eventDataIdにあるラベルを探す
            foreach (LabelData currentLab in colEventLabelList)
            {
                lab = currentLab;
                if (lab.EventDataId == eventDataId)
                {
                    lab.Enable = false;
                    return;
                }
            }

            // eventDataId行目になければその周りを探す
            for (var i = 1; i <= 10; i++)
            {
                foreach (LabelData currentLab1 in colEventLabelList)
                {
                    lab = currentLab1;
                    if (lab.EventDataId == eventDataId - i | lab.EventDataId == eventDataId + i)
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
            int FindLabelRet;
            string lname2;

            // 通常ラベルから検索
            FindLabelRet = FindNormalLabel(lname);
            if (FindLabelRet >= 0)
            {
                return FindLabelRet;
            }

            // イベントラベルから検索
            FindLabelRet = FindEventLabel(lname);
            if (FindLabelRet >= 0)
            {
                return FindLabelRet;
            }

            // パラメータ間の文字列の違いで一致しなかった可能性があるので
            // 文字列を半角スペース一文字のみにして検索してみる
            lname2 = string.Join(" ", GeneralLib.ToList(lname));

            // イベントラベルから検索
            FindLabelRet = FindEventLabel(lname2);
            return FindLabelRet;
        }

        // イベントラベルを探す
        public int FindEventLabel(string lname)
        {
            try
            {
                return colEventLabelList[lname].EventDataId;
            }
            catch
            {
                // オフセットは1->0になっている
                return -1;
            }
        }

        // 通常ラベルを探す
        public int FindNormalLabel(string lname)
        {
            int FindNormalLabelRet = FindNormalLabel0(lname);
            if (FindNormalLabelRet < 0)
            {
                FindNormalLabelRet = FindSysNormalLabel(lname);
            }

            return FindNormalLabelRet;
        }

        // シナリオ側の通常ラベルを探す
        private int FindNormalLabel0(string lname)
        {
            try
            {
                return colNormalLabelList[lname].EventDataId;
            }
            catch
            {
                // オフセットは1->0になっている
                return -1;
            }
        }

        // システム側の通常ラベルを探す
        private int FindSysNormalLabel(string lname)
        {
            try
            {
                return colSysNormalLabelList[lname].EventDataId;
            }
            catch
            {
                // オフセットは1->0になっている
                return -1;
            }
        }
    }
}
