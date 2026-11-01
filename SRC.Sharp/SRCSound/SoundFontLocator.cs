using MeltySynth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCSound
{
    /// <summary>
    /// MIDI の合成に使う SoundFont（.sf2）を探す。利用者が用意したものを優先し、無ければ同梱のもの（SRC#Avalonia の TimGM6mb）を使う。
    /// </summary>
    public static class SoundFontLocator
    {
        /// <summary>SoundFont のパスを指定する環境変数</summary>
        public const string EnvironmentVariable = "SRC_SOUNDFONT";

        /// <summary>主な Linux ディストリビューションで GM の SoundFont が置かれる場所</summary>
        public static readonly IReadOnlyList<string> SystemPaths = new[]
        {
            "/usr/share/sounds/sf2/default-GM.sf2",
            "/usr/share/sounds/sf2/FluidR3_GM.sf2",
            "/usr/share/soundfonts/default.sf2",
            "/usr/share/soundfonts/FluidR3_GM.sf2",
        };

        /// <summary>同梱の SoundFont を置くフォルダ（実行ファイルの隣の SoundFont フォルダ）</summary>
        public static string BundledDirectory => Path.Combine(AppContext.BaseDirectory, "SoundFont");

        /// <summary>
        /// 次の順に探して最初に見つかったものを返す。無ければ null。
        /// 1. configuredPath（設定ファイルの指定）
        /// 2. 環境変数 SRC_SOUNDFONT
        /// 3. appPath（SRC のフォルダ）直下と Midi フォルダの *.sf2
        /// 4. SystemPaths
        /// 5. 同梱のもの（BundledDirectory の *.sf2）
        /// </summary>
        public static string Find(string appPath, string configuredPath = null)
        {
            return Find(appPath, configuredPath, SystemPaths, BundledDirectory);
        }

        /// <summary>探す場所を指定して探す（テスト用）</summary>
        public static string Find(string appPath, string configuredPath, IEnumerable<string> systemPaths, string bundledDirectory)
        {
            var candidates = new List<string> { configuredPath, Environment.GetEnvironmentVariable(EnvironmentVariable) };
            if (!string.IsNullOrEmpty(appPath))
            {
                foreach (var dir in new[] { appPath, Path.Combine(appPath, "Midi") })
                {
                    if (Directory.Exists(dir))
                    {
                        candidates.AddRange(Directory.GetFiles(dir, "*.sf2").OrderBy(x => x, StringComparer.Ordinal));
                    }
                }
            }
            candidates.AddRange(systemPaths);
            if (!string.IsNullOrEmpty(bundledDirectory) && Directory.Exists(bundledDirectory))
            {
                candidates.AddRange(Directory.GetFiles(bundledDirectory, "*.sf2").OrderBy(x => x, StringComparer.Ordinal));
            }
            return candidates.FirstOrDefault(x => !string.IsNullOrEmpty(x) && File.Exists(x));
        }

        public static SoundFont Load(string appPath, string configuredPath = null)
        {
            var path = Find(appPath, configuredPath);
            return path == null ? null : new SoundFont(path);
        }
    }
}
