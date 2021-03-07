using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class ContinueCmd : CmdData
    {
        public ContinueCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ContinueCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            int ExecContinueCmdRet = default;
            string msg;
            short n, i;
            short plevel;
            Unit u;
            switch (ArgNum)
            {
                case 2:
                    {
                        string argvname1 = "次ステージ";
                        if (!Expression.IsGlobalVariableDefined(ref argvname1))
                        {
                            string argvname = "次ステージ";
                            Expression.DefineGlobalVariable(ref argvname);
                        }

                        string argvname2 = "次ステージ";
                        string argnew_value = GetArgAsString((short)2);
                        Expression.SetVariableAsString(ref argvname2, ref argnew_value);
                        break;
                    }

                case 1:
                    {
                        break;
                    }

                default:
                    {
                        Event_Renamed.EventErrorMessage = "Continueコマンドの引数の数が違います";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 199596


                        Input:
                                        Error(0)

                         */
                        break;
                    }
            }

            Status.ClearUnitStatus();

            // 追加経験値を得るパイロットや破壊されたユニットがいなければ処理をスキップ
            n = 0;
            foreach (Unit currentU in SRC.UList)
            {
                u = currentU;
                if (u.Party0 == "味方")
                {
                    if (u.Status_Renamed == "出撃" | u.Status_Renamed == "格納" | u.Status_Renamed == "破壊")
                    {
                        n = 1;
                        break;
                    }
                }
            }

            if (n == 0)
            {
                SRC.Turn = 0;
            }

            // 追加経験値を収得
            string argoname = "追加経験値無効";
            if (SRC.Turn > 0 & !Expression.IsOptionDefined(ref argoname))
            {
                Unit argu1 = null;
                Unit argu2 = null;
                GUI.OpenMessageForm(u1: ref argu1, u2: ref argu2);
                n = 0;
                msg = "";
                foreach (Pilot p in SRC.PList)
                {
                    if (p.Party != "味方")
                    {
                        goto NextPilot;
                    }

                    if (p.MaxSP == 0)
                    {
                        goto NextPilot;
                    }

                    if (p.Unit_Renamed is null)
                    {
                        goto NextPilot;
                    }

                    if (p.Unit_Renamed.Status_Renamed != "出撃" & p.Unit_Renamed.Status_Renamed != "格納")
                    {
                        goto NextPilot;
                    }

                    plevel = p.Level;
                    p.Exp = p.Exp + 2 * p.SP;

                    // 追加パイロットや暴走時パイロットに関する処理
                    if (p.Unit_Renamed.CountPilot() > 0 & !p.IsSupport(ref p.Unit_Renamed))
                    {
                        // 追加パイロットがメインパイロットの場合
                        object argIndex1 = 1;
                        if (ReferenceEquals(p, p.Unit_Renamed.Pilot(ref argIndex1)) & !ReferenceEquals(p, p.Unit_Renamed.MainPilot()) & p.Unit_Renamed.MainPilot().MaxSP > 0)
                        {
                            goto NextPilot;
                        }

                        // 追加パイロットがメインパイロットではなくなった場合
                        if (!ReferenceEquals(p, p.Unit_Renamed.MainPilot()))
                        {
                            // 自分がユニットのパイロット一覧に含まれているか判定
                            var loopTo = p.Unit_Renamed.CountPilot();
                            for (i = 1; i <= loopTo; i++)
                            {
                                Pilot localPilot() { object argIndex1 = i; var ret = p.Unit_Renamed.Pilot(ref argIndex1); return ret; }

                                if (ReferenceEquals(p, localPilot()))
                                {
                                    break;
                                }
                            }

                            if (i > p.Unit_Renamed.CountPilot())
                            {
                                goto NextPilot;
                            }
                        }
                    }

                    if (plevel == p.Level)
                    {
                        msg = msg + ";" + p.get_Nickname(false) + " 経験値 +" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(2 * p.SP);
                    }
                    else
                    {
                        msg = msg + ";" + p.get_Nickname(false) + " 経験値 +" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(2 * p.SP) + " レベルアップ！（Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.Level) + "）";
                    }

                    n = (short)(n + 1);
                    if (n == 4)
                    {
                        string argpname = "システム";
                        GUI.DisplayMessage(ref argpname, Strings.Mid(msg, 2));
                        msg = "";
                        n = 0;
                    }

                NextPilot:
                    ;
                }

                if (n > 0)
                {
                    string argpname1 = "システム";
                    GUI.DisplayMessage(ref argpname1, Strings.Mid(msg, 2));
                }

                GUI.CloseMessageForm();
            }

            GUI.MainForm.Hide();

            // エピローグイベントを実行
            string arglname1 = "エピローグ";
            if (Event_Renamed.IsEventDefined(ref arglname1))
            {
                // ハイパーモードや変身、能力コピーを解除
                foreach (Unit currentU1 in SRC.UList)
                {
                    u = currentU1;
                    if (u.Status_Renamed != "他形態" & u.Status_Renamed != "旧主形態" & u.Status_Renamed != "旧形態")
                    {
                        string argfname = "ノーマルモード";
                        if (u.IsFeatureAvailable(ref argfname))
                        {
                            string localLIndex() { object argIndex1 = "ノーマルモード"; string arglist = u.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

                            string argnew_form = localLIndex();
                            u.Transform(ref argnew_form);
                        }
                    }
                }

                string arglname = "エピローグ";
                if (Event_Renamed.IsEventDefined(ref arglname, true))
                {
                    Sound.StopBGM();
                    string argbgm_name = "Briefing";
                    string argbgm_name1 = Sound.BGMName(ref argbgm_name);
                    Sound.StartBGM(ref argbgm_name1);
                }

                SRC.Stage = "エピローグ";
                Event_Renamed.HandleEvent("エピローグ");
            }

            GUI.MainForm.Hide();

            // インターミッションに移行
            if (!SRC.IsSubStage)
            {
                // 
                InterMission.InterMissionCommand();
                if (!SRC.IsSubStage)
                {
                    string argexpr = "次ステージ";
                    if (string.IsNullOrEmpty(Expression.GetValueAsString(ref argexpr)))
                    {
                        Event_Renamed.EventErrorMessage = "次のステージのファイル名が設定されていません";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 203307


                        Input:
                                            Error(0)

                         */
                    }

                    string argexpr1 = "次ステージ";
                    SRC.StartScenario(Expression.GetValueAsString(ref argexpr1));
                }
                else
                {
                    SRC.IsSubStage = false;
                }
            }

            SRC.IsScenarioFinished = true;
            return -1;
        }
    }
}
