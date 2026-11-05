using System;
using System.Collections.Generic;
using SRCCore.Filesystem;

namespace SRCCore.Config
{
    /// <summary>
    /// ゲームが参照する設定。アプリの設定（<see cref="Base"/>）にシナリオの設定（<see cref="Scenario"/>）を重ねて見せる。
    /// 自分では値を持たず、参照するたびに両方から合成する。
    /// <para>
    /// 元の SRC の ReadIni・WriteIni と同じく、シナリオの設定ファイルにある項目はシナリオの値を使い、変えたらシナリオの設定に書く。
    /// それ以外の項目はアプリの設定を読み書きする。どちらの設定も、保存はそれぞれのファイルに対して行う。
    /// </para>
    /// </summary>
    public class ComposedConfig : ISystemConfig
    {
        public ISystemConfig Base { get; }
        public ScenarioConfig Scenario { get; }
        private readonly IFileSystem fileSystem;
        private readonly Action<Exception> onScenarioSaveError;

        /// <param name="baseConfig">アプリの設定</param>
        /// <param name="scenario">シナリオの設定。無ければ null</param>
        /// <param name="fileSystem">シナリオの設定を書くファイルシステム</param>
        /// <param name="onScenarioSaveError">シナリオの設定を書けなかったとき</param>
        public ComposedConfig(ISystemConfig baseConfig, ScenarioConfig scenario, IFileSystem fileSystem = null, Action<Exception> onScenarioSaveError = null)
        {
            Base = baseConfig ?? throw new ArgumentNullException(nameof(baseConfig));
            Scenario = scenario;
            this.fileSystem = fileSystem;
            this.onScenarioSaveError = onScenarioSaveError;
        }

        /// <summary>シナリオで決めたプロパティか</summary>
        public bool IsScenarioProperty(string name) => Scenario != null && Scenario.TryGetProperty(name, out _);

        /// <summary>シナリオで決めた項目か</summary>
        public bool IsScenarioItem(string section, string name) => Scenario != null && Scenario.TryGetItem(section, name, out _);

        /// <summary>音量（0～100）。シナリオで決めていなければアプリの設定（アプリの設定が音量を持たなければ 50）</summary>
        public int SoundVolume
        {
            get => TryGetProperty(nameof(SoundVolume), out var v) && int.TryParse(v, out var volume)
                ? Math.Max(0, Math.Min(100, volume))
                : (Base as LocalFileConfig)?.SoundVolume ?? 50;
            set => SetProperty(nameof(SoundVolume), value.ToString(), () =>
            {
                if (Base is LocalFileConfig local)
                {
                    local.SoundVolume = value;
                }
            });
        }

        public SRCCompatibilityMode SRCCompatibilityMode
        {
            get => TryGetProperty(nameof(SRCCompatibilityMode), out var v) && TryParseCompatibilityMode(v, out var mode) ? mode : Base.SRCCompatibilityMode;
            set => SetProperty(nameof(SRCCompatibilityMode), value.ToString(), () => Base.SRCCompatibilityMode = value);
        }

        public bool KeepEnemyBGM { get => GetBool(nameof(KeepEnemyBGM), Base.KeepEnemyBGM); set => SetBool(nameof(KeepEnemyBGM), value, () => Base.KeepEnemyBGM = value); }
        public bool AutoMoveCursor { get => GetBool(nameof(AutoMoveCursor), Base.AutoMoveCursor); set => SetBool(nameof(AutoMoveCursor), value, () => Base.AutoMoveCursor = value); }
        public bool ShowSquareLine { get => GetBool(nameof(ShowSquareLine), Base.ShowSquareLine); set => SetBool(nameof(ShowSquareLine), value, () => Base.ShowSquareLine = value); }
        public bool SpecialPowerAnimation { get => GetBool(nameof(SpecialPowerAnimation), Base.SpecialPowerAnimation); set => SetBool(nameof(SpecialPowerAnimation), value, () => Base.SpecialPowerAnimation = value); }
        public bool BattleAnimation { get => GetBool(nameof(BattleAnimation), Base.BattleAnimation); set => SetBool(nameof(BattleAnimation), value, () => Base.BattleAnimation = value); }
        public bool WeaponAnimation { get => GetBool(nameof(WeaponAnimation), Base.WeaponAnimation); set => SetBool(nameof(WeaponAnimation), value, () => Base.WeaponAnimation = value); }
        public bool ExtendedAnimation { get => GetBool(nameof(ExtendedAnimation), Base.ExtendedAnimation); set => SetBool(nameof(ExtendedAnimation), value, () => Base.ExtendedAnimation = value); }
        public bool MoveAnimation { get => GetBool(nameof(MoveAnimation), Base.MoveAnimation); set => SetBool(nameof(MoveAnimation), value, () => Base.MoveAnimation = value); }
        public int ImageBufferSize { get => GetInt(nameof(ImageBufferSize), Base.ImageBufferSize); set => SetProperty(nameof(ImageBufferSize), value.ToString(), () => Base.ImageBufferSize = value); }
        public int MaxImageBufferByteSize { get => GetInt(nameof(MaxImageBufferByteSize), Base.MaxImageBufferByteSize); set => SetProperty(nameof(MaxImageBufferByteSize), value.ToString(), () => Base.MaxImageBufferByteSize = value); }
        public bool KeepStretchedImage { get => GetBool(nameof(KeepStretchedImage), Base.KeepStretchedImage); set => SetBool(nameof(KeepStretchedImage), value, () => Base.KeepStretchedImage = value); }

        // Option セクションの項目。シナリオの [Option] AutoDefense も GetItem を通して効く
        public bool AutoDefense
        {
            get => string.Equals(GetItem("Option", "AutoDefense"), "on", StringComparison.OrdinalIgnoreCase);
            set => SetItem("Option", "AutoDefense", value ? "On" : "Off");
        }

        // アプリ・素材の場所はシナリオでは変えない
        public string AppPath { get => Base.AppPath; set => Base.AppPath = value; }
        public string ExtDataPath { get => Base.ExtDataPath; set => Base.ExtDataPath = value; }
        public string ExtDataPath2 { get => Base.ExtDataPath2; set => Base.ExtDataPath2 = value; }

        public string GetItem(string section, string name)
        {
            return Scenario != null && Scenario.TryGetItem(section, name, out var value)
                ? value
                : Base.GetItem(section, name);
        }

        public void SetItem(string section, string name, string value)
        {
            if (Scenario == null || !Scenario.TrySetItem(section, name, value))
            {
                Base.SetItem(section, name, value);
            }
        }

        /// <summary>
        /// アプリの設定と、変えたシナリオの設定をそれぞれのファイルに保存する。
        /// シナリオの設定を書けなかったとき（アーカイブの中など）は、変えた値はこのゲームの間だけ効く
        /// </summary>
        public void Save()
        {
            Base.Save();
            if (Scenario?.HasChanges ?? false)
            {
                try
                {
                    if (fileSystem == null)
                    {
                        throw new InvalidOperationException("シナリオの設定を書くファイルシステムがありません。");
                    }
                    Scenario.Save(fileSystem);
                }
                catch (Exception ex)
                {
                    onScenarioSaveError?.Invoke(ex);
                }
            }
        }

        /// <summary>アプリの設定を読み直す</summary>
        public void Load() => Base.Load();

        private bool TryGetProperty(string name, out string value)
        {
            value = null;
            return Scenario != null && Scenario.TryGetProperty(name, out value);
        }

        private bool GetBool(string name, bool fallback)
        {
            return TryGetProperty(name, out var v) && TryParseBool(v, out var b) ? b : fallback;
        }

        private int GetInt(string name, int fallback)
        {
            return TryGetProperty(name, out var v) && int.TryParse(v, out var i) ? i : fallback;
        }

        private void SetBool(string name, bool value, Action setBase)
        {
            SetProperty(name, value ? "true" : "false", setBase);
        }

        private void SetProperty(string name, string value, Action setBase)
        {
            if (Scenario == null || !Scenario.TrySetProperty(name, value))
            {
                setBase();
            }
        }

        public static bool TryParseBool(string value, out bool result)
        {
            switch ((value ?? "").Trim().ToLowerInvariant())
            {
                case "on":
                case "true":
                case "1":
                    result = true;
                    return true;
                case "off":
                case "false":
                case "0":
                    result = false;
                    return true;
                default:
                    result = false;
                    return false;
            }
        }

        public static bool TryParseCompatibilityMode(string value, out SRCCompatibilityMode mode)
        {
            value = (value ?? "").Trim();
            if (int.TryParse(value, out var n) && Enum.IsDefined(typeof(SRCCompatibilityMode), n))
            {
                mode = (SRCCompatibilityMode)n;
                return true;
            }
            if (Enum.TryParse(value, true, out mode) && Enum.IsDefined(typeof(SRCCompatibilityMode), mode))
            {
                return true;
            }
            // true・false・On・Off は互換モードの読み書きを両方とも切り替える
            if (TryParseBool(value, out var b))
            {
                mode = b ? SRCCompatibilityMode.ReadWrite : SRCCompatibilityMode.None;
                return true;
            }
            mode = SRCCompatibilityMode.ReadWrite;
            return false;
        }
    }
}
