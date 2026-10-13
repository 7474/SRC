using System.Threading.Tasks;

namespace SRCRender
{
    /// <summary>
    /// 文字入力・ファイル選択を UI フレームワークの部品で行う場合に IScreenHost と併せて実装する。
    /// いずれのメソッドもゲームスレッドから呼ばれる。ScreenGUI は完了するまで UI からの処理を続けながら待つ。
    /// 実装しない場合、文字入力は既定値で確定し、ファイルはシナリオフォルダの既定の名前を使う。
    /// </summary>
    public interface IScreenDialogHost
    {
        /// <summary>文字を入力させる。キャンセル時は null。</summary>
        Task<string> InputTextAsync(string title, string message, string defaultValue);

        /// <summary>開くファイルを選ばせる。キャンセル時は null。</summary>
        Task<string> OpenFileAsync(FileDialogRequest request);

        /// <summary>保存するファイルを選ばせる。キャンセル時は null。</summary>
        Task<string> SaveFileAsync(FileDialogRequest request);
    }

    public class FileDialogRequest
    {
        public string Title { get; set; }
        public string InitialDirectory { get; set; }
        public string InitialFile { get; set; }
        /// <summary>種類の表示名（例: セーブデータ）</summary>
        public string FileType { get; set; }
        /// <summary>拡張子（ドットなし）</summary>
        public string FileExtension { get; set; }
    }
}
