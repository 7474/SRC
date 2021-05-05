using SRCCore.Events;
using SRCCore.Exceptions;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ChangeTerrainCmd : CmdData
    {
        public ChangeTerrainCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ChangeTerrainCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            int ExecChangeTerrainCmdRet = default;
            var B = default(object);
            short tx, ty;
            string tname;
            short tid = default, tbitmap;
            string fname2, fname, fname1, fname3;
            int ret;
            short i;
            if (ArgNum != 5)
            {
                throw new EventErrorException(this, "ChangeTerrainコマンドの引数の数が違います");
            }

            tx = GetArgAsLong(2);
            if (tx < 1 || tx > Map.MapWidth)
            {
                Event.EventErrorMessage = "Ｘ座標の値は1～" + Map.MapWidth + "で指定してください";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 140014


                Input:
                            Error(0)

                 */
            }

            ty = GetArgAsLong(3);
            if (ty < 1 || ty > Map.MapHeight)
            {
                Event.EventErrorMessage = "Ｙ座標の値は1～" + Map.MapHeight + "で指定してください";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 140235


                Input:
                            Error(0)

                 */
            }

            tname = GetArgAsString(4);
            if (Strings.Right(tname, 6) != "(ローカル)")
            {
                {
                    var withBlock = SRC.TDList;
                    var loopTo = withBlock.Count;
                    for (i = 1; i <= loopTo; i++)
                    {
                        tid = withBlock.OrderedID(i);
                        if ((tname ?? "") == (withBlock.Name(tid) ?? ""))
                        {
                            break;
                        }
                    }

                    if (i > withBlock.Count)
                    {
                        Event.EventErrorMessage = "「" + tname + "」という地形は存在しません";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 140642


                        Input:
                                            Error(0)

                         */
                    }
                }

                // MOD START 240a
                // MapData(tx, ty, 0) = tid
                Map.MapData[tx, ty, Map.MapDataIndex.TerrainType] = tid;
                // MOD  END  240a

                tbitmap = GetArgAsLong(5);
                // MOD START 240a
                // MapData(tx, ty, 1) = tbitmap
                Map.MapData[tx, ty, Map.MapDataIndex.BitmapNo] = tbitmap;
            }
            // MOD  END  240a
            else
            {
                tname = Strings.Left(tname, Strings.Len(tname) - 6);
                {
                    var withBlock1 = SRC.TDList;
                    var loopTo1 = withBlock1.Count;
                    for (i = 1; i <= loopTo1; i++)
                    {
                        tid = withBlock1.OrderedID(i);
                        if ((tname ?? "") == (withBlock1.Name(tid) ?? ""))
                        {
                            break;
                        }
                    }

                    if (i > withBlock1.Count)
                    {
                        Event.EventErrorMessage = "「" + tname + "」という地形は存在しません";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 141467


                        Input:
                                            Error(0)

                         */
                    }
                }

                // MOD START 240a
                // MapData(tx, ty, 0) = tid
                Map.MapData[tx, ty, Map.MapDataIndex.TerrainType] = tid;
                // MOD  END  240a

                tbitmap = -GetArgAsLong(5);
                // MOD START 240a
                // MapData(tx, ty, 1) = tbitmap
                Map.MapData[tx, ty, Map.MapDataIndex.BitmapNo] = tbitmap;
                // MOD  END  240a
            }

            // マップ画像を検索
            fname = Map.SearchTerrainImageFile(tid, tbitmap, tx, ty);
            if (string.IsNullOrEmpty(fname))
            {
                Event.EventErrorMessage = "マップビットマップ「" + SRC.TDList.Bitmap(tid) + SrcFormatter.Format((object)tbitmap) + ".bmp" + "」が見つかりません";
                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 142219


                Input:
                            Error(0)

                 */
            }

            {
                var withBlock2 = GUI.MainForm;
                withBlock2.picTmp32(0) = Image.FromFile(fname);
                switch (Map.MapDrawMode ?? "")
                {
                    case "夜":
                        {
                            Graphics.GetImage(GUI.MainForm.picTmp32(0));
                            Graphics.Dark();
                            Graphics.SetImage(GUI.MainForm.picTmp32(0));
                            break;
                        }

                    case "セピア":
                        {
                            Graphics.GetImage(GUI.MainForm.picTmp32(0));
                            Graphics.Sepia();
                            Graphics.SetImage(GUI.MainForm.picTmp32(0));
                            break;
                        }

                    case "白黒":
                        {
                            Graphics.GetImage(GUI.MainForm.picTmp32(0));
                            Graphics.Monotone();
                            Graphics.SetImage(GUI.MainForm.picTmp32(0));
                            break;
                        }

                    case "夕焼け":
                        {
                            Graphics.GetImage(GUI.MainForm.picTmp32(0));
                            Graphics.Sunset();
                            Graphics.SetImage(GUI.MainForm.picTmp32(0));
                            break;
                        }

                    case "水中":
                        {
                            Graphics.GetImage(GUI.MainForm.picTmp32(0));
                            Graphics.Water();
                            Graphics.SetImage(GUI.MainForm.picTmp32(0));
                            break;
                        }

                    case "フィルタ":
                        {
                            Graphics.GetImage(withBlock2.picTmp32(0));
                            Graphics.ColorFilter(Map.MapDrawFilterColor, Map.MapDrawFilterTransPercent);
                            Graphics.SetImage(withBlock2.picTmp32(0));
                            break;
                        }
                }

                // 背景への書き込み
                ret = GUI.BitBlt(withBlock2.picBack.hDC, 32 * (tx - 1), 32 * (ty - 1), 32, 32, withBlock2.picTmp32(0).hDC, 0, 0, GUI.SRCCOPY);

                // マス目の表示
                if (SRC.ShowSquareLine)
                {
                    withBlock2.picBack.Line((32 * (tx - 1), 32 * (ty - 1)) - (32 * tx, 32 * (ty - 1)), Information.RGB(100, 100, 100), B);
                    withBlock2.picBack.Line((32 * (tx - 1), 32 * (ty - 1)) - (32 * (tx - 1), 32 * ty), Information.RGB(100, 100, 100), B);
                }

                // マスク入り背景画面を作成
                ret = GUI.BitBlt(withBlock2.picMaskedBack.hDC, 32 * (tx - 1), 32 * (ty - 1), 32, 32, withBlock2.picBack.hDC, 32 * (tx - 1), 32 * (ty - 1), GUI.SRCCOPY);
                ret = GUI.BitBlt(withBlock2.picMaskedBack.hDC, 32 * (tx - 1), 32 * (ty - 1), 32, 32, withBlock2.picMask.hDC, 0, 0, GUI.SRCAND);
                ret = GUI.BitBlt(withBlock2.picMaskedBack.hDC, 32 * (tx - 1), 32 * (ty - 1), 32, 32, withBlock2.picMask2.hDC, 0, 0, GUI.SRCINVERT);
            }

            // 変更された地形にいたユニットを再表示
            if (Map.MapDataForUnit[tx, ty] is object)
            {
                {
                    var withBlock3 = Map.MapDataForUnit[tx, ty];
                    // UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                    Map.MapDataForUnit[tx, ty] = null;
                    GUI.EraseUnitBitmap(tx, ty, false);
                    withBlock3.StandBy(tx, ty, "非同期");
                }
            }
            else
            {
                {
                    var withBlock4 = GUI.MainForm;
                    ret = GUI.BitBlt(withBlock4.picMain(0).hDC, GUI.MapToPixelX(tx), GUI.MapToPixelY(ty), 32, 32, withBlock4.picTmp32(0).hDC, 0, 0, GUI.SRCCOPY);
                }
            }

            return EventData.NextID;
        }
    }
}
