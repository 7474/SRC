using System;
using System.Collections.Generic;
using System.Text;
using SRC.Core.CmdDatas;
using SRC.Core.Expressions;
using SRC.Core.Units;

namespace SRC.Core.Events
{
    public partial class Event
    {
        // イベントデータ
        public string[] EventData;
        // イベントコマンドリスト
        public CmdData[] EventCmd;
        // 個々の行がどのイベントファイルに属しているか
        public int[] EventFileID;
        // 個々の行がイベントファイルの何行目に位置するか
        public int[] EventLineNum;
        // イベントファイルのファイル名リスト
        public string[] EventFileNames;
        // Requireコマンドで追加されたイベントファイルのファイル名リスト
        public string[] AdditionalEventFileNames;

        // システム側のイベントデータのサイズ(行数)
        private int SysEventDataSize;
        // システム側のイベントファイル数
        private int SysEventFileNum;
        // シナリオ添付のシステムファイルがチェックされたかどうか
        private bool ScenarioLibChecked;

        //// ラベルのリスト
        //public Collection colEventLabelList = new Collection();
        //private Collection colSysNormalLabelList = new Collection();
        //private Collection colNormalLabelList = new Collection();

        // 変数用のコレクション
        public IDictionary<string, VarData> GlobalVariableList = new Dictionary<string, VarData>();
        public IDictionary<string, VarData> LocalVariableList = new Dictionary<string, VarData>();

        // 現在の行番号
        public int CurrentLineNum;

        // イベントで選択されているユニット・ターゲット
        public Unit SelectedUnitForEvent;
        public Unit SelectedTargetForEvent;

        // イベント呼び出しのキュー
        public string[] EventQue;
        // 現在実行中のイベントラベル
        public int CurrentLabel;

        // Askコマンドで選択した選択肢
        public string SelectedAlternative;

        // 関数呼び出し用変数

        // 最大呼び出し階層数
        public const int MaxCallDepth = 50;
        // 引数の最大数
        public const int MaxArgIndex = 200;
        // サブルーチンローカル変数の最大数
        public const int MaxVarIndex = 2000;

        // 呼び出し履歴
        public int CallDepth;
        public int[] CallStack = new int[(MaxCallDepth + 1)];
        // 引数スタック
        public int ArgIndex;
        public int[] ArgIndexStack = new int[(MaxCallDepth + 1)];
        public string[] ArgStack = new string[(MaxArgIndex + 1)];
        // UpVarコマンドによって引数が何段階シフトしているか
        public int UpVarLevel;
        public int[] UpVarLevelStack = new int[(MaxCallDepth + 1)];
        // サブルーチンローカル変数スタック
        public int VarIndex;
        public int[] VarIndexStack = new int[(MaxCallDepth + 1)];
        public VarData[] VarStack = new VarData[(MaxVarIndex + 1)];
        // Forインデックス用スタック
        public int ForIndex;
        public int[] ForIndexStack = new int[(MaxCallDepth + 1)];
        public int[] ForLimitStack = new int[(MaxCallDepth + 1)];

        // ForEachコマンド用変数
        public int ForEachIndex;
        public string[] ForEachSet;

        // Rideコマンド用パイロット搭乗履歴
        public string LastUnitName;
        public string[] LastPilotID;

        // Wait開始時刻
        public int WaitStartTime;
        public int WaitTimeCount;

        // 描画基準座標
        public int BaseX;
        public int BaseY;
        private int[] SavedBaseX = new int[11];
        private int[] SavedBaseY = new int[11];
        private int BasePointIndex;

        // オブジェクトの色
        public int ObjColor;
        // オブジェクトの線の太さ
        public int ObjDrawWidth;
        // オブジェクトの背景色
        public int ObjFillColor;
        // オブジェクトの背景描画方法
        public int ObjFillStyle;
        // オブジェクトの描画方法
        public string ObjDrawOption;

        // ホットポイント
        public HotPoint[] HotPointList;

        // イベントコマンドエラーメッセージ
        public string EventErrorMessage;

        // ユニットがセンタリングされたか？
        public bool IsUnitCenter;
    }
}
