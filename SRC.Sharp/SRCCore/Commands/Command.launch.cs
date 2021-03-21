// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Lib;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Linq;

namespace SRCCore.Commands
{
    public partial class Command
    {
        // 「発進」コマンドを開始
        private void StartLaunchCommand()
        {
            // 母艦に搭載しているユニットの一覧を作成
            var list = SelectedUnit.UnitOnBoards.Select(onBoardUnit =>
            {
                return new ListBoxItem
                {
                    Text = GeneralLib.RightPaddedString(onBoardUnit.Nickname0, 25)
                        + GeneralLib.RightPaddedString(onBoardUnit.MainPilot().get_Nickname(false), 17)
                        + GeneralLib.LeftPaddedString(SrcFormatter.Format(onBoardUnit.MainPilot().Level), 2)
                        + GeneralLib.RightPaddedString(SrcFormatter.Format(onBoardUnit.HP) + "/" + SrcFormatter.Format(onBoardUnit.MaxHP), 12)
                        + " " + GeneralLib.RightPaddedString(SrcFormatter.Format(onBoardUnit.EN) + "/" + SrcFormatter.Format(onBoardUnit.MaxEN), 8),
                    ListItemID = onBoardUnit.ID,
                    ListItemFlag = onBoardUnit.Action <= 0,
                };
            }).ToList();

            // どのユニットを発進させるか選択
            GUI.TopItem = 1;
            var ret = GUI.ListBox(new ListBoxArgs
            {
                HasFlag = true,
                Items = list,
                lb_caption = "ユニット選択",
                lb_info = "ユニット名               パイロット       Lv "
                    + Expression.Term("ＨＰ", null, 8)
                    + Expression.Term("ＥＮ", null),
                lb_mode = "カーソル移動",
            });

            // キャンセルされた？
            if (ret == 0)
            {
                CancelCommand();
                return;
            }

            SelectedCommand = "発進";

            // ユニットの発進処理
            SelectedTarget = SRC.UList.Item(list[ret - 1].ListItemID);
            {
                var launchUnit = SelectedTarget;
                launchUnit.x = SelectedUnit.x;
                launchUnit.y = SelectedUnit.y;
                if (launchUnit.IsFeatureAvailable("テレポート")
                    && (launchUnit.Data.Speed == 0
                    || GeneralLib.LIndex(launchUnit.FeatureData("テレポート"), 2) == "0"))
                {
                    // テレポートによる発進
                    Map.AreaInTeleport(SelectedTarget);
                }
                else if (launchUnit.IsFeatureAvailable("ジャンプ")
                    && (launchUnit.Data.Speed == 0
                    || GeneralLib.LLength(launchUnit.FeatureData("ジャンプ")) < 2
                    || GeneralLib.LIndex(launchUnit.FeatureData("ジャンプ"), 2) == "0"))
                {
                    // ジャンプによる発進
                    Map.AreaInSpeed(SelectedTarget, true);
                }
                else
                {
                    // 通常移動による発進
                    Map.AreaInSpeed(SelectedTarget);
                }

                // 母艦を中央表示
                GUI.Center(launchUnit.x, launchUnit.y);

                // 発進させるユニットを母艦の代わりに表示
                if (launchUnit.BitmapID == 0)
                {
                    // TODO なんか別のやり方考えたいね。
                    //var withBlock3 = SRC.UList.Item(launchUnit.Name);
                    //if ((SelectedTarget.Party0 ?? "") == (withBlock3.Party0 ?? "") & withBlock3.BitmapID != 0 & (SelectedTarget.get_Bitmap(false) ?? "") == (withBlock3.get_Bitmap(false) ?? ""))
                    //{
                    //    SelectedTarget.BitmapID = withBlock3.BitmapID;
                    //}
                    //else
                    //{
                    //    SelectedTarget.BitmapID = GUI.MakeUnitBitmap(SelectedTarget);
                    //}
                }

                GUI.MaskScreen();
            }

            if (CommandState == "コマンド選択")
            {
                CommandState = "ターゲット選択";
            }
        }

        // 「発進」コマンドを終了
        private void FinishLaunchCommand()
        {
            throw new NotImplementedException();
            //// MOD END MARGE
            //int ret;
            //GUI.LockGUI();
            //{
            //    var withBlock = SelectedTarget;
            //    // 発進コマンドの目的地にユニットがいた場合
            //    if (Map.MapDataForUnit[SelectedX, SelectedY] is object)
            //    {
            //        string argfname = "母艦";
            //        if (Map.MapDataForUnit[SelectedX, SelectedY].IsFeatureAvailable(argfname))
            //        {
            //            ret = Interaction.MsgBox("着艦しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "着艦");
            //        }
            //        else
            //        {
            //            ret = Interaction.MsgBox("合体しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "合体");
            //        }

            //        if (ret == MsgBoxResult.Cancel)
            //        {
            //            CancelCommand();
            //            GUI.UnlockGUI();
            //            return;
            //        }
            //    }

            //    // メッセージの表示
            //    string argmain_situation = "発進(" + withBlock.Name + ")";
            //    string argmain_situation1 = "発進";
            //    if (withBlock.IsMessageDefined(argmain_situation))
            //    {
            //        Unit argu1 = null;
            //        Unit argu2 = null;
            //        GUI.OpenMessageForm(u1: argu1, u2: argu2);
            //        string argSituation = "発進(" + withBlock.Name + ")";
            //        string argmsg_mode = "";
            //        withBlock.PilotMessage(argSituation, msg_mode: argmsg_mode);
            //        GUI.CloseMessageForm();
            //    }
            //    else if (withBlock.IsMessageDefined(argmain_situation1))
            //    {
            //        Unit argu11 = null;
            //        Unit argu21 = null;
            //        GUI.OpenMessageForm(u1: argu11, u2: argu21);
            //        string argSituation1 = "発進";
            //        string argmsg_mode1 = "";
            //        withBlock.PilotMessage(argSituation1, msg_mode: argmsg_mode1);
            //        GUI.CloseMessageForm();
            //    }

            //    string argmain_situation2 = "発進";
            //    string argsub_situation = withBlock.Name;
            //    withBlock.SpecialEffect(argmain_situation2, argsub_situation);
            //    withBlock.Name = argsub_situation;
            //    PrevUnitArea = withBlock.Area;
            //    PrevUnitEN = withBlock.EN;
            //    withBlock.Status_Renamed = "出撃";

            //    // 指定した位置に発進したユニットを移動
            //    withBlock.Move(SelectedX, SelectedY);
            //}

            //// 発進したユニットを母艦から降ろす
            //{
            //    var withBlock1 = SelectedUnit;
            //    PrevUnitX = withBlock1.x;
            //    PrevUnitY = withBlock1.y;
            //    withBlock1.UnloadUnit((object)SelectedTarget.ID);

            //    // 母艦の位置には発進したユニットが表示されているので元に戻しておく
            //    Map.MapDataForUnit[withBlock1.x, withBlock1.y] = SelectedUnit;
            //    GUI.PaintUnitBitmap(SelectedUnit);
            //}

            //SelectedUnit = SelectedTarget;
            //{
            //    var withBlock2 = SelectedUnit;
            //    if ((Map.MapDataForUnit[withBlock2.x, withBlock2.y].ID ?? "") != (withBlock2.ID ?? ""))
            //    {
            //        GUI.RedrawScreen();
            //        CommandState = "ユニット選択";
            //        GUI.UnlockGUI();
            //        return;
            //    }
            //}

            //CommandState = "移動後コマンド選択";
            //GUI.UnlockGUI();
            //ProceedCommand();
        }

    }
}