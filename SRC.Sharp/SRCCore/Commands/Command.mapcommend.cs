// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

namespace SRCCore.Commands
{
    public partial class Command
    {
        // マップコマンド実行
        public void MapCommand(int idx)
        {
            CommandState = "ユニット選択";
            switch (idx)
            {
                case EndTurnCmdID: // ターン終了
                    {
                        if (ViewMode)
                        {
                            ViewMode = false;
                            return;
                        }

                        EndTurnCommand();
                        break;
                    }

                case DumpCmdID: // 中断
                    {
                        DumpCommand();
                        break;
                    }

                case UnitListCmdID: // 部隊表
                    {
                        UnitListCommand();
                        break;
                    }

                case SearchSpecialPowerCmdID: // スペシャルパワー検索
                    {
                        SearchSpecialPowerCommand();
                        break;
                    }

                case GlobalMapCmdID: // 全体マップ
                    {
                        GlobalMapCommand();
                        break;
                    }

                case OperationObjectCmdID: // 作戦目的
                    {
                        GUI.LockGUI();
                        Event_Renamed.HandleEvent("勝利条件");
                        GUI.RedrawScreen();
                        GUI.UnlockGUI();
                        break;
                    }

                case var @case when MapCommand1CmdID <= @case && @case <= MapCommand10CmdID: // マップコマンド
                    {
                        GUI.LockGUI();
                        Event_Renamed.HandleEvent(MapCommandLabelList[idx - MapCommand1CmdID + 1]);
                        GUI.UnlockGUI();
                        break;
                    }

                case AutoDefenseCmdID: // 自動反撃モード
                    {
                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                        GUI.MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked = !GUI.MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked;
                        // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                        if (GUI.MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked)
                        {
                            string argini_section = "Option";
                            string argini_entry = "AutoDefense";
                            string argini_data = "On";
                            GeneralLib.WriteIni(ref argini_section, ref argini_entry, ref argini_data);
                        }
                        else
                        {
                            string argini_section1 = "Option";
                            string argini_entry1 = "AutoDefense";
                            string argini_data1 = "Off";
                            GeneralLib.WriteIni(ref argini_section1, ref argini_entry1, ref argini_data1);
                        }

                        break;
                    }

                case ConfigurationCmdID: // 設定変更
                    {
                        // UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
                        Load(My.MyProject.Forms.frmConfiguration);
                        My.MyProject.Forms.frmConfiguration.Left = Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX((Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(Screen.PrimaryScreen.Bounds.Width) - Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(My.MyProject.Forms.frmConfiguration.Width)) / 2d);
                        My.MyProject.Forms.frmConfiguration.Top = Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY((Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(Screen.PrimaryScreen.Bounds.Height) - Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(My.MyProject.Forms.frmConfiguration.Height)) / 3d);
                        My.MyProject.Forms.frmConfiguration.ShowDialog();
                        My.MyProject.Forms.frmConfiguration.Close();
                        // UPGRADE_NOTE: オブジェクト frmConfiguration をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                        My.MyProject.Forms.frmConfiguration = null;
                        break;
                    }

                case RestartCmdID: // リスタート
                    {
                        RestartCommand();
                        break;
                    }

                case QuickLoadCmdID: // クイックロード
                    {
                        QuickLoadCommand();
                        break;
                    }

                case QuickSaveCmdID: // クイックセーブ
                    {
                        QuickSaveCommand();
                        break;
                    }
            }

            SRC.IsScenarioFinished = false;
        }

        // 「ターン終了」コマンド
        // MOD START MARGE
        // Public Sub EndTurnCommand()
        private void EndTurnCommand()
        {
            // MOD END MARGE
            int num;
            int ret;

            // 行動していない味方ユニットの数を数える
            num = 0;
            foreach (Unit u in SRC.UList)
            {
                if (u.Party == "味方" & (u.Status_Renamed == "出撃" | u.Status_Renamed == "格納") & u.Action > 0)
                {
                    num = (num + 1);
                }
            }

            // 行動していないユニットがいれば警告
            if (num > 0)
            {
                ret = Interaction.MsgBox("行動していないユニットが" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(num) + "体あります" + Constants.vbCr + "このターンを終了しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "終了");
            }
            else
            {
                ret = 0;
            }

            switch (ret)
            {
                case 1:
                    {
                        break;
                    }

                case 2:
                    {
                        return;
                    }
            }

            // 行動終了していないユニットに対して行動終了イベントを実施
            foreach (Unit currentSelectedUnit in SRC.UList)
            {
                SelectedUnit = currentSelectedUnit;
                {
                    var withBlock = SelectedUnit;
                    if (withBlock.Party == "味方" & (withBlock.Status_Renamed == "出撃" | withBlock.Status_Renamed == "格納") & withBlock.Action > 0)
                    {
                        Event_Renamed.HandleEvent("行動終了", withBlock.MainPilot().ID);
                        if (SRC.IsScenarioFinished)
                        {
                            SRC.IsScenarioFinished = false;
                            return;
                        }
                    }
                }
            }

            // 各陣営のフェイズに移行

            string arguparty = "敵";
            SRC.StartTurn(ref arguparty);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                return;
            }

            string arguparty1 = "中立";
            SRC.StartTurn(ref arguparty1);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                return;
            }

            string arguparty2 = "ＮＰＣ";
            SRC.StartTurn(ref arguparty2);
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                return;
            }

            // 味方フェイズに戻る
            string arguparty3 = "味方";
            SRC.StartTurn(ref arguparty3);
            SRC.IsScenarioFinished = false;
        }

        // ユニット一覧の表示
        // MOD START MARGE
        // Public Sub UnitListCommand()
        private void UnitListCommand()
        {
            // MOD END MARGE
            string[] list;
            string[] id_list;
            int j, i, ret;
            string uparty, sort_mode;
            string[] mode_list;
            int[] key_list;
            string[] strkey_list;
            int max_item;
            int max_value;
            string max_str;
            string buf;
            Unit u;
            var pilot_status_mode = default(bool);
            GUI.LockGUI();
            GUI.TopItem = 1;
            GUI.EnlargeListBoxHeight();
            GUI.ReduceListBoxWidth();

            // デフォルトのソート方法
            uparty = "味方";
            sort_mode = "レベル";
        Beginning:
            ;


            // ユニット一覧のリストを作成
            list = new string[2];
            id_list = new string[2];
            list[1] = "▽陣営変更・並べ替え▽";
            foreach (Unit currentU in SRC.UList)
            {
                u = currentU;
                {
                    var withBlock = u;
                    if ((withBlock.Party0 ?? "") == (uparty ?? "") & (withBlock.Status_Renamed == "出撃" | withBlock.Status_Renamed == "格納"))
                    {
                        // 未確認のユニットは表示しない
                        string argoname = "ユニット情報隠蔽";
                        object argIndex1 = "識別済み";
                        object argIndex2 = "ユニット情報隠蔽";
                        if (Expression.IsOptionDefined(ref argoname) & !withBlock.IsConditionSatisfied(ref argIndex1) & (withBlock.Party0 == "敵" | withBlock.Party0 == "中立") | withBlock.IsConditionSatisfied(ref argIndex2))
                        {
                            goto NextUnit;
                        }

                        Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                        Array.Resize(ref id_list, Information.UBound(list) + 1);
                        Array.Resize(ref GUI.ListItemFlag, Information.UBound(list) + 1);
                        string argfname = "ダミーユニット";
                        if (!withBlock.IsFeatureAvailable(ref argfname))
                        {
                            // 通常のユニット表示
                            string argoname1 = "等身大基準";
                            if (Expression.IsOptionDefined(ref argoname1))
                            {
                                // 等身大基準を使った場合のユニット表示
                                string localRightPaddedString() { string argbuf = withBlock.Nickname0; var ret = GeneralLib.RightPaddedString(ref argbuf, 33); withBlock.Nickname0 = argbuf; return ret; }

                                string localLeftPaddedString() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.MainPilot().Level); var ret = GeneralLib.LeftPaddedString(ref argbuf, 3); return ret; }

                                list[Information.UBound(list)] = localRightPaddedString() + localLeftPaddedString() + " ";
                            }
                            else
                            {
                                string argbuf = withBlock.Nickname0;
                                list[Information.UBound(list)] = GeneralLib.RightPaddedString(ref argbuf, 23);
                                withBlock.Nickname0 = argbuf;
                                if (withBlock.MainPilot().Nickname0 == "パイロット不在")
                                {
                                    // パイロットが乗っていない場合
                                    string argbuf1 = list[Information.UBound(list)] + "";
                                    string argbuf2 = "";
                                    list[Information.UBound(list)] = GeneralLib.RightPaddedString(ref argbuf1, 34) + GeneralLib.LeftPaddedString(ref argbuf2, 2);
                                }
                                else
                                {
                                    string localLeftPaddedString1() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.MainPilot().Level); var ret = GeneralLib.LeftPaddedString(ref argbuf, 2); return ret; }

                                    string argbuf3 = list[Information.UBound(list)] + withBlock.MainPilot().get_Nickname(false);
                                    list[Information.UBound(list)] = GeneralLib.RightPaddedString(ref argbuf3, 34) + localLeftPaddedString1();
                                }

                                list[Information.UBound(list)] = GeneralLib.RightPaddedString(ref list[Information.UBound(list)], 37);
                            }

                            object argIndex3 = "データ不明";
                            if (withBlock.IsConditionSatisfied(ref argIndex3))
                            {
                                list[Information.UBound(list)] = list[Information.UBound(list)] + "?????/????? ???/???";
                            }
                            else
                            {
                                string localLeftPaddedString2() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.HP); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

                                string localLeftPaddedString3() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.MaxHP); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

                                string localLeftPaddedString4() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.EN); var ret = GeneralLib.LeftPaddedString(ref argbuf, 3); return ret; }

                                string localLeftPaddedString5() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.MaxEN); var ret = GeneralLib.LeftPaddedString(ref argbuf, 3); return ret; }

                                list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString2() + "/" + localLeftPaddedString3() + " " + localLeftPaddedString4() + "/" + localLeftPaddedString5();
                            }
                        }
                        else
                        {
                            // パイロットステータス表示時
                            pilot_status_mode = true;
                            {
                                var withBlock1 = withBlock.MainPilot();
                                string localRightPaddedString1() { string argbuf = withBlock1.get_Nickname(false); var ret = GeneralLib.RightPaddedString(ref argbuf, 21); withBlock1.get_Nickname(false) = argbuf; return ret; }

                                string localLeftPaddedString6() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.Level); var ret = GeneralLib.LeftPaddedString(ref argbuf, 3); return ret; }

                                string localLeftPaddedString7() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.SP) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock1.MaxSP); var ret = GeneralLib.LeftPaddedString(ref argbuf, 9); return ret; }

                                list[Information.UBound(list)] = localRightPaddedString1() + localLeftPaddedString6() + localLeftPaddedString7() + "  ";
                                // 使用可能なスペシャルパワー一覧
                                var loopTo = withBlock1.CountSpecialPower;
                                for (i = 1; i <= loopTo; i++)
                                {
                                    int localSpecialPowerCost() { string argsname = withBlock1.get_SpecialPower(i); var ret = withBlock1.SpecialPowerCost(ref argsname); withBlock1.get_SpecialPower(i) = argsname; return ret; }

                                    if (withBlock1.SP >= localSpecialPowerCost())
                                    {
                                        SpecialPowerData localItem() { object argIndex1 = withBlock1.get_SpecialPower(i); var ret = SRC.SPDList.Item(ref argIndex1); withBlock1.get_SpecialPower(i) = Conversions.ToString(argIndex1); return ret; }

                                        list[Information.UBound(list)] = list[Information.UBound(list)] + localItem().intName;
                                    }
                                }
                            }
                        }

                        if (withBlock.Action == 0)
                        {
                            list[Information.UBound(list)] = list[Information.UBound(list)] + "済";
                        }

                        if (withBlock.Status_Renamed == "格納")
                        {
                            list[Information.UBound(list)] = list[Information.UBound(list)] + "格";
                        }

                        id_list[Information.UBound(list)] = withBlock.ID;
                        GUI.ListItemFlag[Information.UBound(list)] = false;
                    }
                }

            NextUnit:
                ;
            }

        SortList:
            ;


            // ソート
            if (Strings.InStr(sort_mode, "名称") == 0)
            {
                // 数値を使ったソート

                // まず並べ替えに使うキーのリストを作成
                key_list = new int[Information.UBound(list) + 1];
                {
                    var withBlock2 = SRC.UList;
                    switch (sort_mode ?? "")
                    {
                        case "ＨＰ":
                            {
                                var loopTo1 = Information.UBound(list);
                                for (i = 2; i <= loopTo1; i++)
                                {
                                    Unit localItem1() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock2.Item(ref argIndex1); return ret; }

                                    key_list[i] = localItem1().HP;
                                }

                                break;
                            }

                        case "ＥＮ":
                            {
                                var loopTo2 = Information.UBound(list);
                                for (i = 2; i <= loopTo2; i++)
                                {
                                    Unit localItem2() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock2.Item(ref argIndex1); return ret; }

                                    key_list[i] = localItem2().EN;
                                }

                                break;
                            }

                        case "レベル":
                        case "パイロットレベル":
                            {
                                var loopTo3 = Information.UBound(list);
                                for (i = 2; i <= loopTo3; i++)
                                {
                                    var tmp = id_list;
                                    object argIndex4 = tmp[i];
                                    {
                                        var withBlock3 = withBlock2.Item(ref argIndex4);
                                        if (withBlock3.CountPilot() > 0)
                                        {
                                            {
                                                var withBlock4 = withBlock3.MainPilot();
                                                key_list[i] = 500 * withBlock4.Level + withBlock4.Exp;
                                            }
                                        }
                                    }
                                }

                                break;
                            }
                    }
                }

                // キーを使って並べ換え
                var loopTo4 = (Information.UBound(list) - 1);
                for (i = 2; i <= loopTo4; i++)
                {
                    max_item = i;
                    max_value = key_list[i];
                    var loopTo5 = Information.UBound(list);
                    for (j = (i + 1); j <= loopTo5; j++)
                    {
                        if (key_list[j] > max_value)
                        {
                            max_item = j;
                            max_value = key_list[j];
                        }
                    }

                    if (max_item != i)
                    {
                        buf = list[i];
                        list[i] = list[max_item];
                        list[max_item] = buf;
                        buf = id_list[i];
                        id_list[i] = id_list[max_item];
                        id_list[max_item] = buf;
                        key_list[max_item] = key_list[i];
                    }
                }
            }
            else
            {
                // 文字列を使ったソート

                // まず並べ替えに使うキーのリストを作成
                strkey_list = new string[Information.UBound(list) + 1];
                {
                    var withBlock5 = SRC.UList;
                    switch (sort_mode ?? "")
                    {
                        case "名称":
                        case "ユニット名称":
                            {
                                var loopTo6 = Information.UBound(list);
                                for (i = 2; i <= loopTo6; i++)
                                {
                                    Unit localItem3() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock5.Item(ref argIndex1); return ret; }

                                    strkey_list[i] = localItem3().KanaName;
                                }

                                break;
                            }

                        case "パイロット名称":
                            {
                                var loopTo7 = Information.UBound(list);
                                for (i = 2; i <= loopTo7; i++)
                                {
                                    var tmp1 = id_list;
                                    object argIndex5 = tmp1[i];
                                    {
                                        var withBlock6 = withBlock5.Item(ref argIndex5);
                                        if (withBlock6.CountPilot() > 0)
                                        {
                                            strkey_list[i] = withBlock6.MainPilot().KanaName;
                                        }
                                    }
                                }

                                break;
                            }
                    }
                }

                // キーを使って並べ換え
                var loopTo8 = (Information.UBound(strkey_list) - 1);
                for (i = 2; i <= loopTo8; i++)
                {
                    max_item = i;
                    max_str = strkey_list[i];
                    var loopTo9 = Information.UBound(strkey_list);
                    for (j = (i + 1); j <= loopTo9; j++)
                    {
                        if (Strings.StrComp(strkey_list[j], max_str, (CompareMethod)1) == -1)
                        {
                            max_item = j;
                            max_str = strkey_list[j];
                        }
                    }

                    if (max_item != i)
                    {
                        buf = list[i];
                        list[i] = list[max_item];
                        list[max_item] = buf;
                        buf = id_list[i];
                        id_list[i] = id_list[max_item];
                        id_list[max_item] = buf;
                        strkey_list[max_item] = strkey_list[i];
                    }
                }
            }

            GUI.ListItemFlag = new bool[1];
            GUI.ListItemID = new string[Information.UBound(list) + 1];
            var loopTo10 = Information.UBound(list);
            for (i = 1; i <= loopTo10; i++)
                GUI.ListItemID[i] = id_list[i];

            // リストを表示
            string argoname2 = "等身大基準";
            if (pilot_status_mode)
            {
                string arglb_caption = uparty + "パイロット一覧";
                string argtname = "ＳＰ";
                Unit argu = null;
                string argtname1 = "スペシャルパワー";
                Unit argu1 = null;
                string arglb_info = "パイロット名       レベル    " + Expression.Term(ref argtname, ref argu, 4) + "  " + Expression.Term(ref argtname1, u: ref argu1);
                string arglb_mode = "連続表示";
                ret = GUI.ListBox(ref arglb_caption, ref list, ref arglb_info, ref arglb_mode);
            }
            else if (Expression.IsOptionDefined(ref argoname2))
            {
                string arglb_caption2 = uparty + "ユニット一覧";
                string argtname4 = "ＨＰ";
                Unit argu4 = null;
                string argtname5 = "ＥＮ";
                Unit argu5 = null;
                string arglb_info2 = "ユニット名                        Lv     " + Expression.Term(ref argtname4, ref argu4, 8) + Expression.Term(ref argtname5, u: ref argu5);
                string arglb_mode2 = "連続表示";
                ret = GUI.ListBox(ref arglb_caption2, ref list, ref arglb_info2, ref arglb_mode2);
            }
            else
            {
                string arglb_caption1 = uparty + "ユニット一覧";
                string argtname2 = "ＨＰ";
                Unit argu2 = null;
                string argtname3 = "ＥＮ";
                Unit argu3 = null;
                string arglb_info1 = "ユニット               パイロット Lv     " + Expression.Term(ref argtname2, ref argu2, 8) + Expression.Term(ref argtname3, u: ref argu3);
                string arglb_mode1 = "連続表示";
                ret = GUI.ListBox(ref arglb_caption1, ref list, ref arglb_info1, ref arglb_mode1);
            }

            switch (ret)
            {
                case 0:
                    {
                        // キャンセル
                        My.MyProject.Forms.frmListBox.Hide();
                        GUI.ReduceListBoxHeight();
                        GUI.EnlargeListBoxWidth();
                        GUI.ListItemID = new string[1];
                        GUI.UnlockGUI();
                        return;
                    }

                case 1:
                    {
                        // 表示する陣営
                        mode_list = new string[5];
                        mode_list[1] = "味方一覧";
                        mode_list[2] = "ＮＰＣ一覧";
                        mode_list[3] = "敵一覧";
                        mode_list[4] = "中立一覧";

                        // ソート方法を選択
                        string argoname3 = "等身大基準";
                        if (pilot_status_mode)
                        {
                            Array.Resize(ref mode_list, 8);
                            mode_list[5] = "パイロット名称で並べ替え";
                            mode_list[6] = "レベルで並べ替え";
                            string argtname6 = "ＳＰ";
                            Unit argu6 = null;
                            mode_list[7] = Expression.Term(ref argtname6, u: ref argu6) + "で並べ替え";
                        }
                        else if (Expression.IsOptionDefined(ref argoname3))
                        {
                            Array.Resize(ref mode_list, 9);
                            mode_list[5] = "名称で並べ替え";
                            mode_list[6] = "レベルで並べ替え";
                            string argtname9 = "ＨＰ";
                            Unit argu9 = null;
                            mode_list[7] = Expression.Term(ref argtname9, u: ref argu9) + "で並べ替え";
                            string argtname10 = "ＥＮ";
                            Unit argu10 = null;
                            mode_list[8] = Expression.Term(ref argtname10, u: ref argu10) + "で並べ替え";
                        }
                        else
                        {
                            Array.Resize(ref mode_list, 10);
                            string argtname7 = "ＨＰ";
                            Unit argu7 = null;
                            mode_list[5] = Expression.Term(ref argtname7, u: ref argu7) + "で並べ替え";
                            string argtname8 = "ＥＮ";
                            Unit argu8 = null;
                            mode_list[6] = Expression.Term(ref argtname8, u: ref argu8) + "で並べ替え";
                            mode_list[7] = "パイロットレベルで並べ替え";
                            mode_list[8] = "ユニット名称で並べ替え";
                            mode_list[9] = "パイロット名称で並べ替え";
                        }

                        GUI.ListItemID = new string[Information.UBound(mode_list) + 1];
                        GUI.ListItemFlag = new bool[Information.UBound(mode_list) + 1];
                        string arglb_caption3 = "選択";
                        string arglb_info3 = "一覧表示方法";
                        string arglb_mode3 = "連続表示";
                        ret = GUI.ListBox(ref arglb_caption3, ref mode_list, ref arglb_info3, ref arglb_mode3);

                        // 陣営を変更して再表示
                        if (ret > 0)
                        {
                            if (Strings.Right(mode_list[ret], 2) == "一覧")
                            {
                                uparty = Strings.Left(mode_list[ret], Strings.Len(mode_list[ret]) - 2);
                                goto Beginning;
                            }
                            else if (Strings.Right(mode_list[ret], 5) == "で並べ替え")
                            {
                                sort_mode = Strings.Left(mode_list[ret], Strings.Len(mode_list[ret]) - 5);
                                goto SortList;
                            }
                        }

                        goto SortList;
                        break;
                    }
            }

            My.MyProject.Forms.frmListBox.Hide();
            GUI.ReduceListBoxHeight();
            GUI.EnlargeListBoxWidth();

            // 選択されたユニットを画面中央に表示
            var tmp2 = GUI.ListItemID;
            object argIndex6 = tmp2[ret];
            u = SRC.UList.Item(ref argIndex6);
            GUI.Center(u.x, u.y);
            GUI.RefreshScreen();
            Status.DisplayUnitStatus(ref u);

            // カーソル自動移動
            if (SRC.AutoMoveCursor)
            {
                string argcursor_mode = "ユニット選択";
                GUI.MoveCursorPos(ref argcursor_mode, u);
            }

            GUI.ListItemID = new string[1];
            GUI.UnlockGUI();
        }

        // スペシャルパワー検索コマンド
        // MOD START MARGE
        // Public Sub SearchSpecialPowerCommand()
        private void SearchSpecialPowerCommand()
        {
            // MOD END MARGE
            int j, i, ret;
            string[] list;
            string[] list2;
            bool[] flist;
            string[] pid;
            string buf;
            Pilot p;
            string[] id_list;
            var strkey_list = default(string[]);
            int max_item;
            string max_str;
            bool found;
            GUI.LockGUI();

            // イベント専用のコマンドを除いた全スペシャルパワーのリストを作成
            list = new string[1];
            var loopTo = SRC.SPDList.Count();
            for (i = 1; i <= loopTo; i++)
            {
                object argIndex1 = i;
                {
                    var withBlock = SRC.SPDList.Item(ref argIndex1);
                    if (withBlock.intName != "非表示")
                    {
                        Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                        Array.Resize(ref strkey_list, Information.UBound(list) + 1);
                        list[Information.UBound(list)] = withBlock.Name;
                        strkey_list[Information.UBound(list)] = withBlock.KanaName;
                    }
                }
            }

            // ソート
            var loopTo1 = (Information.UBound(strkey_list) - 1);
            for (i = 1; i <= loopTo1; i++)
            {
                max_item = i;
                max_str = strkey_list[i];
                var loopTo2 = Information.UBound(strkey_list);
                for (j = (i + 1); j <= loopTo2; j++)
                {
                    if (Strings.StrComp(strkey_list[j], max_str, (CompareMethod)1) == -1)
                    {
                        max_item = j;
                        max_str = strkey_list[j];
                    }
                }

                if (max_item != i)
                {
                    buf = list[i];
                    list[i] = list[max_item];
                    list[max_item] = buf;
                    buf = strkey_list[i];
                    strkey_list[i] = max_str;
                    strkey_list[max_item] = buf;
                }
            }

            // 個々のスペシャルパワーに対して、そのスペシャルパワーを使用可能なパイロットが
            // いるかどうか判定
            flist = new bool[Information.UBound(list) + 1];
            var loopTo3 = Information.UBound(list);
            for (i = 1; i <= loopTo3; i++)
            {
                flist[i] = true;
                foreach (Pilot currentP in SRC.PList)
                {
                    p = currentP;
                    if (p.Party == "味方")
                    {
                        if (p.Unit_Renamed is object)
                        {
                            object argIndex2 = "憑依";
                            if (p.Unit_Renamed.Status_Renamed == "出撃" & !p.Unit_Renamed.IsConditionSatisfied(ref argIndex2))
                            {
                                // 本当に乗っている？
                                found = false;
                                {
                                    var withBlock1 = p.Unit_Renamed;
                                    if (ReferenceEquals(p, withBlock1.MainPilot()))
                                    {
                                        found = true;
                                    }
                                    else
                                    {
                                        var loopTo4 = withBlock1.CountPilot();
                                        for (j = 2; j <= loopTo4; j++)
                                        {
                                            Pilot localPilot() { object argIndex1 = j; var ret = withBlock1.Pilot(ref argIndex1); return ret; }

                                            if (ReferenceEquals(p, localPilot()))
                                            {
                                                found = true;
                                                break;
                                            }
                                        }

                                        var loopTo5 = withBlock1.CountSupport();
                                        for (j = 1; j <= loopTo5; j++)
                                        {
                                            Pilot localSupport() { object argIndex1 = j; var ret = withBlock1.Support(ref argIndex1); return ret; }

                                            if (ReferenceEquals(p, localSupport()))
                                            {
                                                found = true;
                                                break;
                                            }
                                        }

                                        if (ReferenceEquals(p, withBlock1.AdditionalSupport()))
                                        {
                                            found = true;
                                        }
                                    }
                                }

                                if (found)
                                {
                                    if (p.IsSpecialPowerAvailable(ref list[i]))
                                    {
                                        flist[i] = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            while (true)
            {
                GUI.ListItemFlag = new bool[Information.UBound(list) + 1];
                GUI.ListItemComment = new string[Information.UBound(list) + 1];
                id_list = new string[Information.UBound(list) + 1];
                strkey_list = new string[Information.UBound(list) + 1];

                // 選択出来ないスペシャルパワーをマスク
                var loopTo6 = Information.UBound(GUI.ListItemFlag);
                for (i = 1; i <= loopTo6; i++)
                    GUI.ListItemFlag[i] = flist[i];

                // スペシャルパワーの解説を設定
                var loopTo7 = Information.UBound(GUI.ListItemComment);
                for (i = 1; i <= loopTo7; i++)
                {
                    SpecialPowerData localItem() { var tmp = list; object argIndex1 = tmp[i]; var ret = SRC.SPDList.Item(ref argIndex1); return ret; }

                    GUI.ListItemComment[i] = localItem().Comment;
                }

                // 検索するスペシャルパワーを選択
                GUI.TopItem = 1;
                string argtname = "スペシャルパワー";
                Unit argu = null;
                string arglb_caption = Expression.Term(ref argtname, u: ref argu) + "検索";
                ret = GUI.MultiColumnListBox(ref arglb_caption, ref list, true);
                if (ret == 0)
                {
                    CancelCommand();
                    GUI.UnlockGUI();
                    return;
                }

                SelectedSpecialPower = list[ret];

                // 選択されたスペシャルパワーを使用できるパイロットの一覧を作成
                list2 = new string[1];
                GUI.ListItemFlag = new bool[1];
                id_list = new string[1];
                pid = new string[1];
                foreach (Pilot currentP1 in SRC.PList)
                {
                    p = currentP1;
                    // 選択したスペシャルパワーを使用できるパイロットかどうか判定
                    if (p.Party != "味方")
                    {
                        goto NextLoop;
                    }

                    if (p.Unit_Renamed is null)
                    {
                        goto NextLoop;
                    }

                    if (p.Unit_Renamed.Status_Renamed != "出撃")
                    {
                        goto NextLoop;
                    }

                    if (p.Unit_Renamed.CountPilot() > 0)
                    {
                        object argIndex3 = 1;
                        if ((p.ID ?? "") == (p.Unit_Renamed.Pilot(ref argIndex3).ID ?? "") & (p.ID ?? "") != (p.Unit_Renamed.MainPilot().ID ?? ""))
                        {
                            // 追加パイロットのため、使用されていない
                            goto NextLoop;
                        }
                    }

                    if (!p.IsSpecialPowerAvailable(ref SelectedSpecialPower))
                    {
                        goto NextLoop;
                    }

                    // パイロットをリストに追加
                    Array.Resize(ref list2, Information.UBound(list2) + 1 + 1);
                    Array.Resize(ref GUI.ListItemFlag, Information.UBound(list2) + 1);
                    Array.Resize(ref id_list, Information.UBound(list2) + 1);
                    Array.Resize(ref pid, Information.UBound(list2) + 1);
                    GUI.ListItemFlag[Information.UBound(list2)] = false;
                    id_list[Information.UBound(list2)] = p.Unit_Renamed.ID;
                    pid[Information.UBound(list2)] = p.ID;
                    string localRightPaddedString() { string argbuf = p.get_Nickname(false); var ret = GeneralLib.RightPaddedString(ref argbuf, 19); p.get_Nickname(false) = argbuf; return ret; }

                    string localRightPaddedString1() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.SP) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.MaxSP); var ret = GeneralLib.RightPaddedString(ref argbuf, 10); return ret; }

                    list2[Information.UBound(list2)] = localRightPaddedString() + localRightPaddedString1();
                    buf = "";
                    var loopTo8 = p.CountSpecialPower;
                    for (j = 1; j <= loopTo8; j++)
                    {
                        SpecialPowerData localItem1() { object argIndex1 = p.get_SpecialPower(j); var ret = SRC.SPDList.Item(ref argIndex1); p.get_SpecialPower(j) = Conversions.ToString(argIndex1); return ret; }

                        buf = buf + localItem1().intName;
                    }

                    list2[Information.UBound(list2)] = list2[Information.UBound(list2)] + GeneralLib.RightPaddedString(ref buf, 12);
                    if (p.SP < p.SpecialPowerCost(ref SelectedSpecialPower))
                    {
                        string argtname1 = "ＳＰ";
                        list2[Information.UBound(list2)] = list2[Information.UBound(list2)] + " " + Expression.Term(ref argtname1, ref p.Unit_Renamed) + "不足";
                    }

                    if (p.Unit_Renamed.Action == 0)
                    {
                        list2[Information.UBound(list2)] = list2[Information.UBound(list2)] + " 行動済";
                    }

                NextLoop:
                    ;
                }

                SelectedSpecialPower = "";

                // 検索をかけるパイロットの選択
                GUI.TopItem = 1;
                GUI.EnlargeListBoxHeight();
                string argoname = "等身大基準";
                if (Expression.IsOptionDefined(ref argoname))
                {
                    string arglb_caption1 = "ユニット選択";
                    string argtname2 = "SP";
                    Unit argu1 = null;
                    string argtname3 = "SP";
                    Unit argu2 = null;
                    string argtname4 = "スペシャルパワー";
                    Unit argu3 = null;
                    string arglb_info = "ユニット           " + Expression.Term(ref argtname2, ref argu1, 2) + "/Max" + Expression.Term(ref argtname3, ref argu2, 2) + "  " + Expression.Term(ref argtname4, u: ref argu3);
                    string arglb_mode = "";
                    ret = GUI.ListBox(ref arglb_caption1, ref list2, ref arglb_info, lb_mode: ref arglb_mode);
                }
                else
                {
                    string arglb_caption2 = "パイロット選択";
                    string argtname5 = "SP";
                    Unit argu4 = null;
                    string argtname6 = "SP";
                    Unit argu5 = null;
                    string argtname7 = "スペシャルパワー";
                    Unit argu6 = null;
                    string arglb_info1 = "パイロット         " + Expression.Term(ref argtname5, ref argu4, 2) + "/Max" + Expression.Term(ref argtname6, ref argu5, 2) + "  " + Expression.Term(ref argtname7, u: ref argu6);
                    string arglb_mode1 = "";
                    ret = GUI.ListBox(ref arglb_caption2, ref list2, ref arglb_info1, lb_mode: ref arglb_mode1);
                }

                GUI.ReduceListBoxHeight();

                // パイロットの乗るユニットを画面中央に表示
                if (ret > 0)
                {
                    var tmp = pid;
                    object argIndex4 = tmp[ret];
                    {
                        var withBlock2 = SRC.PList.Item(ref argIndex4);
                        GUI.Center(withBlock2.Unit_Renamed.x, withBlock2.Unit_Renamed.y);
                        GUI.RefreshScreen();
                        Status.DisplayUnitStatus(ref withBlock2.Unit_Renamed);

                        // カーソル自動移動
                        if (SRC.AutoMoveCursor)
                        {
                            string argcursor_mode = "ユニット選択";
                            GUI.MoveCursorPos(ref argcursor_mode, withBlock2.Unit_Renamed);
                        }
                    }

                    id_list = new string[1];
                    GUI.UnlockGUI();
                    return;
                }
            }
        }

        // リスタートコマンド
        // MOD START MARGE
        // Public Sub RestartCommand()
        private void RestartCommand()
        {
            // MOD END MARGE
            int ret;

            // リスタートを行うか確認
            ret = Interaction.MsgBox("リスタートしますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "リスタート");
            if (ret == MsgBoxResult.Cancel)
            {
                return;
            }

            GUI.LockGUI();
            string argfname = SRC.ScenarioPath + "_リスタート.src";
            bool argquick_load = true;
            SRC.RestoreData(ref argfname, ref argquick_load);
            GUI.UnlockGUI();
        }

        // クイックロードコマンド
        // MOD START MARGE
        // Public Sub QuickLoadCommand()
        private void QuickLoadCommand()
        {
            // MOD END MARGE
            int ret;

            // ロードを行うか確認
            ret = Interaction.MsgBox("データをロードしますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "クイックロード");
            if (ret == MsgBoxResult.Cancel)
            {
                return;
            }

            GUI.LockGUI();
            string argfname = SRC.ScenarioPath + "_クイックセーブ.src";
            bool argquick_load = true;
            SRC.RestoreData(ref argfname, ref argquick_load);

            // 画面を書き直してステータスを表示
            GUI.RedrawScreen();
            Status.DisplayGlobalStatus();
            GUI.UnlockGUI();
        }

        // クイックセーブコマンド
        // MOD START MARGE
        // Public Sub QuickSaveCommand()
        private void QuickSaveCommand()
        {
            // MOD END MARGE

            GUI.LockGUI();

            // マウスカーソルを砂時計に
            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            Cursor.Current = Cursors.WaitCursor;

            // 中断データをセーブ
            string argfname = SRC.ScenarioPath + "_クイックセーブ.src";
            SRC.DumpData(ref argfname);
            GUI.UnlockGUI();

            // マウスカーソルを元に戻す
            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            Cursor.Current = Cursors.Default;
        }


        // プレイを中断し、中断用データをセーブする
        // MOD START MARGE
        // Public Sub DumpCommand()
        private void DumpCommand()
        {
            // MOD END MARGE
            string fname, save_path = default;
            int ret, i;

            // プレイを中断するか確認
            ret = Interaction.MsgBox("プレイを中断しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "中断");
            if (ret == MsgBoxResult.Cancel)
            {
                return;
            }

            // 中断データをセーブするファイル名を決定
            var loopTo = Strings.Len(SRC.ScenarioFileName);
            for (i = 1; i <= loopTo; i++)
            {
                if (Strings.Mid(SRC.ScenarioFileName, Strings.Len(SRC.ScenarioFileName) - i + 1, 1) == @"\")
                {
                    break;
                }
            }

            fname = Strings.Mid(SRC.ScenarioFileName, Strings.Len(SRC.ScenarioFileName) - i + 2, i - 5);
            fname = fname + "を中断.src";
            string argdtitle = "データセーブ";
            string argftype = "ｾｰﾌﾞﾃﾞｰﾀ";
            string argfsuffix = "src";
            string argftype2 = "";
            string argfsuffix2 = "";
            string argftype3 = "";
            string argfsuffix3 = "";
            fname = FileDialog.SaveFileDialog(ref argdtitle, ref SRC.ScenarioPath, ref fname, 2, ref argftype, ref argfsuffix, ftype2: ref argftype2, fsuffix2: ref argfsuffix2, ftype3: ref argftype3, fsuffix3: ref argfsuffix3);
            if (string.IsNullOrEmpty(fname))
            {
                // キャンセル
                return;
            }

            // セーブ先はシナリオフォルダ？
            if (Strings.InStr(fname, @"\") > 0)
            {
                string argstr2 = @"\";
                save_path = Strings.Left(fname, GeneralLib.InStr2(ref fname, ref argstr2));
            }
            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            if ((FileSystem.Dir(save_path) ?? "") != (FileSystem.Dir(SRC.ScenarioPath) ?? ""))
            {
                if (Interaction.MsgBox("セーブファイルはシナリオフォルダにないと読み込めません。" + Constants.vbCr + Constants.vbLf + "このままセーブしますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question)) != 1)
                {
                    return;
                }
            }

            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            Cursor.Current = Cursors.WaitCursor; // マウスカーソルを砂時計に
            GUI.LockGUI();

            // 中断データをセーブ
            SRC.DumpData(ref fname);

            // マウスカーソルを元に戻す
            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            Cursor.Current = Cursors.Default;
            GUI.MainForm.Hide();

            // ゲームを終了
            SRC.ExitGame();
        }


        // 全体マップの表示
        // MOD START MARGE
        // Public Sub GlobalMapCommand()
        private void GlobalMapCommand()
        {
            // MOD END MARGE
            PictureBox pic;
            int xx, yy;
            int num = default, num2 = default;
            int mwidth, mheight;
            int ret, smode;
            Unit u;
            int i, j;
            bool prev_mode;
            GUI.LockGUI();
            {
                var withBlock = GUI.MainForm;
                // 見やすいように背景を設定
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                withBlock.picMain(0).BackColor = 0xC0C0C0;
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                withBlock.picMain(0) = Image.FromFile("");

                // マップの縦横の比率を元に縮小マップの大きさを決める
                if (Map.MapWidth > Map.MapHeight)
                {
                    mwidth = 300;
                    mheight = 300 * Map.MapHeight / Map.MapWidth;
                }
                else
                {
                    mheight = 300;
                    mwidth = 300 * Map.MapWidth / Map.MapHeight;
                }

                // マップの全体画像を作成
                // UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                pic = withBlock.picTmp;
                pic.Image = Image.FromFile("");
                pic.Width = Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(GUI.MapPWidth);
                pic.Height = Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(GUI.MapPHeight);
                // UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                ret = GUI.BitBlt(pic.hDC, 0, 0, GUI.MapPWidth, GUI.MapPHeight, withBlock.picBack.hDC, 0, 0, GUI.SRCCOPY);
                var loopTo = Map.MapWidth;
                for (i = 1; i <= loopTo; i++)
                {
                    xx = (32 * (i - 1));
                    var loopTo1 = Map.MapHeight;
                    for (j = 1; j <= loopTo1; j++)
                    {
                        yy = (32 * (j - 1));
                        u = Map.MapDataForUnit[i, j];
                        if (u is object)
                        {
                            if (u.BitmapID > 0)
                            {
                                string argfname = "地形ユニット";
                                if (u.Action > 0 | u.IsFeatureAvailable(ref argfname))
                                {
                                    // ユニット
                                    // UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                                    ret = GUI.BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picUnitBitmap.hDC, 32 * (u.BitmapID % 15), 96 * (u.BitmapID / 15), GUI.SRCCOPY);
                                }
                                else
                                {
                                    // 行動済のユニット
                                    // UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                                    ret = GUI.BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picUnitBitmap.hDC, 32 * (u.BitmapID % 15), 96 * (u.BitmapID / 15) + 32, GUI.SRCCOPY);
                                }

                                // ユニットのいる場所に合わせて表示を変更
                                switch (u.Area ?? "")
                                {
                                    case "空中":
                                        {
                                            // UPGRADE_ISSUE: PictureBox メソッド pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                                            pic.Line(xx, yy + 28); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                                            break;
                                        }

                                    case "水中":
                                        {
                                            // UPGRADE_ISSUE: PictureBox メソッド pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                                            pic.Line(xx, yy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                                            break;
                                        }

                                    case "地中":
                                        {
                                            // UPGRADE_ISSUE: PictureBox メソッド pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                                            pic.Line(xx, yy + 28); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                                            // UPGRADE_ISSUE: PictureBox メソッド pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                                            pic.Line(xx, yy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                                            break;
                                        }

                                    case "宇宙":
                                        {
                                            if (Map.TerrainClass(i, j) == "月面")
                                            {
                                                // UPGRADE_ISSUE: PictureBox メソッド pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                                                pic.Line(xx, yy + 28); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                                            }

                                            break;
                                        }
                                }
                            }
                        }
                    }
                }

                // マップ全体を縮小して描画
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                smode = GUI.GetStretchBltMode(withBlock.picMain(0).hDC);
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                ret = GUI.SetStretchBltMode(withBlock.picMain(0).hDC, GUI.STRETCH_DELETESCANS);
                // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                ret = GUI.StretchBlt(withBlock.picMain(0).hDC, (GUI.MainPWidth - mwidth) / 2, (GUI.MainPHeight - mheight) / 2, mwidth, mheight, pic.hDC, 0, 0, GUI.MapPWidth, GUI.MapPHeight, GUI.SRCCOPY);
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                ret = GUI.SetStretchBltMode(withBlock.picMain(0).hDC, smode);

                // マップ全体画像を破棄
                pic.Image = Image.FromFile("");
                pic.Width = Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(32d);
                pic.Height = Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(32d);

                // 画面を更新
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                withBlock.picMain(0).Refresh();
            }

            // 味方ユニット数、敵ユニット数のカウント
            foreach (Unit currentU in SRC.UList)
            {
                u = currentU;
                if (u.Status_Renamed == "出撃" | u.Status_Renamed == "格納")
                {
                    if (u.Party0 == "味方" | u.Party0 == "ＮＰＣ")
                    {
                        num = (num + 1);
                    }
                    else
                    {
                        num2 = (num2 + 1);
                    }
                }
            }

            // 各ユニット数の表示
            prev_mode = GUI.AutoMessageMode;
            GUI.AutoMessageMode = false;
            Unit argu1 = null;
            Unit argu2 = null;
            GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
            string argpname = "システム";
            GUI.DisplayMessage(ref argpname, "味方ユニット： " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(num) + ";" + "敵ユニット  ： " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(num2));
            GUI.CloseMessageForm();
            GUI.AutoMessageMode = prev_mode;

            // 画面を元に戻す
            // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            GUI.MainForm.picMain(0).BackColor = 0xFFFFFF;
            GUI.RefreshScreen();
            GUI.UnlockGUI();
        }

    }
}