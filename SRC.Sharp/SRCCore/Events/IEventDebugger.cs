using SRCCore.CmdDatas;

namespace SRCCore.Events
{
    /// <summary>
    /// イベントのデバッガ（作成支援ツールのデバッガなど）。<see cref="Event.Debugger"/> に設定すると、
    /// イベントコマンドの実行前とエラーの表示時に呼ばれる。ゲームスレッドで呼ばれ、戻るまで実行は止まる。
    /// </summary>
    public interface IEventDebugger
    {
        /// <summary>コマンドを実行する直前</summary>
        void BeforeExecute(CmdData command);

        /// <summary>イベントのエラーを表示する時</summary>
        void OnError(EventDataLine line, string message);
    }
}
