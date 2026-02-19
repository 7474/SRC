using SRCCore.Events;
using SRCCore.Exceptions;
using System.IO;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class CallIntermissionCommandCmd : CmdData
    {
        public CallIntermissionCommandCmd(SRC src, EventDataLine eventData) : base(src, CmdType.CallInterMissionCommandCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 2)
            {
                throw new EventErrorException(this, "CallInterMissionCommandコマンドの引数の数が違います");
            }

            switch (GetArgAsString(2))
            {
                case "データセーブ":
                    using (var saveStream = GUI.SelectSaveStream(SRCSaveKind.Normal))
                    {
                        if (saveStream != null)
                        {
                            SRC.UList.Update();
                            SRC.SaveData(saveStream);
                        }
                    }
                    break;

                case "機体改造":
                case "ユニットの強化":
                    GUI.EnlargeListBoxHeight();
                    SRC.InterMission.RankUpCommand();
                    GUI.ReduceListBoxHeight();
                    break;

                case "乗り換え":
                    GUI.EnlargeListBoxHeight();
                    SRC.InterMission.ExchangeUnitCommand();
                    GUI.ReduceListBoxHeight();
                    break;

                case "アイテム交換":
                    GUI.EnlargeListBoxHeight();
                    SRC.InterMission.ExchangeItemCommand();
                    GUI.ReduceListBoxHeight();
                    break;

                case "換装":
                    GUI.EnlargeListBoxHeight();
                    SRC.InterMission.ExchangeFormCommand();
                    GUI.ReduceListBoxHeight();
                    break;

                case "パイロットステータス":
                    GUI.CloseListBox();
                    GUI.ReduceListBoxHeight();
                    SRC.IsSubStage = true;
                    {
                        var eveFile = new[] { SRC.ScenarioPath, SRC.AppPath }
                            .Select(x => Path.Combine(x, "Lib", "パイロットステータス表示.eve"))
                            .FirstOrDefault(x => SRC.FileSystem.FileExists(x));
                        if (!string.IsNullOrEmpty(eveFile))
                        {
                            SRC.StartScenario(eveFile);
                        }
                    }
                    SRC.IsSubStage = true;
                    SRC.IsScenarioFinished = true;
                    return -1;

                case "ユニットステータス":
                    GUI.CloseListBox();
                    GUI.ReduceListBoxHeight();
                    SRC.IsSubStage = true;
                    {
                        var eveFile = new[] { SRC.ScenarioPath, SRC.AppPath }
                            .Select(x => Path.Combine(x, "Lib", "ユニットステータス表示.eve"))
                            .FirstOrDefault(x => SRC.FileSystem.FileExists(x));
                        if (!string.IsNullOrEmpty(eveFile))
                        {
                            SRC.StartScenario(eveFile);
                        }
                    }
                    SRC.IsSubStage = true;
                    SRC.IsScenarioFinished = true;
                    return -1;
            }

            return EventData.NextID;
        }
    }
}

