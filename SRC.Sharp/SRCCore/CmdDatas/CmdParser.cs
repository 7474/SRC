﻿using Microsoft.Extensions.Logging;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.Lib;
using SRCCore.VB;
using System;

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
                string[] list;
                var llength = GeneralLib.ListSplit(edata, out list);

                // コマンドの種類を判定
                switch (Strings.LCase(list[0]) ?? "")
                {
                    case "arc":
                        return new NotImplementedCmd(src, data);

                    case "array":
                        return new NotImplementedCmd(src, data);

                    case "ask":
                        return new AskCmd(src, data);

                    case "attack":
                        return new NotImplementedCmd(src, data);

                    case "autotalk":
                        return new AutoTalkCmd(src, data);

                    case "bossrank":
                        return new NotImplementedCmd(src, data);

                    case "break":
                        return new BreakCmd(src, data);

                    case "call":
                        return new CallCmd(src, data);

                    case "return":
                        return new ReturnCmd(src, data);

                    case "callintermissioncommand":
                        return new NotImplementedCmd(src, data);

                    case "cancel":
                        return new CancelCmd(src, data);

                    case "center":
                        return new CenterCmd(src, data);

                    case "changearea":
                        return new NotImplementedCmd(src, data);

                    case "changelayer":
                        return new NotImplementedCmd(src, data);

                    case "changemap":
                        return new ChangeMapCmd(src, data);

                    case "changemode":
                        return new ChangeModeCmd(src, data);

                    case "changeparty":
                        return new ChangePartyCmd(src, data);

                    case "changeterrain":
                        return new NotImplementedCmd(src, data);

                    case "changeunitbitmap":
                        return new NotImplementedCmd(src, data);

                    case "charge":
                        return new NotImplementedCmd(src, data);

                    case "circle":
                        return new NotImplementedCmd(src, data);

                    case "clearevent":
                        return new ClearEventCmd(src, data);

                    case "clearimage":
                        return new NotImplementedCmd(src, data);

                    case "clearlayer":
                        return new NotImplementedCmd(src, data);

                    case "clearobj":
                        return new NotImplementedCmd(src, data);

                    case "clearpicture":
                        return new ClearPictureCmd(src, data);

                    case "clearskill":
                        return new NotImplementedCmd(src, data);

                    case "clearability":
                        return new NotImplementedCmd(src, data);

                    case "clearspecialpower":
                        return new NotImplementedCmd(src, data);

                    case "clearmind":
                        return new NotImplementedCmd(src, data);

                    case "clearstatus":
                        return new NotImplementedCmd(src, data);

                    case "cls":
                        return new NotImplementedCmd(src, data);

                    case "close":
                        return new NotImplementedCmd(src, data);

                    case "color":
                        return new NotImplementedCmd(src, data);

                    case "colorfilter":
                        return new NotImplementedCmd(src, data);

                    case "combine":
                        return new NotImplementedCmd(src, data);

                    case "confirm":
                        return new ConfirmCmd(src, data);

                    case "continue":
                        return new ContinueCmd(src, data);

                    case "copyarray":
                        return new NotImplementedCmd(src, data);

                    case "copyfile":
                        return new NotImplementedCmd(src, data);

                    case "create":
                        return new CreateCmd(src, data);

                    case "createfolder":
                        return new NotImplementedCmd(src, data);

                    case "debug":
                        return new NotImplementedCmd(src, data);

                    case "destroy":
                        return new NotImplementedCmd(src, data);

                    case "disable":
                        return new NotImplementedCmd(src, data);

                    case "do":
                        return new DoCmd(src, data);

                    case "loop":
                        return new LoopCmd(src, data);

                    case "drawoption":
                        return new NotImplementedCmd(src, data);

                    case "drawwidth":
                        return new NotImplementedCmd(src, data);

                    case "enable":
                        return new NotImplementedCmd(src, data);

                    case "equip":
                        return new NotImplementedCmd(src, data);

                    case "escape":
                        return new NotImplementedCmd(src, data);

                    case "exchangeitem":
                        return new NotImplementedCmd(src, data);

                    case "exec":
                        return new NotImplementedCmd(src, data);

                    case "exit":
                        return new ExitCmd(src, data);

                    case "explode":
                        return new NotImplementedCmd(src, data);

                    case "expup":
                        return new NotImplementedCmd(src, data);

                    case "fadein":
                        return new NotImplementedCmd(src, data);

                    case "fadeout":
                        return new NotImplementedCmd(src, data);

                    case "fillcolor":
                        return new NotImplementedCmd(src, data);

                    case "fillstyle":
                        return new NotImplementedCmd(src, data);

                    case "finish":
                        return new FinishCmd(src, data);

                    case "fix":
                        return new NotImplementedCmd(src, data);

                    case "for":
                        return new NotImplementedCmd(src, data);

                    case "foreach":
                        return new NotImplementedCmd(src, data);

                    case "next":
                        return new NotImplementedCmd(src, data);

                    case "font":
                        return new NotImplementedCmd(src, data);

                    case "forget":
                        return new NotImplementedCmd(src, data);

                    case "gameclear":
                        return new GameClearCmd(src, data);

                    case "gameover":
                        return new GameOverCmd(src, data);

                    case "freememory":
                        return new NotImplementedCmd(src, data);

                    case "getoff":
                        return new NotImplementedCmd(src, data);

                    case "global":
                        return new NotImplementedCmd(src, data);

                    case "goto":
                        return new NotImplementedCmd(src, data);

                    case "hide":
                        return new NotImplementedCmd(src, data);

                    case "hotpoint":
                        return new NotImplementedCmd(src, data);

                    case "if":
                        return new IfCmd(src, data);

                    case "else":
                        return new ElseCmd(src, data);

                    case "elseif":
                        return new ElseIfCmd(src, data);

                    case "endif":
                        return new EndIfCmd(src, data);

                    case "incr":
                        return new NotImplementedCmd(src, data);

                    case "increasemorale":
                        return new NotImplementedCmd(src, data);

                    case "input":
                        return new InputCmd(src, data);

                    case "intermissioncommand":
                        return new NotImplementedCmd(src, data);

                    case "item":
                        return new NotImplementedCmd(src, data);

                    case "join":
                        return new NotImplementedCmd(src, data);

                    case "keepbgm":
                        return new KeepBGMCmd(src, data);

                    case "land":
                        return new NotImplementedCmd(src, data);

                    case "launch":
                        return new LaunchCmd(src, data);

                    case "leave":
                        return new NotImplementedCmd(src, data);

                    case "levelup":
                        return new NotImplementedCmd(src, data);

                    case "line":
                        return new NotImplementedCmd(src, data);

                    case "lineread":
                        return new NotImplementedCmd(src, data);

                    case "load":
                        return new NotImplementedCmd(src, data);

                    case "local":
                        return new NotImplementedCmd(src, data);

                    case "makepilotlist":
                        return new NotImplementedCmd(src, data);

                    case "makeunitlist":
                        return new NotImplementedCmd(src, data);

                    case "mapability":
                        return new NotImplementedCmd(src, data);

                    case "mapattack":
                        return new NotImplementedCmd(src, data);
                    case "mapweapon":
                        return new NotImplementedCmd(src, data);

                    case "money":
                        return new NotImplementedCmd(src, data);

                    case "monotone":
                        return new NotImplementedCmd(src, data);

                    case "move":
                        return new MoveCmd(src, data);

                    case "night":
                        return new NotImplementedCmd(src, data);

                    case "noon":
                        return new NotImplementedCmd(src, data);

                    case "open":
                        return new NotImplementedCmd(src, data);

                    case "option":
                        return new NotImplementedCmd(src, data);

                    case "organize":
                        return new NotImplementedCmd(src, data);

                    case "oval":
                        return new NotImplementedCmd(src, data);

                    case "paintpicture":
                        return new PaintPictureCmd(src, data);

                    case "paintstring":
                        return new PaintStringCmd(src, data);

                    case "paintsysstring":
                        return new NotImplementedCmd(src, data);

                    case "pilot":
                        return new PilotCmd(src, data);

                    case "playmidi":
                        return new PlayMIDICmd(src, data);

                    case "playsound":
                        return new PlaySoundCmd(src, data);

                    case "polygon":
                        return new NotImplementedCmd(src, data);

                    case "print":
                        return new NotImplementedCmd(src, data);

                    case "pset":
                        return new NotImplementedCmd(src, data);

                    case "question":
                        return new QuestionCmd(src, data);

                    case "quickload":
                        return new NotImplementedCmd(src, data);

                    case "quit":
                        return new QuitCmd(src, data);

                    case "rankup":
                        return new NotImplementedCmd(src, data);

                    case "read":
                        return new NotImplementedCmd(src, data);

                    case "recoveren":
                        return new NotImplementedCmd(src, data);

                    case "recoverhp":
                        return new NotImplementedCmd(src, data);

                    case "recoverplana":
                        return new NotImplementedCmd(src, data);

                    case "recoversp":
                        return new NotImplementedCmd(src, data);

                    case "redraw":
                        return new RedrawCmd(src, data);

                    case "refresh":
                        return new RefreshCmd(src, data);

                    case "release":
                        return new NotImplementedCmd(src, data);

                    case "removefile":
                        return new NotImplementedCmd(src, data);

                    case "removefolder":
                        return new NotImplementedCmd(src, data);

                    case "removeitem":
                        return new NotImplementedCmd(src, data);

                    case "removepilot":
                        return new NotImplementedCmd(src, data);

                    case "removeunit":
                        return new NotImplementedCmd(src, data);

                    case "renamebgm":
                        return new NotImplementedCmd(src, data);

                    case "renamefile":
                        return new NotImplementedCmd(src, data);

                    case "renameterm":
                        return new NotImplementedCmd(src, data);

                    case "replacepilot":
                        return new NotImplementedCmd(src, data);

                    case "require":
                        return new NotImplementedCmd(src, data);

                    case "restoreevent":
                        return new RestoreEventCmd(src, data);

                    case "ride":
                        return new RideCmd(src, data);

                    case "select":
                        return new NotImplementedCmd(src, data);

                    case "savedata":
                        return new NotImplementedCmd(src, data);

                    case "selecttarget":
                        return new NotImplementedCmd(src, data);

                    case "sepia":
                        return new NotImplementedCmd(src, data);

                    case "set":
                        return new SetCmd(src, data);

                    case "setbullet":
                        return new NotImplementedCmd(src, data);

                    case "setmessage":
                        return new NotImplementedCmd(src, data);

                    case "setrelation":
                        return new NotImplementedCmd(src, data);

                    case "setskill":
                        return new NotImplementedCmd(src, data);
                    case "setability":
                        return new NotImplementedCmd(src, data);

                    case "setstatus":
                        return new NotImplementedCmd(src, data);
                    //// ADD START 240a
                    case "setstatusstringcolor":
                        return new NotImplementedCmd(src, data);
                    //// ADD  END
                    case "setstock":
                        return new NotImplementedCmd(src, data);
                    //// ADD START 240a
                    case "setwindowcolor":
                        return new NotImplementedCmd(src, data);

                    case "setwindowframewidth":
                        return new NotImplementedCmd(src, data);
                    //// ADD  END
                    case "show":
                        return new NotImplementedCmd(src, data);

                    case "showimage":
                        return new NotImplementedCmd(src, data);

                    case "showunitstatus":
                        return new NotImplementedCmd(src, data);

                    case "skip":
                        return new SkipCmd(src, data);

                    case "sort":
                        return new NotImplementedCmd(src, data);

                    case "specialpower":
                        return new NotImplementedCmd(src, data);
                    case "mind":
                        return new NotImplementedCmd(src, data);

                    case "split":
                        return new NotImplementedCmd(src, data);

                    case "startbgm":
                        return new StartBGMCmd(src, data);

                    case "stopbgm":
                        return new StopBGMCmd(src, data);

                    case "stopsummoning":
                        return new NotImplementedCmd(src, data);

                    case "supply":
                        return new NotImplementedCmd(src, data);

                    case "sunset":
                        return new NotImplementedCmd(src, data);

                    case "swap":
                        return new NotImplementedCmd(src, data);

                    case "switch":
                        return new SwitchCmd(src, data);

                    case "playflash":
                        return new NotImplementedCmd(src, data);

                    case "clearflash":
                        return new NotImplementedCmd(src, data);

                    case "case":
                        if (list.Length == 2)
                        {
                            if (Strings.LCase(list[1]) == "else")
                            {
                                return new CaseElseCmd(src, data);
                            }
                        }
                        return new CaseCmd(src, data);

                    case "endsw":
                        return new EndSwCmd(src, data);

                    case "talk":
                        return new TalkCmd(src, data);

                    case "end":
                        return new EndCmd(src, data);

                    case "suspend":
                        return new SuspendCmd(src, data);

                    case "telop":
                        return new NotImplementedCmd(src, data);

                    case "transform":
                        return new NotImplementedCmd(src, data);

                    case "unit":
                        return new UnitCmd(src, data);

                    case "unset":
                        return new NotImplementedCmd(src, data);

                    case "upgrade":
                        return new NotImplementedCmd(src, data);

                    case "upvar":
                        return new NotImplementedCmd(src, data);

                    case "useability":
                        return new NotImplementedCmd(src, data);

                    case "wait":
                        return new WaitCmd(src, data);

                    case "water":
                        return new NotImplementedCmd(src, data);

                    case "whitein":
                        return new NotImplementedCmd(src, data);

                    case "whiteout":
                        return new NotImplementedCmd(src, data);

                    case "write":
                        return new NotImplementedCmd(src, data);

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

                            if (llength == -1)
                            {
                                return new NopCmd(src, data);
                            }

                            // サブルーチンコール？
                            // TODO 多分Talkの中身が壊れる場面がある。
                            return new CallCmd(src, new EventDataLine(
                                data.ID, data.Source, data.File, data.LineNum, "Call " + data.Data
                            ));
                        }
                }

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
            catch (Exception ex)
            {
                // TODO Impl
                src.Log.LogError(ex.Message, ex);
                //Event.DisplayEventErrorMessage(EventDataId, "イベントコマンドの内容が不正です");
                return new NopCmd(src, data);
            }
        }
    }
}
