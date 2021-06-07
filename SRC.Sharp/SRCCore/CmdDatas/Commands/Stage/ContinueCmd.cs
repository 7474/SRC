using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.Pilots;
using SRCCore.Units;
using SRCCore.VB;
using System.Linq;

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

            // 追加経験値を得るパイロットや破壊されたユニットがいなければ処理をスキップ
            var n = 0;
            foreach (Unit u in SRC.UList.Items)
            {
                if (u.Party0 == "味方")
                {
                    if (u.Status == "出撃" || u.Status == "格納" || u.Status == "破壊")
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
            if (SRC.Turn > 0 && !Expression.IsOptionDefined("追加経験値無効"))
            {
                GUI.OpenMessageForm(u1: null, u2: null);
                n = 0;
                var msg = "";
                foreach (Pilot p in SRC.PList.Items)
                {
                    if (p.Party != "味方")
                    {
                        goto NextPilot;
                    }

                    if (p.MaxSP == 0)
                    {
                        goto NextPilot;
                    }

                    if (p.Unit is null)
                    {
                        goto NextPilot;
                    }

                    if (p.Unit.Status != "出撃" && p.Unit.Status != "格納")
                    {
                        goto NextPilot;
                    }

                    var plevel = p.Level;
                    p.Exp = p.Exp + 2 * p.SP;

                    // 追加パイロットや暴走時パイロットに関する処理
                    if (p.Unit.CountPilot() > 0 && !p.IsSupport(p.Unit))
                    {
                        // 追加パイロットがメインパイロットの場合
                        if (ReferenceEquals(p, p.Unit.Pilots.First()) && !ReferenceEquals(p, p.Unit.MainPilot()) && p.Unit.MainPilot().MaxSP > 0)
                        {
                            goto NextPilot;
                        }

                        // 追加パイロットがメインパイロットではなくなった場合
                        if (!ReferenceEquals(p, p.Unit.MainPilot()))
                        {
                            // 自分がユニットのパイロット一覧に含まれているか判定
                            if (!p.Unit.Pilots.Contains(p))
                            {
                                goto NextPilot;
                            }
                        }
                    }

                    if (plevel == p.Level)
                    {
                        msg = msg + ";" + p.get_Nickname(false) + " 経験値 +" + SrcFormatter.Format(2 * p.SP);
                    }
                    else
                    {
                        msg = msg + ";" + p.get_Nickname(false) + " 経験値 +" + SrcFormatter.Format(2 * p.SP) + " レベルアップ！（Lv" + SrcFormatter.Format(p.Level) + "）";
                    }

                    n = (n + 1);
                    if (n == 4)
                    {
                        GUI.DisplayMessage("システム", Strings.Mid(msg, 2));
                        msg = "";
                        n = 0;
                    }

                NextPilot:
                    ;
                }

                if (n > 0)
                {
                    GUI.DisplayMessage("システム", Strings.Mid(msg, 2));
                }

                GUI.CloseMessageForm();
            }

            GUI.MainFormHide();

            // エピローグイベントを実行
            if (Event.IsEventDefined("エピローグ"))
            {
                // ハイパーモードや変身、能力コピーを解除
                foreach (Unit u in SRC.UList.Items.CloneList())
                {
                    if (u.Status != "他形態" && u.Status != "旧主形態" && u.Status != "旧形態")
                    {
                        if (u.IsFeatureAvailable("ノーマルモード"))
                        {
                            u.Transform(GeneralLib.LIndex(u.FeatureData("ノーマルモード"), 1));
                        }
                    }
                }

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
