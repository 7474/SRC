using SRCCore.Events;
using SRCCore.Exceptions;
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
            Unit u;
            int ux, uy;
            int tx, ty;
            string opt = "";
            int idx;
            if (!Information.IsNumeric(GetArgAsString(2)))
            {
                idx = 3;
                u = GetArgAsUnit(2);
            }
            else
            {
                idx = 2;
                u = Event.SelectedUnitForEvent;
            }

            tx = GetArgAsLong(idx);
            if (tx < 1)
            {
                tx = 1;
            }
            else if (tx > Map.MapWidth)
            {
                tx = Map.MapWidth;
            }

            idx = (idx + 1);
            ty = GetArgAsLong(idx);
            if (ty < 1)
            {
                ty = 1;
            }
            else if (ty > Map.MapHeight)
            {
                ty = Map.MapHeight;
            }

            idx = (idx + 1);
            if (idx <= ArgNum)
            {
                opt = GetArgAsString(idx);
            }

            switch (u.Status ?? "")
            {
                case "出撃":
                    if (Strings.InStr(opt, "アニメ表示") == 1)
                    {
                        // 現在位置を記録
                        ux = u.x;
                        uy = u.y;

                        // 目的地にユニットがいて入れない場合があるので
                        // 実際に移動させて到着地点を確かめる
                        u.Jump(tx, ty, false);
                        tx = u.x;
                        ty = u.y;

                        // 一旦元の位置に戻す
                        u.Jump(ux, uy, false);

                        // 移動アニメ表示
                        GUI.MoveUnitBitmap(u, ux, uy, tx, ty, 20);
                    }

                    u.Jump(tx, ty, false);
                    break;

                case "格納":
                    u.StandBy(tx, ty, opt);
                    break;

                default:
                    throw new EventErrorException(this, u.MainPilot().get_Nickname(false) + "は出撃していません");
            }

            if (string.IsNullOrEmpty(opt) || Strings.InStr(opt, "アニメ表示") == 1)
            {
                if (GUI.MainFormVisible && !GUI.IsPictureVisible)
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
                throw new EventErrorException(this, "Moveコマンドの引数の数が違います");
            }

            return EventData.NextID;
        }
    }
}
