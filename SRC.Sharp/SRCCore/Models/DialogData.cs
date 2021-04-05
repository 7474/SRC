// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Extensions;
using SRCCore.Units;
using System.Collections.Generic;

namespace SRCCore.Models
{
    // 特定のパイロットに指定された全ダイアログのクラス
    public class DialogData
    {
        // パイロット一覧
        public string Name;

        // ダイアログ一覧
        private IList<Dialog> Dialoges = new List<Dialog>();

        public IList<Dialog> Items => Dialoges;

        private SRC SRC;
        public DialogData(SRC src)
        {
            SRC = src;
        }

        // ダイアログを追加
        public Dialog AddDialog(string msg_situation)
        {
            var new_dialog = new Dialog
            {
                Situation = msg_situation,
            };
            Dialoges.Add(new_dialog);
            return new_dialog;
        }

        // ダイアログの総数
        public int CountDialog()
        {
            return Dialoges.Count;
        }

        //// idx番目のシチュエーション
        //public string Situation(int idx)
        //{
        //    string SituationRet = default;
        //    SituationRet = strSituation[idx];
        //    return SituationRet;
        //}

        //// idx番目のダイアログ
        //public Dialog Dialog(int idx)
        //{
        //    Dialog DialogRet = default;
        //    DialogRet = Dialoges[idx];
        //    return DialogRet;
        //}

        // ユニット u のシチュエーション msg_situation におけるダイアログを選択
        public Dialog SelectDialog(string msg_situation, Unit u, bool ignore_condition = false)
        {
            // Impl ignore_condition
            return Items.SelectMessage(SRC, msg_situation, u);
        }
        //    Dialog SelectDialogRet = default;
        //    string[] situations;
        //    string[] sub_situations;
        //    int[] list0;
        //    int list0_num;
        //    int[] tlist;
        //    int tlist_num;
        //    int[] list;
        //    int list_num;
        //    int j, i, k;
        //    bool found;
        //    var t = default(Unit);
        //    int w, tw;

        //    // 配列領域確保
        //    list0 = new int[301];
        //    tlist = new int[101];
        //    list = new int[201];

        //    // シチュエーションを設定
        //    switch (msg_situation ?? "")
        //    {
        //        case "格闘":
        //        case "射撃":
        //            {
        //                situations = new string[3];
        //                situations[2] = "攻撃";
        //                break;
        //            }

        //        case "格闘(命中)":
        //        case "射撃(命中)":
        //            {
        //                situations = new string[3];
        //                situations[2] = "攻撃(命中)";
        //                break;
        //            }

        //        case "格闘(回避)":
        //        case "射撃(回避)":
        //            {
        //                situations = new string[3];
        //                situations[2] = "攻撃(回避)";
        //                break;
        //            }

        //        case "格闘(とどめ)":
        //        case "射撃(とどめ)":
        //            {
        //                situations = new string[3];
        //                situations[2] = "攻撃(とどめ)";
        //                break;
        //            }

        //        case "格闘(クリティカル)":
        //        case "射撃(クリティカル)":
        //            {
        //                situations = new string[3];
        //                situations[2] = "攻撃(クリティカル)";
        //                break;
        //            }

        //        case "格闘(反撃)":
        //        case "射撃(反撃)":
        //            {
        //                situations = new string[3];
        //                situations[2] = "攻撃(反撃)";
        //                break;
        //            }

        //        case "格闘(命中)(反撃)":
        //        case "射撃(命中)(反撃)":
        //            {
        //                situations = new string[3];
        //                situations[2] = "攻撃(命中)(反撃)";
        //                break;
        //            }

        //        case "格闘(回避)(反撃)":
        //        case "射撃(回避)(反撃)":
        //            {
        //                situations = new string[3];
        //                situations[2] = "攻撃(回避)(反撃)";
        //                break;
        //            }

        //        case "格闘(とどめ)(反撃)":
        //        case "射撃(とどめ)(反撃)":
        //            {
        //                situations = new string[3];
        //                situations[2] = "攻撃(とどめ)(反撃)";
        //                break;
        //            }

        //        case "格闘(クリティカル)(反撃)":
        //        case "射撃(クリティカル)(反撃)":
        //            {
        //                situations = new string[3];
        //                situations[2] = "攻撃(クリティカル)(反撃)";
        //                break;
        //            }

        //        default:
        //            {
        //                situations = new string[2];
        //                break;
        //            }
        //    }

        //    situations[1] = msg_situation;

        //    // メッセージの候補リスト第一次審査
        //    list0_num = 0;
        //    var loopTo = intDialogNum;
        //    for (i = 1; i <= loopTo; i++)
        //    {
        //        var loopTo1 = Information.UBound(situations);
        //        for (j = 1; j <= loopTo1; j++)
        //        {
        //            if ((Strings.Left(strSituation[i], Strings.Len(situations[j])) ?? "") == (situations[j] ?? ""))
        //            {
        //                if (Dialoges[i].get_IsAvailable(u, ignore_condition))
        //                {
        //                    list0_num = (list0_num + 1);
        //                    if (list0_num > Information.UBound(list0))
        //                    {
        //                        Array.Resize(list0, list0_num + 1);
        //                    }

        //                    list0[list0_num] = i;
        //                }

        //                break;
        //            }
        //        }
        //    }

        //    if (list0_num == 0)
        //    {
        //        return SelectDialogRet;
        //    }

        //    // 最初に相手限定のシチュエーションのみで検索
        //    if (ReferenceEquals(u, Commands.SelectedUnit))
        //    {
        //        t = Commands.SelectedTarget;
        //    }
        //    else if (ReferenceEquals(u, Commands.SelectedTarget))
        //    {
        //        t = Commands.SelectedUnit;
        //    }

        //    if (t is null)
        //    {
        //        goto SkipMessagesWithTarget;
        //    }

        //    // 相手限定メッセージのリストを作成
        //    tlist_num = 0;
        //    var loopTo2 = list0_num;
        //    for (i = 1; i <= loopTo2; i++)
        //    {
        //        if (Strings.InStr(strSituation[list0[i]], "(対") > 0)
        //        {
        //            tlist_num = (tlist_num + 1);
        //            if (tlist_num > Information.UBound(tlist))
        //            {
        //                Array.Resize(tlist, tlist_num + 1);
        //            }

        //            tlist[tlist_num] = list0[i];
        //        }
        //    }

        //    if (tlist_num == 0)
        //    {
        //        // 相手限定メッセージがない
        //        goto SkipMessagesWithTarget;
        //    }

        //    // 自分自身にアビリティを使う場合は必ず「(対自分)」を優先
        //    if (ReferenceEquals(t, u))
        //    {
        //        list_num = 0;
        //        var loopTo3 = tlist_num;
        //        for (i = 1; i <= loopTo3; i++)
        //        {
        //            var loopTo4 = Information.UBound(situations);
        //            for (j = 1; j <= loopTo4; j++)
        //            {
        //                if ((strSituation[tlist[i]] ?? "") == (situations[j] + "(対自分)" ?? ""))
        //                {
        //                    list_num = (list_num + 1);
        //                    if (list_num > Information.UBound(list))
        //                    {
        //                        Array.Resize(list, list_num + 1);
        //                    }

        //                    list[list_num] = tlist[i];
        //                    break;
        //                }
        //            }
        //        }

        //        if (list_num > 0)
        //        {
        //            SelectDialogRet = Dialoges[list[GeneralLib.Dice(list_num)]];
        //            return SelectDialogRet;
        //        }
        //    }

        //    string wclass, ch;
        //    {
        //        var withBlock = t;
        //        if (withBlock.Status != "出撃")
        //        {
        //            goto SkipMessagesWithTarget;
        //        }

        //        sub_situations = new string[9];
        //        // 対パイロット名称
        //        sub_situations[1] = "(対" + withBlock.MainPilot().Name + ")";
        //        // 対パイロット愛称
        //        sub_situations[2] = "(対" + withBlock.MainPilot().get_Nickname(false) + ")";
        //        // 対ユニット名称
        //        sub_situations[3] = "(対" + withBlock.Name + ")";
        //        // 対ユニット愛称
        //        sub_situations[4] = "(対" + withBlock.Nickname + ")";
        //        // 対ユニットクラス
        //        sub_situations[5] = "(対" + withBlock.Class0 + ")";
        //        // 対ユニットサイズ
        //        sub_situations[6] = "(対" + withBlock.Size + ")";
        //        // 対地形名
        //        sub_situations[7] = "(対" + Map.TerrainName(withBlock.x, withBlock.y) + ")";
        //        // 対エリア
        //        sub_situations[8] = "(対" + withBlock.Area + ")";

        //        // 対メッセージクラス
        //        if (withBlock.IsFeatureAvailable("メッセージクラス"))
        //        {
        //            Array.Resize(sub_situations, Information.UBound(sub_situations) + 1 + 1);
        //            sub_situations[Information.UBound(sub_situations)] = "(対" + withBlock.FeatureData("メッセージクラス") + ")";
        //        }

        //        // 対性別
        //        switch (withBlock.MainPilot().Sex ?? "")
        //        {
        //            case "男性":
        //                {
        //                    Array.Resize(sub_situations, Information.UBound(sub_situations) + 1 + 1);
        //                    sub_situations[Information.UBound(sub_situations)] = "(対男性)";
        //                    break;
        //                }

        //            case "女性":
        //                {
        //                    Array.Resize(sub_situations, Information.UBound(sub_situations) + 1 + 1);
        //                    sub_situations[Information.UBound(sub_situations)] = "(対女性)";
        //                    break;
        //                }
        //        }

        //        // 対特殊能力
        //        {
        //            var withBlock1 = withBlock.MainPilot();
        //            var loopTo5 = withBlock1.CountSkill();
        //            for (i = 1; i <= loopTo5; i++)
        //            {
        //                Array.Resize(sub_situations, Information.UBound(sub_situations) + 1 + 1);
        //                string localSkillName0() { object argIndex1 = i; var ret = withBlock1.SkillName0(argIndex1); return ret; }

        //                sub_situations[Information.UBound(sub_situations)] = "(対" + localSkillName0() + ")";
        //                if (sub_situations[Information.UBound(sub_situations)] == "(対非表示)")
        //                {
        //                    string localSkill() { object argIndex1 = i; var ret = withBlock1.Skill(argIndex1); return ret; }

        //                    sub_situations[Information.UBound(sub_situations)] = "(対" + localSkill() + ")";
        //                }
        //            }
        //        }

        //        var loopTo6 = withBlock.CountFeature();
        //        for (i = 1; i <= loopTo6; i++)
        //        {
        //            Array.Resize(sub_situations, Information.UBound(sub_situations) + 1 + 1);
        //            string localFeatureName0() { object argIndex1 = i; var ret = withBlock.FeatureName0(argIndex1); return ret; }

        //            sub_situations[Information.UBound(sub_situations)] = "(対" + localFeatureName0() + ")";
        //            if (sub_situations[Information.UBound(sub_situations)] == "(対)")
        //            {
        //                string localFeature() { object argIndex1 = i; var ret = withBlock.Feature(argIndex1); return ret; }

        //                sub_situations[Information.UBound(sub_situations)] = "(対" + localFeature() + ")";
        //            }
        //        }

        //        // 対弱点
        //        if (Strings.Len(withBlock.strWeakness) > 0)
        //        {
        //            var loopTo7 = Strings.Len(withBlock.strWeakness);
        //            for (i = 1; i <= loopTo7; i++)
        //            {
        //                Array.Resize(sub_situations, Information.UBound(sub_situations) + 1 + 1);
        //                sub_situations[Information.UBound(sub_situations)] = "(対弱点=" + Strings.Mid(withBlock.strWeakness, i, 1) + ")";
        //            }
        //        }

        //        // 対有効
        //        if (Strings.Len(withBlock.strEffective) > 0)
        //        {
        //            var loopTo8 = Strings.Len(withBlock.strEffective);
        //            for (i = 1; i <= loopTo8; i++)
        //            {
        //                Array.Resize(sub_situations, Information.UBound(sub_situations) + 1 + 1);
        //                sub_situations[Information.UBound(sub_situations)] = "(対弱点=" + Strings.Mid(withBlock.strEffective, i, 1) + ")";
        //            }
        //        }

        //        // 対ザコ
        //        if (Strings.InStr(withBlock.MainPilot().Name, "(ザコ)") > 0 & (u.MainPilot().Technique > withBlock.MainPilot().Technique | u.HP > withBlock.HP / 2))
        //        {
        //            Array.Resize(sub_situations, Information.UBound(sub_situations) + 1 + 1);
        //            sub_situations[Information.UBound(sub_situations)] = "(対ザコ)";
        //        }

        //        // 対強敵
        //        if (withBlock.BossRank >= 0 | Strings.InStr(withBlock.MainPilot().Name, "(ザコ)") == 0 & u.MainPilot().Technique <= withBlock.MainPilot().Technique)
        //        {
        //            Array.Resize(sub_situations, Information.UBound(sub_situations) + 1 + 1);
        //            sub_situations[Information.UBound(sub_situations)] = "(対強敵)";
        //        }

        //        // 自分が使用する武器をチェック
        //        w = 0;
        //        if (ReferenceEquals(Commands.SelectedUnit, u))
        //        {
        //            if (0 < Commands.SelectedWeapon & Commands.SelectedWeapon <= u.CountWeapon())
        //            {
        //                w = Commands.SelectedWeapon;
        //            }
        //        }
        //        else if (ReferenceEquals(Commands.SelectedTarget, u))
        //        {
        //            if (0 < Commands.SelectedTWeapon & Commands.SelectedTWeapon <= u.CountWeapon())
        //            {
        //                w = Commands.SelectedTWeapon;
        //            }
        //        }

        //        if (w > 0)
        //        {
        //            // 対瀕死
        //            if (withBlock.HP <= u.Damage(w, t, u.Party == "味方"))
        //            {
        //                Array.Resize(sub_situations, Information.UBound(sub_situations) + 1 + 1);
        //                sub_situations[Information.UBound(sub_situations)] = "(対瀕死)";
        //            }

        //            switch (u.HitProbability(w, t, u.Party == "味方"))
        //            {
        //                case var @case when @case < 50:
        //                    {
        //                        // 対高回避率
        //                        Array.Resize(sub_situations, Information.UBound(sub_situations) + 1 + 1);
        //                        sub_situations[Information.UBound(sub_situations)] = "(対高回避率)";
        //                        break;
        //                    }

        //                case var case1 when case1 >= 100:
        //                    {
        //                        // 対低回避率
        //                        Array.Resize(sub_situations, Information.UBound(sub_situations) + 1 + 1);
        //                        sub_situations[Information.UBound(sub_situations)] = "(対低回避率)";
        //                        break;
        //                    }
        //            }
        //        }

        //        // 相手が使用する武器をチェック
        //        tw = 0;
        //        if (ReferenceEquals(Commands.SelectedUnit, t))
        //        {
        //            if (0 < Commands.SelectedWeapon & Commands.SelectedWeapon <= withBlock.CountWeapon())
        //            {
        //                tw = Commands.SelectedWeapon;
        //            }
        //        }
        //        else if (ReferenceEquals(Commands.SelectedTarget, t))
        //        {
        //            if (0 < Commands.SelectedTWeapon & Commands.SelectedTWeapon <= withBlock.CountWeapon())
        //            {
        //                tw = Commands.SelectedTWeapon;
        //            }
        //        }

        //        if (tw > 0)
        //        {
        //            // 対武器名
        //            Array.Resize(sub_situations, Information.UBound(sub_situations) + 1 + 1);
        //            sub_situations[Information.UBound(sub_situations)] = "(対" + withBlock.Weapon(tw).Name + ")";

        //            // 対武器属性
        //            wclass = withBlock.WeaponClass(tw);
        //            var loopTo9 = Strings.Len(wclass);
        //            for (i = 1; i <= loopTo9; i++)
        //            {
        //                ch = Strings.Mid(wclass, i, 1);
        //                switch (ch ?? "")
        //                {
        //                    case var case2 when 0.ToString() <= case2 && case2 <= 127.ToString():
        //                        {
        //                            break;
        //                        }

        //                    default:
        //                        {
        //                            Array.Resize(sub_situations, Information.UBound(sub_situations) + 1 + 1);
        //                            sub_situations[Information.UBound(sub_situations)] = "(対" + ch + "属性)";
        //                            break;
        //                        }
        //                }
        //            }

        //            switch (withBlock.HitProbability(tw, u, withBlock.Party == "味方"))
        //            {
        //                case var case3 when case3 > 75:
        //                    {
        //                        // 対高命中率
        //                        Array.Resize(sub_situations, Information.UBound(sub_situations) + 1 + 1);
        //                        sub_situations[Information.UBound(sub_situations)] = "(対高命中率)";
        //                        break;
        //                    }

        //                case var case4 when case4 < 25:
        //                    {
        //                        // 対低命中率
        //                        Array.Resize(sub_situations, Information.UBound(sub_situations) + 1 + 1);
        //                        sub_situations[Information.UBound(sub_situations)] = "(対低命中率)";
        //                        break;
        //                    }
        //            }
        //        }
        //    }

        //    // 定義されている相手限定メッセージのうち、状況に合ったメッセージを抜き出す
        //    list_num = 0;
        //    var loopTo10 = tlist_num;
        //    for (i = 1; i <= loopTo10; i++)
        //    {
        //        found = false;
        //        var loopTo11 = Information.UBound(situations);
        //        for (j = 1; j <= loopTo11; j++)
        //        {
        //            var loopTo12 = Information.UBound(sub_situations);
        //            for (k = 1; k <= loopTo12; k++)
        //            {
        //                if ((strSituation[tlist[i]] ?? "") == (situations[j] + sub_situations[k] ?? ""))
        //                {
        //                    found = true;
        //                    break;
        //                }
        //            }

        //            if (found)
        //            {
        //                break;
        //            }
        //        }

        //        if (found)
        //        {
        //            list_num = (list_num + 1);
        //            if (list_num > Information.UBound(list))
        //            {
        //                Array.Resize(list, list_num + 1);
        //            }

        //            list[list_num] = tlist[i];
        //        }
        //    }

        //    // 状況に合った相手限定メッセージが一つでもあれば、その中からメッセージを選択
        //    if (list_num > 0)
        //    {
        //        SelectDialogRet = Dialoges[list[GeneralLib.Dice(list_num)]];
        //        if (GeneralLib.Dice(2) == 1 | Strings.InStr(msg_situation, "(とどめ)") > 0 | msg_situation == "挑発" | msg_situation == "脱力" | msg_situation == "魅惑" | msg_situation == "威圧" | (u.Party ?? "") == (t.Party ?? ""))
        //        {
        //            return SelectDialogRet;
        //        }
        //    }

        //SkipMessagesWithTarget:
        //    ;


        //    // 次にサブシチュエーションなしとユニット限定のサブシチュエーションで検索
        //    if (u is object)
        //    {
        //        sub_situations = new string[4];
        //        sub_situations[1] = "(" + u.Name + ")";
        //        sub_situations[2] = "(" + u.Nickname0 + ")";
        //        sub_situations[3] = "(" + u.Class0 + ")";
        //        switch (msg_situation ?? "")
        //        {
        //            case "格闘":
        //            case "射撃":
        //            case "格闘(反撃)":
        //            case "射撃(反撃)":
        //                {
        //                    if (ReferenceEquals(Commands.SelectedUnit, u))
        //                    {
        //                        // 自分が使用する武器をチェック
        //                        if (0 < Commands.SelectedWeapon & Commands.SelectedWeapon <= u.CountWeapon())
        //                        {
        //                            Array.Resize(sub_situations, 5);
        //                            sub_situations[4] = "(" + u.WeaponNickname(Commands.SelectedWeapon) + ")";
        //                        }
        //                    }

        //                    break;
        //                }
        //        }

        //        if (u.IsFeatureAvailable("メッセージクラス"))
        //        {
        //            Array.Resize(sub_situations, Information.UBound(sub_situations) + 1 + 1);
        //            sub_situations[Information.UBound(sub_situations)] = "(" + u.FeatureData("メッセージクラス") + ")";
        //        }
        //    }
        //    else
        //    {
        //        sub_situations = new string[1];
        //    }

        //    // 上で見つかったメッセージリストの中からシチュエーションに合ったメッセージを抜き出す
        //    list_num = 0;
        //    var loopTo13 = list0_num;
        //    for (i = 1; i <= loopTo13; i++)
        //    {
        //        found = false;
        //        var loopTo14 = Information.UBound(situations);
        //        for (j = 1; j <= loopTo14; j++)
        //        {
        //            if ((strSituation[list0[i]] ?? "") == (situations[j] ?? ""))
        //            {
        //                found = true;
        //                break;
        //            }

        //            var loopTo15 = Information.UBound(sub_situations);
        //            for (k = 1; k <= loopTo15; k++)
        //            {
        //                if ((strSituation[list0[i]] ?? "") == (situations[j] + sub_situations[k] ?? ""))
        //                {
        //                    found = true;
        //                    break;
        //                }
        //            }

        //            if (found)
        //            {
        //                break;
        //            }
        //        }

        //        if (found)
        //        {
        //            list_num = (list_num + 1);
        //            if (list_num > Information.UBound(list))
        //            {
        //                Array.Resize(list, list_num + 1);
        //            }

        //            list[list_num] = list0[i];
        //        }
        //    }

        //    // シチュエーションに合ったメッセージが見つかれば、その中からメッセージを選択
        //    if (list_num > 0)
        //    {
        //        SelectDialogRet = Dialoges[list[GeneralLib.Dice(list_num)]];
        //    }

        //    return SelectDialogRet;
        //}
    }
}
