using MeltySynth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCSound
{
    /// <summary>
    /// MIDI の合成に使う SoundFont（.sf2）を探す。SoundFont は同梱しないため、利用者が用意したものを使う。
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

        /// <summary>
        /// 次の順に探して最初に見つかったものを返す。無ければ null。
        /// 1. configuredPath（設定ファイルの指定）
        /// 2. 環境変数 SRC_SOUNDFONT
        /// 3. appPath（SRC のフォルダ）直下と Midi フォルダの *.sf2
        /// 4. SystemPaths
        /// </summary>
        public static string Find(string appPath, string configuredPath = null)
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
            candidates.AddRange(SystemPaths);
            return candidates.FirstOrDefault(x => !string.IsNullOrEmpty(x) && File.Exists(x));
        }

        public static SoundFont Load(string appPath, string configuredPath = null)
        {
            var path = Find(appPath, configuredPath);
            return path == null ? null : new SoundFont(path);
        }
    }
}
