namespace SRCCore
{
    public partial class SRC
    {
        // 設定ファイルを作成する（存在しない場合のデフォルト設定を書き出す）
        public void CreateIniFile()
        {
            // 元の SRC（SRC.bas の CreateIniFile）は Src.ini を既定の内容で書き出す。
            // SRC# の設定は JSON（LocalFileConfig）で、既定値は LocalFileConfig が持つ。
            // 起動時に設定ファイルが無ければ GUI 側（RootForm など）が config.Save() で書き出すので、
            // ここでは既定の設定を保存するだけにする。DirectMusic・TransparentBlt など Windows の部品の項目は SRC# に無い
            SystemConfig.Save();
        }
    }
}
