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
            switch (ArgNum)
            {
                case 2:
                    if (!Expression.IsGlobalVariableDefined("次ステージ"))
                    {
                        Expression.DefineGlobalVariable("次ステージ");
                    }
                    Expression.SetVariableAsString("次ステージ", GetArgAsString(2));
                    break;

                case 1:
                    break;

                default:
                    throw new EventErrorException(this, "Continueコマンドの引数の数が違います");
            }

            SRC.GUIStatus.ClearUnitStatus();

            // TODO Impl 経験し取得などの後処理
            //// 追加経験値を得るパイロットや破壊されたユニットがいなければ処理をスキップ
            //n = 0;
            //foreach (Unit currentU in SRC.UList.Items)
            //{
            //    u = currentU;
            //    if (u.Party0 == "味方")
            //    {
            //        if (u.Status == "出撃" | u.Status == "格納" | u.Status == "破壊")
            //        {
            //            n = 1;
            //            break;
            //        }
            //    }
            //}

            //if (n == 0)
            //{
            //    SRC.Turn = 0;
            //}

            //// 追加経験値を収得
            //if (SRC.Turn > 0 & !Expression.IsOptionDefined("追加経験値無効"))
            //{
            //    GUI.OpenMessageForm(u1: null, u2: null);
            //    n = 0;
            //    msg = "";
            //    foreach (Pilot p in SRC.PList)
            //    {
            //        if (p.Party != "味方")
            //        {
            //            goto NextPilot;
            //        }

            //        if (p.MaxSP == 0)
            //        {
            //            goto NextPilot;
            //        }

            //        if (p.Unit is null)
            //        {
            //            goto NextPilot;
            //        }

            //        if (p.Unit.Status != "出撃" & p.Unit.Status != "格納")
            //        {
            //            goto NextPilot;
            //        }

            //        plevel = p.Level;
            //        p.Exp = p.Exp + 2 * p.SP;

            //        // 追加パイロットや暴走時パイロットに関する処理
            //        if (p.Unit.CountPilot() > 0 & !p.IsSupport(p.Unit))
            //        {
            //            // 追加パイロットがメインパイロットの場合
            //            if (ReferenceEquals(p, p.Unit.Pilot(1)) & !ReferenceEquals(p, p.Unit.MainPilot()) & p.Unit.MainPilot().MaxSP > 0)
            //            {
            //                goto NextPilot;
            //            }

            //            // 追加パイロットがメインパイロットではなくなった場合
            //            if (!ReferenceEquals(p, p.Unit.MainPilot()))
            //            {
            //                // 自分がユニットのパイロット一覧に含まれているか判定
            //                var loopTo = p.Unit.CountPilot();
            //                for (i = 1; i <= loopTo; i++)
            //                {
            //                    Pilot localPilot() { object argIndex1 = i; var ret = p.Unit.Pilot(argIndex1); return ret; }

            //                    if (ReferenceEquals(p, localPilot()))
            //                    {
            //                        break;
            //                    }
            //                }

            //                if (i > p.Unit.CountPilot())
            //                {
            //                    goto NextPilot;
            //                }
            //            }
            //        }

            //        if (plevel == p.Level)
            //        {
            //            msg = msg + ";" + p.get_Nickname(false) + " 経験値 +" + SrcFormatter.Format(2 * p.SP);
            //        }
            //        else
            //        {
            //            msg = msg + ";" + p.get_Nickname(false) + " 経験値 +" + SrcFormatter.Format(2 * p.SP) + " レベルアップ！（Lv" + SrcFormatter.Format(p.Level) + "）";
            //        }

            //        n = (n + 1);
            //        if (n == 4)
            //        {
            //            GUI.DisplayMessage("システム", Strings.Mid(msg, 2));
            //            msg = "";
            //            n = 0;
            //        }

            //    NextPilot:
            //        ;
            //    }

            //    if (n > 0)
            //    {
            //        GUI.DisplayMessage("システム", Strings.Mid(msg, 2));
            //    }

            //    GUI.CloseMessageForm();
            //}

            GUI.MainFormHide();

            // エピローグイベントを実行
            if (Event.IsEventDefined("エピローグ"))
            {
                // TODO Impl エピローグイベントを実行
                //// ハイパーモードや変身、能力コピーを解除
                //foreach (Unit currentU1 in SRC.UList)
                //{
                //    u = currentU1;
                //    if (u.Status != "他形態" & u.Status != "旧主形態" & u.Status != "旧形態")
                //    {
                //        if (u.IsFeatureAvailable("ノーマルモード"))
                //        {
                //            string localLIndex() { object argIndex1 = "ノーマルモード"; string arglist = u.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

                //            u.Transform(localLIndex());
                //        }
                //    }
                //}

                if (Event.IsEventDefined("エピローグ", true))
                {
                    SRC.Sound.StopBGM();
                    SRC.Sound.StartBGM(SRC.Sound.BGMName("Briefing"));
                }

                SRC.Stage = "エピローグ";
                Event.HandleEvent("エピローグ");
            }

            GUI.MainFormHide();

            // インターミッションに移行
            if (!SRC.IsSubStage)
            {
                SRC.InterMission.InterMissionCommand();
                if (!SRC.IsSubStage)
                {
                    if (string.IsNullOrEmpty(Expression.GetValueAsString("次ステージ")))
                    {
                        throw new EventErrorException(this, "次のステージのファイル名が設定されていません");
                    }

                    SRC.StartScenario(Expression.GetValueAsString("次ステージ"));
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
