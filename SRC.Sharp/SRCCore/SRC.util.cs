using SRCCore.Lib;
using SRCCore.VB;
using System;
using System.Reflection;

namespace SRCCore
{
    public partial class SRC
    {
        // 旧形式のユニットＩＤを新形式に変換
        // 旧形式）ユニット名称+数値
        // 新形式）ユニット名称+":"+数値
        public void ConvertUnitID(ref string ID)
        {
            int i;
            if (Strings.InStr(ID, ":") > 0)
            {
                return;
            }

            // 数値部分を読み飛ばす
            i = Strings.Len(ID);
            while (i > 0)
            {
                switch (Strings.Asc(Strings.Mid(ID, i, 1)))
                {
                    // 0-9
                    case var @case when 48 <= @case && @case <= 57:
                        {
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }

                i = (i - 1);
            }

            // ユニット名称と数値部分の間に「:」を挿入
            ID = Strings.Left(ID, i) + ":" + Strings.Mid(ID, i + 1);
        }

        // 資金の量を変更する
        public void IncrMoney(int earnings)
        {
            Money = GeneralLib.MinLng(Money + earnings, 999999999);
            Money = GeneralLib.MaxLng(Money, 0);
        }

        private Random _generalRand = new Random();
        // ゲーム処理とは独立した0以上、1未満の乱数を返す
        public double Rand()
        {
            return _generalRand.NextDouble();
        }

        // バージョンを取得、設定する
        // 既定値は実行ファイルのバージョン
        // EntryAssemblyが取れない場合はCoreのバージョンを返す
        public Version Version { get; set; } = (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).GetName().Version;

        /// <summary>
        /// 開発版のプレリリースの印（例: dev.123）。正式版では空。
        /// 実行ファイルの InformationalVersion（3.5.6-dev.123+&lt;コミット&gt;）の「-」の後ろ・「+」の前（docs/versioning.md の「開発版」）
        /// </summary>
        public string PreRelease { get; set; } = PreReleaseOf(Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly());

        public static string PreReleaseOf(Assembly assembly)
        {
            var version = (assembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "").Split('+')[0];
            // 開発版の印（dev.<番号>）だけを見る。テストの実行環境など、ほかの実行ファイルのプレリリースは出さない
            var idx = version.IndexOf("-dev.", StringComparison.Ordinal);
            return idx < 0 ? "" : version.Substring(idx + 1);
        }

        // 互換の対象とする元の SRC のバージョン（移植元の SRC Ver2.2.33）
        // シナリオは AppVersion で本体のバージョンを確かめるため、SRC# 自身のバージョンではなくこちらを返す
        public Version CompatibleVersion { get; set; } = new Version(2, 2, 33);
    }
}
