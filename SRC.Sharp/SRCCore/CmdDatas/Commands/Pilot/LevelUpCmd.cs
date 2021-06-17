using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class LevelUpCmd : CmdData
    {
        public LevelUpCmd(SRC src, EventDataLine eventData) : base(src, CmdType.LevelUpCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            var p = default(Pilot);
            //            short num;
            //            double hp_ratio = default, en_ratio = default;
            //            switch (ArgNum)
            //            {
            //                case 3:
            //                    {
            //                        p = GetArgAsPilot(2);
            //                        num = GetArgAsLong(3);
            //                        break;
            //                    }

            //                case 2:
            //                    {
            //                        {
            //                            var withBlock = Event.SelectedUnitForEvent;
            //                            if (withBlock.CountPilot() > 0)
            //                            {
            //                                p = withBlock.Pilot((object)1);
            //                            }
            //                        }

            //                        num = GetArgAsLong(2);
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "LevelUpコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 306544


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            if (p is object)
            //            {
            //                if (p.Unit is object)
            //                {
            //                    {
            //                        var withBlock1 = p.Unit;
            //                        hp_ratio = 100 * withBlock1.HP / (double)withBlock1.MaxHP;
            //                        en_ratio = 100 * withBlock1.EN / (double)withBlock1.MaxEN;
            //                    }
            //                }

            //                if (Expression.IsOptionDefined("レベル限界突破"))
            //                {
            //                    p.Level = GeneralLib.MinLng(GeneralLib.MaxLng(p.Level + num, 1), 999);
            //                }
            //                else
            //                {
            //                    p.Level = GeneralLib.MinLng(GeneralLib.MaxLng(p.Level + num, 1), 99);
            //                }

            //                // 闘争本能入手？
            //                if (p.IsSkillAvailable("闘争本能"))
            //                {
            //                    if (p.MinMorale > 100)
            //                    {
            //                        if (p.Morale == p.MinMorale)
            //                        {
            //                            p.Morale = (p.MinMorale + 5d * p.SkillLevel("闘争本能", ref_mode: ""));
            //                        }
            //                    }
            //                    else if (p.Morale == 100)
            //                    {
            //                        p.Morale = (100d + 5d * p.SkillLevel("闘争本能", ref_mode: ""));
            //                    }
            //                }

            //                // ＳＰ＆霊力をアップデート
            //                p.SP = p.SP;
            //                p.Plana = p.Plana;
            //                if (p.Unit is object)
            //                {
            //                    {
            //                        var withBlock2 = p.Unit;
            //                        withBlock2.Update();
            //                        withBlock2.HP = (withBlock2.MaxHP * hp_ratio / 100d);
            //                        withBlock2.EN = (withBlock2.MaxEN * en_ratio / 100d);
            //                    }

            //                    SRC.PList.UpdateSupportMod(p.Unit);
            //                }
            //            }

            //return EventData.NextID;
        }
    }
}
