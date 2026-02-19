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
                        return new AttackCmd(src, data);

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
                        return new CallIntermissionCommandCmd(src, data);

                    case "cancel":
                        return new CancelCmd(src, data);

                    case "center":
                        return new CenterCmd(src, data);

                    case "changearea":
                        return new ChangeAreaCmd(src, data);

                    case "changelayer":
                        return new ChangeLayerCmd(src, data);

                    case "changemap":
                        return new ChangeMapCmd(src, data);

                    case "changemode":
                        return new ChangeModeCmd(src, data);

                    case "changeparty":
                        return new ChangePartyCmd(src, data);

                    case "changeterrain":
                        return new ChangeTerrainCmd(src, data);

                    case "changeunitbitmap":
                        return new ChangeUnitBitmapCmd(src, data);

                    case "charge":
                        return new ChargeCmd(src, data);

                    case "circle":
                        return new CircleCmd(src, data);

                    case "clearevent":
                        return new ClearEventCmd(src, data);

                    case "clearimage":
                        return new NotSupportedCmd(src, data);

                    case "clearlayer":
                        return new ClearLayerCmd(src, data);

                    case "clearobj":
                        return new ClearObjCmd(src, data);

                    case "clearpicture":
                        return new ClearPictureCmd(src, data);

                    case "clearskill":
                        return new ClearSkillCmd(src, data);
                    case "clearability":
                        return new ClearSkillCmd(src, data);

                    case "clearspecialpower":
                        return new ClearSpecialPowerCmd(src, data);
                    case "clearmind":
                        return new ClearSpecialPowerCmd(src, data);

                    case "clearstatus":
                        return new ClearStatusCmd(src, data);

                    case "cls":
                        return new ClsCmd(src, data);

                    case "close":
                        return new CloseCmd(src, data);

                    case "color":
                        return new ColorCmd(src, data);

                    case "colorfilter":
                        return new ColorFilterCmd(src, data);

                    case "combine":
                        return new CombineCmd(src, data);

                    case "confirm":
                        return new ConfirmCmd(src, data);

                    case "continue":
                        return new ContinueCmd(src, data);

                    case "copyarray":
                        return new CopyArrayCmd(src, data);

                    case "copyfile":
                        return new CopyFileCmd(src, data);

                    case "create":
                        return new CreateCmd(src, data);

                    case "createfolder":
                        return new CreateFolderCmd(src, data);

                    case "debug":
                        return new DebugCmd(src, data);

                    case "destroy":
                        return new DestroyCmd(src, data);

                    case "disable":
                        return new DisableCmd(src, data);

                    case "do":
                        return new DoCmd(src, data);

                    case "loop":
                        return new LoopCmd(src, data);

                    case "drawoption":
                        return new DrawOptionCmd(src, data);

                    case "drawwidth":
                        return new DrawWidthCmd(src, data);

                    case "enable":
                        return new EnableCmd(src, data);

                    case "equip":
                        return new EquipCmd(src, data);

                    case "escape":
                        return new EscapeCmd(src, data);

                    case "exchangeitem":
                        return new ExchangeItemCmd(src, data);

                    case "exec":
                        return new ExecCmd(src, data);

                    case "exit":
                        return new ExitCmd(src, data);

                    case "explode":
                        return new ExplodeCmd(src, data);

                    case "expup":
                        return new ExpUpCmd(src, data);

                    case "fadein":
                        return new FadeInCmd(src, data);

                    case "fadeout":
                        return new FadeOutCmd(src, data);

                    case "fillcolor":
                        return new FillColorCmd(src, data);

                    case "fillstyle":
                        return new FillStyleCmd(src, data);

                    case "finish":
                        return new FinishCmd(src, data);

                    case "fix":
                        return new FixCmd(src, data);

                    case "for":
                        return new ForCmd(src, data);

                    case "foreach":
                        return new ForEachCmd(src, data);

                    case "next":
                        return new NextCmd(src, data);

                    case "font":
                        return new FontCmd(src, data);

                    case "forget":
                        return new ForgetCmd(src, data);

                    case "gameclear":
                        return new GameClearCmd(src, data);

                    case "gameover":
                        return new GameOverCmd(src, data);

                    case "freememory":
                        return new FreeMemoryCmd(src, data);

                    case "getoff":
                        return new GetOffCmd(src, data);

                    case "global":
                        return new GlobalCmd(src, data);

                    case "goto":
                        return new GotoCmd(src, data);

                    case "hide":
                        return new HideCmd(src, data);

                    case "hotpoint":
                        return new HotPointCmd(src, data);

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
                        return new ItemCmd(src, data);

                    case "join":
                        return new JoinCmd(src, data);

                    case "keepbgm":
                        return new KeepBGMCmd(src, data);

                    case "land":
                        return new LandCmd(src, data);

                    case "launch":
                        return new LaunchCmd(src, data);

                    case "leave":
                        return new LeaveCmd(src, data);

                    case "levelup":
                        return new LevelUpCmd(src, data);

                    case "line":
                        return new LineCmd(src, data);

                    case "lineread":
                        return new LineReadCmd(src, data);

                    case "load":
                        return new LoadCmd(src, data);

                    case "local":
                        return new LocalCmd(src, data);

                    case "makepilotlist":
                        return new MakePilotListCmd(src, data);

                    case "makeunitlist":
                        return new MakeUnitListCmd(src, data);

                    case "mapability":
                        return new MapAbilityCmd(src, data);

                    case "mapattack":
                        return new MapAttackCmd(src, data);
                    case "mapweapon":
                        return new MapAttackCmd(src, data);

                    case "money":
                        return new MoneyCmd(src, data);

                    case "monotone":
                        return new MonotoneCmd(src, data);

                    case "move":
                        return new MoveCmd(src, data);

                    case "night":
                        return new NightCmd(src, data);

                    case "noon":
                        return new NoonCmd(src, data);

                    case "open":
                        return new OpenCmd(src, data);

                    case "option":
                        return new OptionCmd(src, data);

                    case "organize":
                        return new OrganizeCmd(src, data);

                    case "oval":
                        return new OvalCmd(src, data);

                    case "paintpicture":
                        return new PaintPictureCmd(src, data);

                    case "paintstring":
                        return new PaintStringCmd(src, data);

                    case "paintsysstring":
                        return new PaintSysStringCmd(src, data);

                    case "pilot":
                        return new PilotCmd(src, data);

                    case "playmidi":
                        return new PlayMIDICmd(src, data);

                    case "playsound":
                        return new PlaySoundCmd(src, data);

                    case "polygon":
                        return new PolygonCmd(src, data);

                    case "print":
                        return new PrintCmd(src, data);

                    case "pset":
                        return new PSetCmd(src, data);

                    case "question":
                        return new QuestionCmd(src, data);

                    case "quickload":
                        return new QuickLoadCmd(src, data);

                    case "quit":
                        return new QuitCmd(src, data);

                    case "rankup":
                        return new RankUpCmd(src, data);

                    case "read":
                        return new NotSupportedCmd(src, data);

                    case "recoveren":
                        return new RecoverENCmd(src, data);

                    case "recoverhp":
                        return new RecoverHPCmd(src, data);

                    case "recoverplana":
                        return new RecoverPlanaCmd(src, data);

                    case "recoversp":
                        return new RecoverSPCmd(src, data);

                    case "redraw":
                        return new RedrawCmd(src, data);

                    case "refresh":
                        return new RefreshCmd(src, data);

                    case "release":
                        return new ReleaseCmd(src, data);

                    case "removefile":
                        return new RemoveFileCmd(src, data);

                    case "removefolder":
                        return new RemoveFolderCmd(src, data);

                    case "removeitem":
                        return new RemoveItemCmd(src, data);

                    case "removepilot":
                        return new RemovePilotCmd(src, data);

                    case "removeunit":
                        return new RemoveUnitCmd(src, data);

                    case "renamebgm":
                        return new RenameBGMCmd(src, data);

                    case "renamefile":
                        return new RenameFileCmd(src, data);

                    case "renameterm":
                        return new RenameTermCmd(src, data);

                    case "replacepilot":
                        return new ReplacePilotCmd(src, data);

                    case "require":
                        return new RequireCmd(src, data);

                    case "restoreevent":
                        return new RestoreEventCmd(src, data);

                    case "ride":
                        return new RideCmd(src, data);

                    case "select":
                        return new SelectCmd(src, data);

                    case "savedata":
                        return new SaveDataCmd(src, data);

                    case "selecttarget":
                        return new SelectTargetCmd(src, data);

                    case "sepia":
                        return new SepiaCmd(src, data);

                    case "set":
                        return new SetCmd(src, data);

                    case "setbullet":
                        return new SetBulletCmd(src, data);

                    case "setmessage":
                        return new SetMessageCmd(src, data);

                    case "setrelation":
                        return new SetRelationCmd(src, data);

                    case "setskill":
                        return new SetSkillCmd(src, data);
                    case "setability":
                        return new SetSkillCmd(src, data);

                    case "setstatus":
                        return new SetStatusCmd(src, data);

                    case "setstatusstringcolor":
                        return new SetStatusStringColorCmd(src, data);

                    case "setstock":
                        return new SetStockCmd(src, data);

                    case "setwindowcolor":
                        return new SetWindowColorCmd(src, data);

                    case "setwindowframewidth":
                        return new SetWindowFrameWidthCmd(src, data);

                    case "show":
                        return new ShowCmd(src, data);

                    case "showimage":
                        return new NotSupportedCmd(src, data);

                    case "showunitstatus":
                        return new ShowUnitStatusCmd(src, data);

                    case "skip":
                        return new SkipCmd(src, data);

                    case "sort":
                        return new SortCmd(src, data);

                    case "specialpower":
                        return new SpecialPowerCmd(src, data);
                    case "mind":
                        return new SpecialPowerCmd(src, data);

                    case "split":
                        return new SplitCmd(src, data);

                    case "startbgm":
                        return new StartBGMCmd(src, data);

                    case "stopbgm":
                        return new StopBGMCmd(src, data);

                    case "stopsummoning":
                        return new StopSummoningCmd(src, data);

                    case "supply":
                        return new SupplyCmd(src, data);

                    case "sunset":
                        return new SunsetCmd(src, data);

                    case "swap":
                        return new SwapCmd(src, data);

                    case "switch":
                        return new SwitchCmd(src, data);

                    case "playflash":
                        return new NotSupportedCmd(src, data);

                    case "clearflash":
                        return new NotSupportedCmd(src, data);

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
                        return new TelopCmd(src, data);

                    case "transform":
                        return new TransformCmd(src, data);

                    case "unit":
                        return new UnitCmd(src, data);

                    case "unset":
                        return new UnSetCmd(src, data);

                    case "upgrade":
                        return new UpgradeCmd(src, data);

                    case "upvar":
                        return new UpVarCmd(src, data);

                    case "useability":
                        return new UseAbilityCmd(src, data);

                    case "wait":
                        return new WaitCmd(src, data);

                    case "water":
                        return new WaterCmd(src, data);

                    case "whitein":
                        return new WhiteInCmd(src, data);

                    case "whiteout":
                        return new WhiteOutCmd(src, data);

                    case "write":
                        return new NotSupportedCmd(src, data);

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
                src.Log.LogError(ex.Message, ex);
                src.Event.DisplayEventErrorMessage(data.LineNum, "イベントコマンドの内容が不正です");
                return new NopCmd(src, data);
            }
        }
    }
}
