using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class SetRelationCmd : CmdData
    {
        public SetRelationCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SetRelationCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            string pname1, pname2;
            int rel;
            switch (ArgNum)
            {
                case 3:
                    {
                        pname1 = Event.SelectedUnitForEvent.MainPilot().Name;
                        pname2 = GetArgAsString(2);
                        if (!SRC.PDList.IsDefined(pname2))
                        {
                            throw new EventErrorException(this, "キャラクター名が間違っています");
                        }
                        pname2 = SRC.PDList.Item(pname2).Name;

                        rel = GetArgAsLong(3);
                        break;
                    }

                case 4:
                    {
                        pname1 = GetArgAsString(2);
                        if (!SRC.PDList.IsDefined(pname1))
                        {
                            throw new EventErrorException(this, "キャラクター名が間違っています");
                        }
                        pname1 = SRC.PDList.Item(pname1).Name;

                        pname2 = GetArgAsString(3);
                        if (!SRC.PDList.IsDefined(pname2))
                        {
                            throw new EventErrorException(this, "キャラクター名が間違っています");
                        }
                        pname2 = SRC.PDList.Item(pname2).Name;

                        rel = GetArgAsLong(4);
                        break;
                    }

                default:
                    throw new EventErrorException(this, "SetRelationコマンドの引数の数が違います");
            }

           var vname = "関係:" + pname1 + ":" + pname2;
            if (rel != 0)
            {
                if (!Expression.IsGlobalVariableDefined(vname))
                {
                    Expression.DefineGlobalVariable(vname);
                }

                Expression.SetVariableAsLong(vname, rel);
            }
            else if (Expression.IsGlobalVariableDefined(vname))
            {
                Expression.UndefineVariable(vname);
            }

            // 信頼度補正による気力修正を更新
            if (Expression.IsOptionDefined("信頼度補正"))
            {
                if (SRC.PList.IsDefined(pname1))
                {
                    SRC.PList.Item(pname1).UpdateSupportMod();
                }

                if (SRC.PList.IsDefined(pname2))
                {
                    SRC.PList.Item(pname2).UpdateSupportMod();
                }
            }
            return EventData.NextID;
        }
    }
}
