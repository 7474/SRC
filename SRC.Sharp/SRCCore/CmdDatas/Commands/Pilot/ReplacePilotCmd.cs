using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ReplacePilotCmd : CmdData
    {
        public ReplacePilotCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ReplacePilotCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            string pname;
            //            Pilot p1, p2;
            //            short i;
            //            bool is_support;
            //            if (ArgNum != 3)
            //            {
            //                Event.EventErrorMessage = "ReplacePilotの引数の数が違います";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 433545


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            p1 = GetArgAsPilot(2);
            //            pname = GetArgAsString(3);
            //            bool localIsDefined() { object argIndex1 = pname; var ret = SRC.PDList.IsDefined(argIndex1); return ret; }

            //            if (!localIsDefined())
            //            {
            //                Event.EventErrorMessage = "パイロット名が間違っています";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 433766


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            p2 = SRC.PList.Add(pname, p1.Level, p1.Party, gid: "");
            //            {
            //                var withBlock = p2;
            //                withBlock.FullRecover();
            //                withBlock.Morale = p1.Morale;
            //                withBlock.Exp = p1.Exp;
            //                if (withBlock.Data.SP > 0 && p1.MaxSP > 0)
            //                {
            //                    withBlock.SP = withBlock.MaxSP * p1.SP / p1.MaxSP;
            //                }

            //                if (withBlock.IsSkillAvailable("霊力"))
            //                {
            //                    if (p1.IsSkillAvailable("霊力"))
            //                    {
            //                        withBlock.Plana = withBlock.MaxPlana() * p1.Plana / p1.MaxPlana();
            //                    }
            //                }
            //            }

            //            // 乗せ換え
            //            if (p1.Unit is object)
            //            {
            //                {
            //                    var withBlock1 = p1.Unit;
            //                    // パイロットがサポートがどうか判定
            //                    is_support = false;
            //                    var loopTo = withBlock1.CountSupport();
            //                    for (i = 1; i <= loopTo; i++)
            //                    {
            //                        if (ReferenceEquals(withBlock1.Support(i), p1))
            //                        {
            //                            is_support = true;
            //                            break;
            //                        }
            //                    }

            //                    if (withBlock1.IsFeatureAvailable("追加サポート"))
            //                    {
            //                        if (ReferenceEquals(withBlock1.AdditionalSupport(), p1))
            //                        {
            //                            is_support = true;
            //                        }
            //                    }

            //                    if (is_support)
            //                    {
            //                        withBlock1.ReplaceSupport(p2, (object)p1.ID);
            //                    }
            //                    else
            //                    {
            //                        withBlock1.ReplacePilot(p2, (object)p1.ID);
            //                    }
            //                }

            //                SRC.PList.UpdateSupportMod(p1.Unit);
            //            }

            //            p1.Alive = false;
            //return EventData.NextID;
        }
    }
}
