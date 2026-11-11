// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SRCCore.Filesystem
{
    /// <summary>
    /// 画像ファイルを探す（元の SRC の GUI.bas の DrawPicture・FindUnitBitmap の検索）。
    /// 元の SRC と同じく、フォルダ（シナリオのフォルダ、ExtDataPath、ExtDataPath2、SRC のフォルダの順）を外側、
    /// ファイル名の候補を内側にして探す。SRC# の素材パック（Packs の zip）なども <see cref="IFileSystem"/> のフォルダとして探す。
    /// </summary>
    public static class BitmapSearch
    {
        /// <summary>
        /// PaintPicture などで描く画像のファイルを探す。見つからなければ null。
        /// 返すパスは <see cref="IFileSystem.Open"/> にそのまま渡せる。
        /// </summary>
        public static string FindPicture(SRC src, string fname)
        {
            if (string.IsNullOrEmpty(fname))
            {
                return null;
            }
            var fs = src.FileSystem;
            if (fs.IsAbsolutePath(fname))
            {
                // フルパスで指定されている
                return fs.FileExists(fname) ? fname : null;
            }
            foreach (var candidates in PictureCandidates(src, fname))
            {
                var found = fs.FindFirstFile(candidates);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        /// <summary>
        /// 画像の候補。外側のリストの順に、各リストの候補をフォルダごとに探す。
        /// </summary>
        internal static IEnumerable<IList<string>> PictureCandidates(SRC src, string fname)
        {
            var fs = src.FileSystem;
            var name = fname.Replace('/', '\\');
            string Bitmap(params string[] paths) => fs.PathCombine(new[] { "Bitmap" }.Concat(paths).ToArray());

            // 戦闘アニメ用？
            if (name.Contains(@"\EFFECT_"))
            {
                yield return new[] { Bitmap("Anime", fname) };
            }

            // パス名入り？
            var hasBitmapPath = name.Contains(@"Bitmap\");
            if (hasBitmapPath)
            {
                yield return new[] { fname };
            }

            // フォルダ指定あり？
            string tdir = "", tname = "";
            if (name.Contains(@"\"))
            {
                yield return new[] { Bitmap(fname) };

                if (name.Length >= 4 && name.Substring(0, 4).ToLowerInvariant() == @"map\")
                {
                    var t = name.Substring(4);
                    if (!t.Contains(@"\"))
                    {
                        var i = NumberStart(t);
                        if (i > 0)
                        {
                            tdir = t.Substring(0, i);
                            tname = FourFiguresName(t, i);
                        }
                    }
                }
            }
            else
            {
                // 地形画像検索用の地形画像ディレクトリ名と4桁ファイル名を作成
                // （元の SRC の Like "*#.bmp" と Like "[a-z]" は大文字・小文字を区別する）
                if (name.Length >= 5 && name.EndsWith(".bmp", System.StringComparison.Ordinal)
                    && char.IsDigit(name[name.Length - 5]) && name[0] >= 'a' && name[0] <= 'z')
                {
                    var i = NumberStart(name);
                    if (i > 0)
                    {
                        var dir = name.Substring(0, i);
                        if (src.TDList?.HasBitmap(dir) ?? false)
                        {
                            tdir = dir;
                            tname = FourFiguresName(name, i);
                        }
                    }
                }
            }

            // 各フォルダを検索する
            var list = new List<string>
            {
                // Bitmap フォルダに直置き
                Bitmap(fname),
                Bitmap("Anime", fname),
                Bitmap("Event", fname),
                Bitmap("Cutin", fname),
                Bitmap("Pilot", fname),
                Bitmap("Unit", fname),
            };
            if (!string.IsNullOrEmpty(tdir))
            {
                list.Add(Bitmap("Map", tdir, fname));
                list.Add(Bitmap("Map", tdir, tname));
                list.Add(Bitmap("Map", tname));
            }
            list.Add(Bitmap("Map", fname));
            yield return list;

            // SRC# では、読み込みアクセス対象のフォルダからのパスでも指定できる
            if (!hasBitmapPath)
            {
                yield return new[] { fname };
            }
        }

        /// <summary>
        /// 地形ユニットの画像のファイル名（元の SRC の FindUnitBitmap）。
        /// Bitmap\Map の画像を使い、地形画像の番号付きのファイル名（plain1.bmp → plain\plain0001.bmp など）にも読み替える。
        /// 返すパスは <see cref="FindPicture"/> で探せる。
        /// </summary>
        public static string TerrainUnitBitmap(SRC src, string fname)
        {
            if (string.IsNullOrEmpty(fname))
            {
                return fname;
            }
            var fs = src.FileSystem;
            var name = fname.Replace('/', '\\');
            if (fs.FileExists(fs.PathCombine("Bitmap", "Map", fname)))
            {
                return fs.PathCombine("Bitmap", "Map", fname);
            }

            // 地形画像検索用の地形画像ディレクトリ名と4桁ファイル名を作成
            string tdir = "", tname = "";
            var i = NumberStart(name);
            if (i > 0)
            {
                var dir = name.Substring(0, i);
                if (src.TDList?.HasBitmap(dir) ?? false)
                {
                    tdir = dir;
                    tname = FourFiguresName(name, i);
                }
            }
            if (!string.IsNullOrEmpty(tdir))
            {
                return fs.FileExists(fs.PathCombine("Bitmap", "Map", tname))
                    ? fs.PathCombine("Bitmap", "Map", tname)
                    : fs.PathCombine("Bitmap", "Map", tdir, tname);
            }
            return name.Contains(@"\")
                // フォルダ指定あり
                ? fs.PathCombine("Bitmap", fname)
                // 通常のユニット画像
                : fs.PathCombine("Bitmap", "Unit", fname);
        }

        /// <summary>
        /// ファイル名の末尾（拡張子の前）の数字の始まる位置（元の SRC の i）。
        /// 拡張子の 4 文字を除いた末尾から、数字と「-」でない文字を探す。見つからなければ 0。
        /// </summary>
        private static int NumberStart(string name)
        {
            // VB の Mid$(name, i, 1) は 1 始まり
            var i = name.Length - 5;
            while (i > 0)
            {
                var c = name[i - 1];
                if (!(c == '-' || (c >= '0' && c <= '9')))
                {
                    break;
                }
                i--;
            }
            return i;
        }

        /// <summary>地形画像の 4 桁の番号付きのファイル名（plain1.bmp → plain0001.bmp）</summary>
        private static string FourFiguresName(string name, int i)
        {
            var tnum = name.Substring(i, name.Length - i - 4);
            return name.Substring(0, i) + GeneralLib.StrToLng(tnum).ToString("0000", CultureInfo.InvariantCulture) + ".bmp";
        }
    }
}
