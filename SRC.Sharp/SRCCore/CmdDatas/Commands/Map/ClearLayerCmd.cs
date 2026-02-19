using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Maps;

namespace SRCCore.CmdDatas.Commands
{
    public class ClearLayerCmd : CmdData
    {
        public ClearLayerCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ClearLayerCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum > 4)
            {
                throw new EventErrorException(this, "ClearLayerコマンドの引数の数が違います");
            }

            // 全体クリアかどうか
            bool isAllClear = ArgNum < 3;

            // オプション取得
            bool isDataOnly = false;
            bool isBitmapOnly = false;
            if (ArgNum == 2 || ArgNum == 4)
            {
                var loption = GetArgAsString(ArgNum == 2 ? 2 : 4);
                switch (loption)
                {
                    case "情報限定":
                        isDataOnly = true;
                        break;
                    case "画像限定":
                        isBitmapOnly = true;
                        break;
                    case "通常":
                        break;
                    default:
                        throw new EventErrorException(this, "ClearLayerコマンドの引数Optionが不正です");
                }
            }

            if (isAllClear)
            {
                // マップ全体のレイヤーをクリア
                for (int i = 1; i <= Map.MapWidth; i++)
                {
                    for (int j = 1; j <= Map.MapHeight; j++)
                    {
                        ClearCellLayer(Map.CellAtPoint(i, j), isDataOnly, isBitmapOnly);
                    }
                }
            }
            else
            {
                // 指定座標のレイヤーのみクリア
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

                ClearCellLayer(Map.CellAtPoint(x, y), isDataOnly, isBitmapOnly);
            }

            // マップ背景を再描画
            GUI.SetupBackground(Map.MapDrawMode, "", Map.MapDrawFilterColor, Map.MapDrawFilterTransPercent);
            return EventData.NextID;
        }

        private void ClearCellLayer(MapCell cell, bool isDataOnly, bool isBitmapOnly)
        {
            if (isDataOnly)
            {
                cell.BoxType = BoxTypes.UpperBmpOnly;
            }
            else if (isBitmapOnly)
            {
                cell.BoxType = BoxTypes.UpperDataOnly;
            }
            else
            {
                cell.LayerType = MapCell.NO_LAYER_NUM;
                cell.UpperTerrain = null;
                cell.LayerBitmapNo = MapCell.NO_LAYER_NUM;
                cell.BoxType = BoxTypes.Under;
            }
        }
    }
}
