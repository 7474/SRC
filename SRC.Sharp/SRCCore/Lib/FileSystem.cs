using System;
using System.IO;

namespace SRCCore.Lib
{

    // VBの Microsoft.VisualBasic.FileSystem のうちSRCで使用していたものの仮実装。
    // いい感じにファイルIOをラップできないかなぁ。と思っているけれど分からん。
    public static class FileSystem
    {
        public static string Dir(string PathName, FileAttribute Attributes = FileAttribute.Normal)
        {
            if (Attributes.HasFlag(FileAttribute.Volume))
            {
                throw new NotSupportedException("FileAttribute.Volume");
            }

            if (Attributes.HasFlag(FileAttribute.Directory))
            {
                if (Directory.Exists(PathName)) { return new DirectoryInfo(PathName).ToString(); }
            }
            if (File.Exists(PathName)) { return new FileInfo(PathName).ToString(); }
            return "";
        }
    }

    //
    // 概要:
    //     ファイル アクセス用の関数を呼び出すときに、使用するファイル属性を示します。
    [Flags]
    public enum FileAttribute
    {
        //
        // 概要:
        //     通常 (Dir および SetAttr の既定)。 このファイルには特殊文字は適用されません。 このメンバーは、Visual Basic 定数の vbNormal
        //     に相当します。
        Normal = 0,
        //
        // 概要:
        //     読み取り専用。 このメンバーは、Visual Basic 定数の vbReadOnly に相当します。
        ReadOnly = 1,
        //
        // 概要:
        //     非表示。 このメンバーは、Visual Basic 定数の vbHidden に相当します。
        Hidden = 2,
        //
        // 概要:
        //     システム ファイル。 このメンバーは、Visual Basic 定数の vbSystem に相当します。
        System = 4,
        //
        // 概要:
        //     ボリューム ラベル。 この属性は、SetAttr では使用できません。 このメンバーは、Visual Basic 定数の vbVolume に相当します。
        Volume = 8,
        //
        // 概要:
        //     ディレクトリまたはフォルダー。 このメンバーは、Visual Basic 定数の vbDirectory に相当します。
        Directory = 16,
        //
        // 概要:
        //     ファイルは前回のバックアップ以降に変更されています。 このメンバーは、Visual Basic 定数の vbArchive に相当します。
        Archive = 32
    }
}
