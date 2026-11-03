// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SRCCore.Config;
using SRCCore.Events;
using SRCCore.Expressions;
using SRCCore.Filesystem;
using SRCCore.Logging;
using SRCCore.Maps;
using SRCCore.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SRCCore
{
    public partial class SRC
    {
        /// <summary>ログの出力先。ホスト（各アプリ）が渡す</summary>
        public ILoggerFactory LoggerFactory { get; private set; }
        /// <summary>起動・データの読み込み・セーブ・ロード（カテゴリ SRCCore）</summary>
        public ILogger Log { get; private set; }
        /// <summary>イベントの実行・コマンド（カテゴリ SRCCore.Event）</summary>
        public ILogger EventLog { get; private set; }
        /// <summary>式・変数・関数（カテゴリ SRCCore.Expression）</summary>
        public ILogger ExpressionLog { get; private set; }
        /// <summary>ユニットへのコマンド・COM（カテゴリ SRCCore.Command）</summary>
        public ILogger CommandLog { get; private set; }

        public IGUI GUI { get; set; }

        /// <summary>
        /// 汎用グラフィック集・効果音集・地形データなどの素材が無くても実行を続けるか。
        /// true の場合、素材の不足は警告ログに留め、地形データが無いマップは仮の地形で読み込む。
        /// </summary>
        public bool AllowMissingAssets { get; set; }
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
        /// <summary>アプリの設定（srcs.json）。設定の画面で読み書きし、保存する</summary>
        public ISystemConfig SystemConfig { get; set; }

        /// <summary>シナリオのフォルダの設定（Src.ini・srcs.json）。無ければ null</summary>
        public ScenarioConfig ScenarioConfig
        {
            get => scenarioConfig;
            set
            {
                scenarioConfig = value;
                composedConfig = null;
            }
        }
        private ScenarioConfig scenarioConfig;
        private ComposedConfig composedConfig;

        /// <summary>
        /// ゲームが参照する設定。アプリの設定にシナリオの設定を重ねたもの（<see cref="ComposedConfig"/>）。
        /// アプリの設定が差し替えられたら作り直す
        /// </summary>
        public ISystemConfig Config
        {
            get
            {
                if (composedConfig == null || composedConfig.Base != SystemConfig)
                {
                    composedConfig = new ComposedConfig(SystemConfig, scenarioConfig, FileSystem,
                        ex => Log.LogWarning(ex, "シナリオの設定ファイルを書けませんでした。変えた設定はこのゲームの間だけ効きます。"));
                }
                return composedConfig;
            }
        }

        /// <summary>
        /// オートプレイ（味方フェイズを CPU と同じ思考で自動で行い、ターンを終える）。マップコマンドで切り替える。
        /// 設定ファイル・セーブデータには残さない
        /// </summary>
        public bool AutoPlay { get; set; }

        /// <summary>敵の攻撃への反撃・防御・回避を CPU と同じ判断で選ぶか（自動反撃モードかオートプレイ中）</summary>
        public bool IsAutoDefense => Config.AutoDefense || AutoPlay;

        // マス目の表示をするか
        public bool ShowSquareLine => Config.ShowSquareLine;
        // 敵フェイズにはＢＧＭを変更しないか
        public bool KeepEnemyBGM => Config.KeepEnemyBGM;
        // 拡張データフォルダへのパス
        public string ExtDataPath => Config.ExtDataPath;
        public string ExtDataPath2 => Config.ExtDataPath2;
        // 自動防御モードを使うか
        public bool AutoMoveCursor => Config.AutoMoveCursor;
        // スペシャルパワーアニメを表示するか
        public bool SpecialPowerAnimation => Config.SpecialPowerAnimation;
        // 戦闘アニメを表示するか
        public bool BattleAnimation => !BattleAnimationUnavailable && Config.BattleAnimation;
        /// <summary>汎用戦闘アニメのインクルードファイルが無く、戦闘アニメを使えないか。設定ファイルには残さない</summary>
        public bool BattleAnimationUnavailable { get; set; }
        // 武器準備アニメを表示するか
        public bool WeaponAnimation => Config.WeaponAnimation;
        // 拡大戦闘アニメを表示するか
        public bool ExtendedAnimation => Config.ExtendedAnimation;
        // 移動アニメを表示するか
        public bool MoveAnimation => Config.MoveAnimation;
        // 画像バッファの枚数
        public int ImageBufferSize => Config.ImageBufferSize;
        // 画像バッファの最大バイト数
        public int MaxImageBufferByteSize => Config.MaxImageBufferByteSize;
        // 拡大画像を画像バッファに保存するか
        public bool KeepStretchedImage => Config.KeepStretchedImage;
        // SRC.exeのある場所
        public string AppPath => Config.AppPath;

        /// <summary>
        /// ログを出さずに作る。ログの出力先はホストが <see cref="SRC(ILoggerFactory)"/> で渡す。
        /// </summary>
        public SRC()
            : this(NullLoggerFactory.Instance)
        {
        }

        public SRC(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
            Log = LoggerFactory.CreateLogger(LogCategories.Core);
            EventLog = LoggerFactory.CreateLogger(LogCategories.Event);
            ExpressionLog = LoggerFactory.CreateLogger(LogCategories.Expression);
            CommandLog = LoggerFactory.CreateLogger(LogCategories.Command);

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
            Log.Write(LogLevel.Error, ex, message, param);
        }

        public void LogWarn(Exception ex)
        {
            LogWarn("", ex);
        }
        public void LogWarn(string message, Exception ex, params string[] param)
        {
            Log.Write(LogLevel.Warning, ex, message, param);
        }

        public void LogInfo(string message, params string[] param)
        {
            Log.Write(LogLevel.Information, null, message, param);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void LogDebug(string message = "", params string[] param)
        {
            Log.WriteWithCaller(LogLevel.Debug, message, param);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void LogTrace(string message = "", params string[] param)
        {
            Log.WriteWithCaller(LogLevel.Trace, message, param);
        }
    }
}
