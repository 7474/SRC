using SRCCore.Events;
using SRCCore.Exceptions;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class CallCmd : CmdData
    {
        public CallCmd(SRC src, EventDataLine eventData) : base(src, CmdType.CallCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            // サブルーチンを探す
            string subName = GetArgAsString(2);
            var ret = Event.FindNormalLabel(subName);

            // 見つかった？
            if (ret < 0)
            {
                // TODO コマンド実装し終わったら。。。
                //throw new EventErrorException(this, "サブルーチンの呼び出し先ラベルである「" + subName + "」がみつかりません");
                SRC.LogDebug($"{subName} is not found.");
                return EventData.ID + 1;
            }

            // 呼び出し階層をチェック
            if (Event.CallDepth >= Event.MaxCallDepth)
            {
                Event.CallDepth = Event.MaxCallDepth;
                throw new EventErrorException(this, Event.MaxCallDepth + "階層を越えるサブルーチンの呼び出しは出来ません");
            }

            // 引数用スタックが溢れないかチェック
            if (Event.ArgIndex + ArgNum - 2 >= Event.MaxArgIndex)
            {
                throw new EventErrorException(this, "サブルーチンの引数の総数が" + Event.MaxArgIndex + "個を超えています");
            }

            // 引数の値を先に求めておく
            // (スタックに積みながら計算すると、引数での関数呼び出しで不正になる)
            var subParams = ArgNum >= 3
                ? Enumerable.Range(3, ArgNum - 3).Select(x => GetArgAsString(x)).ToArray()
                : new string[] { };

            // 現在の状態を保存
            Event.CallStack[Event.CallDepth] = EventData.ID;
            Event.ArgIndexStack[Event.CallDepth] = Event.ArgIndex;
            Event.VarIndexStack[Event.CallDepth] = Event.VarIndex;
            Event.ForIndexStack[Event.CallDepth] = Event.ForIndex;

            // UpVarが実行された場合、UpVar実行数は累計する
            if (Event.UpVarLevel > 0)
            {
                Event.UpVarLevelStack[Event.CallDepth] = (Event.UpVarLevel + Event.UpVarLevelStack[Event.CallDepth - 1]);
            }
            else
            {
                Event.UpVarLevelStack[Event.CallDepth] = 0;
            }

            // UpVarの階層数を初期化
            Event.UpVarLevel = 0;

            // 引数をスタックに積む
            for (var i = 0; i < subParams.Length; i++)
            {
                Event.ArgStack[Event.ArgIndex + i + 1] = subParams[i];
            }

            Event.ArgIndex = Event.ArgIndex + subParams.Length;

            // 呼び出し階層数をインクリメント
            Event.CallDepth = Event.CallDepth + 1;
            return ret + 1;
        }
    }
}
