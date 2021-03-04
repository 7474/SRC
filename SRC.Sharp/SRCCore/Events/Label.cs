using SRCCore.Lib;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore.Events
{
    public partial class Event
    {
        // ラベルの検索
        public int SearchLabel(string lname, int start = -1)
        {
            int SearchLabelRet = default;
            LabelType ltype;
            var litem = default(string[]);
            var lnum = new string[5];
            var is_unit = new bool[5];
            var is_num = new bool[5];
            var is_condition = new bool[5];
            string str2, str1, lname2;
            int i;
            Unit tmp_u;
            bool revrersible = default, reversed;

            // ラベルの各要素をあらかじめ解析
            int llen = GeneralLib.ListSplit(lname, out litem);

            // ラベルの種類を判定
            switch (litem[0] ?? "")
            {
                case "プロローグ":
                    {
                        ltype = LabelType.PrologueEventLabel;
                        break;
                    }

                case "スタート":
                    {
                        ltype = LabelType.StartEventLabel;
                        break;
                    }

                case "エピローグ":
                    {
                        ltype = LabelType.EpilogueEventLabel;
                        break;
                    }

                //case "ターン":
                //    ltype = LabelType.TurnEventLabel;
                //    if (Information.IsNumeric(litem[2]))
                //    {
                //        is_num[2] = true;
                //    }
                //    lnum[2] = GeneralLib.StrToLng(litem[2]).ToString();
                //    break;

                //case "損傷率":
                //    {
                //        ltype = LabelType.DamageEventLabel;
                //        is_unit[2] = true;
                //        is_num[3] = true;
                //        lnum[3] = GeneralLib.StrToLng(litem[3]).ToString();
                //        break;
                //    }

                //case "破壊":
                //case "マップ攻撃破壊":
                //    {
                //        ltype = LabelType.DestructionEventLabel;
                //        is_unit[2] = true;
                //        break;
                //    }

                //case "全滅":
                //    {
                //        ltype = LabelType.TotalDestructionEventLabel;
                //        break;
                //    }

                //case "攻撃":
                //    {
                //        ltype = LabelType.AttackEventLabel;
                //        revrersible = true;
                //        is_unit[2] = true;
                //        is_unit[3] = true;
                //        break;
                //    }

                //case "攻撃後":
                //    {
                //        ltype = LabelType.AfterAttackEventLabel;
                //        revrersible = true;
                //        is_unit[2] = true;
                //        is_unit[3] = true;
                //        break;
                //    }

                //case "会話":
                //    {
                //        ltype = LabelType.TalkEventLabel;
                //        is_unit[2] = true;
                //        is_unit[3] = true;
                //        break;
                //    }

                //case "接触":
                //    {
                //        ltype = LabelType.ContactEventLabel;
                //        revrersible = true;
                //        is_unit[2] = true;
                //        is_unit[3] = true;
                //        break;
                //    }

                //case "進入":
                //    {
                //        ltype = LabelType.EnterEventLabel;
                //        is_unit[2] = true;
                //        if (llen == 4)
                //        {
                //            is_num[3] = true;
                //            is_num[4] = true;
                //            lnum[3] = GeneralLib.StrToLng(litem[3]).ToString();
                //            lnum[4] = GeneralLib.StrToLng(litem[4]).ToString();
                //        }

                //        break;
                //    }

                //case "脱出":
                //    {
                //        ltype = LabelType.EscapeEventLabel;
                //        is_unit[2] = true;
                //        break;
                //    }

                //case "収納":
                //    {
                //        ltype = LabelType.LandEventLabel;
                //        is_unit[2] = true;
                //        break;
                //    }

                //case "使用":
                //    {
                //        ltype = LabelType.UseEventLabel;
                //        is_unit[2] = true;
                //        break;
                //    }

                //case "使用後":
                //    {
                //        ltype = LabelType.AfterUseEventLabel;
                //        is_unit[2] = true;
                //        break;
                //    }

                //case "変形":
                //    {
                //        ltype = LabelType.TransformEventLabel;
                //        is_unit[2] = true;
                //        break;
                //    }

                //case "合体":
                //    {
                //        ltype = LabelType.CombineEventLabel;
                //        is_unit[2] = true;
                //        break;
                //    }

                //case "分離":
                //    {
                //        ltype = LabelType.SplitEventLabel;
                //        is_unit[2] = true;
                //        break;
                //    }

                //case "行動終了":
                //    {
                //        ltype = LabelType.FinishEventLabel;
                //        is_unit[2] = true;
                //        break;
                //    }

                //case "レベルアップ":
                //    {
                //        ltype = LabelType.LevelUpEventLabel;
                //        is_unit[2] = true;
                //        break;
                //    }

                //case "勝利条件":
                //    {
                //        ltype = LabelType.RequirementEventLabel;
                //        break;
                //    }

                //case "再開":
                //    {
                //        ltype = LabelType.ResumeEventLabel;
                //        break;
                //    }

                //case "マップコマンド":
                //    {
                //        ltype = LabelType.MapCommandEventLabel;
                //        is_condition[3] = true;
                //        break;
                //    }

                //case "ユニットコマンド":
                //    {
                //        ltype = LabelType.UnitCommandEventLabel;
                //        is_condition[4] = true;
                //        break;
                //    }

                //case "特殊効果":
                //    {
                //        ltype = LabelType.EffectEventLabel;
                //        break;
                //    }

                default:
                    {
                        ltype = LabelType.NormalLabel;
                        break;
                    }
            }

            // 各ラベルについて一致しているかチェック
            foreach (LabelData lab in colEventLabelList)
            {
                // ラベルの種類が一致している？
                if (ltype != lab.Name)
                {
                    continue;
                }

                // ClearEventされていない？
                if (!lab.Enable)
                {
                    continue;
                }

                // 検索開始行より後ろ？
                if (lab.EventDataId < start)
                {
                    continue;
                }

                // パラメータ数が一致している？
                if (llen != lab.CountPara())
                {
                    if (ltype != LabelType.MapCommandEventLabel & ltype != LabelType.UnitCommandEventLabel)
                    {
                        continue;
                    }
                }

                //    // 各パラメータが一致している？
                //    reversed = false;
                //CheckPara:
                //    ;
                //    var loopTo = (int)llen;
                //    for (i = 2; i <= loopTo; i++)
                //    {
                //        // コマンド関連ラベルの最後のパラメータは条件式なのでチェックを省く
                //        if (is_condition[i])
                //        {
                //            break;
                //        }

                //        // 比較するパラメータ
                //        str1 = litem[i];
                //        if (reversed)
                //        {
                //            str2 = lab.Para((short)(5 - i));
                //        }
                //        else
                //        {
                //            str2 = lab.Para((short)i);
                //        }

                //        // 「全」は全てに一致
                //        if (str2 == "全")
                //        {
                //            // だだし、「ターン 全」が２回実行されるのは防ぐ
                //            if (ltype != LabelType.TurnEventLabel | i != 2)
                //            {
                //                goto NextPara;
                //            }
                //        }

                //        // 数値として比較？
                //        if (is_num[i])
                //        {
                //            if (Information.IsNumeric(str2))
                //            {
                //                if (Conversions.ToDouble(lnum[i]) == Conversions.ToInteger(str2))
                //                {
                //                    goto NextPara;
                //                }
                //                else if (ltype == LabelType.DamageEventLabel)
                //                {
                //                    // 損傷率ラベルの処理
                //                    if (Conversions.ToDouble(lnum[i]) > Conversions.ToInteger(str2))
                //                    {
                //                        break;
                //                    }
                //                }
                //            }

                //            continue;
                //        }

                //        // ユニット指定として比較？
                //        if (is_unit[i])
                //        {
                //            bool localIsDefined() { object argIndex1 = str2; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                //            bool localIsDefined1() { object argIndex1 = str2; var ret = SRC.PDList.IsDefined(argIndex1); return ret; }

                //            bool localIsDefined2() { object argIndex1 = str2; var ret = SRC.UDList.IsDefined(argIndex1); return ret; }

                //            if (str2 == "味方" | str2 == "ＮＰＣ" | str2 == "敵" | str2 == "中立")
                //            {
                //                // 陣営名で比較
                //                if (str1 != "味方" & str1 != "ＮＰＣ" & str1 != "敵" & str1 != "中立")
                //                {
                //                    object argIndex1 = str1;
                //                    if (SRC.PList.IsDefined(argIndex1))
                //                    {
                //                        Pilot localItem() { object argIndex1 = str1; var ret = SRC.PList.Item(argIndex1); return ret; }

                //                        str1 = localItem().Party;
                //                    }
                //                }
                //            }
                //            else if (localIsDefined())
                //            {
                //                // パイロットで比較
                //                object argIndex5 = str2;
                //                {
                //                    var withBlock = SRC.PList.Item(argIndex5);
                //                    if ((str2 ?? "") == (withBlock.Data.Name ?? "") | (str2 ?? "") == (withBlock.Data.Nickname ?? ""))
                //                    {
                //                        // グループＩＤが付けられていない場合は
                //                        // パイロット名で比較
                //                        str2 = withBlock.Name;
                //                        object argIndex3 = str1;
                //                        if (SRC.PList.IsDefined(argIndex3))
                //                        {
                //                            Pilot localItem2() { object argIndex1 = str1; var ret = SRC.PList.Item(argIndex1); return ret; }

                //                            str1 = localItem2().Name;
                //                        }
                //                    }
                //                    else
                //                    {
                //                        // グループＩＤが付けられている場合は
                //                        // グループＩＤで比較
                //                        object argIndex4 = str1;
                //                        if (SRC.PList.IsDefined(argIndex4))
                //                        {
                //                            Pilot localItem3() { object argIndex1 = str1; var ret = SRC.PList.Item(argIndex1); return ret; }

                //                            str1 = localItem3().ID;
                //                        }

                //                        if (Strings.InStr(str1, ":") > 0)
                //                        {
                //                            str1 = Strings.Left(str1, Strings.InStr(str1, ":") - 1);
                //                        }
                //                    }
                //                }
                //            }
                //            else if (localIsDefined1())
                //            {
                //                // パイロット名で比較
                //                PilotData localItem4() { object argIndex1 = str2; var ret = SRC.PDList.Item(argIndex1); return ret; }

                //                str2 = localItem4().Name;
                //                object argIndex6 = str1;
                //                if (SRC.PList.IsDefined(argIndex6))
                //                {
                //                    Pilot localItem5() { object argIndex1 = str1; var ret = SRC.PList.Item(argIndex1); return ret; }

                //                    str1 = localItem5().Name;
                //                }
                //            }
                //            else if (localIsDefined2())
                //            {
                //                // ユニット名で比較
                //                object argIndex8 = str1;
                //                if (SRC.PList.IsDefined(argIndex8))
                //                {
                //                    object argIndex7 = str1;
                //                    {
                //                        var withBlock1 = SRC.PList.Item(argIndex7);
                //                        if (withBlock1.Unit_Renamed is object)
                //                        {
                //                            str1 = withBlock1.Unit_Renamed.Name;
                //                        }
                //                    }
                //                }
                //            }
                //            else
                //            {
                //                // グループＩＤが付けられているおり、なおかつ同じＩＤの
                //                // ２番目以降のユニットの場合はグループＩＤで比較
                //                object argIndex2 = str1;
                //                if (SRC.PList.IsDefined(argIndex2))
                //                {
                //                    Pilot localItem1() { object argIndex1 = str1; var ret = SRC.PList.Item(argIndex1); return ret; }

                //                    str1 = localItem1().ID;
                //                }

                //                if (Strings.InStr(str1, ":") > 0)
                //                {
                //                    str1 = Strings.Left(str1, Strings.InStr(str1, ":") - 1);
                //                }

                //                if (Strings.InStr(str2, ":") > 0)
                //                {
                //                    str2 = Strings.Left(str2, Strings.InStr(str2, ":") - 1);
                //                }
                //            }
                //        }

                //        // 一致したか？
                //        if ((str1 ?? "") != (str2 ?? ""))
                //        {
                //            if (revrersible & !reversed)
                //            {
                //                // 対象と相手を入れ替えたイベントラベルが存在するか判定
                //                string localListIndex() { string arglist = lab.Data; var ret = GeneralLib.ListIndex(arglist, 3); lab.Data = arglist; return ret; }

                //                string localListIndex1() { string arglist = lab.Data; var ret = GeneralLib.ListIndex(arglist, 2); lab.Data = arglist; return ret; }

                //                lname2 = litem[1] + " " + localListIndex() + " " + localListIndex1();
                //                if (lab.AsterNum > 0)
                //                {
                //                    lname2 = "*" + lname2;
                //                }

                //                if (FindLabel(lname2) == 0)
                //                {
                //                    // 対象と相手を入れ替えて判定し直す
                //                    reversed = true;
                //                    goto CheckPara;
                //                }
                //            }

                //            continue;
                //        }

                //    NextPara:
                //        ;
                //    }

                // ここまでたどり付けばラベルは一致している
                SearchLabelRet = lab.EventDataId;
                SRC.LogDebug("Found", lab.Name.ToString(), lab.Data);

                //// 対象と相手を入れ替えて一致した場合はグローバル変数も入れ替え
                //if (reversed)
                //{
                //    tmp_u = SelectedUnitForEvent;
                //    SelectedUnitForEvent = SelectedTargetForEvent;
                //    SelectedTargetForEvent = tmp_u;
                //}

                return SearchLabelRet;
            }

            return -1;
        }

        // 指定したイベントへのイベントラベルが定義されているか
        // 常時イベントではない通常イベントのみを探す場合は
        // normal_event_only = True を指定する
        public bool IsEventDefined(string lname, bool normal_event_only = false)
        {
            // イベントラベルを探す
            int i = 0;
            while (true)
            {
                var ret = SearchLabel(lname, i + 1);
                if (ret < 0)
                {
                    return false;
                }

                if (normal_event_only)
                {
                    // 常時イベントではない通常イベントのみを探す場合
                    if (!EventData[ret].IsAlwaysEventLabel)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }

                i = ret;
            }
        }

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
                return colNormalLabelList[lname]?.EventDataId ?? -1;
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
                return colSysNormalLabelList[lname]?.EventDataId ?? -1;
            }
            catch
            {
                // オフセットは1->0になっている
                return -1;
            }
        }
    }
}
