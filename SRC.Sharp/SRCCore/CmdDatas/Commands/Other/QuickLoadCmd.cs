using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    public class QuickLoadCmd : CmdData
    {
        public QuickLoadCmd(SRC src, EventDataLine eventData) : base(src, CmdType.QuickLoadCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            GUI.LockGUI();
            SRC.GUIStatus.ClearUnitStatus();
            Sound.StopBGM();
            if (SRC.FileSystem.FileExists(SRC.LastSaveDataFileName))
            {
                // セーブしたファイルが存在すればそれをロード
                SRC.RestoreData(SRC.LastSaveDataFileName, SRCSaveKind.Quik);
            }
            else
            {
                // セーブファイルが見つからなければ強制終了
                GUI.ErrorMessage("セーブデータが見つかりません");
                SRC.TerminateSRC();
            }

            // TODO 詰まないように乱数系列をリセット
            //// 詰まないように乱数系列をリセット
            //GeneralLib.RndSeed = GeneralLib.RndSeed + 1;
            //GeneralLib.RndReset();

            // 再開イベントによるマップ画像の書き換え処理を行う
            Event.HandleEvent("再開");
            Map.IsMapDirty = false;

            // 画面を書き直してステータスを表示
            GUI.RedrawScreen();
            SRC.GUIStatus.DisplayGlobalStatus();
            GUI.MainFormShow();

            // 操作可能にする
            Commands.CommandState = "ユニット選択";
            GUI.UnlockGUI();
            SRC.IsScenarioFinished = true;
            return -1;
        }
    }
}
