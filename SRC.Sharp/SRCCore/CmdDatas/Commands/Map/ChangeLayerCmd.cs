using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Maps;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class ChangeLayerCmd : CmdData
    {
        public ChangeLayerCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ChangeLayerCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 5 && ArgNum != 6)
            {
                throw new EventErrorException(this, "ChangeLayerコマンドの引数の数が違います");
            }

            int x = GetArgAsLong(2);
            if (x < 1 || x > Map.MapWidth)
            {
                throw new EventErrorException(this, "Ｘ座標の値は1～" + Map.MapWidth + "で指定してください");
            }

            int y = GetArgAsLong(3);
            if (y < 1 || y > Map.MapHeight)
            {
                throw new EventErrorException(this, "Ｙ座標の値は1～" + Map.MapHeight + "で指定してください");
            }

            // レイヤー地形名を取得
            var lname = GetArgAsString(4);
            if (Strings.Right(lname, 6) == "(ローカル)")
            {
                lname = Strings.Left(lname, Strings.Len(lname) - 6);
            }

            var td = SRC.TDList.ItemByName(lname);
            if (td == null)
            {
                throw new EventErrorException(this, "「" + lname + "」という地形は存在しません");
            }

            var cell = Map.CellAtPoint(x, y);
            cell.LayerType = td.ID;
            cell.UpperTerrain = td;
            cell.LayerBitmapNo = GetArgAsLong(5);

            // オプション取得
            if (ArgNum == 6)
            {
                var ltypename = GetArgAsString(6);
                switch (ltypename)
                {
                    case "通常":
                        cell.BoxType = BoxTypes.Upper;
                        break;
                    case "情報限定":
                        cell.BoxType = BoxTypes.UpperDataOnly;
                        break;
                    case "画像限定":
                        cell.BoxType = BoxTypes.UpperBmpOnly;
                        break;
                    default:
                        throw new EventErrorException(this, "ChangeLayerコマンドのOptionが不正です");
                }
            }
            else
            {
                cell.BoxType = BoxTypes.Upper;
            }

            // マップ背景を再描画
            GUI.SetupBackground(Map.MapDrawMode, "", Map.MapDrawFilterColor, Map.MapDrawFilterTransPercent);
            return EventData.NextID;
        }
    }
}
