using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class DisableCmd : CmdData
    {
        public DisableCmd(SRC src, EventDataLine eventData) : base(src, CmdType.DisableCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            string aname;
            string uname = null;
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
                    throw new EventErrorException(this, "Disableコマンドの引数の数が違います");
            }

            if (string.IsNullOrEmpty(aname))
            {
                throw new EventErrorException(this, "Disableコマンドに指定された能力名が空文字列です");
            }

            string vname;
            if (!string.IsNullOrEmpty(uname))
            {
                vname = "Disable(" + uname + "," + aname + ")";
            }
            else
            {
                vname = "Disable(" + aname + ")";
            }

            // Disable用変数を設定
            if (!Expression.IsGlobalVariableDefined(vname))
            {
                Expression.DefineGlobalVariable(vname);
                Expression.SetVariableAsLong(vname, 1);
            }
            else
            {
                // 既に設定済みであればそのまま終了
                return EventData.NextID;
            }

            // ユニットのステータスを更新
            if (!string.IsNullOrEmpty(uname))
            {
                {
                    var withBlock = SRC.UList;
                    if (withBlock.IsDefined(uname))
                    {
                        withBlock.Item(uname).CurrentForm().Update();
                    }
                }
            }
            else
            {
                foreach (Unit u in SRC.UList.Items)
                {
                    if (u.Status == "出撃")
                    {
                        // ステータスを更新する必要があるかどうかチェックする
                    var    need_update = false;
                        if (u.IsFeatureAvailable(aname))
                        {
                            need_update = true;
                        }
                        else
                        {
                            need_update = u.ItemList.Any(x => x.Name == aname);
                        }

                        // 必要がある場合はステータスを更新
                        if (need_update)
                        {
                            u.Update();
                        }
                    }
                }
            }

            return EventData.NextID;
        }
    }
}
