using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;

namespace SRCSharpAvalonia
{
    /// <summary>
    /// 文字入力のダイアログ（WinForms 版 frmInputBox 相当）。IME による入力は TextBox が扱う。
    /// 閉じるときの結果は入力した文字列、キャンセル時は null。
    /// </summary>
    public class InputWindow : Window
    {
        public InputWindow(string title, string message, string defaultValue)
        {
            Title = title;
            CanResize = false;
            SizeToContent = SizeToContent.WidthAndHeight;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ShowInTaskbar = false;

            TextBox = new TextBox { Text = defaultValue ?? "", Width = 360 };
            var ok = new Button { Content = "OK", IsDefault = true, Width = 90 };
            var cancel = new Button { Content = Strings.Cancel, IsCancel = true, Width = 90 };
            ok.Click += (s, e) => Close(TextBox.Text ?? "");
            cancel.Click += (s, e) => Close(null);

            Content = new StackPanel
            {
                Margin = new Thickness(12),
                Spacing = 8,
                Children =
                {
                    new TextBlock { Text = message ?? "", TextWrapping = Avalonia.Media.TextWrapping.Wrap, MaxWidth = 360 },
                    TextBox,
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Spacing = 8,
                        Children = { ok, cancel },
                    },
                },
            };
            Opened += (s, e) =>
            {
                TextBox.Focus();
                TextBox.SelectAll();
            };
        }

        public TextBox TextBox { get; }
    }
}
