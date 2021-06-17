using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class GetOffCmd : CmdData
    {
        public GetOffCmd(SRC src, EventDataLine eventData) : base(src, CmdType.GetOffCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            Unit u;
            //            switch (ArgNum)
            //            {
            //                case 1:
            //                    {
            //                        u = Event.SelectedUnitForEvent;
            //                        break;
            //                    }

            //                case 2:
            //                    {
            //                        u = GetArgAsUnit(2, true);
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "GetOffコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 284690


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            if (u is object)
            //            {
            //                if (u.CountPilot() > 0)
            //                {
            //                    if (u.Status == "出撃")
            //                    {
            //                        // ユニットをマップ上から削除した状態で支援効果を更新
            //                        // UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //                        Map.MapDataForUnit[u.x, u.y] = null;
            //                        SRC.PList.UpdateSupportMod(u);
            //                    }

            //                    // パイロットを下ろす
            //                    u.Pilot(1).GetOff(true);
            //                    if (u.Status == "出撃")
            //                    {
            //                        // ユニットをマップ上に戻す
            //                        Map.MapDataForUnit[u.x, u.y] = u;
            //                    }
            //                }
            //            }

            //return EventData.NextID;
        }
    }
}
