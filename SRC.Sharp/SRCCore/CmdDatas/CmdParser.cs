using Microsoft.Extensions.Logging;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.Lib;
using SRCCore.VB;
using System;
using System.Linq;

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
                        return new ArcCmd(src, data);

                    case "array":
                        return new ArrayCmd(src, data);

                    case "ask":
                        return new AskCmd(src, data);

                    case "attack":
                        return new NotImplementedCmd(src, data);

                    case "autotalk":
                        return new AutoTalkCmd(src, data);

                    case "bossrank":
                        return new BossRankCmd(src, data);

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
                        return new ChangeAreaCmd(src, data);

                    case "changelayer":
                        return new NotImplementedCmd(src, data);

                    case "changemap":
                        return new ChangeMapCmd(src, data);

                    case "changemode":
                        return new ChangeModeCmd(src, data);

                    case "changeparty":
                        return new ChangePartyCmd(src, data);

                    case "changeterrain":
                        return new ChangeTerrainCmd(src, data);

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
                        return new ColorFilterCmd(src, data);

                    case "combine":
                        return new NotImplementedCmd(src, data);

                    case "confirm":
                        return new ConfirmCmd(src, data);

                    case "continue":
                        return new ContinueCmd(src, data);

                    case "copyarray":
                        return new CopyArrayCmd(src, data);

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
                        return new FadeInCmd(src, data);

                    case "fadeout":
                        return new FadeOutCmd(src, data);

                    case "fillcolor":
                        return new NotImplementedCmd(src, data);

                    case "fillstyle":
                        return new NotImplementedCmd(src, data);

                    case "finish":
                        return new FinishCmd(src, data);

                    case "fix":
                        return new NotImplementedCmd(src, data);

                    case "for":
                        return new ForCmd(src, data);

                    case "foreach":
                        return new ForEachCmd(src, data);

                    case "next":
                        return new NextCmd(src, data);

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
                        return new GlobalCmd(src, data);

                    case "goto":
                        return new GotoCmd(src, data);

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
                        return new IncrCmd(src, data);

                    case "increasemorale":
                        return new IncreaseMoraleCmd(src, data);

                    case "input":
                        return new InputCmd(src, data);

                    case "intermissioncommand":
                        return new IntermissionCommandCmd(src, data);

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
                        return new LocalCmd(src, data);

                    case "makepilotlist":
                        return new MakePilotListCmd(src, data);

                    case "makeunitlist":
                        return new MakeUnitListCmd(src, data);

                    case "mapability":
                        return new NotImplementedCmd(src, data);

                    case "mapattack":
                        return new NotImplementedCmd(src, data);
                    case "mapweapon":
                        return new NotImplementedCmd(src, data);

                    case "money":
                        return new NotImplementedCmd(src, data);

                    case "monotone":
                        return new MonotoneCmd(src, data);

                    case "move":
                        return new MoveCmd(src, data);

                    case "night":
                        return new NightCmd(src, data);

                    case "noon":
                        return new NoonCmd(src, data);

                    case "open":
                        return new NotImplementedCmd(src, data);

                    case "option":
                        return new OptionCmd(src, data);

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
                        return new SepiaCmd(src, data);

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

                    case "setstatusstringcolor":
                        return new NotImplementedCmd(src, data);

                    case "setstock":
                        return new NotImplementedCmd(src, data);

                    case "setwindowcolor":
                        return new NotImplementedCmd(src, data);

                    case "setwindowframewidth":
                        return new NotImplementedCmd(src, data);

                    case "show":
                        return new NotImplementedCmd(src, data);

                    case "showimage":
                        return new NotImplementedCmd(src, data);

                    case "showunitstatus":
                        return new NotImplementedCmd(src, data);

                    case "skip":
                        return new SkipCmd(src, data);

                    case "sort":
                        return new SortCmd(src, data);

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
                        return new SunsetCmd(src, data);

                    case "swap":
                        return new SwapCmd(src, data);

                    case "switch":
                        return new SwitchCmd(src, data);

                    case "playflash":
                        return new PlayFlashCmd(src, data);

                    case "clearflash":
                        return new ClearFlashCmd(src, data);

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
                        return new UnSetCmd(src, data);

                    case "upgrade":
                        return new NotImplementedCmd(src, data);

                    case "upvar":
                        return new UpVarCmd(src, data);

                    case "useability":
                        return new NotImplementedCmd(src, data);

                    case "wait":
                        return new WaitCmd(src, data);

                    case "water":
                        return new WaterCmd(src, data);

                    case "whitein":
                        return new WhiteInCmd(src, data);

                    case "whiteout":
                        return new WhiteOutCmd(src, data);

                    case "write":
                        return new NotImplementedCmd(src, data);

                    default:
                        {
                            // 定義済みのイベントコマンドではない
                            if (list.Length >= 3)
                            {
                                if (list[1] == "=")
                                {
                                    // 代入式
                                    // GetValueAsStringの呼び出しの際に、Argsの内容は必ず項と仮定
                                    // されているので、わざと項にしておく
                                    return new SetCmd(src, new EventDataLine(
                                        data.ID, data.Source, data.File, data.LineNum,
                                        "Set " + list[0] + " " + "(" + string.Join(" ", list.Skip(2)) + ")"
                                    ));
                                }
                            }

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
