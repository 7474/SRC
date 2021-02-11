using Markdig;
using Microsoft.AspNetCore.Components;

namespace SRCTestBlazor.Models
{
    public class MessagePreviewData
    {
        public string BitmapUri { get; }
        public MarkupString Message { get; }

        public MessagePreviewData(string bitmapUri, string message)
        {
            BitmapUri = bitmapUri;
            Message = new MarkupString(Markdown.ToHtml(
                message
                    .Replace(";", "<br>")
                    .Replace(".", "<br>")
                    .Replace(":", "")
                ));
        }
    }
}
