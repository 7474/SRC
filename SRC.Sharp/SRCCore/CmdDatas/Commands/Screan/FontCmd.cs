using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class FontCmd : CmdData
    {
        public FontCmd(SRC src, EventDataLine eventData) : base(src, CmdType.FontCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            {
                var withBlock = GUI.MainForm.picMain(0);
                fname = withBlock.Font.Name;

                // デフォルトの設定
                if (ArgNum == 1)
                {
                    withBlock.ForeColor = ColorTranslator.ToOle(Color.White);
                    {
                        var withBlock1 = withBlock.Font;
                        fname = "ＭＳ Ｐ明朝";
                        withBlock1.Size = 16;
                        withBlock1.Bold = true;
                        withBlock1.Italic = false;
                    }

                    GUI.PermanentStringMode = false;
                    GUI.KeepStringMode = false;
                }
                else
                {
                    var loopTo = ArgNum;
                    for (i = 2; i <= loopTo; i++)
                    {
                        opt = GetArgAsString(i);
                        switch (opt ?? "")
                        {
                            case "Ｐ明朝":
                                {
                                    fname = "ＭＳ Ｐ明朝";
                                    break;
                                }

                            case "Ｐゴシック":
                                {
                                    fname = "ＭＳ Ｐゴシック";
                                    break;
                                }

                            case "明朝":
                                {
                                    fname = "ＭＳ 明朝";
                                    break;
                                }

                            case "ゴシック":
                                {
                                    fname = "ＭＳ ゴシック";
                                    break;
                                }

                            case "Bold":
                                {
                                    withBlock.Font.Bold = true;
                                    break;
                                }

                            case "Italic":
                                {
                                    withBlock.Font.Italic = true;
                                    break;
                                }

                            case "Regular":
                                {
                                    withBlock.Font.Bold = false;
                                    withBlock.Font.Italic = false;
                                    break;
                                }

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
                                        withBlock.Font.Size = Conversions.ToShort(opt);
                                    }
                                    else if (Strings.Asc(opt) == 35 && Strings.Len(opt) == 7)
                                    {
                                        // 文字色
                                        cname = new string(Conversions.ToChar(Constants.vbNullChar), 8);
                                        StringType.MidStmtStr(cname, 1, 2, "&H");
                                        var midTmp = Strings.Mid(opt, 6, 2);
                                        StringType.MidStmtStr(cname, 3, 2, midTmp);
                                        var midTmp1 = Strings.Mid(opt, 4, 2);
                                        StringType.MidStmtStr(cname, 5, 2, midTmp1);
                                        var midTmp2 = Strings.Mid(opt, 2, 2);
                                        StringType.MidStmtStr(cname, 7, 2, midTmp2);
                                        if (Information.IsNumeric(cname))
                                        {
                                            withBlock.ForeColor = Conversions.ToInteger(cname);
                                        }
                                    }
                                    else
                                    {
                                        // その他のフォント
                                        fname = opt;
                                    }

                                    break;
                                }
                        }
                    }
                }

                // フォント名が変更されている？
                if (fname != withBlock.Font.Name)
                {
                    sf = (Font)Control.DefaultFont.Clone();
                    {
                        var withBlock2 = withBlock.Font;
                        sf = SrcFormatter.FontChangeName(sf, fname);
                        sf = SrcFormatter.FontChangeSize(sf, withBlock2.Size);
                        sf = SrcFormatter.FontChangeBold(sf, withBlock2.Bold);
                        sf = SrcFormatter.FontChangeItalic(sf, withBlock2.Italic);
                    }
                    withBlock.Font = sf;
                }
            }
            return EventData.NextID;
        }
    }
}
