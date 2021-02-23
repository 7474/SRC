using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.Lib;
using SRCCore.VB;

namespace SRCCore.CmdDatas
{
    public class CmdParser
    {
        // イベントデータ行を読み込んで解析する
        public CmdData Parse(SRC src, EventDataLine data)
        {
            var edata = data.Data;
            try
            {
                // 正常に解析が終了した場合はTrueを返すこと
                bool ParseRet = true;

                // 空行は無視
                if (string.IsNullOrWhiteSpace(edata))
                {
                    return new NopCmd(src, data);
                }

                // ラベルは無視
                if (Strings.Right(edata, 1) == ":")
                {
                    return new NopCmd(src, data);
                }

                // コマンドのパラメータ分割
                var list = GeneralLib.ToList(edata);

                // コマンドの種類を判定
                switch (Strings.LCase(list[0]) ?? "")
                {
                    //case "arc":
                    //    {
                    //        CmdName = CmdType.ArcCmd;
                    //        break;
                    //    }

                    //case "array":
                    //    {
                    //        CmdName = CmdType.ArrayCmd;
                    //        break;
                    //    }

                    //case "ask":
                    //    {
                    //        CmdName = CmdType.AskCmd;
                    //        break;
                    //    }

                    //case "attack":
                    //    {
                    //        CmdName = CmdType.AttackCmd;
                    //        break;
                    //    }

                    //case "autotalk":
                    //    {
                    //        CmdName = CmdType.AutoTalkCmd;
                    //        break;
                    //    }

                    //case "bossrank":
                    //    {
                    //        CmdName = CmdType.BossRankCmd;
                    //        break;
                    //    }

                    //case "break":
                    //    {
                    //        CmdName = CmdType.BreakCmd;
                    //        break;
                    //    }

                    //case "call":
                    //    {
                    //        CmdName = CmdType.CallCmd;
                    //        break;
                    //    }

                    //case "return":
                    //    {
                    //        CmdName = CmdType.ReturnCmd;
                    //        break;
                    //    }

                    //case "callintermissioncommand":
                    //    {
                    //        CmdName = CmdType.CallInterMissionCommandCmd;
                    //        break;
                    //    }

                    //case "cancel":
                    //    {
                    //        CmdName = CmdType.CancelCmd;
                    //        break;
                    //    }

                    case "center":
                        return new CenterCmd(src, data);

                    //case "changearea":
                    //    {
                    //        CmdName = CmdType.ChangeAreaCmd;
                    //        break;
                    //    }
                    //// ADD START 240a
                    //case "changelayer":
                    //    {
                    //        CmdName = CmdType.ChangeLayerCmd;
                    //        break;
                    //    }
                    //// ADD  END  240a
                    case "changemap":
                        return new ChangeMapCmd(src, data);

                    //case "changemode":
                    //    {
                    //        CmdName = CmdType.ChangeModeCmd;
                    //        break;
                    //    }

                    case "changeparty":
                        return new ChangePartyCmd(src, data);

                    //case "changeterrain":
                    //    {
                    //        CmdName = CmdType.ChangeTerrainCmd;
                    //        break;
                    //    }

                    //case "changeunitbitmap":
                    //    {
                    //        CmdName = CmdType.ChangeUnitBitmapCmd;
                    //        break;
                    //    }

                    //case "charge":
                    //    {
                    //        CmdName = CmdType.ChargeCmd;
                    //        break;
                    //    }

                    //case "circle":
                    //    {
                    //        CmdName = CmdType.CircleCmd;
                    //        break;
                    //    }

                    //case "clearevent":
                    //    {
                    //        CmdName = CmdType.ClearEventCmd;
                    //        break;
                    //    }

                    //case "clearimage":
                    //    {
                    //        CmdName = CmdType.ClearImageCmd;
                    //        break;
                    //    }
                    //// ADD START 240a
                    //case "clearlayer":
                    //    {
                    //        CmdName = CmdType.ClearLayerCmd;
                    //        break;
                    //    }
                    //// ADD  END  240a
                    //case "clearobj":
                    //    {
                    //        CmdName = CmdType.ClearObjCmd;
                    //        break;
                    //    }

                    case "clearpicture":
                        return new ClearPictureCmd(src, data);

                    //case "clearskill":
                    //case "clearability":
                    //    {
                    //        CmdName = CmdType.ClearSkillCmd;
                    //        break;
                    //    }

                    //case "clearspecialpower":
                    //case "clearmind":
                    //    {
                    //        CmdName = CmdType.ClearSpecialPowerCmd;
                    //        break;
                    //    }

                    //case "clearstatus":
                    //    {
                    //        CmdName = CmdType.ClearStatusCmd;
                    //        break;
                    //    }

                    //case "cls":
                    //    {
                    //        CmdName = CmdType.ClsCmd;
                    //        break;
                    //    }

                    //case "close":
                    //    {
                    //        CmdName = CmdType.CloseCmd;
                    //        break;
                    //    }

                    //case "color":
                    //    {
                    //        CmdName = CmdType.ColorCmd;
                    //        break;
                    //    }

                    //case "colorfilter":
                    //    {
                    //        CmdName = CmdType.ColorFilterCmd;
                    //        break;
                    //    }

                    //case "combine":
                    //    {
                    //        CmdName = CmdType.CombineCmd;
                    //        break;
                    //    }

                    //case "confirm":
                    //    {
                    //        CmdName = CmdType.ConfirmCmd;
                    //        break;
                    //    }

                    //case "continue":
                    //    {
                    //        CmdName = CmdType.ContinueCmd;
                    //        break;
                    //    }

                    //case "copyarray":
                    //    {
                    //        CmdName = CmdType.CopyArrayCmd;
                    //        break;
                    //    }

                    //case "copyfile":
                    //    {
                    //        CmdName = CmdType.CopyFileCmd;
                    //        break;
                    //    }

                    case "create":
                        return new CreateCmd(src, data);

                    //case "createfolder":
                    //    {
                    //        CmdName = CmdType.CreateFolderCmd;
                    //        break;
                    //    }

                    //case "debug":
                    //    {
                    //        CmdName = CmdType.DebugCmd;
                    //        break;
                    //    }

                    //case "destroy":
                    //    {
                    //        CmdName = CmdType.DestroyCmd;
                    //        break;
                    //    }

                    //case "disable":
                    //    {
                    //        CmdName = CmdType.DisableCmd;
                    //        break;
                    //    }

                    //case "do":
                    //    {
                    //        CmdName = CmdType.DoCmd;
                    //        if (ArgNum == 3)
                    //        {
                    //            args[1].strArg = Strings.LCase(args[1].strArg);
                    //        }

                    //        break;
                    //    }

                    //case "loop":
                    //    {
                    //        CmdName = CmdType.LoopCmd;
                    //        if (ArgNum == 3)
                    //        {
                    //            args[1].strArg = Strings.LCase(args[1].strArg);
                    //        }

                    //        break;
                    //    }

                    //case "drawoption":
                    //    {
                    //        CmdName = CmdType.DrawOptionCmd;
                    //        break;
                    //    }

                    //case "drawwidth":
                    //    {
                    //        CmdName = CmdType.DrawWidthCmd;
                    //        break;
                    //    }

                    //case "enable":
                    //    {
                    //        CmdName = CmdType.EnableCmd;
                    //        break;
                    //    }

                    //case "equip":
                    //    {
                    //        CmdName = CmdType.EquipCmd;
                    //        break;
                    //    }

                    //case "escape":
                    //    {
                    //        CmdName = CmdType.EscapeCmd;
                    //        break;
                    //    }

                    //case "exchangeitem":
                    //    {
                    //        CmdName = CmdType.ExchangeItemCmd;
                    //        break;
                    //    }

                    //case "exec":
                    //    {
                    //        CmdName = CmdType.ExecCmd;
                    //        break;
                    //    }

                    case "exit":
                        return new ExitCmd(src, data);

                    //case "explode":
                    //    {
                    //        CmdName = CmdType.ExplodeCmd;
                    //        break;
                    //    }

                    //case "expup":
                    //    {
                    //        CmdName = CmdType.ExpUpCmd;
                    //        break;
                    //    }

                    //case "fadein":
                    //    {
                    //        CmdName = CmdType.FadeInCmd;
                    //        break;
                    //    }

                    //case "fadeout":
                    //    {
                    //        CmdName = CmdType.FadeOutCmd;
                    //        break;
                    //    }

                    //case "fillcolor":
                    //    {
                    //        CmdName = CmdType.FillColorCmd;
                    //        break;
                    //    }

                    //case "fillstyle":
                    //    {
                    //        CmdName = CmdType.FillStyleCmd;
                    //        break;
                    //    }

                    //case "finish":
                    //    {
                    //        CmdName = CmdType.FinishCmd;
                    //        break;
                    //    }

                    //case "fix":
                    //    {
                    //        CmdName = CmdType.FixCmd;
                    //        break;
                    //    }

                    //case "for":
                    //    {
                    //        CmdName = CmdType.ForCmd;
                    //        break;
                    //    }

                    //case "foreach":
                    //    {
                    //        CmdName = CmdType.ForEachCmd;
                    //        break;
                    //    }

                    //case "next":
                    //    {
                    //        CmdName = CmdType.NextCmd;
                    //        break;
                    //    }

                    //case "font":
                    //    {
                    //        CmdName = CmdType.FontCmd;
                    //        break;
                    //    }

                    //case "forget":
                    //    {
                    //        CmdName = CmdType.ForgetCmd;
                    //        break;
                    //    }

                    //case "gameclear":
                    //    {
                    //        CmdName = CmdType.GameClearCmd;
                    //        break;
                    //    }

                    //case "gameover":
                    //    {
                    //        CmdName = CmdType.GameOverCmd;
                    //        break;
                    //    }

                    //case "freememory":
                    //    {
                    //        CmdName = CmdType.FreeMemoryCmd;
                    //        break;
                    //    }

                    //case "getoff":
                    //    {
                    //        CmdName = CmdType.GetOffCmd;
                    //        break;
                    //    }

                    //case "global":
                    //    {
                    //        CmdName = CmdType.GlobalCmd;
                    //        break;
                    //    }

                    //case "goto":
                    //    {
                    //        CmdName = CmdType.GotoCmd;
                    //        break;
                    //    }

                    //case "hide":
                    //    {
                    //        CmdName = CmdType.HideCmd;
                    //        break;
                    //    }

                    //case "hotpoint":
                    //    {
                    //        CmdName = CmdType.HotPointCmd;
                    //        break;
                    //    }

                    //case "if":
                    //    {
                    //        CmdName = CmdType.IfCmd;
                    //        break;
                    //    }

                    //case "else":
                    //    {
                    //        CmdName = CmdType.ElseCmd;
                    //        break;
                    //    }

                    //case "elseif":
                    //    {
                    //        CmdName = CmdType.ElseIfCmd;
                    //        break;
                    //    }

                    //case "endif":
                    //    {
                    //        CmdName = CmdType.EndIfCmd;
                    //        break;
                    //    }

                    //case "incr":
                    //    {
                    //        CmdName = CmdType.IncrCmd;
                    //        break;
                    //    }

                    //case "increasemorale":
                    //    {
                    //        CmdName = CmdType.IncreaseMoraleCmd;
                    //        break;
                    //    }

                    //case "input":
                    //    {
                    //        CmdName = CmdType.InputCmd;
                    //        break;
                    //    }

                    //case "intermissioncommand":
                    //    {
                    //        CmdName = CmdType.IntermissionCommandCmd;
                    //        break;
                    //    }

                    //case "item":
                    //    {
                    //        CmdName = CmdType.ItemCmd;
                    //        break;
                    //    }

                    //case "join":
                    //    {
                    //        CmdName = CmdType.JoinCmd;
                    //        break;
                    //    }

                    //case "keepbgm":
                    //    {
                    //        CmdName = CmdType.KeepBGMCmd;
                    //        break;
                    //    }

                    //case "land":
                    //    {
                    //        CmdName = CmdType.LandCmd;
                    //        break;
                    //    }

                    //case "launch":
                    //    {
                    //        CmdName = CmdType.LaunchCmd;
                    //        break;
                    //    }

                    //case "leave":
                    //    {
                    //        CmdName = CmdType.LeaveCmd;
                    //        break;
                    //    }

                    //case "levelup":
                    //    {
                    //        CmdName = CmdType.LevelUpCmd;
                    //        break;
                    //    }

                    //case "line":
                    //    {
                    //        CmdName = CmdType.LineCmd;
                    //        break;
                    //    }

                    //case "lineread":
                    //    {
                    //        CmdName = CmdType.LineReadCmd;
                    //        break;
                    //    }

                    //case "load":
                    //    {
                    //        CmdName = CmdType.LoadCmd;
                    //        break;
                    //    }

                    //case "local":
                    //    {
                    //        CmdName = CmdType.LocalCmd;
                    //        break;
                    //    }

                    //case "makepilotlist":
                    //    {
                    //        CmdName = CmdType.MakePilotListCmd;
                    //        break;
                    //    }

                    //case "makeunitlist":
                    //    {
                    //        CmdName = CmdType.MakeUnitListCmd;
                    //        break;
                    //    }

                    //case "mapability":
                    //    {
                    //        CmdName = CmdType.MapAbilityCmd;
                    //        break;
                    //    }

                    //case "mapattack":
                    //case "mapweapon":
                    //    {
                    //        CmdName = CmdType.MapAttackCmd;
                    //        break;
                    //    }

                    //case "money":
                    //    {
                    //        CmdName = CmdType.MoneyCmd;
                    //        break;
                    //    }

                    //case "monotone":
                    //    {
                    //        CmdName = CmdType.MonotoneCmd;
                    //        break;
                    //    }

                    //case "move":
                    //    {
                    //        CmdName = CmdType.MoveCmd;
                    //        break;
                    //    }

                    //case "night":
                    //    {
                    //        CmdName = CmdType.NightCmd;
                    //        break;
                    //    }

                    //case "noon":
                    //    {
                    //        CmdName = CmdType.NoonCmd;
                    //        break;
                    //    }

                    //case "open":
                    //    {
                    //        CmdName = CmdType.OpenCmd;
                    //        break;
                    //    }

                    //case "option":
                    //    {
                    //        CmdName = CmdType.OptionCmd;
                    //        break;
                    //    }

                    //case "organize":
                    //    {
                    //        CmdName = CmdType.OrganizeCmd;
                    //        break;
                    //    }

                    //case "oval":
                    //    {
                    //        CmdName = CmdType.OvalCmd;
                    //        break;
                    //    }

                    case "paintpicture":
                        return new PaintPictureCmd(src, data);

                    case "paintstring":
                        return new PaintStringCmd(src, data);

                    //case "paintsysstring":
                    //    {
                    //        CmdName = CmdType.PaintSysStringCmd;
                    //        break;
                    //    }

                    //case "pilot":
                    //    {
                    //        CmdName = CmdType.PilotCmd;
                    //        break;
                    //    }

                    //case "playmidi":
                    //    {
                    //        CmdName = CmdType.PlayMIDICmd;
                    //        break;
                    //    }

                    //case "playsound":
                    //    {
                    //        CmdName = CmdType.PlaySoundCmd;
                    //        break;
                    //    }

                    //case "polygon":
                    //    {
                    //        CmdName = CmdType.PolygonCmd;
                    //        break;
                    //    }

                    //case "print":
                    //    {
                    //        CmdName = CmdType.PrintCmd;
                    //        break;
                    //    }

                    //case "pset":
                    //    {
                    //        CmdName = CmdType.PSetCmd;
                    //        break;
                    //    }

                    //case "question":
                    //    {
                    //        CmdName = CmdType.QuestionCmd;
                    //        break;
                    //    }

                    //case "quickload":
                    //    {
                    //        CmdName = CmdType.QuickLoadCmd;
                    //        break;
                    //    }

                    //case "quit":
                    //    {
                    //        CmdName = CmdType.QuitCmd;
                    //        break;
                    //    }

                    //case "rankup":
                    //    {
                    //        CmdName = CmdType.RankUpCmd;
                    //        break;
                    //    }

                    //case "read":
                    //    {
                    //        CmdName = CmdType.ReadCmd;
                    //        break;
                    //    }

                    //case "recoveren":
                    //    {
                    //        CmdName = CmdType.RecoverENCmd;
                    //        break;
                    //    }

                    //case "recoverhp":
                    //    {
                    //        CmdName = CmdType.RecoverHPCmd;
                    //        break;
                    //    }

                    //case "recoverplana":
                    //    {
                    //        CmdName = CmdType.RecoverPlanaCmd;
                    //        break;
                    //    }

                    //case "recoversp":
                    //    {
                    //        CmdName = CmdType.RecoverSPCmd;
                    //        break;
                    //    }

                    case "redraw":
                        return new RedrawCmd(src, data);

                    case "refresh":
                        return new RefreshCmd(src, data);

                    //case "release":
                    //    {
                    //        CmdName = CmdType.ReleaseCmd;
                    //        break;
                    //    }

                    //case "removefile":
                    //    {
                    //        CmdName = CmdType.RemoveFileCmd;
                    //        break;
                    //    }

                    //case "removefolder":
                    //    {
                    //        CmdName = CmdType.RemoveFolderCmd;
                    //        break;
                    //    }

                    //case "removeitem":
                    //    {
                    //        CmdName = CmdType.RemoveItemCmd;
                    //        break;
                    //    }

                    //case "removepilot":
                    //    {
                    //        CmdName = CmdType.RemovePilotCmd;
                    //        break;
                    //    }

                    //case "removeunit":
                    //    {
                    //        CmdName = CmdType.RemoveUnitCmd;
                    //        break;
                    //    }

                    //case "renamebgm":
                    //    {
                    //        CmdName = CmdType.RenameBGMCmd;
                    //        break;
                    //    }

                    //case "renamefile":
                    //    {
                    //        CmdName = CmdType.RenameFileCmd;
                    //        break;
                    //    }

                    //case "renameterm":
                    //    {
                    //        CmdName = CmdType.RenameTermCmd;
                    //        break;
                    //    }

                    //case "replacepilot":
                    //    {
                    //        CmdName = CmdType.ReplacePilotCmd;
                    //        break;
                    //    }

                    //case "require":
                    //    {
                    //        CmdName = CmdType.RequireCmd;
                    //        break;
                    //    }

                    //case "restoreevent":
                    //    {
                    //        CmdName = CmdType.RestoreEventCmd;
                    //        break;
                    //    }

                    //case "ride":
                    //    {
                    //        CmdName = CmdType.RideCmd;
                    //        break;
                    //    }

                    //case "select":
                    //    {
                    //        CmdName = CmdType.SelectCmd;
                    //        break;
                    //    }

                    //case "savedata":
                    //    {
                    //        CmdName = CmdType.SaveDataCmd;
                    //        break;
                    //    }

                    //case "selecttarget":
                    //    {
                    //        CmdName = CmdType.SelectTargetCmd;
                    //        break;
                    //    }

                    //case "sepia":
                    //    {
                    //        CmdName = CmdType.SepiaCmd;
                    //        break;
                    //    }

                    //case "set":
                    //    {
                    //        CmdName = CmdType.SetCmd;
                    //        break;
                    //    }

                    //case "setbullet":
                    //    {
                    //        CmdName = CmdType.SetBulletCmd;
                    //        break;
                    //    }

                    //case "setmessage":
                    //    {
                    //        CmdName = CmdType.SetMessageCmd;
                    //        break;
                    //    }

                    //case "setrelation":
                    //    {
                    //        CmdName = CmdType.SetRelationCmd;
                    //        break;
                    //    }

                    //case "setskill":
                    //case "setability":
                    //    {
                    //        CmdName = CmdType.SetSkillCmd;
                    //        break;
                    //    }

                    //case "setstatus":
                    //    {
                    //        CmdName = CmdType.SetStatusCmd;
                    //        break;
                    //    }
                    //// ADD START 240a
                    //case "setstatusstringcolor":
                    //    {
                    //        CmdName = CmdType.SetStatusStringColorCmd;
                    //        break;
                    //    }
                    //// ADD  END
                    //case "setstock":
                    //    {
                    //        CmdName = CmdType.SetStockCmd;
                    //        break;
                    //    }
                    //// ADD START 240a
                    //case "setwindowcolor":
                    //    {
                    //        CmdName = CmdType.SetWindowColorCmd;
                    //        break;
                    //    }

                    //case "setwindowframewidth":
                    //    {
                    //        CmdName = CmdType.SetWindowFrameWidthCmd;
                    //        break;
                    //    }
                    //// ADD  END
                    //case "show":
                    //    {
                    //        CmdName = CmdType.ShowCmd;
                    //        break;
                    //    }

                    //case "showimage":
                    //    {
                    //        CmdName = CmdType.ShowImageCmd;
                    //        break;
                    //    }

                    //case "showunitstatus":
                    //    {
                    //        CmdName = CmdType.ShowUnitStatusCmd;
                    //        break;
                    //    }

                    //case "skip":
                    //    {
                    //        CmdName = CmdType.SkipCmd;
                    //        break;
                    //    }

                    //case "sort":
                    //    {
                    //        CmdName = CmdType.SortCmd;
                    //        break;
                    //    }

                    //case "specialpower":
                    //case "mind":
                    //    {
                    //        CmdName = CmdType.SpecialPowerCmd;
                    //        break;
                    //    }

                    //case "split":
                    //    {
                    //        CmdName = CmdType.SplitCmd;
                    //        break;
                    //    }

                    //case "startbgm":
                    //    {
                    //        CmdName = CmdType.StartBGMCmd;
                    //        break;
                    //    }

                    //case "stopbgm":
                    //    {
                    //        CmdName = CmdType.StopBGMCmd;
                    //        break;
                    //    }

                    //case "stopsummoning":
                    //    {
                    //        CmdName = CmdType.StopSummoningCmd;
                    //        break;
                    //    }

                    //case "supply":
                    //    {
                    //        CmdName = CmdType.SupplyCmd;
                    //        break;
                    //    }

                    //case "sunset":
                    //    {
                    //        CmdName = CmdType.SunsetCmd;
                    //        break;
                    //    }

                    //case "swap":
                    //    {
                    //        CmdName = CmdType.SwapCmd;
                    //        break;
                    //    }

                    //case "switch":
                    //    {
                    //        CmdName = CmdType.SwitchCmd;
                    //        break;
                    //    }

                    //case "playflash":
                    //    {
                    //        CmdName = CmdType.PlayFlashCmd;
                    //        break;
                    //    }

                    //case "clearflash":
                    //    {
                    //        CmdName = CmdType.ClearFlashCmd;
                    //        break;
                    //    }

                    //case "case":
                    //    {
                    //        CmdName = CmdType.CaseCmd;
                    //        if (ArgNum == 2)
                    //        {
                    //            if (Strings.LCase(list[1]) == "else")
                    //            {
                    //                CmdName = CmdType.CaseElseCmd;
                    //            }
                    //        }

                    //        break;
                    //    }

                    //case "endsw":
                    //    {
                    //        CmdName = CmdType.EndSwCmd;
                    //        break;
                    //    }

                    case "talk":
                        return new TalkCmd(src, data);

                    case "end":
                        return new EndCmd(src, data);

                    case "suspend":
                        return new SuspendCmd(src, data);

                    //case "suspend":
                    //    {
                    //        CmdName = CmdType.SuspendCmd;
                    //        break;
                    //    }

                    //case "telop":
                    //    {
                    //        CmdName = CmdType.TelopCmd;
                    //        break;
                    //    }

                    //case "transform":
                    //    {
                    //        CmdName = CmdType.TransformCmd;
                    //        break;
                    //    }

                    //case "unit":
                    //    {
                    //        CmdName = CmdType.UnitCmd;
                    //        break;
                    //    }

                    //case "unset":
                    //    {
                    //        CmdName = CmdType.UnsetCmd;
                    //        break;
                    //    }

                    //case "upgrade":
                    //    {
                    //        CmdName = CmdType.UpgradeCmd;
                    //        break;
                    //    }

                    //case "upvar":
                    //    {
                    //        CmdName = CmdType.UpVarCmd;
                    //        break;
                    //    }

                    //case "useability":
                    //    {
                    //        CmdName = CmdType.UseAbilityCmd;
                    //        break;
                    //    }

                    case "wait":
                        return new WaitCmd(src, data);

                    //case "water":
                    //    {
                    //        CmdName = CmdType.WaterCmd;
                    //        break;
                    //    }

                    //case "whitein":
                    //    {
                    //        CmdName = CmdType.WhiteInCmd;
                    //        break;
                    //    }

                    //case "whiteout":
                    //    {
                    //        CmdName = CmdType.WhiteOutCmd;
                    //        break;
                    //    }

                    //case "write":
                    //    {
                    //        CmdName = CmdType.WriteCmd;
                    //        break;
                    //    }

                    default:
                        {
                            // TODO Impl
                            //// 定義済みのイベントコマンドではない
                            //if (ArgNum >= 3)
                            //{
                            //    if (list[2] == "=")
                            //    {
                            //        // 代入式

                            //        CmdName = CmdType.SetCmd;
                            //        // UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                            //        Array.Resize(strArgs, 4);
                            //        // UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                            //        Array.Resize(lngArgs, 4);
                            //        // UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                            //        Array.Resize(dblArgs, 4);
                            //        // UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                            //        Array.Resize(ArgsType, 4);

                            //        // 代入先の変数名
                            //        strArgs[2] = list[1];
                            //        ArgsType[2] = Expressions.ValueType.StringType;

                            //        // 代入する値
                            //        // (値が項の場合は既に引数の処理が済んでいるのでなにもしなくてよい)
                            //        if (ArgNum > 3)
                            //        {
                            //            ArgsType[3] = Expressions.ValueType.UndefinedType;
                            //            // GetValueAsStringの呼び出しの際に、Argsの内容は必ず項と仮定
                            //            // されているので、わざと項にしておく
                            //            strArgs[3] = "(" + GeneralLib.ListTail(edata, 3) + ")";
                            //        }

                            //        ArgNum = 3;
                            //        return ParseRet;
                            //    }
                            //}

                            //if (ArgNum == -1)
                            //{
                            //    CmdName = CmdType.NopCmd;
                            //    return ParseRet;
                            //}

                            //// サブルーチンコール？
                            //CmdName = CmdType.CallCmd;
                            //// UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                            //Array.Resize(strArgs, ArgNum + 1 + 1);
                            //// UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                            //Array.Resize(lngArgs, ArgNum + 1 + 1);
                            //// UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                            //Array.Resize(dblArgs, ArgNum + 1 + 1);
                            //// UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                            //Array.Resize(ArgsType, ArgNum + 1 + 1);
                            //// 引数を１個ずらす
                            //var loopTo1 = (int)(ArgNum - 2);
                            //for (i = 0; i <= loopTo1; i++)
                            //{
                            //    strArgs[ArgNum + 1 - i] = strArgs[ArgNum - i];
                            //    lngArgs[ArgNum + 1 - i] = lngArgs[ArgNum - i];
                            //    dblArgs[ArgNum + 1 - i] = dblArgs[ArgNum - i];
                            //    ArgsType[ArgNum + 1 - i] = ArgsType[ArgNum - i];
                            //}

                            //ArgNum = (int)(ArgNum + 1);
                            //// 第２引数をサブルーチン名に設定
                            //strArgs[2] = list[1];
                            //if (Event_Renamed.FindNormalLabel(list[1]) > 0)
                            //{
                            //    ArgsType[2] = Expressions.ValueType.StringType;
                            //}
                            //else
                            //{
                            //    ArgsType[2] = Expressions.ValueType.UndefinedType;
                            //}

                            return new NopCmd(src, data);
                        }
                }

                // TODO Impl
                //if (CmdName == CmdType.IfCmd | CmdName == CmdType.ElseIfCmd)
                //{
                //    // If文の処理の高速化のため、あらかじめ構文解析しておく
                //    if (ArgNum == 1)
                //    {
                //        // 書式エラー
                //        Event_Renamed.DisplayEventErrorMessage(Event_Renamed.CurrentLineNum, "Ifコマンドの書式に合っていません");
                //        ParseRet = false;
                //        return ParseRet;
                //    }

                //    expr = list[2];
                //    var loopTo2 = ArgNum;
                //    for (i = 3; i <= loopTo2; i++)
                //    {
                //        buf = list[i];
                //        switch (Strings.LCase(buf) ?? "")
                //        {
                //            case "then":
                //            case "exit":
                //                {
                //                    // UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                //                    strArgs = new string[5];
                //                    // UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                //                    lngArgs = new int[5];
                //                    // UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                //                    dblArgs = new double[5];
                //                    // UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                //                    ArgsType = new Expressions.ValueType[5];
                //                    strArgs[2] = expr;
                //                    lngArgs[3] = ArgNum - 2;
                //                    ArgsType[3] = Expressions.ValueType.NumericType;
                //                    strArgs[4] = Strings.LCase(buf);
                //                    break;
                //                }

                //            case "goto":
                //                {
                //                    buf = GetArg((int)(i + 1));
                //                    // UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                //                    strArgs = new string[6];
                //                    // UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                //                    lngArgs = new int[6];
                //                    // UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                //                    dblArgs = new double[6];
                //                    // UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                //                    ArgsType = new Expressions.ValueType[6];
                //                    strArgs[2] = expr;
                //                    lngArgs[3] = ArgNum - 3;
                //                    ArgsType[3] = Expressions.ValueType.NumericType;
                //                    strArgs[4] = "goto";
                //                    strArgs[5] = buf;
                //                    break;
                //                }

                //            case var case1 when case1 == "":
                //                {
                //                    buf = "\"\"";
                //                    break;
                //                }
                //        }

                //        expr = expr + " " + buf;
                //    }

                //    if (i > ArgNum)
                //    {
                //        if (CmdName == CmdType.IfCmd)
                //        {
                //            Event_Renamed.DisplayEventErrorMessage(LineNum, "Ifに対応する Then または Exit または Goto がありません");
                //        }
                //        else
                //        {
                //            Event_Renamed.DisplayEventErrorMessage(LineNum, "ElseIfに対応する Then または Exit または Goto がありません");
                //        }

                //        SRC.TerminateSRC();
                //    }

                //    // 条件式が式であることが確定していれば条件式の項数を0に
                //    switch (lngArgs[3])
                //    {
                //        case 0:
                //            {
                //                if (CmdName == CmdType.IfCmd)
                //                {
                //                    Event_Renamed.DisplayEventErrorMessage(LineNum, "Ifコマンドの条件式がありません");
                //                }
                //                else
                //                {
                //                    Event_Renamed.DisplayEventErrorMessage(LineNum, "ElseIfコマンドの条件式がありません");
                //                }

                //                SRC.TerminateSRC();
                //                break;
                //            }

                //        case 1:
                //            {
                //                switch (Strings.Asc(expr))
                //                {
                //                    case 36: // $
                //                        {
                //                            lngArgs[3] = 0;
                //                            break;
                //                        }

                //                    case 40: // (
                //                        {
                //                            // ()を除去
                //                            strArgs[2] = Strings.Mid(expr, 2, Strings.Len(expr) - 2);
                //                            lngArgs[3] = 0;
                //                            break;
                //                        }
                //                }

                //                break;
                //            }

                //        case 2:
                //            {
                //                if (Strings.LCase(GeneralLib.LIndex(expr, 1)) == "not")
                //                {
                //                    switch (Strings.Asc(GeneralLib.ListIndex(expr, 2)))
                //                    {
                //                        case 36:
                //                        case 40: // $, (
                //                            {
                //                                lngArgs[3] = 0;
                //                                break;
                //                            }
                //                    }
                //                }
                //                else
                //                {
                //                    lngArgs[3] = 0;
                //                }

                //                break;
                //            }

                //        default:
                //            {
                //                lngArgs[3] = 0;
                //                break;
                //            }
                //    }

                //    return ParseRet;
                //}

                //if (CmdName == CmdType.CallCmd)
                //{
                //    // Callコマンドのサブルーチン指定が式かどうか調べておく
                //    if (Event_Renamed.FindNormalLabel(strArgs[2]) > 0)
                //    {
                //        ArgsType[2] = Expressions.ValueType.StringType;
                //    }
                //    else
                //    {
                //        ArgsType[2] = Expressions.ValueType.UndefinedType;
                //    }
                //}

                //if (CmdName == CmdType.LocalCmd)
                //{
                //    if (ArgNum > 4)
                //    {
                //        if (list[3] == "=")
                //        {
                //            // Localコマンドが複数項から成る代入式を伴う場合

                //            // UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                //            Array.Resize(strArgs, 5);
                //            // UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                //            Array.Resize(lngArgs, 5);
                //            // UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                //            Array.Resize(dblArgs, 5);
                //            // UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                //            Array.Resize(ArgsType, 5);

                //            // 代入する値
                //            ArgsType[4] = Expressions.ValueType.UndefinedType;
                //            strArgs[4] = "(" + GeneralLib.ListTail(edata, 4) + ")";
                //            ArgNum = 4;
                //            return ParseRet;
                //        }
                //    }
                //}
            }
            catch
            {
                // TODO Impl
                //Event.DisplayEventErrorMessage(EventDataId, "イベントコマンドの内容が不正です");
                return new NopCmd(src, data);
            }
        }


    }
}
