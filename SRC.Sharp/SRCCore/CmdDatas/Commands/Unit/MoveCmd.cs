using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Pilots;
using SRCCore.Units;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class MoveCmd : CmdData
    {
        public MoveCmd(SRC src, EventDataLine eventData) : base(src, CmdType.MoveCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            int ExecMoveCmdRet = default;
            Unit u;
            short ux, uy;
            short tx, ty;
            var opt = default(string);
            short idx;
            if (!Information.IsNumeric(GetArgAsString(2)))
            {
                idx = 3;
                u = GetArgAsUnit(2);
            }
            else
            {
                idx = 2;
                u = Event_Renamed.SelectedUnitForEvent;
            }

            tx = (short)GetArgAsLong(idx);
            if (tx < 1)
            {
                tx = 1;
            }
            else if (tx > Map.MapWidth)
            {
                tx = Map.MapWidth;
            }

            idx = (short)(idx + 1);
            ty = (short)GetArgAsLong(idx);
            if (ty < 1)
            {
                ty = 1;
            }
            else if (ty > Map.MapHeight)
            {
                ty = Map.MapHeight;
            }

            idx = (short)(idx + 1);
            if (idx <= ArgNum)
            {
                opt = GetArgAsString(idx);
            }

            {
                var withBlock = u;
                switch (u.Status_Renamed ?? "")
                {
                    case "出撃":
                        {
                            if (Strings.InStr(opt, "アニメ表示") == 1)
                            {
                                // 現在位置を記録
                                ux = withBlock.x;
                                uy = withBlock.y;

                                // 目的地にユニットがいて入れない場合があるので
                                // 実際に移動させて到着地点を確かめる
                                withBlock.Jump(tx, ty, false);
                                tx = withBlock.x;
                                ty = withBlock.y;

                                // 一旦元の位置に戻す
                                withBlock.Jump(ux, uy, false);

                                // 移動アニメ表示
                                GUI.MoveUnitBitmap(ref u, ux, uy, tx, ty, 20);
                            }

                            withBlock.Jump(tx, ty, false);
                            break;
                        }

                    case "格納":
                        {
                            withBlock.StandBy(tx, ty, opt);
                            break;
                        }

                    default:
                        {
                            Event_Renamed.EventErrorMessage = withBlock.MainPilot().get_Nickname(false) + "は出撃していません";
                            ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 347546


                            Input:
                                                Error(0)

                             */
                            break;
                        }
                }
            }

            if (string.IsNullOrEmpty(opt) | Strings.InStr(opt, "アニメ表示") == 1)
            {
                if (GUI.MainForm.Visible & !GUI.IsPictureVisible)
                {
                    GUI.RedrawScreen();
                }
            }
            else if (opt == "非同期")
            {
            }
            // 画面更新しない
            else
            {
                Event_Renamed.EventErrorMessage = "Moveコマンドの引数の数が違います";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 347943


                Input:
                            Error(0)

                 */
            }

            return EventData.ID + 1;
        }
    }
}
