using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;

namespace SRCCore.CmdDatas.Commands
{
    public class NextCmd : CmdData
    {
        public NextCmd(SRC src, EventDataLine eventData) : base(src, CmdType.NextCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            // 対応するForまたはForEachを探す
            var depth = 1;
            foreach (var i in BeforeEventIdRange())
            {
                var cmd = Event.EventCmd[i];
                switch (cmd.Name)
                {
                    case CmdType.ForCmd:
                        depth = (depth - 1);
                        if (depth == 0)
                        {
                            // インデックス変数の値を1増やす
                            var vname = cmd.GetArg(2);
                            double idx;

                            // Step句が設定されている場合、インデックス変数に引数8の値を加算
                            if (cmd.ArgNum == 6)
                            {
                                idx = Expression.GetValueAsDouble(vname, true) + 1d;
                            }
                            else
                            {
                                idx = Expression.GetValueAsDouble(vname, true) + cmd.GetArgAsLong(8);
                            }

                            Expression.SetVariableAsDouble(vname, idx);

                            // インデックス変数の値は範囲内？
                            var isincr = 1;
                            if (cmd.ArgNum == 8)
                            {
                                if (cmd.GetArgAsLong(8) < 0)
                                {
                                    isincr = -1;
                                }
                            }

                            if (idx * isincr > Event.ForLimitStack[Event.ForIndex] * isincr)
                            {
                                // ループ終了
                                Event.ForIndex = (Event.ForIndex - 1);
                                return EventData.NextID;
                            }

                            // ループ継続
                            return cmd.EventData.NextID;
                        }
                        break;

                    case CmdType.ForEachCmd:
                        depth = (depth - 1);
                        if (depth == 0)
                        {
                            // XXX Index再考するならする
                            Event.ForEachIndex = (Event.ForEachIndex + 1);
                            if (Event.ForEachIndex > Event.ForEachSet.Count)
                            {
                                // ループ終了
                                Event.ForIndex = (Event.ForIndex - 1);
                                return EventData.NextID;
                            }
                            else if (cmd.ArgNum < 4)
                            {
                                // ユニット＆パイロットに対するForEach
                                Event.SelectedUnitForEvent = SRC.UList.Item(Event.ForEachSet[Event.ForEachIndex - 1]);
                            }
                            else
                            {
                                // 配列に対するForEach
                                Expression.SetVariableAsString(cmd.GetArg(2), Event.ForEachSet[Event.ForEachIndex - 1]);
                            }

                            // ループ継続
                            return cmd.EventData.NextID;
                        }
                        break;

                    case CmdType.NextCmd:
                        depth = (depth + 1);
                        break;
                }
            }

            throw new EventErrorException(this, "ForまたはForEachとNextが対応していません");
        }
    }
}
