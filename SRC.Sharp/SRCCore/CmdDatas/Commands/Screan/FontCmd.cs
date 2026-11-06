using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Extensions;
using SRCCore.VB;
using System.Drawing;

namespace SRCCore.CmdDatas.Commands
{
    public class FontCmd : CmdData
    {
        public FontCmd(SRC src, EventDataLine eventData) : base(src, CmdType.FontCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            var fontOption = new DrawFontOption
            {
                FontFamily = GUI.CurrentPaintFont.Name,
                Bold = GUI.CurrentPaintFont.Bold,
                Italic = GUI.CurrentPaintFont.Italic,
                Size = GUI.CurrentPaintFont.Size,
                Color = GUI.CurrentPaintColor,
            };
            // デフォルトの設定
            if (ArgNum == 1)
            {
                // 元の SRC と同じく、白・ＭＳ Ｐ明朝・16pt・太字・斜体なしに戻す。
                // 最後の SetDrawFont でいまの書式を設定し直さないように、渡す書式も既定にする
                GUI.ResetDrawString();
                fontOption.FontFamily = "ＭＳ Ｐ明朝";
                fontOption.Size = 16;
                fontOption.Bold = true;
                fontOption.Italic = false;
                fontOption.Color = Color.White;
                GUI.PermanentStringMode = false;
                GUI.KeepStringMode = false;
            }
            else
            {
                for (var i = 2; i <= ArgNum; i++)
                {
                    var opt = GetArgAsString(i);
                    switch (opt ?? "")
                    {
                        case "Ｐ明朝":
                            {
                                fontOption.FontFamily = "ＭＳ Ｐ明朝";
                                break;
                            }

                        case "Ｐゴシック":
                            {
                                fontOption.FontFamily = "ＭＳ Ｐゴシック";
                                break;
                            }

                        case "明朝":
                            {
                                fontOption.FontFamily = "ＭＳ 明朝";
                                break;
                            }

                        case "ゴシック":
                            {
                                fontOption.FontFamily = "ＭＳ ゴシック";
                                break;
                            }

                        case "Bold":
                            fontOption.Bold = true;
                            break;

                        case "Italic":
                            fontOption.Italic = true;
                            break;

                        case "Regular":
                            // 元の SRC と同じく太字・斜体を解除する（#905）
                            fontOption.Bold = false;
                            fontOption.Italic = false;
                            break;

                        case "通常":
                            {
                                GUI.PermanentStringMode = false;
                                GUI.KeepStringMode = false;
                                break;
                            }

                        case "背景":
                            {
                                GUI.PermanentStringMode = true;
                                break;
                            }

                        case "保持":
                            {
                                GUI.KeepStringMode = true;
                                break;
                            }
                        // 無視
                        case " ":
                        case var @case when @case == "":
                            {
                                break;
                            }

                        default:
                            {
                                if (Strings.Right(opt, 2) == "pt")
                                {
                                    // 文字サイズ
                                    // 元の SRC は CInt(opt) なので整数に丸め（偶数丸め）、数値でなければ実行時エラー
                                    opt = Strings.Left(opt, Strings.Len(opt) - 2);
                                    if (!Information.IsNumeric(opt))
                                    {
                                        throw new EventErrorException(this, "イベントデータが不正です");
                                    }
                                    fontOption.Size = Conversions.ToInteger(opt);
                                }
                                else if (Strings.Asc(opt) == 35 && Strings.Len(opt) == 7)
                                {
                                    // 文字色
                                    Color color;
                                    if (ColorExtension.TryFromHexString(opt, out color))
                                    {
                                        fontOption.Color = color;
                                    }
                                }
                                else
                                {
                                    // その他のフォント
                                    fontOption.FontFamily = opt;
                                }

                                break;
                            }
                    }
                }
            }

            GUI.SetDrawFont(fontOption);
            return EventData.NextID;
        }
    }
}
