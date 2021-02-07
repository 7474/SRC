// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRC.Core.Events;
using SRC.Core.Expressions;
using SRC.Core.Lib;
using SRC.Core.Maps;
using SRC.Core.Models;
using SRC.Core.VB;
using System;
using System.Collections.Generic;
using System.IO;

namespace SRC.Core
{
    public partial class SRC
    {
        public IGUI GUI { get; set; }
        public IGUIMap GUIMap { get; set; }

        public Expression Expression { get; }
        public Event Event { get; }
        public Map Map { get; }

        // パイロットデータのリスト
        public PilotDataList PDList = new PilotDataList();
        // ノンパイロットデータのリスト
        public NonPilotDataList NPDList = new NonPilotDataList();
        // ユニットデータのリスト
        public UnitDataList UDList = new UnitDataList();
        //// アイテムデータのリスト
        //public ItemDataList IDList = new ItemDataList();
        // メッセージデータのリスト
        public MessageDataList MDList = new MessageDataList();
        // 特殊効果データのリスト
        public MessageDataList EDList = new MessageDataList();
        // 戦闘アニメデータのリスト
        public MessageDataList ADList = new MessageDataList();
        // 拡張戦闘アニメデータのリスト
        public MessageDataList EADList = new MessageDataList();
        //// ダイアログデータのリスト
        //public DialogDataList DDList = new DialogDataList();
        //// スペシャルパワーデータのリスト
        //public SpecialPowerDataList SPDList = new SpecialPowerDataList();
        //// エリアスデータのリスト
        //public AliasDataList ALDList = new AliasDataList();
        //// 地形データのリスト
        //public TerrainDataList TDList = new TerrainDataList();
        //// バトルコンフィグデータのリスト
        //public BattleConfigDataList BCList = new BattleConfigDataList();

        // パイロットのリスト
        public Pilots.Pilots PList;
        // ユニットのリスト
        public Units.Units UList;
        //// アイテムのリスト
        //public Items IList = new Items();

        // イベントファイル名
        public string ScenarioFileName = "";
        // イベントファイル名のあるフォルダ
        public string ScenarioPath = "";
        // セーブデータのファイルディスクリプタ
        public int SaveDataFileNumber;
        // セーブデータのバージョン
        public int SaveDataVersion;

        // そのステージが終了したかを示すフラグ
        public bool IsScenarioFinished;
        // インターミッションコマンドによるステージかどうかを示すフラグ
        public bool IsSubStage;
        // コマンドがキャンセルされたかどうかを示すフラグ
        public bool IsCanceled;

        // フェイズ名
        public string Stage;
        // ターン数
        public int Turn;
        // 総ターン数
        public int TotalTurn;
        // 総資金
        public int Money;
        // 読み込まれているデータ数
        public IList<string> Titles;
        // ローカルデータが読み込まれているか？
        public bool IsLocalDataLoaded;

        // 最新のセーブデータのファイル名
        public string LastSaveDataFileName;
        // リスタート用セーブデータが利用可能かどうか
        public bool IsRestartSaveDataAvailable;
        // クイックロード用セーブデータが利用可能かどうか
        public bool IsQuickSaveDataAvailable;

        // システムオプション
        // マス目の表示をするか
        public bool ShowSquareLine;
        // 敵フェイズにはＢＧＭを変更しないか
        public bool KeepEnemyBGM;
        // 拡張データフォルダへのパス
        public string ExtDataPath = "";
        public string ExtDataPath2 = "";
        // MIDI音源リセットの種類
        public string MidiResetType;
        // 自動防御モードを使うか
        public bool AutoMoveCursor;
        // スペシャルパワーアニメを表示するか
        public bool SpecialPowerAnimation;
        // 戦闘アニメを表示するか
        public bool BattleAnimation;
        // 武器準備アニメを表示するか
        public bool WeaponAnimation;
        // 拡大戦闘アニメを表示するか
        public bool ExtendedAnimation;
        // 移動アニメを表示するか
        public bool MoveAnimation;
        // 画像バッファの枚数
        public int ImageBufferSize;
        // 画像バッファの最大バイト数
        public int MaxImageBufferByteSize;
        // 拡大画像を画像バッファに保存するか
        public bool KeepStretchedImage;
        // 透過描画にTransparentBltを使うか
        public bool UseTransparentBlt;

        // SRC.exeのある場所
        public string AppPath = "";

        public SRC()
        {
            Event = new Event(this);
            Expression = new Expression(this);
            Map = new Map(this);

            PList = new Pilots.Pilots(this);
            UList = new Units.Units(this);
        }
    }
}