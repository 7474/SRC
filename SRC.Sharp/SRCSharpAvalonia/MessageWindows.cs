using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using System.Reflection;

namespace SRCSharpAvalonia
{
    /// <summary>
    /// はい・いいえで確かめるダイアログ。閉じるときの結果は、はいなら true。
    /// </summary>
    public class ConfirmWindow : Window
    {
        public ConfirmWindow(string title, string message)
        {
            Title = title;
            CanResize = false;
            SizeToContent = SizeToContent.WidthAndHeight;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ShowInTaskbar = false;

            YesButton = new Button { Content = Strings.Yes, IsDefault = true, MinWidth = 90 };
            var no = new Button { Content = Strings.No, IsCancel = true, MinWidth = 90 };
            YesButton.Click += (s, e) => Close(true);
            no.Click += (s, e) => Close(false);

            Content = new StackPanel
            {
                Margin = new Thickness(12),
                Spacing = 12,
                Children =
                {
                    new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap, MaxWidth = 400 },
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Spacing = 8,
                        Children = { YesButton, no },
                    },
                },
            };
        }

        internal Button YesButton { get; }
    }

    /// <summary>
    /// バージョン情報（版・ライセンス・サイト）。
    /// </summary>
    public class AboutWindow : Window
    {
        public AboutWindow()
        {
            Title = Strings.AboutTitle;
            CanResize = false;
            SizeToContent = SizeToContent.WidthAndHeight;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ShowInTaskbar = false;

            var website = new Button { Content = Strings.WebsiteMenu };
            website.Click += async (s, e) => await Launcher.LaunchUriAsync(MainWindow.WebsiteUri);
            var ok = new Button { Content = "OK", IsDefault = true, IsCancel = true, MinWidth = 90 };
            ok.Click += (s, e) => Close();

            Content = new StackPanel
            {
                Margin = new Thickness(16),
                Spacing = 8,
                Children =
                {
                    new TextBlock { Text = MainWindow.AppTitle, FontSize = 20, FontWeight = FontWeight.Bold },
                    new TextBlock { Text = "Simulation RPG Construction Sharp" },
                    new TextBlock { Text = string.Format(Strings.AboutVersion, Version) },
                    new TextBlock { Text = Strings.AboutLicense, TextWrapping = TextWrapping.Wrap, MaxWidth = 400 },
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Spacing = 8,
                        Margin = new Thickness(0, 8, 0, 0),
                        Children = { website, ok },
                    },
                },
            };
        }

        /// <summary>アプリの版（3.5.6 など。開発版は 3.5.6-dev.123。ビルドの識別子は除く）</summary>
        public static string Version
        {
            get
            {
                var version = typeof(AboutWindow).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "";
                var plus = version.IndexOf('+');
                return plus >= 0 ? version.Substring(0, plus) : version;
            }
        }
    }
}
