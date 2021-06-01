using SRCCore.CmdDatas;
using SRCCore.Expressions;
using SRCCore.Units;
using SRCCore.VB;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace SRCCore.Events
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
        public SrcCollection<LabelData> colEventLabelList = new SrcCollection<LabelData>();
        private SrcCollection<LabelData> colSysNormalLabelList = new SrcCollection<LabelData>();
        private SrcCollection<LabelData> colNormalLabelList = new SrcCollection<LabelData>();

        // 変数用のコレクション
        // XXX 列挙時の順番がDictionaryだと問題になるかも
        public IDictionary<string, VarData> GlobalVariableList = new Dictionary<string, VarData>();
        public IDictionary<string, VarData> LocalVariableList = new Dictionary<string, VarData>();
        public BCVariable BCVariable = new BCVariable();

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
        public IList<int> CallStack = new List<int>(Enumerable.Range(0, MaxCallDepth).Select(x => 0));
        // 引数スタック
        public int ArgIndex;
        public IList<int> ArgIndexStack = new List<int>(Enumerable.Range(0, MaxCallDepth).Select(x => 0));
        public IList<string> ArgStack = new List<string>(Enumerable.Range(0, MaxArgIndex).Select(x => ""));
        // UpVarコマンドによって引数が何段階シフトしているか
        public int UpVarLevel;
        public IList<int> UpVarLevelStack = new List<int>(Enumerable.Range(0, MaxCallDepth).Select(x => 0));
        // サブルーチンローカル変数スタック
        public int VarIndex;
        public IList<int> VarIndexStack = new List<int>(Enumerable.Range(0, MaxCallDepth).Select(x => 0));
        public IList<VarData> VarStack = new List<VarData>(Enumerable.Range(0, MaxVarIndex).Select(x => new VarData()));
        // Forインデックス用スタック
        public int ForIndex;
        public IList<int> ForIndexStack = new List<int>(Enumerable.Range(0, MaxCallDepth).Select(x => 0));
        public IList<int> ForLimitStack = new List<int>(Enumerable.Range(0, MaxCallDepth).Select(x => 0));

        // ForEachコマンド用変数
        public int ForEachIndex;
        public IList<string> ForEachSet;

        // Rideコマンド用パイロット搭乗履歴
        public string LastUnitName;
        public IList<string> LastPilotID = new List<string>();

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
        public FillStyle ObjFillStyle;
        // オブジェクトの描画方法
        public string ObjDrawOption;

        // ホットポイント
        public IList<HotPoint> HotPointList;

        // ユニットがセンタリングされたか？
        public bool IsUnitCenter;

        /// <summary>
        /// 真なら他のファイルを読み込まない。
        /// Lint目的などで指定する。
        /// </summary>
        public bool SkipExternalSourceLoad { get; set; }
    }
}
