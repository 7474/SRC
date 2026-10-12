using Avalonia.Controls;
using Avalonia.Threading;
using SkiaSharp;
using SRCRender;
using System;

namespace SRCSharpAvalonia
{
    public class MainWindow : Window, IScreenHost
    {
        public const string AppTitle = "SRC#Avalonia";

        public MainWindow()
        {
            Title = AppTitle;
            CanResize = false;
            SizeToContent = SizeToContent.WidthAndHeight;
            View = new ScreenView();
            Content = View;
        }

        public ScreenView View { get; }

        /// <summary>
        /// 終了要求時の処理。既定ではアプリケーションを終了する。
        /// </summary>
        public Func<bool> TerminateHandler { get; set; }

        public void Present(SKBitmap frame)
        {
            View.SetFrame(frame);
            Dispatcher.UIThread.Post(View.Flush);
        }

        public void SetTitle(string title)
        {
            var text = string.IsNullOrEmpty(title) ? AppTitle : title + " - " + AppTitle;
            Dispatcher.UIThread.Post(() => Title = text);
        }

        public bool Terminate()
        {
            return TerminateHandler?.Invoke() ?? true;
        }
    }
}
