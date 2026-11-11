using SRCCore.Events;
using SRCCore.Lib;
using SRCCore.VB;
using System.IO;

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
            // 最後にセーブしたデータを読む（VB: RestoreData LastSaveDataFileName, True）
            if (SRC.IsLastSaveDataQuikSaveStream)
            {
                // 最後のセーブがクイックセーブなら、クイックセーブのデータをロード（#1530）
                Stream stream = null;
                try
                {
                    stream = GUI.OpenQuikSaveStream(FileAccess.Read);
                }
                catch (IOException)
                {
                }
                if (stream != null)
                {
                    using (stream)
                    {
                        SRC.RestoreData(stream, SRCSaveKind.Quik);
                    }
                }
                else
                {
                    // セーブファイルが見つからなければ強制終了
                    GUI.ErrorMessage("セーブデータが見つかりません");
                    SRC.TerminateSRC();
                }
            }
            else if (SRC.FileSystem.FileExists(SRC.LastSaveDataFileName))
            {
                // セーブしたファイルが存在すればそれをロード
                // 元の SRC は常にクイックロードとして読み、ファイル名でリスタート・クイックセーブのフラグを立てる（#1530）
                var fname = SRC.LastSaveDataFileName.Replace('/', '\\');
                if (Strings.InStr(fname, @"\_リスタート.src") > 0)
                {
                    SRC.RestoreData(SRC.LastSaveDataFileName, SRCSaveKind.Restart);
                }
                else if (Strings.InStr(fname, @"\_クイックセーブ.src") > 0)
                {
                    SRC.RestoreData(SRC.LastSaveDataFileName, SRCSaveKind.Quik);
                }
                else
                {
                    // 中断データなど。クイックロードとして読むが、どちらのフラグも立てない
                    var isQuickSaveDataAvailable = SRC.IsQuickSaveDataAvailable;
                    SRC.RestoreData(SRC.LastSaveDataFileName, SRCSaveKind.Quik);
                    SRC.IsQuickSaveDataAvailable = isQuickSaveDataAvailable;
                }
            }
            else
            {
                // セーブファイルが見つからなければ強制終了
                GUI.ErrorMessage("セーブデータが見つかりません");
                SRC.TerminateSRC();
            }

            // 詰まないように乱数系列をリセット
            GeneralLib.RndSeed = GeneralLib.RndSeed + 1;
            GeneralLib.RndReset();

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
