namespace SRCCore.Logging
{
    /// <summary>
    /// ログのカテゴリ名。設定ファイルの Log/Level:&lt;カテゴリ&gt; でカテゴリごとのレベルを指定できる。
    /// </summary>
    public static class LogCategories
    {
        /// <summary>起動・データの読み込み・セーブ・ロード</summary>
        public const string Core = "SRCCore";
        /// <summary>イベントの実行・コマンド</summary>
        public const string Event = "SRCCore.Event";
        /// <summary>式・変数・関数</summary>
        public const string Expression = "SRCCore.Expression";
        /// <summary>ユニットへのコマンド・COM</summary>
        public const string Command = "SRCCore.Command";
        /// <summary>画面・画像・フォント</summary>
        public const string Render = "SRCRender";
        /// <summary>音の再生・SoundFont</summary>
        public const string Sound = "SRCSound";

        /// <summary>設定ファイルでカテゴリごとのレベルを指定できるカテゴリ</summary>
        public static readonly string[] All =
        {
            Core,
            Event,
            Expression,
            Command,
            Render,
            Sound,
            "SRCSharpForm",
            "SRCSharpAvalonia",
            "SRCSharpWeb",
            "SRCSharp.Tools",
            "SRCSharp.Lsp",
            "SRCSharp.Dap",
        };
    }
}
