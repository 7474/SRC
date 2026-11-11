using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCSharpWeb
{
    /// <summary>ファイルを開くダイアログ（ファイルの一覧から選ぶモーダル）で使う処理（WebScreenHost.OpenFileAsync）</summary>
    public static class FileChoice
    {
        /// <summary>
        /// 一覧で初めに選んでおく名前。初めに選ぶファイル（LoadFileDialog 関数の第 3 引数）と同じ名前のファイルが一覧にあればその名前。
        /// フォルダを含む指定はファイル名の部分で比べ、大文字・小文字は区別しない（Windows のファイル名と同じ）。無ければ null
        /// </summary>
        public static string Initial(IEnumerable<string> names, string initialFile)
        {
            if (string.IsNullOrEmpty(initialFile))
            {
                return null;
            }
            var normalized = initialFile.Replace('\\', '/');
            var name = normalized.Substring(normalized.LastIndexOf('/') + 1);
            if (name.Length == 0)
            {
                return null;
            }
            return names.FirstOrDefault(x => string.Equals(x, name, StringComparison.Ordinal))
                ?? names.FirstOrDefault(x => string.Equals(x, name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
