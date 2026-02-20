using Newtonsoft.Json;
using SRCCore.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCCore.Config
{
    // 設定項目の説明をファイルに出力する機能は将来の拡張として検討する。
    // XXX 説明をどこでエントリーするか？　別にJSONでなくてもいい。
    public class LocalFileConfig : ISystemConfig
    {
        public SRCCompatibilityMode SRCCompatibilityMode { get; set; }
        public bool ShowSquareLine { get; set; }
        public bool KeepEnemyBGM { get; set; }
        public string ExtDataPath { get; set; }
        public string ExtDataPath2 { get; set; }
        public string MidiResetType { get; set; }
        public bool AutoMoveCursor { get; set; }
        public bool SpecialPowerAnimation { get; set; }
        public bool BattleAnimation { get; set; }
        public bool WeaponAnimation { get; set; }
        public bool ExtendedAnimation { get; set; }
        public bool MoveAnimation { get; set; }
        public int ImageBufferSize { get; set; }
        public int MaxImageBufferByteSize { get; set; }
        public bool KeepStretchedImage { get; set; }

        // XXX Configの余地ないしConfigのプロパティではないかもな。
        [JsonIgnore]
        public string AppPath { get; set; }

        //
        public int SoundVolume { get; set; }

        public List<ConfigSection> Sections { get; set; }
        public bool AutoDefense { get => this.GetFlag("Option", "AutoDefense"); set => this.SetFlag("Option", "AutoDefense", value); }

        public LocalFileConfig()
        {
            Sections = new List<ConfigSection>();

            // SRC.exeのある場所を調べる
            AppPath = AppContext.BaseDirectory;
            ExtDataPath = "";
            ExtDataPath2 = "";

            // 互換モードは既定でOnにしておく
            SRCCompatibilityMode = SRCCompatibilityMode.ReadWrite;
            SoundVolume = 50;
        }

        public string GetItem(string section, string name)
        {
            return Item(section, name)?.Value ?? "";
        }

        public void SetItem(string section, string name, string value)
        {
            var s = Section(section);
            if (s == null)
            {
                s = new ConfigSection { Name = section };
                Sections.Add(s);
            }
            var item = Item(section, name);
            if (item == null)
            {
                item = new ConfigItem { Name = name };
                s.Items.Add(item);
            }
            item.Value = value;
        }

        private ConfigSection Section(string section)
        {
            return Sections.FirstOrDefault(x => x.Name == section);
        }

        private ConfigItem Item(string section, string name)
        {
            return Section(section)?.Items.FirstOrDefault(x => x.Name == name);
        }

        public void Load()
        {
            var json = File.ReadAllText(Path.Combine(AppPath, "srcs.json"));
            JsonConvert.PopulateObject(json, this);
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(Path.Combine(AppPath, "srcs.json"), json);
        }
    }

    public class ConfigSection
    {
        public string Name { get; set; }
        public List<ConfigItem> Items { get; set; } = new List<ConfigItem>();
    }

    public class ConfigItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
