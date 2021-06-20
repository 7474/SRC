using SRCCore.Events;
using SRCCore.Exceptions;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class ReplacePilotCmd : CmdData
    {
        public ReplacePilotCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ReplacePilotCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            bool is_support;
            if (ArgNum != 3)
            {
                throw new EventErrorException(this, "ReplacePilotの引数の数が違います");
            }

            var p1 = GetArgAsPilot(2);
            var pname = GetArgAsString(3);
            if (!SRC.PDList.IsDefined(pname))
            {
                throw new EventErrorException(this, "パイロット名が間違っています");
            }

            var p2 = SRC.PList.Add(pname, p1.Level, p1.Party, gid: "");
            p2.FullRecover();
            p2.Morale = p1.Morale;
            p2.Exp = p1.Exp;
            if (p2.Data.SP > 0 && p1.MaxSP > 0)
            {
                p2.SP = p2.MaxSP * p1.SP / p1.MaxSP;
            }

            if (p2.IsSkillAvailable("霊力"))
            {
                if (p1.IsSkillAvailable("霊力"))
                {
                    p2.Plana = p2.MaxPlana() * p1.Plana / p1.MaxPlana();
                }
            }

            // 乗せ換え
            if (p1.Unit is object)
            {
                var u = p1.Unit;
                // パイロットがサポートがどうか判定
                is_support = u.Supports.Any(x => x.ID == p1.ID);

                if (u.IsFeatureAvailable("追加サポート"))
                {
                    if (ReferenceEquals(u.AdditionalSupport(), p1))
                    {
                        is_support = true;
                    }
                }

                if (is_support)
                {
                    u.ReplaceSupport(p2, p1);
                }
                else
                {
                    u.ReplacePilot(p2, p1);
                }

                SRC.PList.UpdateSupportMod(p1.Unit);
            }

            p1.Alive = false;
            return EventData.NextID;
        }
    }
}
