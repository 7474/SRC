using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SRCCore.Config
{
    /// <summary>
    /// 元の SRC の設定ファイル（Src.ini）を読む。Shift_JIS（UTF-8 として正しく読めれば UTF-8）の INI ファイル。
    /// セクション・項目の名前の大文字・小文字は区別しない（元の SRC の GetPrivateProfileString と同じ）。
    /// </summary>
    public static class SrcIni
    {
        public const string FileName = "Src.ini";

        /// <summary>INI ファイルの 1 項目</summary>
        public readonly struct Entry
        {
            public Entry(int line, string section, string name, string value)
            {
                Line = line;
                Section = section;
                Name = name;
                Value = value;
            }

            /// <summary>行の番号（0 から）</summary>
            public int Line { get; }
            public string Section { get; }
            public string Name { get; }
            public string Value { get; }
        }

        /// <summary>バイト列を文字列にする。UTF-8 として正しく読めれば UTF-8、そうでなければ Shift_JIS</summary>
        public static string Decode(byte[] bytes, out Encoding encoding)
        {
            var utf8 = new UTF8Encoding(false, true);
            try
            {
                var text = utf8.GetString(bytes);
                encoding = new UTF8Encoding(false);
                return text;
            }
            catch (DecoderFallbackException)
            {
                encoding = Filesystem.ZipNameEncoding.ShiftJis;
                return encoding.GetString(bytes);
            }
        }

        /// <summary>行に分ける（行末の CR は行に残す）</summary>
        public static string[] SplitLines(string text) => text.Split('\n');

        /// <summary>行から項目を取り出す。セクションの外の項目・コメント（;）は除く</summary>
        public static IEnumerable<Entry> Entries(IReadOnlyList<string> lines)
        {
            string section = null;
            for (var i = 0; i < lines.Count; i++)
            {
                var line = lines[i].Trim().TrimStart('﻿');
                if (line.Length == 0 || line.StartsWith(";"))
                {
                    continue;
                }
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    section = line.Substring(1, line.Length - 2).Trim();
                    continue;
                }
                var eq = line.IndexOf('=');
                if (eq > 0 && section != null)
                {
                    yield return new Entry(i, section, line.Substring(0, eq).Trim(), line.Substring(eq + 1).Trim());
                }
            }
        }

        /// <summary>すべてのセクション（セクション名 → 項目名 → 値）</summary>
        public static Dictionary<string, Dictionary<string, string>> Parse(byte[] bytes)
        {
            var sections = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in Entries(SplitLines(Decode(bytes, out _))))
            {
                if (!sections.TryGetValue(entry.Section, out var items))
                {
                    items = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    sections[entry.Section] = items;
                }
                items[entry.Name] = entry.Value;
            }
            return sections;
        }

        /// <summary>section の項目（名前 → 値）。名前の大文字・小文字は区別しない。無ければ空</summary>
        public static IReadOnlyDictionary<string, string> ReadSection(string path, string section)
        {
            return Parse(File.ReadAllBytes(path)).TryGetValue(section, out var items)
                ? items
                : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
    }
}
