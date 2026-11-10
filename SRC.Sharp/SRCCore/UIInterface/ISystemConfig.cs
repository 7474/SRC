using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore
{
    // システムオプション
    public interface ISystemConfig
    {
        SRCCompatibilityMode SRCCompatibilityMode { get; set; }

        // 敵フェイズにはＢＧＭを変更しないか
        bool KeepEnemyBGM { get; set; }

        // 自動防御モードを使うか
        bool AutoMoveCursor { get; set; }

        // 以下は表示の設定（元の SRC の SRC.bas の「システムオプション」の変数と Src.ini の [Option]）
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
        // メッセージのウェイト（メッセージスピード。元の SRC の Src.ini の [Option] MessageWait）
        int MessageWait { get; set; }

        // 画像バッファの設定（元の SRC の Src.ini の [Option]。SRC# のコアの処理では使わず、画面が使う）
        // 画像バッファの枚数
        int ImageBufferSize { get; set; }
        // 画像バッファの最大バイト数
        int MaxImageBufferByteSize { get; set; }
        // 拡大画像を画像バッファに保存するか
        bool KeepStretchedImage { get; set; }

        // Optionセクションから展開したもの
        bool AutoDefense { get; set; }

        // フォルダの場所（元の SRC の AppPath・ExtDataPath・ExtDataPath2）
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

    public enum SRCCompatibilityMode
    {
        None = 0,
        Read = 1,
        Write = 2,
        ReadWrite = 3,
    }
}
