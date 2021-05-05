using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Models;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class ChangeTerrainCmd : CmdData
    {
        public ChangeTerrainCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ChangeTerrainCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 5)
            {
                throw new EventErrorException(this, "ChangeTerrainコマンドの引数の数が違います");
            }

            int tx, ty;
            tx = GetArgAsLong(2);
            if (tx < 1 || tx > Map.MapWidth)
            {
                throw new EventErrorException(this, "Ｘ座標の値は1～" + Map.MapWidth + "で指定してください");
            }

            ty = GetArgAsLong(3);
            if (ty < 1 || ty > Map.MapHeight)
            {
                throw new EventErrorException(this, "Ｙ座標の値は1～" + Map.MapHeight + "で指定してください");
            }

            var cell = Map.CellAtPoint(tx, ty);
            var tname = GetArgAsString(4);
            TerrainData td;
            if (Strings.Right(tname, 6) != "(ローカル)")
            {
                td = SRC.TDList.ItemByName(tname);
                if (td == null)
                {
                    throw new EventErrorException(this, "「" + tname + "」という地形は存在しません");
                }
                cell.UnderTerrain = td;
                cell.BitmapNo = GetArgAsLong(5);
            }
            else
            {
                tname = Strings.Left(tname, Strings.Len(tname) - 6);
                td = SRC.TDList.ItemByName(tname);
                if (td == null)
                {
                    throw new EventErrorException(this, "「" + tname + "」という地形は存在しません");
                }
                cell.UnderTerrain = td;
                cell.BitmapNo = -GetArgAsLong(5);
            }

            // マップ画像を検索
            var fname = Map.SearchTerrainImageFile(cell);
            if (string.IsNullOrEmpty(fname))
            {
                // XXX Bitmap名
                throw new EventErrorException(this, "マップビットマップ「"
                    + "XXX" +
                    "」が見つかりません");
            }

            // XXX 地形変更するときは一気に行うだろうから全書き換えは不利かもしれない
            GUI.SetupBackground(Map.MapDrawMode, "", Map.MapDrawFilterColor, Map.MapDrawFilterTransPercent);
            return EventData.NextID;
        }
    }
}
