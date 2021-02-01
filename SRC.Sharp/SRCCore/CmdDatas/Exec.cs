using SRC.Core.Events;
using SRC.Core.Lib;
using SRC.Core.VB;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRC.Core.CmdDatas
{
    public partial class CmdData
    {

        // コマンドを実行し、実行後の行番号を返す
        public int Exec()
        {
            int ExecRet = default;

            try
            {
                // XXX 多態をとりたい
                switch (Name)
                {
                    case Events.CmdType.NopCmd:
                        {
                            // スキップ
                            ExecRet = EventDataId + 1;
                            break;
                        }

                    //case Events.CmdType.ArcCmd:
                    //    {
                    //        ExecRet = ExecArcCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ArrayCmd:
                    //    {
                    //        ExecRet = ExecArrayCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.AskCmd:
                    //    {
                    //        ExecRet = ExecAskCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.AttackCmd:
                    //    {
                    //        ExecRet = ExecAttackCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.AutoTalkCmd:
                    //    {
                    //        ExecRet = ExecAutoTalkCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.BossRankCmd:
                    //    {
                    //        ExecRet = ExecBossRankCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.BreakCmd:
                    //    {
                    //        ExecRet = ExecBreakCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.CallCmd:
                    //    {
                    //        ExecRet = ExecCallCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ReturnCmd:
                    //    {
                    //        ExecRet = ExecReturnCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.CallInterMissionCommandCmd:
                    //    {
                    //        ExecRet = ExecCallInterMissionCommandCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.CancelCmd:
                    //    {
                    //        ExecRet = ExecCancelCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.CenterCmd:
                    //    {
                    //        ExecRet = ExecCenterCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ChangeAreaCmd:
                    //    {
                    //        ExecRet = ExecChangeAreaCmd();
                    //        break;
                    //    }
                    //// ADD START 240a
                    //case Events.CmdType.ChangeLayerCmd:
                    //    {
                    //        ExecRet = ExecChangeLayerCmd();
                    //        break;
                    //    }
                    //// ADD  END  240a
                    //case Events.CmdType.ChangeMapCmd:
                    //    {
                    //        ExecRet = ExecChangeMapCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ChangeModeCmd:
                    //    {
                    //        ExecRet = ExecChangeModeCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ChangePartyCmd:
                    //    {
                    //        ExecRet = ExecChangePartyCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ChangeTerrainCmd:
                    //    {
                    //        ExecRet = ExecChangeTerrainCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ChangeUnitBitmapCmd:
                    //    {
                    //        ExecRet = ExecChangeUnitBitmapCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ChargeCmd:
                    //    {
                    //        ExecRet = ExecChargeCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.CircleCmd:
                    //    {
                    //        ExecRet = ExecCircleCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ClearEventCmd:
                    //    {
                    //        ExecRet = ExecClearEventCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ClearImageCmd:
                    //    {
                    //        ExecRet = ExecClearImageCmd();
                    //        break;
                    //    }
                    //// ADD START 240a
                    //case Events.CmdType.ClearLayerCmd:
                    //    {
                    //        ExecRet = ExecClearLayerCmd();
                    //        break;
                    //    }
                    //// ADD  END  240a
                    //case Events.CmdType.ClearObjCmd:
                    //    {
                    //        ExecRet = ExecClearObjCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ClearPictureCmd:
                    //    {
                    //        ExecRet = ExecClearPictureCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ClearSkillCmd:
                    //    {
                    //        ExecRet = ExecClearSkillCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ClearSpecialPowerCmd:
                    //    {
                    //        ExecRet = ExecClearSpecialPowerCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ClearStatusCmd:
                    //    {
                    //        ExecRet = ExecClearStatusCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.CloseCmd:
                    //    {
                    //        ExecRet = ExecCloseCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ClsCmd:
                    //    {
                    //        ExecRet = ExecClsCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ColorCmd:
                    //    {
                    //        ExecRet = ExecColorCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ColorFilterCmd:
                    //    {
                    //        ExecRet = ExecColorFilterCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.CombineCmd:
                    //    {
                    //        ExecRet = ExecCombineCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ConfirmCmd:
                    //    {
                    //        ExecRet = ExecConfirmCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ContinueCmd:
                    //    {
                    //        ExecRet = ExecContinueCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.CopyArrayCmd:
                    //    {
                    //        ExecRet = ExecCopyArrayCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.CopyFileCmd:
                    //    {
                    //        ExecRet = ExecCopyFileCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.CreateCmd:
                    //    {
                    //        ExecRet = ExecCreateCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.CreateFolderCmd:
                    //    {
                    //        ExecRet = ExecCreateFolderCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.DebugCmd:
                    //    {
                    //        ExecRet = ExecDebugCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.DestroyCmd:
                    //    {
                    //        ExecRet = ExecDestroyCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.DisableCmd:
                    //    {
                    //        ExecRet = ExecDisableCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.DoCmd:
                    //    {
                    //        ExecRet = ExecDoCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.LoopCmd:
                    //    {
                    //        ExecRet = ExecLoopCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.DrawOptionCmd:
                    //    {
                    //        ExecRet = ExecDrawOptionCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.DrawWidthCmd:
                    //    {
                    //        ExecRet = ExecDrawWidthCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.EnableCmd:
                    //    {
                    //        ExecRet = ExecEnableCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.EquipCmd:
                    //    {
                    //        ExecRet = ExecEquipCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.EscapeCmd:
                    //    {
                    //        ExecRet = ExecEscapeCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ExchangeItemCmd:
                    //    {
                    //        ExecRet = ExecExchangeItemCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ExecCmd:
                    //    {
                    //        ExecRet = ExecExecCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ExitCmd:
                    //    {
                    //        ExecRet = ExecExitCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ExplodeCmd:
                    //    {
                    //        ExecRet = ExecExplodeCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ExpUpCmd:
                    //    {
                    //        ExecRet = ExecExpUpCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.FadeInCmd:
                    //    {
                    //        ExecRet = ExecFadeInCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.FadeOutCmd:
                    //    {
                    //        ExecRet = ExecFadeOutCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.FillColorCmd:
                    //    {
                    //        ExecRet = ExecFillColorCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.FillStyleCmd:
                    //    {
                    //        ExecRet = ExecFillStyleCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.FinishCmd:
                    //    {
                    //        ExecRet = ExecFinishCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.FixCmd:
                    //    {
                    //        ExecRet = ExecFixCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.FontCmd:
                    //    {
                    //        ExecRet = ExecFontCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ForCmd:
                    //    {
                    //        ExecRet = ExecForCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ForEachCmd:
                    //    {
                    //        ExecRet = ExecForEachCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.NextCmd:
                    //    {
                    //        ExecRet = ExecNextCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ForgetCmd:
                    //    {
                    //        ExecRet = ExecForgetCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.GameClearCmd:
                    //    {
                    //        ExecRet = ExecGameClearCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.GameOverCmd:
                    //    {
                    //        ExecRet = ExecGameOverCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.FreeMemoryCmd:
                    //    {
                    //        ExecRet = ExecFreeMemoryCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.GetOffCmd:
                    //    {
                    //        ExecRet = ExecGetOffCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.GlobalCmd:
                    //    {
                    //        ExecRet = ExecGlobalCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.GotoCmd:
                    //    {
                    //        ExecRet = ExecGotoCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.HideCmd:
                    //    {
                    //        ExecRet = ExecHideCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.HotPointCmd:
                    //    {
                    //        ExecRet = ExecHotPointCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.IfCmd:
                    //    {
                    //        ExecRet = ExecIfCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ElseCmd:
                    //case Events.CmdType.ElseIfCmd:
                    //    {
                    //        ExecRet = ExecElseCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.EndIfCmd:
                    //    {
                    //        // スキップ
                    //        ExecRet = EventDataId + 1;
                    //        break;
                    //    }

                    //case Events.CmdType.IncrCmd:
                    //    {
                    //        ExecRet = ExecIncrCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.IncreaseMoraleCmd:
                    //    {
                    //        ExecRet = ExecIncreaseMoraleCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.InputCmd:
                    //    {
                    //        ExecRet = ExecInputCmd();
                    //        break;
                    //    }
                    //// MOD START マージ
                    //// Case InterMissionCommandCmd
                    //// Exec = ExecInterMissionCommandCmd()
                    //case Events.CmdType.IntermissionCommandCmd:
                    //    {
                    //        ExecRet = ExecIntermissionCommandCmd();
                    //        break;
                    //    }
                    //// MOD ENDマージ
                    //case Events.CmdType.ItemCmd:
                    //    {
                    //        ExecRet = ExecItemCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.JoinCmd:
                    //    {
                    //        ExecRet = ExecJoinCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.KeepBGMCmd:
                    //    {
                    //        ExecRet = ExecKeepBGMCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.LandCmd:
                    //    {
                    //        ExecRet = ExecLandCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.LaunchCmd:
                    //    {
                    //        ExecRet = ExecLaunchCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.LeaveCmd:
                    //    {
                    //        ExecRet = ExecLeaveCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.LevelUpCmd:
                    //    {
                    //        ExecRet = ExecLevelUpCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.LineCmd:
                    //    {
                    //        ExecRet = ExecLineCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.LineReadCmd:
                    //    {
                    //        ExecRet = ExecLineReadCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.LoadCmd:
                    //    {
                    //        ExecRet = ExecLoadCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.LocalCmd:
                    //    {
                    //        ExecRet = ExecLocalCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.MakePilotListCmd:
                    //    {
                    //        ExecRet = ExecMakePilotListCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.MakeUnitListCmd:
                    //    {
                    //        ExecRet = ExecMakeUnitListCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.MapAbilityCmd:
                    //    {
                    //        ExecRet = ExecMapAbilityCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.MapAttackCmd:
                    //    {
                    //        ExecRet = ExecMapAttackCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.SpecialPowerCmd:
                    //    {
                    //        ExecRet = ExecSpecialPowerCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.MoneyCmd:
                    //    {
                    //        ExecRet = ExecMoneyCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.MonotoneCmd:
                    //    {
                    //        ExecRet = ExecMonotoneCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.MoveCmd:
                    //    {
                    //        ExecRet = ExecMoveCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.NightCmd:
                    //    {
                    //        ExecRet = ExecNightCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.NoonCmd:
                    //    {
                    //        ExecRet = ExecNoonCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.OpenCmd:
                    //    {
                    //        ExecRet = ExecOpenCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.OptionCmd:
                    //    {
                    //        ExecRet = ExecOptionCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.OrganizeCmd:
                    //    {
                    //        ExecRet = ExecOrganizeCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.OvalCmd:
                    //    {
                    //        ExecRet = ExecOvalCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.PaintPictureCmd:
                    //    {
                    //        ExecRet = ExecPaintPictureCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.PaintStringCmd:
                    //case Events.CmdType.PaintStringRCmd:
                    //    {
                    //        ExecRet = ExecPaintStringCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.PaintSysStringCmd:
                    //    {
                    //        ExecRet = ExecPaintSysStringCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.PilotCmd:
                    //    {
                    //        ExecRet = ExecPilotCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.PlayMIDICmd:
                    //    {
                    //        ExecRet = ExecPlayMIDICmd();
                    //        break;
                    //    }

                    //case Events.CmdType.PlaySoundCmd:
                    //    {
                    //        ExecRet = ExecPlaySoundCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.PolygonCmd:
                    //    {
                    //        ExecRet = ExecPolygonCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.PrintCmd:
                    //    {
                    //        ExecRet = ExecPrintCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.PSetCmd:
                    //    {
                    //        ExecRet = ExecPSetCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.QuestionCmd:
                    //    {
                    //        ExecRet = ExecQuestionCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.QuickLoadCmd:
                    //    {
                    //        ExecRet = ExecQuickLoadCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.QuitCmd:
                    //    {
                    //        ExecRet = ExecQuitCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.RankUpCmd:
                    //    {
                    //        ExecRet = ExecRankUpCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ReadCmd:
                    //    {
                    //        ExecRet = ExecReadCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.RecoverENCmd:
                    //    {
                    //        ExecRet = ExecRecoverENCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.RecoverHPCmd:
                    //    {
                    //        ExecRet = ExecRecoverHPCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.RecoverPlanaCmd:
                    //    {
                    //        ExecRet = ExecRecoverPlanaCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.RecoverSPCmd:
                    //    {
                    //        ExecRet = ExecRecoverSPCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.RedrawCmd:
                    //    {
                    //        ExecRet = ExecRedrawCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.RefreshCmd:
                    //    {
                    //        ExecRet = ExecRefreshCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ReleaseCmd:
                    //    {
                    //        ExecRet = ExecReleaseCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.RemoveFileCmd:
                    //    {
                    //        ExecRet = ExecRemoveFileCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.RemoveFolderCmd:
                    //    {
                    //        ExecRet = ExecRemoveFolderCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.RemoveItemCmd:
                    //    {
                    //        ExecRet = ExecRemoveItemCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.RemovePilotCmd:
                    //    {
                    //        ExecRet = ExecRemovePilotCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.RemoveUnitCmd:
                    //    {
                    //        ExecRet = ExecRemoveUnitCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.RenameBGMCmd:
                    //    {
                    //        ExecRet = ExecRenameBGMCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.RenameFileCmd:
                    //    {
                    //        ExecRet = ExecRenameFileCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.RenameTermCmd:
                    //    {
                    //        ExecRet = ExecRenameTermCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ReplacePilotCmd:
                    //    {
                    //        ExecRet = ExecReplacePilotCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.RequireCmd:
                    //    {
                    //        ExecRet = ExecRequireCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.RestoreEventCmd:
                    //    {
                    //        ExecRet = ExecRestoreEventCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.RideCmd:
                    //    {
                    //        ExecRet = ExecRideCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.SaveDataCmd:
                    //    {
                    //        ExecRet = ExecSaveDataCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.SelectCmd:
                    //    {
                    //        ExecRet = ExecSelectCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.SelectTargetCmd:
                    //    {
                    //        ExecRet = ExecSelectTargetCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.SepiaCmd:
                    //    {
                    //        ExecRet = ExecSepiaCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.SetCmd:
                    //    {
                    //        ExecRet = ExecSetCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.SetSkillCmd:
                    //    {
                    //        ExecRet = ExecSetSkillCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.SetBulletCmd:
                    //    {
                    //        ExecRet = ExecSetBulletCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.SetMessageCmd:
                    //    {
                    //        ExecRet = ExecSetMessageCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.SetRelationCmd:
                    //    {
                    //        ExecRet = ExecSetRelationCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.SetStatusCmd:
                    //    {
                    //        ExecRet = ExecSetStatusCmd();
                    //        break;
                    //    }
                    //// ADD START 240a
                    //case Events.CmdType.SetStatusStringColorCmd:
                    //    {
                    //        ExecRet = ExecSetStatusStringColor();
                    //        break;
                    //    }
                    //// ADD  END
                    //case Events.CmdType.SetStockCmd:
                    //    {
                    //        ExecRet = ExecSetStockCmd();
                    //        break;
                    //    }
                    //// ADD START 240a
                    //case Events.CmdType.SetWindowColorCmd:
                    //    {
                    //        ExecRet = ExecSetWindowColor();
                    //        break;
                    //    }

                    //case Events.CmdType.SetWindowFrameWidthCmd:
                    //    {
                    //        ExecRet = ExecSetWindowFrameWidth();
                    //        break;
                    //    }
                    //// ADD  END
                    //case Events.CmdType.ShowCmd:
                    //    {
                    //        ExecRet = ExecShowCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ShowImageCmd:
                    //    {
                    //        ExecRet = ExecShowImageCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ShowUnitStatusCmd:
                    //    {
                    //        ExecRet = ExecShowUnitStatusCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.SkipCmd:
                    //    {
                    //        ExecRet = ExecSkipCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.SortCmd:
                    //    {
                    //        // UPGRADE_WARNING: オブジェクト ExecSortCmd() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    //        ExecRet = Conversions.ToInteger(ExecSortCmd());
                    //        break;
                    //    }

                    //case Events.CmdType.SplitCmd:
                    //    {
                    //        ExecRet = ExecSplitCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.StartBGMCmd:
                    //    {
                    //        ExecRet = ExecStartBGMCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.StopBGMCmd:
                    //    {
                    //        ExecRet = ExecStopBGMCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.StopSummoningCmd:
                    //    {
                    //        ExecRet = ExecStopSummoningCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.SunsetCmd:
                    //    {
                    //        ExecRet = ExecSunsetCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.SupplyCmd:
                    //    {
                    //        ExecRet = ExecSupplyCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.SwapCmd:
                    //    {
                    //        ExecRet = ExecSwapCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.SwitchCmd:
                    //    {
                    //        ExecRet = ExecSwitchCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.CaseCmd:
                    //case Events.CmdType.CaseElseCmd:
                    //    {
                    //        ExecRet = ExecCaseCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.EndSwCmd:
                    //    {
                    //        // スキップ
                    //        ExecRet = EventDataId + 1;
                    //        break;
                    //    }

                    //case Events.CmdType.TalkCmd:
                    //    {
                    //        ExecRet = ExecTalkCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.TelopCmd:
                    //    {
                    //        ExecRet = ExecTelopCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.TransformCmd:
                    //    {
                    //        ExecRet = ExecTransformCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.UnitCmd:
                    //    {
                    //        ExecRet = ExecUnitCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.UnsetCmd:
                    //    {
                    //        ExecRet = ExecUnsetCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.UpgradeCmd:
                    //    {
                    //        ExecRet = ExecUpgradeCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.UpVarCmd:
                    //    {
                    //        ExecRet = ExecUpvarCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.UseAbilityCmd:
                    //    {
                    //        ExecRet = ExecUseAbilityCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.WaitCmd:
                    //    {
                    //        ExecRet = ExecWaitCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.WaterCmd:
                    //    {
                    //        ExecRet = ExecWaterCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.WhiteInCmd:
                    //    {
                    //        ExecRet = ExecWhiteInCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.WhiteOutCmd:
                    //    {
                    //        ExecRet = ExecWhiteOutCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.WriteCmd:
                    //    {
                    //        ExecRet = ExecWriteCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.PlayFlashCmd:
                    //    {
                    //        ExecRet = ExecPlayFlashCmd();
                    //        break;
                    //    }

                    //case Events.CmdType.ClearFlashCmd:
                    //    {
                    //        ExecRet = ExecClearFlashCmd();
                    //        break;
                    //    }

                    default:
                        {
                            Event.EventErrorMessage = GeneralLib.ListIndex(Event.EventData[EventDataId].Data, 1) + "というコマンドは存在しません";
                            throw new Exception();
                        }
                }

                return ExecRet;
            }
            catch
            {
                // TODO Impl
                if (Strings.Len(Event.EventErrorMessage) > 0)
                {
                    Event.DisplayEventErrorMessage(EventDataId, Event.EventErrorMessage);
                    Event.EventErrorMessage = "";
                }
                else if (Strings.LCase(GeneralLib.ListIndex(Event.EventData[EventDataId].Data, 1)) == "talk")
                {
                    Event.DisplayEventErrorMessage(EventDataId, "Talkコマンド実行中に不正な処理が行われました。" + "MIDIがソフトウェアシンセサイザで演奏されているか、" + "フォントキャッシュが壊れている可能性があります。" + "詳しくはSRC公式ホームページの「よくある質問集」をご覧下さい。");
                }
                else if (Strings.LCase(GeneralLib.ListIndex(Event.EventData[EventDataId].Data, 1)) == "autotalk")
                {
                    Event.DisplayEventErrorMessage(EventDataId, "AutoTalkコマンド実行中に不正な処理が行われました。" + "MIDIがソフトウェアシンセサイザで演奏されているか、" + "フォントキャッシュが壊れている可能性があります。" + "詳しくはSRC公式ホームページの「よくある質問集」をご覧下さい。");
                }
                else
                {
                    Event.DisplayEventErrorMessage(EventDataId, "イベントデータが不正です");
                }
                return -1;
            }
        }
    }
}
