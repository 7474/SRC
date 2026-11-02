using SkiaSharp;
using SRCRender;
using System.Threading.Tasks;

namespace SRCSharpAvalonia
{
    /// <summary>
    /// 1 回のシナリオの実行（ScreenSession）からウィンドウへの窓口。シナリオを開き直したり最初からやり直したりするとき、
    /// 前のシナリオを止めてから止まりきるまでの間に描いた画面などが、新しいシナリオの表示に混ざらないよう、止めた後は何もしない。
    /// </summary>
    internal sealed class SessionHost : IScreenHost, IScreenDialogHost, IScreenCursorHost
    {
        private readonly MainWindow window;
        private volatile bool active = true;

        public SessionHost(MainWindow window)
        {
            this.window = window;
        }

        /// <summary>このシナリオの表示・入力をやめる</summary>
        public void Deactivate()
        {
            active = false;
        }

        public bool IsActive => active;

        public void Present(SKBitmap frame)
        {
            if (active)
            {
                window.Present(frame);
            }
        }

        public void SetTitle(string title)
        {
            if (active)
            {
                window.SetTitle(title);
            }
        }

        public bool Terminate()
        {
            // 止めたシナリオの終了（Quit など）でアプリを終えない
            return active && window.Terminate();
        }

        public Task<string> InputTextAsync(string title, string message, string defaultValue)
        {
            return active ? window.InputTextAsync(title, message, defaultValue) : Task.FromResult<string>(null);
        }

        public Task<string> OpenFileAsync(FileDialogRequest request)
        {
            return active ? window.OpenFileAsync(request) : Task.FromResult<string>(null);
        }

        public Task<string> SaveFileAsync(FileDialogRequest request)
        {
            return active ? window.SaveFileAsync(request) : Task.FromResult<string>(null);
        }

        public bool TryGetCursorPosition(out float x, out float y)
        {
            x = 0;
            y = 0;
            return active && window.TryGetCursorPosition(out x, out y);
        }

        public bool SetCursorPosition(float x, float y)
        {
            return active && window.SetCursorPosition(x, y);
        }
    }
}
