using Markdig;
using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;

namespace SRCTestBlazor.Models
{
    public class MessagePreviewData
    {
        // 閉じタグなしはとりあえず諦める
        private static Regex sizeReg = new Regex(@"<size=([0-9]+)>(.*?)(</size>|$)", RegexOptions.IgnoreCase);
        private static Regex colorReg = new Regex(@"<color=([#\(\), 0-9a-f]+)>(.*?)(</color>|$)", RegexOptions.IgnoreCase);
        private static Regex removeTagReg = new Regex(@"<(size|color)=([#\(\), 0-9a-f]+)>", RegexOptions.IgnoreCase);

        public string BitmapUri { get; }
        public MarkupString Message { get; }

        public MessagePreviewData(string bitmapUri, string message)
        {
            BitmapUri = bitmapUri;

            message = message
                    .Replace(";", "<br>")
                    .Replace(".", "<br>")
                    .Replace(":", "");
            message = sizeReg.Replace(message, (m) =>
            {
                // デフォルトサイズは12
                var size = int.Parse(m.Groups[1].Value);
                var mes = m.Groups[2].Value;
                var em = size / 12f;
                return $"<span style=\"font-size: {em}em;\">{mes}</span>";
            });
            message = colorReg.Replace(message, (m) =>
            {
                // RGB関数も含めて生のままでCSS指定できそう
                var color = m.Groups[1].Value.ToLower();
                var mes = m.Groups[2].Value;
                return $"<span style=\"color: {color};\">{mes}</span>";
            });
            // フォローできなかったノイズを消す
            message = removeTagReg.Replace(message, "");

            Message = new MarkupString(Markdown.ToHtml(message));
        }
    }
}
