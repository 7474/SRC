using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class LoadCmd : CmdData
    {
        public LoadCmd(SRC src, EventDataLine eventData) : base(src, CmdType.LoadCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            string[] new_titles;
            //            string tname, tfolder;
            //            int i;
            //            short j;
            //            int cur_data_size;
            //            bool flag;
            //            new_titles = new string[1];
            //            var loopTo = ArgNum;
            //            for (i = 2; i <= loopTo; i++)
            //            {
            //                tname = GetArgAsString(i);
            //                flag = false;
            //                var loopTo1 = Information.UBound(SRC.Titles);
            //                for (j = 1; j <= loopTo1; j++)
            //                {
            //                    if ((tname ?? "") == (SRC.Titles[j] ?? ""))
            //                    {
            //                        flag = true;
            //                        break;
            //                    }
            //                }

            //                if (!flag)
            //                {
            //                    Array.Resize(new_titles, Information.UBound(new_titles) + 1 + 1);
            //                    Array.Resize(SRC.Titles, Information.UBound(SRC.Titles) + 1 + 1);
            //                    new_titles[Information.UBound(new_titles)] = tname;
            //                    SRC.Titles[Information.UBound(SRC.Titles)] = tname;
            //                }
            //            }

            //            // 新規のデータがなかった？
            //            if (Information.UBound(new_titles) == 0)
            //            {
            //                ExecLoadCmdRet = LineNum + 1;
            //                return ExecLoadCmdRet;
            //            }

            //            // マウスカーソルを砂時計に
            //            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            //            GUI.ChangeStatus(GuiStatus.WaitCursor);
            //            cur_data_size = Information.UBound(Event.EventData);

            //            // 使用しているタイトルのデータをロード
            //            var loopTo2 = Information.UBound(new_titles);
            //            for (i = 1; i <= loopTo2; i++)
            //            {
            //                SRC.IncludeData(new_titles[i]);
            //                tfolder = SRC.SearchDataFolder(new_titles[i]);
            //                if (SRC.FileSystem.FileExists(tfolder + @"\include.eve"))
            //                {
            //                    Event.LoadEventData2(tfolder + @"\include.eve", Information.UBound(Event.EventData));
            //                }
            //            }

            //            // ローカルデータの読みこみ
            //            if (SRC.FileSystem.FileExists(SRC.ScenarioPath + @"Data\alias.txt"))
            //            {
            //                SRC.ALDList.Load(SRC.ScenarioPath + @"Data\alias.txt");
            //            }

            //            bool localFileExists() { string argfname = SRC.ScenarioPath + @"Data\mind.txt"; var ret = SRC.FileSystem.FileExists(argfname); return ret; }

            //            if (SRC.FileSystem.FileExists(SRC.ScenarioPath + @"Data\sp.txt"))
            //            {
            //                SRC.SPDList.Load(SRC.ScenarioPath + @"Data\sp.txt");
            //            }
            //            else if (localFileExists())
            //            {
            //                SRC.SPDList.Load(SRC.ScenarioPath + @"Data\mind.txt");
            //            }

            //            if (SRC.FileSystem.FileExists(SRC.ScenarioPath + @"Data\pilot.txt"))
            //            {
            //                SRC.PDList.Load(SRC.ScenarioPath + @"Data\pilot.txt");
            //            }

            //            if (SRC.FileSystem.FileExists(SRC.ScenarioPath + @"Data\non_pilot.txt"))
            //            {
            //                SRC.NPDList.Load(SRC.ScenarioPath + @"Data\non_pilot.txt");
            //            }

            //            if (SRC.FileSystem.FileExists(SRC.ScenarioPath + @"Data\robot.txt"))
            //            {
            //                SRC.UDList.Load(SRC.ScenarioPath + @"Data\robot.txt");
            //            }

            //            if (SRC.FileSystem.FileExists(SRC.ScenarioPath + @"Data\unit.txt"))
            //            {
            //                SRC.UDList.Load(SRC.ScenarioPath + @"Data\unit.txt");
            //            }

            //            if (SRC.FileSystem.FileExists(SRC.ScenarioPath + @"Data\pilot_message.txt"))
            //            {
            //                SRC.MDList.Load(SRC.ScenarioPath + @"Data\pilot_message.txt");
            //            }

            //            if (SRC.FileSystem.FileExists(SRC.ScenarioPath + @"Data\pilot_dialog.txt"))
            //            {
            //                SRC.DDList.Load(SRC.ScenarioPath + @"Data\pilot_dialog.txt");
            //            }

            //            if (SRC.FileSystem.FileExists(SRC.ScenarioPath + @"Data\item.txt"))
            //            {
            //                SRC.IDList.Load(SRC.ScenarioPath + @"Data\item.txt");
            //            }

            //            var loopTo3 = Information.UBound(Event.EventData);
            //            for (i = cur_data_size + 1; i <= loopTo3; i++)
            //            {
            //                // 複数行に分割されたコマンドを結合
            //                if (Strings.Right(Event.EventData[i], 1) == "_")
            //                {
            //                    if (Information.UBound(Event.EventData) > i)
            //                    {
            //                        Event.EventData[i + 1] = Strings.Left(Event.EventData[i], Strings.Len(Event.EventData[i]) - 1) + Event.EventData[i + 1];
            //                        Event.EventData[i] = " ";
            //                    }
            //                }
            //            }

            //            // ラベルの登録
            //            var loopTo4 = Information.UBound(Event.EventData);
            //            for (i = cur_data_size + 1; i <= loopTo4; i++)
            //            {
            //                if (Strings.Right(Event.EventData[i], 1) == ":")
            //                {
            //                    Event.AddLabel(Strings.Left(Event.EventData[i], Strings.Len(Event.EventData[i]) - 1), i);
            //                }
            //            }

            //            // コマンドデータ配列を増やす
            //            if (Information.UBound(Event.EventData) > Information.UBound(Event.EventCmd))
            //            {
            //                Array.Resize(Event.EventCmd, Information.UBound(Event.EventData) + 1);
            //            }

            //            // イベントデータの構文解析
            //            var loopTo5 = Information.UBound(Event.EventData);
            //            for (i = cur_data_size + 1; i <= loopTo5; i++)
            //            {
            //                if (Event.EventCmd[i] is null)
            //                {
            //                    Event.EventCmd[i] = new CmdData();
            //                }

            //                {
            //                    var withBlock = Event.EventCmd[i];
            //                    withBlock.LineNum = i;
            //                    withBlock.Parse(Event.EventData[i]);
            //                }
            //            }

            //            // マウスカーソルを元に戻す
            //            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            //            GUI.ChangeStatus(GuiStatus.Default);
            //return EventData.NextID;
        }
    }
}
