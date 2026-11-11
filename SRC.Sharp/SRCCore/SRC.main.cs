using Microsoft.Extensions.Logging;
using SRCCore.Config;
using SRCCore.Exceptions;
using SRCCore.Filesystem;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SRCCore
{
    public partial class SRC
    {
        /// <summary>
        /// fname（シナリオのイベントファイルかセーブデータ）を実行する。
        /// scenarioPath はシナリオのフォルダ（Data・Lib・Continue の基準）。省略すると fname のあるフォルダ（元の SRC と同じ）。
        /// サブフォルダに置いたイベントファイルを、シナリオのフォルダを基準に動かすのに使う。
        /// </summary>
        public void Execute(string fname, string scenarioPath = null)
        {
            // SRCが正しくインストールされているかをチェック
            ValidateEnvironment();

            // メインウィンドウのロードとFlashの登録を実施
            GUI.LoadMainFormAndRegisterFlash();

            // タイトル画面を表示
            GUI.OpenTitleForm();

            ConfigureSound();

            ConfigureExecuteFile(fname, scenarioPath);

            // Src.iniから各種パラメータの読み込み
            LoadIni();

            // マップを初期化
            Map.InitMap();

            // 元の SRC は起動のたびに乱数の種を変える（Randomize の後の Rnd）。自動のプレイで結果を固定するときは InitialRndSeed で決める
            GeneralLib.RndSeed = InitialRndSeed ?? new Random().Next(1000000);
            GeneralLib.RndReset();

            ExecuteFile(fname);
        }

        /// <summary>
        /// 起動のときの乱数の種。null（既定）なら元の SRC と同じく起動のたびに変える。
        /// 自動のプレイ（srcsharp play --seed）で、同じ手順なら同じ結果になるようにするのに使う
        /// </summary>
        public int? InitialRndSeed { get; set; }

        private void ValidateEnvironment()
        {
            if (AllowMissingAssets)
            {
                WarnMissingAssets();
                return;
            }

            // Bitmap関係のチェック
            if (!FileSystem.GetFileSystemEntries(AppPath, "Bitmap", EntryOption.Directory).Any())
            {
                GUI.ErrorMessage("Bitmapフォルダがありません。" + Environment.NewLine + "SRC.exeと同じフォルダに汎用グラフィック集をインストールしてください。");
                TerminateSRC();
                return;
            }

            if (FileSystem.GetFileSystemEntries(AppPath, "Ｂｉｔｍａｐ", EntryOption.Directory).Any())
            {
                GUI.ErrorMessage("Bitmapフォルダのフォルダ名が全角文字になっています。" + Environment.NewLine + FileSystem.PathCombine(AppPath, "Ｂｉｔｍａｐ") + Environment.NewLine + "フォルダ名を半角文字に直してください。");
                TerminateSRC();
                return;
            }

            if (FileSystem.GetFileSystemEntries(FileSystem.PathCombine(AppPath, "Bitmap"), "Bitmap", EntryOption.Directory).Any())
            {
                GUI.ErrorMessage("Bitmapフォルダ内にさらにBitmapフォルダが存在します。" + Environment.NewLine + FileSystem.PathCombine(AppPath, "Bitmap", "Bitmap") + Environment.NewLine + "フォルダ構造を直してください。");
                TerminateSRC();
                return;
            }

            // イベントグラフィック
            if (!FileSystem.GetFileSystemEntries(FileSystem.PathCombine(AppPath, "Bitmap"), "Event", EntryOption.Directory).Any())
            {
                GUI.ErrorMessage("Bitmap\\Eventフォルダが見つかりません。" + Environment.NewLine + "汎用グラフィック集が正しくインストールされていないと思われます。");
                TerminateSRC();
                return;
            }

            // マップグラフィック
            if (!FileSystem.GetFileSystemEntries(FileSystem.PathCombine(AppPath, "Bitmap"), "Map", EntryOption.Directory).Any())
            {
                GUI.ErrorMessage("Bitmap\\Mapフォルダがありません。" + Environment.NewLine + "汎用グラフィック集が正しくインストールされていないと思われます。");
                TerminateSRC();
                return;
            }

            if (!FileSystem.FileExists(AppPath, "Bitmap", "Map", "plain", "plain0000.bmp")
                && !FileSystem.FileExists(AppPath, "Bitmap", "Map", "plain0000.bmp")
                && !FileSystem.FileExists(AppPath, "Bitmap", "Map", "plain0.bmp"))
            {
                if (FileSystem.GetFileSystemEntries(FileSystem.PathCombine(AppPath, "Bitmap", "Map"), "Map", EntryOption.Directory).Any())
                {
                    GUI.ErrorMessage("Bitmap\\Mapフォルダ内にさらにMapフォルダが存在します。" + Environment.NewLine + FileSystem.PathCombine(AppPath, "Bitmap", "Map", "Map") + Environment.NewLine + "フォルダ構造を直してください。");
                    TerminateSRC();
                    return;
                }

                if (!FileSystem.GetFileSystemEntries(FileSystem.PathCombine(AppPath, "Bitmap", "Map"), "*", EntryOption.File).Any())
                {
                    GUI.ErrorMessage("Bitmap\\Mapフォルダ内にファイルがありません。" + Environment.NewLine + "汎用グラフィック集が正しくインストールされていないと思われます。");
                    TerminateSRC();
                    return;
                }

                GUI.ErrorMessage("Bitmap\\Mapフォルダ内にplain0000.bmpがありません。" + Environment.NewLine + "一部のマップ画像ファイルしかインストールされていない恐れがあります。" + Environment.NewLine + "新規インストールのファイルを使って汎用グラフィック集をインストールしてください。");
                TerminateSRC();
                return;
            }

            // 効果音
            if (!FileSystem.GetFileSystemEntries(AppPath, "Sound", EntryOption.Directory).Any())
            {
                GUI.ErrorMessage("Soundフォルダがありません。" + Environment.NewLine + "SRC.exeと同じフォルダに効果音集をインストールしてください。");
                TerminateSRC();
                return;
            }

            if (FileSystem.GetFileSystemEntries(AppPath, "Ｓｏｕｎｄ", EntryOption.Directory).Any())
            {
                GUI.ErrorMessage("Soundフォルダのフォルダ名が全角文字になっています。" + Environment.NewLine + FileSystem.PathCombine(AppPath, "Ｓｏｕｎｄ") + Environment.NewLine + "フォルダ名を半角文字に直してください。");
                TerminateSRC();
                return;
            }

            if (FileSystem.GetFileSystemEntries(FileSystem.PathCombine(AppPath, "Sound"), "Sound", EntryOption.Directory).Any())
            {
                GUI.ErrorMessage("Soundフォルダ内にさらにSoundフォルダが存在します。" + Environment.NewLine + FileSystem.PathCombine(AppPath, "Sound", "Sound") + Environment.NewLine + "フォルダ構造を直してください。");
                TerminateSRC();
                return;
            }

            if (!FileSystem.GetFileSystemEntries(FileSystem.PathCombine(AppPath, "Sound"), "*", EntryOption.File).Any())
            {
                GUI.ErrorMessage("Soundフォルダ内にファイルがありません。" + Environment.NewLine + "Soundフォルダ内に効果音集をインストールしてください。");
                TerminateSRC();
                return;
            }
        }

        // 素材の不足をログに残す（AllowMissingAssets の場合）
        private void WarnMissingAssets()
        {
            var checks = new[]
            {
                (Path: FileSystem.PathCombine(AppPath, "Bitmap", "Map"), Name: "汎用グラフィック集（Bitmap\\Map）"),
                (Path: FileSystem.PathCombine(AppPath, "Bitmap", "Anime"), Name: "汎用戦闘アニメ画像（Bitmap\\Anime）"),
                (Path: FileSystem.PathCombine(AppPath, "Sound"), Name: "効果音集（Sound）"),
            };
            foreach (var check in checks)
            {
                var parent = System.IO.Path.GetDirectoryName(check.Path);
                var name = System.IO.Path.GetFileName(check.Path);
                if (!FileSystem.GetFileSystemEntries(parent, name, EntryOption.Directory).Any())
                {
                    LogInfo(check.Name + "が見つかりません。素材なしで実行します");
                }
            }
        }

        private void ConfigureSound()
        {
            // サウンド設定は IPlayer/ISystemConfig の抽象化で管理する。
            // VB5版の DirectSound/DirectMusic 初期化はプラットフォーム依存のため
            // WindowsManagedPlayer 等の実装クラスに委ねる。
        }

        /// <summary>
        /// シナリオのフォルダの設定ファイル（Src.ini・srcs.json）を読む。
        /// 元の SRC の ReadIni と同じく、そこにある項目はアプリの設定より優先する（<see cref="Config"/>）
        /// </summary>
        public void LoadScenarioConfig()
        {
            // シナリオのフォルダがアプリのフォルダなら、そこの srcs.json はアプリの設定そのもの
            if (string.IsNullOrEmpty(ScenarioPath) || (!string.IsNullOrEmpty(AppPath) && FileSystem.PathEquals(ScenarioPath, AppPath)))
            {
                ScenarioConfig = null;
                return;
            }
            try
            {
                ScenarioConfig = SRCCore.Config.ScenarioConfig.Read(FileSystem, ScenarioPath);
            }
            catch (Exception ex)
            {
                ScenarioConfig = null;
                Log.LogWarning(ex, "シナリオの設定ファイルを読めませんでした。");
            }
            if (ScenarioConfig != null)
            {
                Log.LogInformation("シナリオの設定ファイルを読みました: {Files}", string.Join(", ", ScenarioConfig.Files));
            }
        }

        private void LoadIni()
        {
            // シナリオで音量を決めていれば、その音量にする
            if (Config is ComposedConfig composed && composed.IsScenarioProperty(nameof(LocalFileConfig.SoundVolume)))
            {
                Sound.Player.SoundVolume = composed.SoundVolume / 100f;
            }

            // メッセージ速度を設定（元の SRC の Src.ini の [Option] MessageWait）
            GUI.MessageWait = SRCCore.Config.MessageSpeeds.Normalize(Config.MessageWait);

            // 各ウィンドウをロード (メインウィンドウは先にロード済み)
            GUI.LoadForms();

            // エリアスデータをロード（元の SRC と同じく、シナリオの Data\System を優先する）7474/SRC#1425
            var aliasFiles = new[] { FileSystem.PathCombine(ScenarioPath, "Data", "System", "alias.txt") }
                .Concat(SystemDataFiles("alias.txt"));
            foreach (var aliasFile in aliasFiles)
            {
                if (FileSystem.FileExists(aliasFile))
                {
                    ALDList.Load(aliasFile);
                    break;
                }
            }

            // スペシャルパワーデータをロード
            var spFiles = new string[]
            {
                FileSystem.PathCombine(ScenarioPath, "Data", "System", "sp.txt"),
                FileSystem.PathCombine(ScenarioPath, "Data", "System", "mind.txt"),
            }.Concat(SystemDataRoots().SelectMany(root => new[]
            {
                FileSystem.PathCombine(root, "Data", "System", "sp.txt"),
                FileSystem.PathCombine(root, "Data", "System", "mind.txt"),
            }));
            foreach (var spFile in spFiles)
            {
                if (FileSystem.FileExists(spFile))
                {
                    SPDList.Load(spFile);
                    break;
                }
            }

            // 汎用アイテムデータをロード
            if (FileSystem.FileExists(ScenarioPath, @"Data\System\item.txt"))
            {
                IDList.Load(FileSystem.PathCombine(ScenarioPath, @"Data\System\item.txt"));
            }
            else if (SystemDataFiles("item.txt").FirstOrDefault(x => FileSystem.FileExists(x)) is string itemFile)
            {
                IDList.Load(itemFile);
            }
            // 地形データをロード
            string appTerrainPath = SystemDataFiles("terrain.txt").FirstOrDefault(x => FileSystem.FileExists(x));
            if (appTerrainPath != null)
            {
                TDList.Load(appTerrainPath);
            }
            else if (AllowMissingAssets)
            {
                LogInfo(@"地形データファイル「Data\System\terrain.txt」が見つかりません。マップの地形は仮の地形として読み込みます");
            }
            else
            {
                throw new TerminateException(@"地形データファイル「Data\System\terrain.txt」が見つかりません");
            }
            string scenarioTerrainPath = FileSystem.PathCombine(ScenarioPath, "Data", "System", "terrain.txt");
            if (FileSystem.FileExists(scenarioTerrainPath))
            {
                TDList.Load(scenarioTerrainPath);
            }

            // バトルコンフィグデータをロード
            var bcFiles = new string[]
            {
                FileSystem.PathCombine(ScenarioPath, "Data", "System", "battle.txt"),
            }.Concat(SystemDataFiles("battle.txt"));
            foreach (var bcFile in bcFiles)
            {
                if (FileSystem.FileExists(bcFile))
                {
                    BCList.Load(bcFile);
                    break;
                }
            }
        }

        /// <summary>
        /// SRC のシステムデータ（Data\System）を探すフォルダ。元の SRC は SRC のフォルダ（AppPath）だけを見る。
        /// SRC# は SRC 本体と別のフォルダに置き、拡張データフォルダ（ExtDataPath・ExtDataPath2）で SRC 本体を指すことがあるため、
        /// AppPath に無ければ拡張データフォルダも見る
        /// </summary>
        internal IEnumerable<string> SystemDataRoots()
        {
            return new[] { AppPath, ExtDataPath, ExtDataPath2 }
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct();
        }

        /// <summary>SystemDataRoots の Data\System\name（探す順）</summary>
        internal IEnumerable<string> SystemDataFiles(string name)
        {
            return SystemDataRoots().Select(root => FileSystem.PathCombine(root, "Data", "System", name));
        }

        private void ConfigureExecuteFile(string fname, string scenarioPath)
        {
            // ファイルは GUI 側で選んでから呼ばれる（元の SRC のダイアログで選ぶ分岐は要らない）

            // ファイル名が無効の場合はそのまま終了
            if (string.IsNullOrEmpty(fname))
            {
                TerminateSRC();
                Environment.Exit(0);
            }

            // シナリオのあるフォルダのパスを収得
            ScenarioPath = string.IsNullOrEmpty(scenarioPath) ? Path.GetDirectoryName(fname) : scenarioPath;

            // シナリオのフォルダの設定ファイル（Src.ini・srcs.json）を読む
            LoadScenarioConfig();

            // オープニング曲演奏
            Sound.StopBGM(true);
            Sound.StartBGM(Sound.BGMName("Opening"), true);
            Event.InitEventData();
            GUI.CloseTitleForm();

            // 元の SRC はここでロングネームにしてから、ファイルがあるかを確かめる
            // （セーブデータは FileSystem を通さずに開くので、FileSystem の外のファイルも認める）
            if (!FileSystem.FileExists(fname) && !File.Exists(fname))
            {
                GUI.ErrorMessage("指定したファイルが存在しません");
                TerminateSRC();
            }
        }

        private void ExecuteFile(string fname)
        {
            if (Strings.LCase(Strings.Right(fname, 5)) == ".srcs")
            {
                using (var stream = File.OpenRead(fname))
                {
                    // セーブデータの読み込み
                    GUI.OpenNowLoadingForm();
                    LoadData(stream);
                    GUI.CloseNowLoadingForm();

                    // インターミッション
                    InterMission.InterMissionCommand(true);
                    if (!IsSubStage)
                    {
                        if (string.IsNullOrEmpty(Expression.GetValueAsString("次ステージ")))
                        {
                            GUI.ErrorMessage("次のステージのファイル名が設定されていません");
                            TerminateSRC();
                        }

                        StartScenario(Expression.GetValueAsString("次ステージ"));
                    }
                    else
                    {
                        IsSubStage = false;
                    }
                }
            }
            else if (Strings.LCase(Strings.Right(fname, 5)) == ".srcq")
            {
                // 中断データの読み込み
                GUI.LockGUI();
                RestoreData(fname, SRCSaveKind.Suspend);

                // 画面を書き直してステータスを表示
                GUI.RedrawScreen();
                GUIStatus.DisplayGlobalStatus();
                GUI.UnlockGUI();
            }
            else if (Strings.LCase(Strings.Right(fname, 4)) == ".eve")
            {
                // イベントファイルの実行
                StartScenario(fname);
            }
            else
            {
                GUI.ErrorMessage("「" + fname + "」はSRC用のファイルではありません！" + Constants.vbCr + Constants.vbLf + "拡張子が「.eve」のイベントファイル、" + "拡張子が「.srcs」のセーブデータファイル、" + "または拡張子が「.srcq」の中断データファイルを指定して下さい。");
                TerminateSRC();
            }
        }
    }
}
