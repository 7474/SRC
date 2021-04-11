// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using Microsoft.Extensions.Logging;
using SRCCore.Events;
using SRCCore.Expressions;
using SRCCore.Filesystem;
using SRCCore.Maps;
using SRCCore.Models;
using System.Collections.Generic;
using System.Diagnostics;

namespace SRCCore
{
    public partial class SRC
    {
        public ILogger Log { get; private set; }

        public IGUI GUI { get; set; }
        public IGUIMap GUIMap { get; set; }
        public IGUIStatus GUIStatus { get; set; }
        public IFileSystem FileSystem { get; set; }

        public Expression Expression { get; }
        public Event Event { get; }
        public Map Map { get; }
        public Commands.Command Commands { get; }
        public Sound Sound { get; }
        public COM COM { get; }

        // パイロットデータのリスト
        public PilotDataList PDList;
        // ノンパイロットデータのリスト
        public NonPilotDataList NPDList;
        // ユニットデータのリスト
        public UnitDataList UDList;
        // アイテムデータのリスト
        public ItemDataList IDList;
        // メッセージデータのリスト
        public MessageDataList MDList;
        // 特殊効果データのリスト
        public MessageDataList EDList;
        // 戦闘アニメデータのリスト
        public MessageDataList ADList;
        // 拡張戦闘アニメデータのリスト
        public MessageDataList EADList;
        // ダイアログデータのリスト
        public DialogDataList DDList;
        // スペシャルパワーデータのリスト
        public SpecialPowerDataList SPDList;
        // エリアスデータのリスト
        public AliasDataList ALDList;
        // 地形データのリスト
        public TerrainDataList TDList;
        // バトルコンフィグデータのリスト
        public BattleConfigDataList BCList;

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
        // TODO Impl
        public bool BattleAnimation = true;
        // 武器準備アニメを表示するか
        public bool WeaponAnimation;
        // 拡大戦闘アニメを表示するか
        public bool ExtendedAnimation;
        // 移動アニメを表示するか
        public bool MoveAnimation = true;
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
            // TODO 外部からの注入にする
            using (var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .SetMinimumLevel(LogLevel.Debug)
                    .AddDebug();
            }))
            {
                Log = loggerFactory.CreateLogger("SRCCore");
            }

            Event = new Event(this);
            Expression = new Expression(this);
            Map = new Map(this);
            Commands = new Commands.Command(this);
            Sound = new Sound(this);
            COM = new COM(this);

            PDList = new PilotDataList(this);
            NPDList = new NonPilotDataList(this);
            UDList = new UnitDataList(this);
            IDList = new ItemDataList(this);
            MDList = new MessageDataList(this);
            EDList = new MessageDataList(this);
            ADList = new MessageDataList(this);
            EADList = new MessageDataList(this);
            DDList = new DialogDataList(this);
            SPDList = new SpecialPowerDataList(this);
            ALDList = new AliasDataList(this);
            TDList = new TerrainDataList(this);
            BCList = new BattleConfigDataList(this);

            PList = new Pilots.Pilots(this);
            UList = new Units.Units(this);
        }

        public void LogDebug(string message = "", params string[] param)
        {
            try
            {
                if (!Log.IsEnabled(LogLevel.Debug)) { return; }
                string method = new StackFrame(1).GetMethod().Name;
                Log.LogDebug(method
                    + " "
                    + message
                    + " "
                    + string.Join(", ", param));
            }
            catch
            {
                // ignore
            }
        }
    }
}
