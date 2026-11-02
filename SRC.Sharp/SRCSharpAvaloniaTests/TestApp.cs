using Avalonia;
using Avalonia.Headless;

namespace SRCSharpAvalonia.Tests
{
    public class TestApp : Application
    {
        public const double TestFontSize = 12;

        public override void Initialize()
        {
            // 実際の起動と同じテーマ。文字は Windows の既定のメニューの文字（9 ポイント＝12 ピクセル）
            App.ApplyTheme(this, new SystemUiFont(null, TestFontSize));
        }

        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<TestApp>()
                .UseSkia()
                .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false });
        }
    }
}
