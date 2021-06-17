using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ExplodeCmd : CmdData
    {
        public ExplodeCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ExplodeCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            string esize;
            //            short tx, ty;
            //            switch (ArgNum)
            //            {
            //                case 2:
            //                    {
            //                        esize = GetArgAsString(2);
            //                        tx = GUI.MapX;
            //                        ty = GUI.MapY;
            //                        break;
            //                    }

            //                case 4:
            //                    {
            //                        esize = GetArgAsString(2);
            //                        tx = GetArgAsLong(3);
            //                        ty = GetArgAsLong(4);
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "Explodeコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 235613


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            // 爆発の表示
            //            Effect.ExplodeAnimation(esize, tx, ty);
            //return EventData.NextID;
        }
    }
}
