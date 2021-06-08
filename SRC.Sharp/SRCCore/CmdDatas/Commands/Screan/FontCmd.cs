using SRCCore.Events;
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
                GUI.ResetDrawString();
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
                            fontOption.Bold = true;
                            fontOption.Italic = true;
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
                                    opt = Strings.Left(opt, Strings.Len(opt) - 2);
                                    fontOption.Size = (float)Conversions.ToDouble(opt);
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
