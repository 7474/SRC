// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using Microsoft.Extensions.Logging;
using SRCCore.Config;
using SRCCore.Events;
using SRCCore.Expressions;
using SRCCore.Filesystem;
using SRCCore.Maps;
using SRCCore.Models;
using System;
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
        public IGUIScrean GUIScrean { get; set; }
        public IFileSystem FileSystem { get; set; }
        public FileHandleManager FileHandleManager { get; private set; }

        public Help Help { get; }
        public Expression Expression { get; }
        public Event Event { get; }
        public Map Map { get; }
        public Commands.Command Commands { get; }
        public Sound Sound { get; }
        public Effect Effect { get; }
        public COM COM { get; }
        public InterMission InterMission { get; }

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
        // アイテムのリスト
        public Items.Items IList;

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
        public ISystemConfig SystemConfig { get; set; }

        // マス目の表示をするか
        public bool ShowSquareLine => SystemConfig.ShowSquareLine;
        // 敵フェイズにはＢＧＭを変更しないか
        public bool KeepEnemyBGM => SystemConfig.KeepEnemyBGM;
        // 拡張データフォルダへのパス
        public string ExtDataPath => SystemConfig.ExtDataPath;
        public string ExtDataPath2 => SystemConfig.ExtDataPath2;
        // MIDI音源リセットの種類
        public string MidiResetType => SystemConfig.MidiResetType;
        // 自動防御モードを使うか
        public bool AutoMoveCursor => SystemConfig.AutoMoveCursor;
        // スペシャルパワーアニメを表示するか
        public bool SpecialPowerAnimation => SystemConfig.SpecialPowerAnimation;
        // 戦闘アニメを表示するか
        public bool BattleAnimation => SystemConfig.BattleAnimation;
        // 武器準備アニメを表示するか
        public bool WeaponAnimation => SystemConfig.WeaponAnimation;
        // 拡大戦闘アニメを表示するか
        public bool ExtendedAnimation => SystemConfig.ExtendedAnimation;
        // 移動アニメを表示するか
        public bool MoveAnimation => SystemConfig.MoveAnimation;
        // 画像バッファの枚数
        public int ImageBufferSize => SystemConfig.ImageBufferSize;
        // 画像バッファの最大バイト数
        public int MaxImageBufferByteSize => SystemConfig.MaxImageBufferByteSize;
        // 拡大画像を画像バッファに保存するか
        public bool KeepStretchedImage => SystemConfig.KeepStretchedImage;
        // SRC.exeのある場所
        public string AppPath => SystemConfig.AppPath;

        // XXX 既定のログ構成
        public SRC()
            : this(LoggerFactory.Create(builder =>
             {
                 builder
                     .SetMinimumLevel(LogLevel.Debug)
                     .AddDebug();
             }))
        {
        }

        public SRC(ILoggerFactory loggerFactory)
        {
            Log = loggerFactory.CreateLogger("SRCCore");

            Help = new Help(this);
            Event = new Event(this);
            Expression = new Expression(this);
            Map = new Map(this);
            Commands = new Commands.Command(this);
            Sound = new Sound(this);
            Effect = new Effect(this);
            COM = new COM(this);
            InterMission = new InterMission(this);
            FileHandleManager = new FileHandleManager();

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
            IList = new Items.Items(this);

            // XXX 別な実装をするならコンストラクタでは設定しない
            SystemConfig = new LocalFileConfig();
        }

        public void LogError(Exception ex)
        {
            LogError("", ex);
        }
        public void LogError(string message)
        {
            LogError(message, null);
        }
        public void LogError(string message, Exception ex, params string[] param)
        {
            try
            {
                if (!Log.IsEnabled(LogLevel.Error)) { return; }
                Log.LogDebug(message
                    + " "
                    + string.Join(", ", param)
                    + " "
                    + ex?.Message
                    + ex?.StackTrace);
            }
            catch
            {
                // ignore
            }
        }

        public void LogWarn(Exception ex)
        {
            LogWarn("", ex);
        }
        public void LogWarn(string message, Exception ex, params string[] param)
        {
            try
            {
                if (!Log.IsEnabled(LogLevel.Warning)) { return; }
                Log.LogDebug(message
                    + " "
                    + string.Join(", ", param)
                    + " "
                    + ex?.Message
                    + ex?.StackTrace);
            }
            catch
            {
                // ignore
            }
        }

        public void LogInfo(string message, params string[] param)
        {
            try
            {
                Log.LogInformation(message
                    + " "
                    + string.Join(", ", param));
            }
            catch
            {
                // ignore
            }
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

        public void LogTrace(string message = "", params string[] param)
        {
            try
            {
                if (!Log.IsEnabled(LogLevel.Trace)) { return; }
                string method = new StackFrame(1).GetMethod().Name;
                Log.LogTrace(method
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
