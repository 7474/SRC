using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;

namespace SRCCore.CmdDatas.Commands
{
    public class ForCmd : CmdData
    {
        public ForCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ForCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 6 & ArgNum != 8)
            {
                throw new EventErrorException(this, "Forコマンドの引数の数が違います");
            }

            // インデックス変数に初期値を設定
            var vname = GetArg(2);
            var idx = GetArgAsLong(4);
            Expression.SetVariableAsLong(vname, idx);

            // ループの終端値
            var limit = GetArgAsLong(6);

            // ArgNumが8かつ引数8が<0の場合、インデックスが減算されるループとして
            // ループ終了の条件式の不等号を逆にします
            // (idxおよびlimitの値に-1を乗算することで、擬似的に不等号を反対にしています)
            // ExecNextCmdでも同様の処理をしています
            var isincr = 1;
            if (ArgNum == 8)
            {
                if (GetArgAsLong(8) < 0)
                {
                    isincr = -1;
                }
            }


            if (idx * isincr <= limit * isincr)
            {
                // 終端値をスタックに格納
                Event.ForIndex = (Event.ForIndex + 1);
                Event.ForLimitStack[Event.ForIndex] = limit;
                // 初回のループを実行
                return EventData.NextID;
            }
            else
            {
                // 最初から条件式を満たしていない場合

                // 対応するNextコマンドを探す
                var depth = 1;
                foreach (var i in AfterEventIdRange())
                {
                    var cmd = Event.EventCmd[i];
                    switch (cmd.Name)
                    {
                        case CmdType.ForCmd:
                        case CmdType.ForEachCmd:
                            depth = (depth + 1);
                            break;

                        case CmdType.NextCmd:
                            depth = (depth - 1);
                            if (depth == 0)
                            {
                                return cmd.EventData.NextID;
                            }
                            break;
                    }
                }
            }
            throw new EventErrorException(this, "ForまたはForEachとNextが対応していません");
        }
    }
}
