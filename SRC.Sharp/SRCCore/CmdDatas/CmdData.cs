// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRC.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRC.Core.CmdDatas
{
    // イベントコマンドのクラス
    public partial class CmdData
    {
        // コマンドの種類
        private CmdType CmdName;
        // 引数の数
        public short ArgNum;
        // コマンドのEventDataにおける位置
        public int EventDataId;

        // 引数の値
        private int[] lngArgs;
        private double[] dblArgs;
        private string[] strArgs;

        // 引数の型
        private Expressions.ValueType[] ArgsType;

        // コマンドの種類
        public CmdType Name
        {
            get
            {
                // XXX これ要るの？
                //object NameRet = default;
                //if (CmdName == CmdType.NullCmd)
                //{
                //    Parse(ref Event_Renamed.EventData[LineNum]);
                //}

                //NameRet = CmdName;
                //return NameRet;
                return CmdName;
            }

            set
            {
                CmdName = value;
            }
        }

        // イベントデータ行を読み込んで解析する
        public bool Parse(string edata)
        {
            bool ParseRet = default;
            string buf = default, expr;
            var list = default(string[]);
            short i;

            // 正常に解析が終了した場合はTrueを返すこと
            ParseRet = true;
            ;

            // 空行は無視
            if (Strings.Len(edata) == 0)
            {
                CmdName = Event_Renamed.CmdType.NopCmd;
                ArgNum = 0;
                return ParseRet;
            }

            // ラベルは無視
            if (Strings.Right(edata, 1) == ":")
            {
                CmdName = Event_Renamed.CmdType.NopCmd;
                ArgNum = 0;
                return ParseRet;
            }

            // コマンドのパラメータ分割
            ArgNum = GeneralLib.ListSplit(ref edata, ref list);

            // 空行は無視
            if (ArgNum == 0)
            {
                CmdName = Event_Renamed.CmdType.NopCmd;
                return ParseRet;
            }

            // パラメータの処理
            if (ArgNum > 1)
            {
                // UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                strArgs = new string[(ArgNum + 1)];
                // UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                lngArgs = new int[(ArgNum + 1)];
                // UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                dblArgs = new double[(ArgNum + 1)];
                // UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                ArgsType = new Expression.ValueType[(ArgNum + 1)];
                var loopTo = ArgNum;
                for (i = 2; i <= loopTo; i++)
                {
                    buf = list[i];
                    strArgs[i] = buf;
                    ArgsType[i] = Expression.ValueType.UndefinedType;

                    // 先頭の一文字からパラメータの属性を判定
                    switch (Strings.Asc(buf))
                    {
                        case 0: // 空文字列
                            {
                                ArgsType[i] = Expression.ValueType.StringType;
                                break;
                            }

                        case 34: // "
                            {
                                if (Strings.Right(buf, 1) == "\"")
                                {
                                    if (Strings.InStr(buf, "$(") == 0)
                                    {
                                        ArgsType[i] = Expression.ValueType.StringType;
                                        strArgs[i] = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
                                    }
                                }
                                else
                                {
                                    ArgsType[i] = Expression.ValueType.StringType;
                                }

                                break;
                            }

                        case 40: // (
                            {
                                break;
                            }
                        // 式
                        case 45: // -
                            {
                                if (Information.IsNumeric(buf))
                                {
                                    lngArgs[i] = GeneralLib.StrToLng(ref buf);
                                    dblArgs[i] = Conversions.ToDouble(buf);
                                    ArgsType[i] = Expression.ValueType.NumericType;
                                }
                                else
                                {
                                    ArgsType[i] = Expression.ValueType.StringType;
                                }

                                break;
                            }

                        case var @case when 48 <= @case && @case <= 57: // 0～9
                            {
                                if (Information.IsNumeric(buf))
                                {
                                    lngArgs[i] = GeneralLib.StrToLng(ref buf);
                                    dblArgs[i] = Conversions.ToDouble(buf);
                                    ArgsType[i] = Expression.ValueType.NumericType;
                                }
                                else
                                {
                                    ArgsType[i] = Expression.ValueType.StringType;
                                }

                                break;
                            }

                        case 96: // `
                            {
                                if (Strings.Right(buf, 1) == "`")
                                {
                                    strArgs[i] = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
                                }

                                ArgsType[i] = Expression.ValueType.StringType;
                                break;
                            }
                    }
                }
            }

            // コマンドの種類を判定
            switch (Strings.LCase(list[1]) ?? "")
            {
                case "arc":
                    {
                        CmdName = Event_Renamed.CmdType.ArcCmd;
                        break;
                    }

                case "array":
                    {
                        CmdName = Event_Renamed.CmdType.ArrayCmd;
                        break;
                    }

                case "ask":
                    {
                        CmdName = Event_Renamed.CmdType.AskCmd;
                        break;
                    }

                case "attack":
                    {
                        CmdName = Event_Renamed.CmdType.AttackCmd;
                        break;
                    }

                case "autotalk":
                    {
                        CmdName = Event_Renamed.CmdType.AutoTalkCmd;
                        break;
                    }

                case "bossrank":
                    {
                        CmdName = Event_Renamed.CmdType.BossRankCmd;
                        break;
                    }

                case "break":
                    {
                        CmdName = Event_Renamed.CmdType.BreakCmd;
                        break;
                    }

                case "call":
                    {
                        CmdName = Event_Renamed.CmdType.CallCmd;
                        break;
                    }

                case "return":
                    {
                        CmdName = Event_Renamed.CmdType.ReturnCmd;
                        break;
                    }

                case "callintermissioncommand":
                    {
                        CmdName = Event_Renamed.CmdType.CallInterMissionCommandCmd;
                        break;
                    }

                case "cancel":
                    {
                        CmdName = Event_Renamed.CmdType.CancelCmd;
                        break;
                    }

                case "center":
                    {
                        CmdName = Event_Renamed.CmdType.CenterCmd;
                        break;
                    }

                case "changearea":
                    {
                        CmdName = Event_Renamed.CmdType.ChangeAreaCmd;
                        break;
                    }
                // ADD START 240a
                case "changelayer":
                    {
                        CmdName = Event_Renamed.CmdType.ChangeLayerCmd;
                        break;
                    }
                // ADD  END  240a
                case "changemap":
                    {
                        CmdName = Event_Renamed.CmdType.ChangeMapCmd;
                        break;
                    }

                case "changemode":
                    {
                        CmdName = Event_Renamed.CmdType.ChangeModeCmd;
                        break;
                    }

                case "changeparty":
                    {
                        CmdName = Event_Renamed.CmdType.ChangePartyCmd;
                        break;
                    }

                case "changeterrain":
                    {
                        CmdName = Event_Renamed.CmdType.ChangeTerrainCmd;
                        break;
                    }

                case "changeunitbitmap":
                    {
                        CmdName = Event_Renamed.CmdType.ChangeUnitBitmapCmd;
                        break;
                    }

                case "charge":
                    {
                        CmdName = Event_Renamed.CmdType.ChargeCmd;
                        break;
                    }

                case "circle":
                    {
                        CmdName = Event_Renamed.CmdType.CircleCmd;
                        break;
                    }

                case "clearevent":
                    {
                        CmdName = Event_Renamed.CmdType.ClearEventCmd;
                        break;
                    }

                case "clearimage":
                    {
                        CmdName = Event_Renamed.CmdType.ClearImageCmd;
                        break;
                    }
                // ADD START 240a
                case "clearlayer":
                    {
                        CmdName = Event_Renamed.CmdType.ClearLayerCmd;
                        break;
                    }
                // ADD  END  240a
                case "clearobj":
                    {
                        CmdName = Event_Renamed.CmdType.ClearObjCmd;
                        break;
                    }

                case "clearpicture":
                    {
                        CmdName = Event_Renamed.CmdType.ClearPictureCmd;
                        break;
                    }

                case "clearskill":
                case "clearability":
                    {
                        CmdName = Event_Renamed.CmdType.ClearSkillCmd;
                        break;
                    }

                case "clearspecialpower":
                case "clearmind":
                    {
                        CmdName = Event_Renamed.CmdType.ClearSpecialPowerCmd;
                        break;
                    }

                case "clearstatus":
                    {
                        CmdName = Event_Renamed.CmdType.ClearStatusCmd;
                        break;
                    }

                case "cls":
                    {
                        CmdName = Event_Renamed.CmdType.ClsCmd;
                        break;
                    }

                case "close":
                    {
                        CmdName = Event_Renamed.CmdType.CloseCmd;
                        break;
                    }

                case "color":
                    {
                        CmdName = Event_Renamed.CmdType.ColorCmd;
                        break;
                    }

                case "colorfilter":
                    {
                        CmdName = Event_Renamed.CmdType.ColorFilterCmd;
                        break;
                    }

                case "combine":
                    {
                        CmdName = Event_Renamed.CmdType.CombineCmd;
                        break;
                    }

                case "confirm":
                    {
                        CmdName = Event_Renamed.CmdType.ConfirmCmd;
                        break;
                    }

                case "continue":
                    {
                        CmdName = Event_Renamed.CmdType.ContinueCmd;
                        break;
                    }

                case "copyarray":
                    {
                        CmdName = Event_Renamed.CmdType.CopyArrayCmd;
                        break;
                    }

                case "copyfile":
                    {
                        CmdName = Event_Renamed.CmdType.CopyFileCmd;
                        break;
                    }

                case "create":
                    {
                        CmdName = Event_Renamed.CmdType.CreateCmd;
                        break;
                    }

                case "createfolder":
                    {
                        CmdName = Event_Renamed.CmdType.CreateFolderCmd;
                        break;
                    }

                case "debug":
                    {
                        CmdName = Event_Renamed.CmdType.DebugCmd;
                        break;
                    }

                case "destroy":
                    {
                        CmdName = Event_Renamed.CmdType.DestroyCmd;
                        break;
                    }

                case "disable":
                    {
                        CmdName = Event_Renamed.CmdType.DisableCmd;
                        break;
                    }

                case "do":
                    {
                        CmdName = Event_Renamed.CmdType.DoCmd;
                        if (ArgNum == 3)
                        {
                            strArgs[2] = Strings.LCase(strArgs[2]);
                        }

                        break;
                    }

                case "loop":
                    {
                        CmdName = Event_Renamed.CmdType.LoopCmd;
                        if (ArgNum == 3)
                        {
                            strArgs[2] = Strings.LCase(strArgs[2]);
                        }

                        break;
                    }

                case "drawoption":
                    {
                        CmdName = Event_Renamed.CmdType.DrawOptionCmd;
                        break;
                    }

                case "drawwidth":
                    {
                        CmdName = Event_Renamed.CmdType.DrawWidthCmd;
                        break;
                    }

                case "enable":
                    {
                        CmdName = Event_Renamed.CmdType.EnableCmd;
                        break;
                    }

                case "equip":
                    {
                        CmdName = Event_Renamed.CmdType.EquipCmd;
                        break;
                    }

                case "escape":
                    {
                        CmdName = Event_Renamed.CmdType.EscapeCmd;
                        break;
                    }

                case "exchangeitem":
                    {
                        CmdName = Event_Renamed.CmdType.ExchangeItemCmd;
                        break;
                    }

                case "exec":
                    {
                        CmdName = Event_Renamed.CmdType.ExecCmd;
                        break;
                    }

                case "exit":
                    {
                        CmdName = Event_Renamed.CmdType.ExitCmd;
                        break;
                    }

                case "explode":
                    {
                        CmdName = Event_Renamed.CmdType.ExplodeCmd;
                        break;
                    }

                case "expup":
                    {
                        CmdName = Event_Renamed.CmdType.ExpUpCmd;
                        break;
                    }

                case "fadein":
                    {
                        CmdName = Event_Renamed.CmdType.FadeInCmd;
                        break;
                    }

                case "fadeout":
                    {
                        CmdName = Event_Renamed.CmdType.FadeOutCmd;
                        break;
                    }

                case "fillcolor":
                    {
                        CmdName = Event_Renamed.CmdType.FillColorCmd;
                        break;
                    }

                case "fillstyle":
                    {
                        CmdName = Event_Renamed.CmdType.FillStyleCmd;
                        break;
                    }

                case "finish":
                    {
                        CmdName = Event_Renamed.CmdType.FinishCmd;
                        break;
                    }

                case "fix":
                    {
                        CmdName = Event_Renamed.CmdType.FixCmd;
                        break;
                    }

                case "for":
                    {
                        CmdName = Event_Renamed.CmdType.ForCmd;
                        break;
                    }

                case "foreach":
                    {
                        CmdName = Event_Renamed.CmdType.ForEachCmd;
                        break;
                    }

                case "next":
                    {
                        CmdName = Event_Renamed.CmdType.NextCmd;
                        break;
                    }

                case "font":
                    {
                        CmdName = Event_Renamed.CmdType.FontCmd;
                        break;
                    }

                case "forget":
                    {
                        CmdName = Event_Renamed.CmdType.ForgetCmd;
                        break;
                    }

                case "gameclear":
                    {
                        CmdName = Event_Renamed.CmdType.GameClearCmd;
                        break;
                    }

                case "gameover":
                    {
                        CmdName = Event_Renamed.CmdType.GameOverCmd;
                        break;
                    }

                case "freememory":
                    {
                        CmdName = Event_Renamed.CmdType.FreeMemoryCmd;
                        break;
                    }

                case "getoff":
                    {
                        CmdName = Event_Renamed.CmdType.GetOffCmd;
                        break;
                    }

                case "global":
                    {
                        CmdName = Event_Renamed.CmdType.GlobalCmd;
                        break;
                    }

                case "goto":
                    {
                        CmdName = Event_Renamed.CmdType.GotoCmd;
                        break;
                    }

                case "hide":
                    {
                        CmdName = Event_Renamed.CmdType.HideCmd;
                        break;
                    }

                case "hotpoint":
                    {
                        CmdName = Event_Renamed.CmdType.HotPointCmd;
                        break;
                    }

                case "if":
                    {
                        CmdName = Event_Renamed.CmdType.IfCmd;
                        break;
                    }

                case "else":
                    {
                        CmdName = Event_Renamed.CmdType.ElseCmd;
                        break;
                    }

                case "elseif":
                    {
                        CmdName = Event_Renamed.CmdType.ElseIfCmd;
                        break;
                    }

                case "endif":
                    {
                        CmdName = Event_Renamed.CmdType.EndIfCmd;
                        break;
                    }

                case "incr":
                    {
                        CmdName = Event_Renamed.CmdType.IncrCmd;
                        break;
                    }

                case "increasemorale":
                    {
                        CmdName = Event_Renamed.CmdType.IncreaseMoraleCmd;
                        break;
                    }

                case "input":
                    {
                        CmdName = Event_Renamed.CmdType.InputCmd;
                        break;
                    }

                case "intermissioncommand":
                    {
                        CmdName = Event_Renamed.CmdType.IntermissionCommandCmd;
                        break;
                    }

                case "item":
                    {
                        CmdName = Event_Renamed.CmdType.ItemCmd;
                        break;
                    }

                case "join":
                    {
                        CmdName = Event_Renamed.CmdType.JoinCmd;
                        break;
                    }

                case "keepbgm":
                    {
                        CmdName = Event_Renamed.CmdType.KeepBGMCmd;
                        break;
                    }

                case "land":
                    {
                        CmdName = Event_Renamed.CmdType.LandCmd;
                        break;
                    }

                case "launch":
                    {
                        CmdName = Event_Renamed.CmdType.LaunchCmd;
                        break;
                    }

                case "leave":
                    {
                        CmdName = Event_Renamed.CmdType.LeaveCmd;
                        break;
                    }

                case "levelup":
                    {
                        CmdName = Event_Renamed.CmdType.LevelUpCmd;
                        break;
                    }

                case "line":
                    {
                        CmdName = Event_Renamed.CmdType.LineCmd;
                        break;
                    }

                case "lineread":
                    {
                        CmdName = Event_Renamed.CmdType.LineReadCmd;
                        break;
                    }

                case "load":
                    {
                        CmdName = Event_Renamed.CmdType.LoadCmd;
                        break;
                    }

                case "local":
                    {
                        CmdName = Event_Renamed.CmdType.LocalCmd;
                        break;
                    }

                case "makepilotlist":
                    {
                        CmdName = Event_Renamed.CmdType.MakePilotListCmd;
                        break;
                    }

                case "makeunitlist":
                    {
                        CmdName = Event_Renamed.CmdType.MakeUnitListCmd;
                        break;
                    }

                case "mapability":
                    {
                        CmdName = Event_Renamed.CmdType.MapAbilityCmd;
                        break;
                    }

                case "mapattack":
                case "mapweapon":
                    {
                        CmdName = Event_Renamed.CmdType.MapAttackCmd;
                        break;
                    }

                case "money":
                    {
                        CmdName = Event_Renamed.CmdType.MoneyCmd;
                        break;
                    }

                case "monotone":
                    {
                        CmdName = Event_Renamed.CmdType.MonotoneCmd;
                        break;
                    }

                case "move":
                    {
                        CmdName = Event_Renamed.CmdType.MoveCmd;
                        break;
                    }

                case "night":
                    {
                        CmdName = Event_Renamed.CmdType.NightCmd;
                        break;
                    }

                case "noon":
                    {
                        CmdName = Event_Renamed.CmdType.NoonCmd;
                        break;
                    }

                case "open":
                    {
                        CmdName = Event_Renamed.CmdType.OpenCmd;
                        break;
                    }

                case "option":
                    {
                        CmdName = Event_Renamed.CmdType.OptionCmd;
                        break;
                    }

                case "organize":
                    {
                        CmdName = Event_Renamed.CmdType.OrganizeCmd;
                        break;
                    }

                case "oval":
                    {
                        CmdName = Event_Renamed.CmdType.OvalCmd;
                        break;
                    }

                case "paintpicture":
                    {
                        CmdName = Event_Renamed.CmdType.PaintPictureCmd;
                        break;
                    }

                case "paintstring":
                    {
                        CmdName = Event_Renamed.CmdType.PaintStringCmd;
                        break;
                    }

                case "paintsysstring":
                    {
                        CmdName = Event_Renamed.CmdType.PaintSysStringCmd;
                        break;
                    }

                case "pilot":
                    {
                        CmdName = Event_Renamed.CmdType.PilotCmd;
                        break;
                    }

                case "playmidi":
                    {
                        CmdName = Event_Renamed.CmdType.PlayMIDICmd;
                        break;
                    }

                case "playsound":
                    {
                        CmdName = Event_Renamed.CmdType.PlaySoundCmd;
                        break;
                    }

                case "polygon":
                    {
                        CmdName = Event_Renamed.CmdType.PolygonCmd;
                        break;
                    }

                case "print":
                    {
                        CmdName = Event_Renamed.CmdType.PrintCmd;
                        break;
                    }

                case "pset":
                    {
                        CmdName = Event_Renamed.CmdType.PSetCmd;
                        break;
                    }

                case "question":
                    {
                        CmdName = Event_Renamed.CmdType.QuestionCmd;
                        break;
                    }

                case "quickload":
                    {
                        CmdName = Event_Renamed.CmdType.QuickLoadCmd;
                        break;
                    }

                case "quit":
                    {
                        CmdName = Event_Renamed.CmdType.QuitCmd;
                        break;
                    }

                case "rankup":
                    {
                        CmdName = Event_Renamed.CmdType.RankUpCmd;
                        break;
                    }

                case "read":
                    {
                        CmdName = Event_Renamed.CmdType.ReadCmd;
                        break;
                    }

                case "recoveren":
                    {
                        CmdName = Event_Renamed.CmdType.RecoverENCmd;
                        break;
                    }

                case "recoverhp":
                    {
                        CmdName = Event_Renamed.CmdType.RecoverHPCmd;
                        break;
                    }

                case "recoverplana":
                    {
                        CmdName = Event_Renamed.CmdType.RecoverPlanaCmd;
                        break;
                    }

                case "recoversp":
                    {
                        CmdName = Event_Renamed.CmdType.RecoverSPCmd;
                        break;
                    }

                case "redraw":
                    {
                        CmdName = Event_Renamed.CmdType.RedrawCmd;
                        break;
                    }

                case "refresh":
                    {
                        CmdName = Event_Renamed.CmdType.RefreshCmd;
                        break;
                    }

                case "release":
                    {
                        CmdName = Event_Renamed.CmdType.ReleaseCmd;
                        break;
                    }

                case "removefile":
                    {
                        CmdName = Event_Renamed.CmdType.RemoveFileCmd;
                        break;
                    }

                case "removefolder":
                    {
                        CmdName = Event_Renamed.CmdType.RemoveFolderCmd;
                        break;
                    }

                case "removeitem":
                    {
                        CmdName = Event_Renamed.CmdType.RemoveItemCmd;
                        break;
                    }

                case "removepilot":
                    {
                        CmdName = Event_Renamed.CmdType.RemovePilotCmd;
                        break;
                    }

                case "removeunit":
                    {
                        CmdName = Event_Renamed.CmdType.RemoveUnitCmd;
                        break;
                    }

                case "renamebgm":
                    {
                        CmdName = Event_Renamed.CmdType.RenameBGMCmd;
                        break;
                    }

                case "renamefile":
                    {
                        CmdName = Event_Renamed.CmdType.RenameFileCmd;
                        break;
                    }

                case "renameterm":
                    {
                        CmdName = Event_Renamed.CmdType.RenameTermCmd;
                        break;
                    }

                case "replacepilot":
                    {
                        CmdName = Event_Renamed.CmdType.ReplacePilotCmd;
                        break;
                    }

                case "require":
                    {
                        CmdName = Event_Renamed.CmdType.RequireCmd;
                        break;
                    }

                case "restoreevent":
                    {
                        CmdName = Event_Renamed.CmdType.RestoreEventCmd;
                        break;
                    }

                case "ride":
                    {
                        CmdName = Event_Renamed.CmdType.RideCmd;
                        break;
                    }

                case "select":
                    {
                        CmdName = Event_Renamed.CmdType.SelectCmd;
                        break;
                    }

                case "savedata":
                    {
                        CmdName = Event_Renamed.CmdType.SaveDataCmd;
                        break;
                    }

                case "selecttarget":
                    {
                        CmdName = Event_Renamed.CmdType.SelectTargetCmd;
                        break;
                    }

                case "sepia":
                    {
                        CmdName = Event_Renamed.CmdType.SepiaCmd;
                        break;
                    }

                case "set":
                    {
                        CmdName = Event_Renamed.CmdType.SetCmd;
                        break;
                    }

                case "setbullet":
                    {
                        CmdName = Event_Renamed.CmdType.SetBulletCmd;
                        break;
                    }

                case "setmessage":
                    {
                        CmdName = Event_Renamed.CmdType.SetMessageCmd;
                        break;
                    }

                case "setrelation":
                    {
                        CmdName = Event_Renamed.CmdType.SetRelationCmd;
                        break;
                    }

                case "setskill":
                case "setability":
                    {
                        CmdName = Event_Renamed.CmdType.SetSkillCmd;
                        break;
                    }

                case "setstatus":
                    {
                        CmdName = Event_Renamed.CmdType.SetStatusCmd;
                        break;
                    }
                // ADD START 240a
                case "setstatusstringcolor":
                    {
                        CmdName = Event_Renamed.CmdType.SetStatusStringColorCmd;
                        break;
                    }
                // ADD  END
                case "setstock":
                    {
                        CmdName = Event_Renamed.CmdType.SetStockCmd;
                        break;
                    }
                // ADD START 240a
                case "setwindowcolor":
                    {
                        CmdName = Event_Renamed.CmdType.SetWindowColorCmd;
                        break;
                    }

                case "setwindowframewidth":
                    {
                        CmdName = Event_Renamed.CmdType.SetWindowFrameWidthCmd;
                        break;
                    }
                // ADD  END
                case "show":
                    {
                        CmdName = Event_Renamed.CmdType.ShowCmd;
                        break;
                    }

                case "showimage":
                    {
                        CmdName = Event_Renamed.CmdType.ShowImageCmd;
                        break;
                    }

                case "showunitstatus":
                    {
                        CmdName = Event_Renamed.CmdType.ShowUnitStatusCmd;
                        break;
                    }

                case "skip":
                    {
                        CmdName = Event_Renamed.CmdType.SkipCmd;
                        break;
                    }

                case "sort":
                    {
                        CmdName = Event_Renamed.CmdType.SortCmd;
                        break;
                    }

                case "specialpower":
                case "mind":
                    {
                        CmdName = Event_Renamed.CmdType.SpecialPowerCmd;
                        break;
                    }

                case "split":
                    {
                        CmdName = Event_Renamed.CmdType.SplitCmd;
                        break;
                    }

                case "startbgm":
                    {
                        CmdName = Event_Renamed.CmdType.StartBGMCmd;
                        break;
                    }

                case "stopbgm":
                    {
                        CmdName = Event_Renamed.CmdType.StopBGMCmd;
                        break;
                    }

                case "stopsummoning":
                    {
                        CmdName = Event_Renamed.CmdType.StopSummoningCmd;
                        break;
                    }

                case "supply":
                    {
                        CmdName = Event_Renamed.CmdType.SupplyCmd;
                        break;
                    }

                case "sunset":
                    {
                        CmdName = Event_Renamed.CmdType.SunsetCmd;
                        break;
                    }

                case "swap":
                    {
                        CmdName = Event_Renamed.CmdType.SwapCmd;
                        break;
                    }

                case "switch":
                    {
                        CmdName = Event_Renamed.CmdType.SwitchCmd;
                        break;
                    }

                case "playflash":
                    {
                        CmdName = Event_Renamed.CmdType.PlayFlashCmd;
                        break;
                    }

                case "clearflash":
                    {
                        CmdName = Event_Renamed.CmdType.ClearFlashCmd;
                        break;
                    }

                case "case":
                    {
                        CmdName = Event_Renamed.CmdType.CaseCmd;
                        if (ArgNum == 2)
                        {
                            if (Strings.LCase(list[2]) == "else")
                            {
                                CmdName = Event_Renamed.CmdType.CaseElseCmd;
                            }
                        }

                        break;
                    }

                case "endsw":
                    {
                        CmdName = Event_Renamed.CmdType.EndSwCmd;
                        break;
                    }

                case "talk":
                    {
                        CmdName = Event_Renamed.CmdType.TalkCmd;
                        break;
                    }

                case "end":
                    {
                        CmdName = Event_Renamed.CmdType.EndCmd;
                        break;
                    }

                case "suspend":
                    {
                        CmdName = Event_Renamed.CmdType.SuspendCmd;
                        break;
                    }

                case "telop":
                    {
                        CmdName = Event_Renamed.CmdType.TelopCmd;
                        break;
                    }

                case "transform":
                    {
                        CmdName = Event_Renamed.CmdType.TransformCmd;
                        break;
                    }

                case "unit":
                    {
                        CmdName = Event_Renamed.CmdType.UnitCmd;
                        break;
                    }

                case "unset":
                    {
                        CmdName = Event_Renamed.CmdType.UnsetCmd;
                        break;
                    }

                case "upgrade":
                    {
                        CmdName = Event_Renamed.CmdType.UpgradeCmd;
                        break;
                    }

                case "upvar":
                    {
                        CmdName = Event_Renamed.CmdType.UpVarCmd;
                        break;
                    }

                case "useability":
                    {
                        CmdName = Event_Renamed.CmdType.UseAbilityCmd;
                        break;
                    }

                case "wait":
                    {
                        CmdName = Event_Renamed.CmdType.WaitCmd;
                        break;
                    }

                case "water":
                    {
                        CmdName = Event_Renamed.CmdType.WaterCmd;
                        break;
                    }

                case "whitein":
                    {
                        CmdName = Event_Renamed.CmdType.WhiteInCmd;
                        break;
                    }

                case "whiteout":
                    {
                        CmdName = Event_Renamed.CmdType.WhiteOutCmd;
                        break;
                    }

                case "write":
                    {
                        CmdName = Event_Renamed.CmdType.WriteCmd;
                        break;
                    }

                default:
                    {
                        // 定義済みのイベントコマンドではない

                        if (ArgNum >= 3)
                        {
                            if (list[2] == "=")
                            {
                                // 代入式

                                CmdName = Event_Renamed.CmdType.SetCmd;
                                // UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                Array.Resize(ref strArgs, 4);
                                // UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                Array.Resize(ref lngArgs, 4);
                                // UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                Array.Resize(ref dblArgs, 4);
                                // UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                Array.Resize(ref ArgsType, 4);

                                // 代入先の変数名
                                strArgs[2] = list[1];
                                ArgsType[2] = Expression.ValueType.StringType;

                                // 代入する値
                                // (値が項の場合は既に引数の処理が済んでいるのでなにもしなくてよい)
                                if (ArgNum > 3)
                                {
                                    ArgsType[3] = Expression.ValueType.UndefinedType;
                                    // GetValueAsStringの呼び出しの際に、Argsの内容は必ず項と仮定
                                    // されているので、わざと項にしておく
                                    strArgs[3] = "(" + GeneralLib.ListTail(ref edata, 3) + ")";
                                }

                                ArgNum = 3;
                                return ParseRet;
                            }
                        }

                        if (ArgNum == -1)
                        {
                            CmdName = Event_Renamed.CmdType.NopCmd;
                            return ParseRet;
                        }

                        // サブルーチンコール？
                        CmdName = Event_Renamed.CmdType.CallCmd;
                        // UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                        Array.Resize(ref strArgs, ArgNum + 1 + 1);
                        // UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                        Array.Resize(ref lngArgs, ArgNum + 1 + 1);
                        // UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                        Array.Resize(ref dblArgs, ArgNum + 1 + 1);
                        // UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                        Array.Resize(ref ArgsType, ArgNum + 1 + 1);
                        // 引数を１個ずらす
                        var loopTo1 = (short)(ArgNum - 2);
                        for (i = 0; i <= loopTo1; i++)
                        {
                            strArgs[ArgNum + 1 - i] = strArgs[ArgNum - i];
                            lngArgs[ArgNum + 1 - i] = lngArgs[ArgNum - i];
                            dblArgs[ArgNum + 1 - i] = dblArgs[ArgNum - i];
                            ArgsType[ArgNum + 1 - i] = ArgsType[ArgNum - i];
                        }

                        ArgNum = (short)(ArgNum + 1);
                        // 第２引数をサブルーチン名に設定
                        strArgs[2] = list[1];
                        if (Event_Renamed.FindNormalLabel(ref list[1]) > 0)
                        {
                            ArgsType[2] = Expression.ValueType.StringType;
                        }
                        else
                        {
                            ArgsType[2] = Expression.ValueType.UndefinedType;
                        }

                        return ParseRet;
                    }
            }

            if (CmdName == Event_Renamed.CmdType.IfCmd | CmdName == Event_Renamed.CmdType.ElseIfCmd)
            {
                // If文の処理の高速化のため、あらかじめ構文解析しておく
                if (ArgNum == 1)
                {
                    // 書式エラー
                    Event_Renamed.DisplayEventErrorMessage(Event_Renamed.CurrentLineNum, "Ifコマンドの書式に合っていません");
                    ParseRet = false;
                    return ParseRet;
                }

                expr = list[2];
                var loopTo2 = ArgNum;
                for (i = 3; i <= loopTo2; i++)
                {
                    buf = list[i];
                    switch (Strings.LCase(buf) ?? "")
                    {
                        case "then":
                        case "exit":
                            {
                                // UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                strArgs = new string[5];
                                // UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                lngArgs = new int[5];
                                // UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                dblArgs = new double[5];
                                // UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                ArgsType = new Expression.ValueType[5];
                                strArgs[2] = expr;
                                lngArgs[3] = ArgNum - 2;
                                ArgsType[3] = Expression.ValueType.NumericType;
                                strArgs[4] = Strings.LCase(buf);
                                break;
                            }

                        case "goto":
                            {
                                buf = GetArg((short)(i + 1));
                                // UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                strArgs = new string[6];
                                // UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                lngArgs = new int[6];
                                // UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                dblArgs = new double[6];
                                // UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                ArgsType = new Expression.ValueType[6];
                                strArgs[2] = expr;
                                lngArgs[3] = ArgNum - 3;
                                ArgsType[3] = Expression.ValueType.NumericType;
                                strArgs[4] = "goto";
                                strArgs[5] = buf;
                                break;
                            }

                        case var case1 when case1 == "":
                            {
                                buf = "\"\"";
                                break;
                            }
                    }

                    expr = expr + " " + buf;
                }

                if (i > ArgNum)
                {
                    if (CmdName == Event_Renamed.CmdType.IfCmd)
                    {
                        Event_Renamed.DisplayEventErrorMessage(LineNum, "Ifに対応する Then または Exit または Goto がありません");
                    }
                    else
                    {
                        Event_Renamed.DisplayEventErrorMessage(LineNum, "ElseIfに対応する Then または Exit または Goto がありません");
                    }

                    SRC.TerminateSRC();
                }

                // 条件式が式であることが確定していれば条件式の項数を0に
                switch (lngArgs[3])
                {
                    case 0:
                        {
                            if (CmdName == Event_Renamed.CmdType.IfCmd)
                            {
                                Event_Renamed.DisplayEventErrorMessage(LineNum, "Ifコマンドの条件式がありません");
                            }
                            else
                            {
                                Event_Renamed.DisplayEventErrorMessage(LineNum, "ElseIfコマンドの条件式がありません");
                            }

                            SRC.TerminateSRC();
                            break;
                        }

                    case 1:
                        {
                            switch (Strings.Asc(expr))
                            {
                                case 36: // $
                                    {
                                        lngArgs[3] = 0;
                                        break;
                                    }

                                case 40: // (
                                    {
                                        // ()を除去
                                        strArgs[2] = Strings.Mid(expr, 2, Strings.Len(expr) - 2);
                                        lngArgs[3] = 0;
                                        break;
                                    }
                            }

                            break;
                        }

                    case 2:
                        {
                            if (Strings.LCase(GeneralLib.LIndex(ref expr, 1)) == "not")
                            {
                                switch (Strings.Asc(GeneralLib.ListIndex(ref expr, 2)))
                                {
                                    case 36:
                                    case 40: // $, (
                                        {
                                            lngArgs[3] = 0;
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                lngArgs[3] = 0;
                            }

                            break;
                        }

                    default:
                        {
                            lngArgs[3] = 0;
                            break;
                        }
                }

                return ParseRet;
            }

            if (CmdName == Event_Renamed.CmdType.PaintStringCmd)
            {
                // PaintString文の処理の高速化のため、あらかじめ構文解析しておく

                // 「;」を含む場合は改めて項に分解
                // (正しくリストの処理が行えないため)
                if (Strings.Right(buf, 1) == ";")
                {
                    buf = edata;
                    CmdName = Event_Renamed.CmdType.PaintStringRCmd;
                    buf = Strings.Left(buf, Strings.Len(buf) - 1);
                    if (Strings.Right(buf, 1) == " ")
                    {
                        // メッセージが空文字列
                        buf = buf + "\"\"";
                    }

                    ArgNum = GeneralLib.ListSplit(ref buf, ref list);
                }

                switch (ArgNum)
                {
                    case 2:
                        {
                            // 引数が１個の場合
                            ArgNum = 2;
                            // UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                            strArgs = new string[3];
                            // UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                            lngArgs = new int[3];
                            // UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                            dblArgs = new double[3];
                            // UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                            ArgsType = new Expression.ValueType[3];
                            buf = list[2];

                            // 表示文字列が式の場合にも対応
                            if (Strings.Left(buf, 1) == "\"" & Strings.Right(buf, 1) == "\"")
                            {
                                if (Strings.InStr(buf, "$(") > 0)
                                {
                                    strArgs[2] = buf;
                                }
                                else
                                {
                                    strArgs[2] = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
                                    ArgsType[2] = Expression.ValueType.StringType;
                                }
                            }
                            else if (Strings.Left(buf, 1) == "`" & Strings.Right(buf, 1) == "`")
                            {
                                strArgs[2] = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
                                ArgsType[2] = Expression.ValueType.StringType;
                            }
                            else if (Strings.InStr(buf, "$(") > 0)
                            {
                                strArgs[2] = "\"" + buf + "\"";
                            }
                            else
                            {
                                strArgs[2] = buf;
                            }

                            break;
                        }

                    case 3:
                        {
                            // 引数が２個の場合
                            ArgNum = 2;
                            // UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                            strArgs = new string[3];
                            // UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                            lngArgs = new int[3];
                            // UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                            dblArgs = new double[3];
                            // UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                            ArgsType = new Expression.ValueType[3];

                            // 表示文字列は必ず文字列
                            buf = GeneralLib.ListTail(ref edata, 2);
                            if (Strings.InStr(buf, "$(") > 0)
                            {
                                strArgs[2] = "\"" + buf + "\"";
                            }
                            else
                            {
                                strArgs[2] = buf;
                                ArgsType[2] = Expression.ValueType.StringType;
                            }

                            break;
                        }

                    case 4:
                        {
                            // 引数が３個の場合

                            // 座標指定があるかどうかが確定しているか？
                            if ((list[2] == "-" | Information.IsNumeric(list[2]) | Expression.IsExpr(ref list[2])) & (list[3] == "-" | Information.IsNumeric(list[3]) | Expression.IsExpr(ref list[3])))
                            {
                                // 座標指定があることが確定
                                ArgNum = 4;
                                // UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                strArgs = new string[5];
                                // UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                lngArgs = new int[5];
                                // UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                dblArgs = new double[5];
                                // UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                ArgsType = new Expression.ValueType[5];
                                strArgs[2] = list[2];
                                strArgs[3] = list[3];
                                if (!Expression.IsExpr(ref list[2]))
                                {
                                    ArgsType[2] = Expression.ValueType.StringType;
                                }

                                if (!Expression.IsExpr(ref list[3]))
                                {
                                    ArgsType[3] = Expression.ValueType.StringType;
                                }
                            }
                            else
                            {
                                // 実行時まで座標指定があるかどうか不明
                                ArgNum = 5;
                                // UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                strArgs = new string[6];
                                // UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                lngArgs = new int[6];
                                // UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                dblArgs = new double[6];
                                // UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                ArgsType = new Expression.ValueType[6];
                                strArgs[2] = list[2];
                                strArgs[3] = list[3];

                                // 座標指定がなかった場合の表示文字列
                                buf = GeneralLib.ListTail(ref edata, 2);
                                if (Strings.InStr(buf, "$(") > 0)
                                {
                                    strArgs[5] = "\"" + buf + "\"";
                                }
                                else
                                {
                                    strArgs[5] = buf;
                                    ArgsType[5] = Expression.ValueType.StringType;
                                }
                            }

                            // 座標指定があった場合の表示文字列
                            buf = list[4];
                            if (Strings.Left(buf, 1) == "\"" & Strings.Right(buf, 1) == "\"")
                            {
                                if (Strings.InStr(buf, "$(") > 0)
                                {
                                    strArgs[4] = buf;
                                }
                                else
                                {
                                    strArgs[4] = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
                                    ArgsType[4] = Expression.ValueType.StringType;
                                }
                            }
                            else if (Strings.Left(buf, 1) == "`" & Strings.Right(buf, 1) == "`")
                            {
                                strArgs[4] = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
                                ArgsType[4] = Expression.ValueType.StringType;
                            }
                            else if (Strings.InStr(buf, "$(") > 0)
                            {
                                strArgs[4] = "\"" + buf + "\"";
                            }
                            else
                            {
                                strArgs[4] = buf;
                            }

                            break;
                        }

                    default:
                        {
                            // 引数が４個以上の場合

                            // 座標指定があるかどうかが確定しているか？
                            if ((list[2] == "-" | Information.IsNumeric(list[2]) | Expression.IsExpr(ref list[2])) & (list[3] == "-" | Information.IsNumeric(list[3]) | Expression.IsExpr(ref list[3])))
                            {
                                // 座標指定があることが確定
                                ArgNum = 4;
                                // UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                strArgs = new string[5];
                                // UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                lngArgs = new int[5];
                                // UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                dblArgs = new double[5];
                                // UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                ArgsType = new Expression.ValueType[5];
                                strArgs[2] = list[2];
                                strArgs[3] = list[3];
                                if (!Expression.IsExpr(ref list[2]))
                                {
                                    ArgsType[2] = Expression.ValueType.StringType;
                                }

                                if (!Expression.IsExpr(ref list[3]))
                                {
                                    ArgsType[3] = Expression.ValueType.StringType;
                                }
                            }
                            else
                            {
                                // 実行時まで座標指定があるかどうか不明
                                ArgNum = 5;
                                // UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                strArgs = new string[6];
                                // UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                lngArgs = new int[6];
                                // UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                dblArgs = new double[6];
                                // UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                                ArgsType = new Expression.ValueType[6];
                                strArgs[2] = list[2];
                                strArgs[3] = list[3];

                                // 座標指定がなかった場合の表示文字列
                                buf = GeneralLib.ListTail(ref edata, 2);
                                if (Strings.InStr(buf, "$(") > 0)
                                {
                                    strArgs[5] = "\"" + buf + "\"";
                                }
                                else
                                {
                                    strArgs[5] = buf;
                                    ArgsType[5] = Expression.ValueType.StringType;
                                }
                            }

                            // 座標指定があった場合の表示文字列
                            buf = GeneralLib.ListTail(ref edata, 4);
                            if (Strings.InStr(buf, "$(") > 0)
                            {
                                strArgs[4] = "\"" + buf + "\"";
                            }
                            else
                            {
                                strArgs[4] = buf;
                                ArgsType[4] = Expression.ValueType.StringType;
                            }

                            break;
                        }
                }

                return ParseRet;
            }

            if (CmdName == Event_Renamed.CmdType.CallCmd)
            {
                // Callコマンドのサブルーチン指定が式かどうか調べておく
                if (Event_Renamed.FindNormalLabel(ref strArgs[2]) > 0)
                {
                    ArgsType[2] = Expression.ValueType.StringType;
                }
                else
                {
                    ArgsType[2] = Expression.ValueType.UndefinedType;
                }
            }

            if (CmdName == Event_Renamed.CmdType.LocalCmd)
            {
                if (ArgNum > 4)
                {
                    if (list[3] == "=")
                    {
                        // Localコマンドが複数項から成る代入式を伴う場合

                        // UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                        Array.Resize(ref strArgs, 5);
                        // UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                        Array.Resize(ref lngArgs, 5);
                        // UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                        Array.Resize(ref dblArgs, 5);
                        // UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
                        Array.Resize(ref ArgsType, 5);

                        // 代入する値
                        ArgsType[4] = Expression.ValueType.UndefinedType;
                        strArgs[4] = "(" + GeneralLib.ListTail(ref edata, 4) + ")";
                        ArgNum = 4;
                        return ParseRet;
                    }
                }
            }

            return ParseRet;
        ErrorHandler:
            ;
            Event_Renamed.DisplayEventErrorMessage(LineNum, "イベントコマンドの内容が不正です");
            ParseRet = false;
        }

    }
}
