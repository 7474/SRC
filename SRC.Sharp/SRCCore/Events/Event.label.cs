using SRCCore.Lib;
using SRCCore.VB;
using System.Linq;

namespace SRCCore.Events
{
    public partial class Event
    {
        // ラベルの検索
        public int SearchLabel(string lname, int start = -1)
        {
            // ラベルの各要素をあらかじめ解析
            // XXX Indexがずれていて辛い。
            string[] litem;
            int llen = GeneralLib.ListSplit(lname, out litem);
            // XXX 死にたい
            litem = (new string[] { "" }).Concat(litem).ToArray();

            // ラベルの種類を判定
            LabelType ltype;
            var lnum = new string[5];
            var is_unit = new bool[5];
            var is_num = new bool[5];
            var is_condition = new bool[5];
            var revrersible = false;
            switch (litem[1] ?? "")
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

                case "ターン":
                    ltype = LabelType.TurnEventLabel;
                    if (Information.IsNumeric(litem[2]))
                    {
                        is_num[2] = true;
                    }
                    lnum[2] = GeneralLib.StrToLng(litem[2]).ToString();
                    break;

                case "損傷率":
                    ltype = LabelType.DamageEventLabel;
                    is_unit[2] = true;
                    is_num[3] = true;
                    lnum[3] = GeneralLib.StrToLng(litem[3]).ToString();
                    break;

                case "破壊":
                case "マップ攻撃破壊":
                    ltype = LabelType.DestructionEventLabel;
                    is_unit[2] = true;
                    break;

                case "全滅":
                    ltype = LabelType.TotalDestructionEventLabel;
                    break;

                case "攻撃":
                    ltype = LabelType.AttackEventLabel;
                    revrersible = true;
                    is_unit[2] = true;
                    is_unit[3] = true;
                    break;

                case "攻撃後":
                    ltype = LabelType.AfterAttackEventLabel;
                    revrersible = true;
                    is_unit[2] = true;
                    is_unit[3] = true;
                    break;

                case "会話":
                    ltype = LabelType.TalkEventLabel;
                    is_unit[2] = true;
                    is_unit[3] = true;
                    break;

                case "接触":
                    ltype = LabelType.ContactEventLabel;
                    revrersible = true;
                    is_unit[2] = true;
                    is_unit[3] = true;
                    break;

                case "進入":
                    ltype = LabelType.EnterEventLabel;
                    is_unit[2] = true;
                    if (llen == 4)
                    {
                        is_num[3] = true;
                        is_num[4] = true;
                        lnum[3] = GeneralLib.StrToLng(litem[3]).ToString();
                        lnum[4] = GeneralLib.StrToLng(litem[4]).ToString();
                    }
                    break;

                case "脱出":
                    ltype = LabelType.EscapeEventLabel;
                    is_unit[2] = true;
                    break;

                case "収納":
                    ltype = LabelType.LandEventLabel;
                    is_unit[2] = true;
                    break;

                case "使用":
                    ltype = LabelType.UseEventLabel;
                    is_unit[2] = true;
                    break;

                case "使用後":
                    ltype = LabelType.AfterUseEventLabel;
                    is_unit[2] = true;
                    break;

                case "変形":
                    ltype = LabelType.TransformEventLabel;
                    is_unit[2] = true;
                    break;

                case "合体":
                    ltype = LabelType.CombineEventLabel;
                    is_unit[2] = true;
                    break;

                case "分離":
                    ltype = LabelType.SplitEventLabel;
                    is_unit[2] = true;
                    break;

                case "行動終了":
                    ltype = LabelType.FinishEventLabel;
                    is_unit[2] = true;
                    break;

                case "レベルアップ":
                    ltype = LabelType.LevelUpEventLabel;
                    is_unit[2] = true;
                    break;

                case "勝利条件":
                    ltype = LabelType.RequirementEventLabel;
                    break;

                case "再開":
                    ltype = LabelType.ResumeEventLabel;
                    break;

                case "マップコマンド":
                    ltype = LabelType.MapCommandEventLabel;
                    is_condition[3] = true;
                    break;

                case "ユニットコマンド":
                    ltype = LabelType.UnitCommandEventLabel;
                    is_condition[4] = true;
                    break;

                case "特殊効果":
                    ltype = LabelType.EffectEventLabel;
                    break;

                default:
                    ltype = LabelType.NormalLabel;
                    break;
            }

            // 各ラベルについて一致しているかチェック
            foreach (LabelData lab in colEventLabelList.List)
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
                    if (ltype != LabelType.MapCommandEventLabel && ltype != LabelType.UnitCommandEventLabel)
                    {
                        continue;
                    }
                }

                // 各パラメータが一致している？
                var reversed = false;
                var isMatch = IsMatch(ltype, lab, litem, lnum, is_unit, is_num, is_condition, reversed);
                if (!isMatch && revrersible)
                {
                    // 対象と相手を入れ替えたイベントラベルが存在するか判定
                    var lname2 = litem[1] + " " + GeneralLib.ListIndex(lab.Data, 3) + " " + GeneralLib.ListIndex(lab.Data, 2);
                    if (lab.AsterNum > 0)
                    {
                        lname2 = "*" + lname2;
                    }

                    if (FindLabel(lname2) == 0)
                    {
                        // 対象と相手を入れ替えて判定し直す
                        reversed = true;
                        isMatch = IsMatch(ltype, lab, litem, lnum, is_unit, is_num, is_condition, reversed);
                    }
                }
                if (!isMatch)
                {
                    continue;
                }

                // ここまでたどり付けばラベルは一致している
                SRC.LogDebug("Found", lab.Name.ToString(), lab.Data);

                // 対象と相手を入れ替えて一致した場合はグローバル変数も入れ替え
                if (reversed)
                {
                    var tmp_u = SelectedUnitForEvent;
                    SelectedUnitForEvent = SelectedTargetForEvent;
                    SelectedTargetForEvent = tmp_u;
                }

                return lab.EventDataId;
            }

            return -1;
        }

        private bool IsMatch(
            LabelType ltype,
            LabelData lab,
            string[] litem,
            string[] lnum, bool[] is_unit, bool[] is_num, bool[] is_condition,
            bool reversed)
        {
            string str1;
            string str2;
            // XXX 考えさせられる
            int llen = litem.Length - 1;
            for (var i = 2; i <= llen; i++)
            {
                // コマンド関連ラベルの最後のパラメータは条件式なのでチェックを省く
                if (is_condition[i])
                {
                    return true;
                }

                // 比較するパラメータ
                str1 = litem[i];
                if (reversed)
                {
                    str2 = lab.Para((5 - i));
                }
                else
                {
                    str2 = lab.Para(i);
                }

                // 「全」は全てに一致
                if (str2 == "全")
                {
                    // だだし、「ターン 全」が２回実行されるのは防ぐ
                    // （ターン毎に「ターン 全」「ターン n」の2つのイベントが発行される）
                    if (ltype != LabelType.TurnEventLabel || i != 2)
                    {
                        continue;
                    }
                }

                // 数値として比較？
                if (is_num[i])
                {
                    if (Information.IsNumeric(str2))
                    {
                        if (Conversions.ToDouble(lnum[i]) == Conversions.ToInteger(str2))
                        {

                            continue;
                        }
                        else if (ltype == LabelType.DamageEventLabel)
                        {
                            // 損傷率ラベルの処理
                            if (Conversions.ToDouble(lnum[i]) > Conversions.ToInteger(str2))
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }

                // ユニット指定として比較？
                if (is_unit[i])
                {
                    if (str2 == "味方" || str2 == "ＮＰＣ" || str2 == "敵" || str2 == "中立")
                    {
                        // 陣営名で比較
                        if (str1 != "味方" && str1 != "ＮＰＣ" && str1 != "敵" && str1 != "中立")
                        {
                            if (SRC.PList.IsDefined(str1))
                            {
                                str1 = SRC.PList.Item(str1).Party;
                            }
                        }
                    }
                    else if (SRC.PList.IsDefined(str2))
                    {
                        // パイロットで比較
                        var p = SRC.PList.Item(str2);
                        if ((str2 ?? "") == (p.Data.Name ?? "") || (str2 ?? "") == (p.Data.Nickname ?? ""))
                        {
                            // グループＩＤが付けられていない場合は
                            // パイロット名で比較
                            str2 = p.Name;
                            if (SRC.PList.IsDefined(str1))
                            {
                                str1 = SRC.PList.Item(str1).Name;
                            }
                        }
                        else
                        {
                            // グループＩＤが付けられている場合は
                            // グループＩＤで比較
                            if (SRC.PList.IsDefined(str1))
                            {
                                str1 = SRC.PList.Item(str1).ID;
                            }

                            if (Strings.InStr(str1, ":") > 0)
                            {
                                str1 = Strings.Left(str1, Strings.InStr(str1, ":") - 1);
                            }
                        }
                    }
                    else if (SRC.PDList.IsDefined(str2))
                    {
                        // パイロット名で比較
                        str2 = SRC.PDList.Item(str2).Name;
                        if (SRC.PList.IsDefined(str1))
                        {
                            str1 = SRC.PList.Item(str1).Name;
                        }
                    }
                    else if (SRC.UDList.IsDefined(str2))
                    {
                        // ユニット名で比較
                        if (SRC.PList.IsDefined(str1))
                        {
                            {
                                var u = SRC.PList.Item(str1);
                                if (u.Unit != null)
                                {
                                    str1 = u.Unit.Name;
                                }
                            }
                        }
                    }
                    else
                    {
                        // グループＩＤが付けられているおり、なおかつ同じＩＤの
                        // ２番目以降のユニットの場合はグループＩＤで比較
                        if (SRC.PList.IsDefined(str1))
                        {
                            str1 = SRC.PList.Item(str1).ID;
                        }

                        if (Strings.InStr(str1, ":") > 0)
                        {
                            str1 = Strings.Left(str1, Strings.InStr(str1, ":") - 1);
                        }

                        if (Strings.InStr(str2, ":") > 0)
                        {
                            str2 = Strings.Left(str2, Strings.InStr(str2, ":") - 1);
                        }
                    }
                }

                // 一致したか？
                if ((str1 ?? "") != (str2 ?? ""))
                {
                    return false;
                }
            }
            return true;
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
            return colEventLabelList.ContainsKey(Index);
        }

        // ラベルを追加
        public void AddLabel(string lname, int eventDataId)
        {
            var new_label = new LabelData(SRC);
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
                    var lname3 = lname2 + "(" + SrcFormatter.Format(eventDataId) + ")";
                    if (!IsLabelDefined(lname3))
                    {
                        colEventLabelList.Add(new_label, lname3);
                    }
                }
            }
        }

        // システム側のラベルを追加
        public void AddSysLabel(string lname, int eventDataId)
        {
            var new_label = new LabelData(SRC);
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
                    if (lab.EventDataId == eventDataId - i || lab.EventDataId == eventDataId + i)
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
            // オフセットは1->0になっている
            return colEventLabelList[lname]?.EventDataId ?? -1;
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
            // オフセットは1->0になっている
            return colNormalLabelList[lname]?.EventDataId ?? -1;
        }

        // システム側の通常ラベルを探す
        private int FindSysNormalLabel(string lname)
        {
            // オフセットは1->0になっている
            return colSysNormalLabelList[lname]?.EventDataId ?? -1;
        }
    }
}
