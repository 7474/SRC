using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SRCSharpAvalonia
{
    /// <summary>
    /// 元の SRC の設定ファイル（Src.ini）を読む。Shift_JIS（UTF-8 として正しく読めれば UTF-8）の INI ファイル。
    /// </summary>
    internal static class SrcIni
    {
        /// <summary>section の項目（名前 → 値）。名前の大文字・小文字は区別しない。無ければ空</summary>
        public static IReadOnlyDictionary<string, string> ReadSection(string path, string section)
        {
            var items = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var bytes = File.ReadAllBytes(path);
            string text;
            try
            {
                text = new UTF8Encoding(false, true).GetString(bytes);
            }
            catch (DecoderFallbackException)
            {
                text = SRCCore.Filesystem.ZipNameEncoding.ShiftJis.GetString(bytes);
            }
            string current = null;
            foreach (var raw in text.Split('\n'))
            {
                var line = raw.Trim().TrimStart('﻿');
                if (line.Length == 0 || line.StartsWith(";"))
                {
                    continue;
                }
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    current = line.Substring(1, line.Length - 2).Trim();
                    continue;
                }
                var eq = line.IndexOf('=');
                if (eq > 0 && string.Equals(current, section, StringComparison.OrdinalIgnoreCase))
                {
                    items[line.Substring(0, eq).Trim()] = line.Substring(eq + 1).Trim();
                }
            }
            return items;
        }
    }
}
