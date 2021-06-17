using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class QuickLoadCmd : CmdData
    {
        public QuickLoadCmd(SRC src, EventDataLine eventData) : base(src, CmdType.QuickLoadCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            GUI.LockGUI();
            //            Status.ClearUnitStatus();
            //            Sound.StopBGM();
            //            if (SRC.FileSystem.FileExists(SRC.LastSaveDataFileName))
            //            {
            //                // セーブしたファイルが存在すればそれをロード
            //                SRC.RestoreData(SRC.LastSaveDataFileName, true);
            //            }
            //            else
            //            {
            //                // セーブファイルが見つからなければ強制終了
            //                GUI.ErrorMessage("セーブデータが見つかりません");
            //                SRC.TerminateSRC();
            //            }

            //            // 詰まないように乱数系列をリセット
            //            GeneralLib.RndSeed = GeneralLib.RndSeed + 1;
            //            GeneralLib.RndReset();

            //            // 再開イベントによるマップ画像の書き換え処理を行う
            //            Event.HandleEvent("再開");
            //            Map.IsMapDirty = false;

            //            // 画面を書き直してステータスを表示
            //            GUI.RedrawScreen();
            //            Status.DisplayGlobalStatus();
            //            GUI.MainForm.Show();

            //            // 操作可能にする
            //            Commands.CommandState = "ユニット選択";
            //            GUI.UnlockGUI();
            //            SRC.IsScenarioFinished = true;
            //            ExecQuickLoadCmdRet = 0;
            //return EventData.NextID;
        }
    }
}
