using System.Collections.Generic;

namespace SRCCore.Models
{
    /// <summary>
    /// データの定義のあるファイルと行（行番号は 1 始まり）。
    /// SRC# の作成支援ツール（lint の規則）が診断の場所を示すのに使う。元の SRC には無く、ゲームの動作とセーブデータには使わない。
    /// </summary>
    public class DataLocation
    {
        /// <summary>読み込んだときのファイル名（読み込みに渡した名前のまま）</summary>
        public string FileName { get; }
        /// <summary>データの名前の行</summary>
        public int Line { get; }

        private readonly Dictionary<string, int> fieldLines = new Dictionary<string, int>();

        public DataLocation(string fileName, int line)
        {
            FileName = fileName ?? "";
            Line = line;
        }

        /// <summary>欄（「移動可能地形」など）の行を記録する</summary>
        public void SetFieldLine(string field, int line)
        {
            fieldLines[field] = line;
        }

        /// <summary>欄の行。記録が無ければデータの名前の行</summary>
        public int FieldLine(string field)
        {
            return fieldLines.TryGetValue(field, out var line) ? line : Line;
        }
    }
}
