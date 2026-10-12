using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace SRCCore.Extensions
{
    public static class ColorExtension
    {
        public static string ToHexString(this Color color)
        {
            return string.Format("#{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B);
        }

        /// <summary>
        /// System.Drawing.ColorTranslator.FromHtml 相当の変換を行う。
        /// ColorTranslator は netstandard2.1 で利用できないため自前で実装している。
        /// </summary>
        public static Color FromHexString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return Color.Empty;
            }

            if (str[0] == '#' && (str.Length == 7 || str.Length == 4))
            {
                if (str.Length == 7)
                {
                    return Color.FromArgb(
                        Convert.ToInt32(str.Substring(1, 2), 16),
                        Convert.ToInt32(str.Substring(3, 2), 16),
                        Convert.ToInt32(str.Substring(5, 2), 16));
                }
                else
                {
                    var r = new string(str[1], 2);
                    var g = new string(str[2], 2);
                    var b = new string(str[3], 2);
                    return Color.FromArgb(
                        Convert.ToInt32(r, 16),
                        Convert.ToInt32(g, 16),
                        Convert.ToInt32(b, 16));
                }
            }

            // HTML では LightGrey だが .NET では LightGray
            if (string.Equals(str, "LightGrey", StringComparison.OrdinalIgnoreCase))
            {
                return Color.LightGray;
            }

            if (htmlSysColorTable.TryGetValue(str.ToLowerInvariant(), out var sysColor))
            {
                return Color.FromKnownColor(sysColor);
            }

            try
            {
                return FromColorString(str);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message, nameof(str), ex);
            }
        }

        public static bool TryFromHexString(string str, out Color color)
        {
            try
            {
                color = FromHexString(str);
                return true;
            }
            catch
            {
                // ignore
                color = Color.Empty;
                return false;
            }
        }

        private static readonly Dictionary<string, KnownColor> htmlSysColorTable = new Dictionary<string, KnownColor>
        {
            { "activeborder", KnownColor.ActiveBorder },
            { "activecaption", KnownColor.ActiveCaption },
            { "appworkspace", KnownColor.AppWorkspace },
            { "background", KnownColor.Desktop },
            { "buttonface", KnownColor.Control },
            { "buttonhighlight", KnownColor.ControlLightLight },
            { "buttonshadow", KnownColor.ControlDark },
            { "buttontext", KnownColor.ControlText },
            { "captiontext", KnownColor.ActiveCaptionText },
            { "graytext", KnownColor.GrayText },
            { "highlight", KnownColor.Highlight },
            { "highlighttext", KnownColor.HighlightText },
            { "inactiveborder", KnownColor.InactiveBorder },
            { "inactivecaption", KnownColor.InactiveCaption },
            { "inactivecaptiontext", KnownColor.InactiveCaptionText },
            { "infobackground", KnownColor.Info },
            { "infotext", KnownColor.InfoText },
            { "menu", KnownColor.Menu },
            { "menutext", KnownColor.MenuText },
            { "scrollbar", KnownColor.ScrollBar },
            { "threeddarkshadow", KnownColor.ControlDarkDark },
            { "threedface", KnownColor.Control },
            { "threedhighlight", KnownColor.ControlLight },
            { "threedlightshadow", KnownColor.ControlLightLight },
            { "window", KnownColor.Window },
            { "windowframe", KnownColor.WindowFrame },
            { "windowtext", KnownColor.WindowText },
        };

        // System.Drawing.ColorConverter.ConvertFromString 相当
        private static Color FromColorString(string str)
        {
            var text = str.Trim();
            if (text.Length == 0)
            {
                return Color.Empty;
            }

            var named = Color.FromName(text);
            if (named.IsKnownColor)
            {
                return named;
            }

            var culture = CultureInfo.CurrentCulture;
            var sep = culture.TextInfo.ListSeparator[0];
            if (text.IndexOf(sep) < 0)
            {
                if (text.Length >= 2 && (text[0] == '\'' || text[0] == '"') && text[0] == text[text.Length - 1])
                {
                    return Color.FromName(text.Substring(1, text.Length - 2));
                }
                else if ((text.Length == 7 && text[0] == '#')
                    || (text.Length == 8 && (text.StartsWith("0x") || text.StartsWith("0X")))
                    || (text.Length == 8 && (text.StartsWith("&h") || text.StartsWith("&H"))))
                {
                    return PossibleKnownColor(Color.FromArgb(unchecked((int)(0xFF000000 | (uint)IntFromString(text, culture)))));
                }
            }

            var tokens = text.Split(sep);
            var values = new int[tokens.Length];
            for (var i = 0; i < values.Length; i++)
            {
                values[i] = IntFromString(tokens[i], culture);
            }

            switch (values.Length)
            {
                case 1: return PossibleKnownColor(Color.FromArgb(values[0]));
                case 3: return PossibleKnownColor(Color.FromArgb(values[0], values[1], values[2]));
                case 4: return PossibleKnownColor(Color.FromArgb(values[0], values[1], values[2], values[3]));
            }

            throw new ArgumentException($"{text} is not a valid value for Color.");
        }

        // 同じ ARGB 値を持つ名前付きの色（システムカラーを除く）があればそれを返す
        private static Color PossibleKnownColor(Color color)
        {
            var targetArgb = color.ToArgb();
            foreach (KnownColor knownColor in Enum.GetValues(typeof(KnownColor)))
            {
                var c = Color.FromKnownColor(knownColor);
                if (!c.IsSystemColor && c.ToArgb() == targetArgb)
                {
                    return c;
                }
            }
            return color;
        }

        private static int IntFromString(string text, CultureInfo culture)
        {
            text = text.Trim();
            if (text.StartsWith("#"))
            {
                return Convert.ToInt32(text.Substring(1), 16);
            }
            else if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                || text.StartsWith("&h", StringComparison.OrdinalIgnoreCase))
            {
                return Convert.ToInt32(text.Substring(2), 16);
            }
            else
            {
                return int.Parse(text, NumberStyles.Integer, culture.NumberFormat);
            }
        }
    }
}
