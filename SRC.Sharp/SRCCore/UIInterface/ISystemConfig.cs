using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore
{
    // システムオプション
    public interface ISystemConfig
    {
        bool SRCCompatibilityMode { get; set; }

        // 敵フェイズにはＢＧＭを変更しないか
        bool KeepEnemyBGM { get; set; }

        // 自動防御モードを使うか
        bool AutoMoveCursor { get; set; }

        // XXX GUIの関心毎
        // マス目の表示をするか
        bool ShowSquareLine { get; set; }
        // スペシャルパワーアニメを表示するか
        bool SpecialPowerAnimation { get; set; }
        // 戦闘アニメを表示するか
        bool BattleAnimation { get; set; }
        // 武器準備アニメを表示するか
        bool WeaponAnimation { get; set; }
        // 拡大戦闘アニメを表示するか
        bool ExtendedAnimation { get; set; }
        // 移動アニメを表示するか
        bool MoveAnimation { get; set; }

        // XXX この辺はSRCのコア的にはどうでもいい
        // MIDI音源リセットの種類
        string MidiResetType { get; set; }
        // 画像バッファの枚数
        int ImageBufferSize { get; set; }
        // 画像バッファの最大バイト数
        int MaxImageBufferByteSize { get; set; }
        // 拡大画像を画像バッファに保存するか
        bool KeepStretchedImage { get; set; }
        // XXX /この辺はSRCのコア的にはどうでもいい

        // Optionセクションから展開したもの
        bool AutoDefense { get; set; }

        // XXX この辺に依存する処理はあんまり嬉しくない
        // SRC.exeのある場所
        string AppPath { get; set; }
        // 拡張データフォルダへのパス
        string ExtDataPath { get; set; }
        string ExtDataPath2 { get; set; }

        void SetItem(string section, string name, string value);
        string GetItem(string section, string name);

        void Save();
        void Load();
    }
}
