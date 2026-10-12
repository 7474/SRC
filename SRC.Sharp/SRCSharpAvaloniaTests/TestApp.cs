using Avalonia;
using Avalonia.Headless;
using Avalonia.Themes.Fluent;

namespace SRCSharpAvalonia.Tests
{
    public class TestApp : Application
    {
        public override void Initialize()
        {
            Styles.Add(new FluentTheme());
        }

        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<TestApp>()
                .UseSkia()
                .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false });
        }
    }
}
