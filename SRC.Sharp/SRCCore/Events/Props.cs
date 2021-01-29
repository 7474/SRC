using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using SRC.Core.CmdDatas;
using SRC.Core.Expressions;
using SRC.Core.Units;
using SRC.Core.VB;

namespace SRC.Core.Events
{
    public partial class Event
    {
        // イベントデータ
        public IList<EventDataLine> EventData;
        // イベントファイルのファイル名リスト
        public IList<string> EventFileNames;
        // Requireコマンドで追加されたイベントファイルのファイル名リスト
        public IList<string> AdditionalEventFileNames;

        // イベントコマンドリスト
        public IList<CmdData> EventCmd;

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
        public SrcCollection<LabelData> colEventLabelList = new SrcCollection<LabelData>();
        private SrcCollection<LabelData> colSysNormalLabelList = new SrcCollection<LabelData>();
        private SrcCollection<LabelData> colNormalLabelList = new SrcCollection<LabelData>();

        // 変数用のコレクション
        public IDictionary<string, VarData> GlobalVariableList = new Dictionary<string, VarData>();
        public IDictionary<string, VarData> LocalVariableList = new Dictionary<string, VarData>();

        // 現在の行番号
        public int CurrentLineNum;

        // イベントで選択されているユニット・ターゲット
        public Unit SelectedUnitForEvent;
        public Unit SelectedTargetForEvent;

        // イベント呼び出しのキュー
        public Queue<string> EventQue;
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
        public Stack<int> CallStack = new Stack<int>(MaxCallDepth);
        // 引数スタック
        public int ArgIndex;
        public Stack<int> ArgIndexStack = new Stack<int>(MaxCallDepth);
        public Stack<string> ArgStack = new Stack<string>(MaxArgIndex);
        // UpVarコマンドによって引数が何段階シフトしているか
        public int UpVarLevel;
        public Stack<int> UpVarLevelStack = new Stack<int>(MaxCallDepth);
        // サブルーチンローカル変数スタック
        public int VarIndex;
        public Stack<int> VarIndexStack = new Stack<int>(MaxCallDepth);
        public Stack<VarData> VarStack = new Stack<VarData>(MaxVarIndex);
        // Forインデックス用スタック
        public int ForIndex;
        public Stack<int> ForIndexStack = new Stack<int>(MaxCallDepth);
        public Stack<int> ForLimitStack = new Stack<int>(MaxCallDepth);

        // ForEachコマンド用変数
        public int ForEachIndex;
        public IList<string> ForEachSet;

        // Rideコマンド用パイロット搭乗履歴
        public string LastUnitName;
        public IList<string> LastPilotID;

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
        public Color ObjColor;
        // オブジェクトの線の太さ
        public int ObjDrawWidth;
        // オブジェクトの背景色
        public Color ObjFillColor;
        // オブジェクトの背景描画方法
        public HatchStyle ObjFillStyle;
        // オブジェクトの描画方法
        public string ObjDrawOption;

        // ホットポイント
        public IList<HotPoint> HotPointList;

        // イベントコマンドエラーメッセージ
        public string EventErrorMessage;

        // ユニットがセンタリングされたか？
        public bool IsUnitCenter;
    }
}
