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
                    {
                        // シナリオ → ExtDataPath → ExtDataPath2 → SRC のフォルダの順に探す（VB: CmdData.cls）
                        var eveFile = SRC.InterMission.SearchLibFile("パイロットステータス表示.eve");
                        if (string.IsNullOrEmpty(eveFile))
                        {
                            // 元の SRC は AppPath の Lib を必ず読む（無ければ読み込みのエラーで終了する）。
                            // インターミッションのコマンド（Intermission.InterMissionCommand）と同じく、知らせて次の行へ進む
                            GUI.ErrorMessage(@"Lib\パイロットステータス表示.eve が見つかりません");
                            return EventData.NextID;
                        }

                        GUI.CloseListBox();
                        GUI.ReduceListBoxHeight();
                        SRC.IsSubStage = true;
                        SRC.StartScenario(eveFile);
                    }
                    // サブステージを通常のステージとして実行
                    SRC.IsSubStage = true;
                    SRC.IsScenarioFinished = true;
                    return -1;

                case "ユニットステータス":
                    {
                        // シナリオ → ExtDataPath → ExtDataPath2 → SRC のフォルダの順に探す（VB: CmdData.cls）
                        var eveFile = SRC.InterMission.SearchLibFile("ユニットステータス表示.eve");
                        if (string.IsNullOrEmpty(eveFile))
                        {
                            // 元の SRC は AppPath の Lib を必ず読む（無ければ読み込みのエラーで終了する）。
                            // インターミッションのコマンド（Intermission.InterMissionCommand）と同じく、知らせて次の行へ進む
                            GUI.ErrorMessage(@"Lib\ユニットステータス表示.eve が見つかりません");
                            return EventData.NextID;
                        }

                        GUI.CloseListBox();
                        GUI.ReduceListBoxHeight();
                        SRC.IsSubStage = true;
                        SRC.StartScenario(eveFile);
                    }
                    // サブステージを通常のステージとして実行
                    SRC.IsSubStage = true;
                    SRC.IsScenarioFinished = true;
                    return -1;
            }

            return EventData.NextID;
        }
    }
}

