using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;

namespace SRCCore.CmdDatas.Commands
{
    public class EnableCmd : CmdData
    {
        public EnableCmd(SRC src, EventDataLine eventData) : base(src, CmdType.EnableCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            var aname = "";
            var uname = "";
            var vname = "";
            switch (ArgNum)
            {
                case 2:
                    {
                        aname = GetArgAsString(2);
                        break;
                    }

                case 3:
                    {
                        uname = GetArgAsString(2);
                        aname = GetArgAsString(3);
                        break;
                    }

                default:
                    {
                        throw new EventErrorException(this, "Enableコマンドの引数の数が違います");
                    }
            }

            if (!string.IsNullOrEmpty(uname))
            {
                vname = "Disable(" + uname + "," + aname + ")";
            }
            else
            {
                vname = "Disable(" + aname + ")";
            }

            // Disable用変数を削除
            if (Expression.IsGlobalVariableDefined(vname))
            {
                Expression.UndefineVariable(vname);
            }
            else
            {
                // 既に設定済みであればそのまま終了
                return EventData.NextID;
            }

            // ユニットのステータスを更新
            if (!string.IsNullOrEmpty(uname))
            {
                if (SRC.UList.IsDefined(uname))
                {
                    SRC.UList.Item(uname).CurrentForm().Update();
                }
            }
            else
            {
                foreach (Unit u in SRC.UList.Items)
                {
                    if (u.Status == "出撃")
                    {
                        u.Update();
                    }
                }
            }
            return EventData.NextID;
        }
    }
}
