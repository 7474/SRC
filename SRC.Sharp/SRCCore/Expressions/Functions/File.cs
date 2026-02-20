using SRCCore.Filesystem;
using SRCCore.Lib;
using SRCCore.VB;
using System.Collections.Generic;
using System.IO;

namespace SRCCore.Expressions.Functions
{
    //ファイル処理関数
    //Dirファイルの存在判定
    //EOFファイルの末尾か判定
    //LoadFileDialog読み出し用ファイルの選択ダイアログを表示
    //SaveFileDialog書き込み用ファイルの選択ダイアログを表示

    public class Dir : AFunction
    {
        private IEnumerator<string> _lastResults;

        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            if (pcount > 0)
            {
                var fname = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                // フルパス指定でなければシナリオフォルダを起点に検索
                if (!SRC.FileSystem.IsAbsolutePath(fname))
                {
                    fname = SRC.FileSystem.PathCombine(SRC.ScenarioPath, fname);
                }

                var searchOption = EntryOption.All;
                if (pcount >= 2)
                {
                    switch (SRC.Expression.GetValueAsString(@params[2], is_term[2]) ?? "")
                    {
                        case "ファイル":
                            searchOption = EntryOption.File;
                            break;

                        case "フォルダ":
                            searchOption = EntryOption.Directory;
                            break;
                    }
                }

                _lastResults = SRC.FileSystem.GetFileSystemEntries(
                    Path.GetDirectoryName(fname), Path.GetFileName(fname), searchOption)
                    .GetEnumerator();
            }

            if (_lastResults != null && _lastResults.MoveNext())
            {
                str_result = _lastResults.Current;
            }
            return ValueType.StringType;
        }
    }

    public class EOF : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            var f = SRC.FileHandleManager.Get(SRC.Expression.GetValueAsLong(@params[1], is_term[1]));

            if (f.Reader.EndOfStream)
            {
                str_result = "1";
                num_result = 1d;
            }
            else
            {
                str_result = "0";
                num_result = 0d;
            }

            if (etype == ValueType.StringType)
            {
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }


    public class Loadfiledialog : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            if (pcount >= 2)
            {
                var ftype = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                var fsuffix = SRC.Expression.GetValueAsString(@params[2], is_term[2]);
                var initialFile = pcount >= 3 ? SRC.Expression.GetValueAsString(@params[3], is_term[3]) : "";
                var subdir = pcount >= 4 ? SRC.Expression.GetValueAsString(@params[4], is_term[4]) : "";

                var initialDir = string.IsNullOrEmpty(subdir)
                    ? SRC.ScenarioPath
                    : SRC.FileSystem.ToAbsolutePath(SRC.ScenarioPath, subdir);

                var selectedPath = SRC.GUI.SelectLoadFile("ファイルを開く", initialDir, ftype, fsuffix);
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    var relPath = SRC.FileSystem.ToRelativePath(SRC.ScenarioPath, selectedPath);
                    // ファイルがシナリオフォルダ外の場合は空文字列を返す
                    if (!SRC.FileSystem.IsAbsolutePath(relPath))
                    {
                        str_result = relPath;
                    }
                }
            }

            if (etype == ValueType.StringType)
            {
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }

    public class Savefiledialog : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            if (pcount >= 2)
            {
                var ftype = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                var fsuffix = SRC.Expression.GetValueAsString(@params[2], is_term[2]);
                var initialFile = pcount >= 3 ? SRC.Expression.GetValueAsString(@params[3], is_term[3]) : "";
                var subdir = pcount >= 4 ? SRC.Expression.GetValueAsString(@params[4], is_term[4]) : "";

                var initialDir = string.IsNullOrEmpty(subdir)
                    ? SRC.ScenarioPath
                    : SRC.FileSystem.ToAbsolutePath(SRC.ScenarioPath, subdir);

                var selectedPath = SRC.GUI.SelectSaveFile("ファイルを保存", initialDir, initialFile, ftype, fsuffix);
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    var relPath = SRC.FileSystem.ToRelativePath(SRC.ScenarioPath, selectedPath);
                    // ファイルがシナリオフォルダ外の場合は空文字列を返す
                    if (!SRC.FileSystem.IsAbsolutePath(relPath))
                    {
                        str_result = relPath;
                    }
                }
            }

            if (etype == ValueType.StringType)
            {
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }
}
