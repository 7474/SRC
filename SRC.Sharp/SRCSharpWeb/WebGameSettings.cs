using SRCCore;
using SRCCore.Config;
using System;
using System.Text.Json;

namespace SRCSharpWeb
{
    /// <summary>
    /// SRC#Web のゲームの設定（元の SRC の設定変更の画面と同じ項目・自動反撃モード・音量）。
    /// SRC#Web の srcs.json はブラウザを閉じると消えるため、この項目だけを localStorage（<see cref="StorageKey"/>）に残し、
    /// シナリオを始めるときに srcs.json の設定へ重ねる。デスクトップ版の設定のウィンドウの「ゲーム」タブにあたる
    /// </summary>
    public class WebGameSettings
    {
        /// <summary>localStorage のキー</summary>
        public const string StorageKey = "srcweb.gameSettings";

        /// <summary>メッセージスピード（メッセージのウェイト。前の版で保存した設定に無ければ「普通」）</summary>
        public int MessageWait { get; set; } = MessageSpeeds.DefaultWait;
        public bool BattleAnimation { get; set; }
        public bool ExtendedAnimation { get; set; }
        public bool WeaponAnimation { get; set; }
        public bool SpecialPowerAnimation { get; set; }
        public bool MoveAnimation { get; set; }
        public bool AutoMoveCursor { get; set; }
        public bool ShowSquareLine { get; set; }
        public bool ShowTurn { get; set; }
        public bool KeepEnemyBGM { get; set; }
        public bool AutoDefense { get; set; }
        public int SoundVolume { get; set; }

        /// <summary>設定（srcs.json）から読む</summary>
        public static WebGameSettings FromConfig(ISystemConfig config)
        {
            return new WebGameSettings
            {
                MessageWait = config.MessageWait,
                BattleAnimation = config.BattleAnimation,
                ExtendedAnimation = config.ExtendedAnimation,
                WeaponAnimation = config.WeaponAnimation,
                SpecialPowerAnimation = config.SpecialPowerAnimation,
                MoveAnimation = config.MoveAnimation,
                AutoMoveCursor = config.AutoMoveCursor,
                ShowSquareLine = config.ShowSquareLine,
                ShowTurn = string.Equals(config.GetItem("Option", "Turn"), "On", StringComparison.OrdinalIgnoreCase),
                KeepEnemyBGM = config.KeepEnemyBGM,
                AutoDefense = config.AutoDefense,
                SoundVolume = config is LocalFileConfig local ? local.SoundVolume : 50,
            };
        }

        /// <summary>設定（srcs.json）に書く。ファイルには保存しない</summary>
        public void ApplyTo(ISystemConfig config)
        {
            config.MessageWait = MessageWait;
            config.BattleAnimation = BattleAnimation;
            config.ExtendedAnimation = ExtendedAnimation;
            config.WeaponAnimation = WeaponAnimation;
            config.SpecialPowerAnimation = SpecialPowerAnimation;
            config.MoveAnimation = MoveAnimation;
            config.AutoMoveCursor = AutoMoveCursor;
            config.ShowSquareLine = ShowSquareLine;
            config.SetItem("Option", "Turn", ShowTurn ? "On" : "Off");
            config.KeepEnemyBGM = KeepEnemyBGM;
            config.AutoDefense = AutoDefense;
            if (config is LocalFileConfig local)
            {
                local.SoundVolume = Math.Max(0, Math.Min(100, SoundVolume));
            }
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        /// <summary>localStorage の値を読む。無い・読めなければ null（srcs.json の設定のまま）</summary>
        public static WebGameSettings FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }
            try
            {
                return JsonSerializer.Deserialize<WebGameSettings>(json);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        /// <summary>同じ値か（ゲームの中の設定変更で変わったかを調べる）</summary>
        public bool SameAs(WebGameSettings other)
        {
            return other != null && ToJson() == other.ToJson();
        }
    }
}
