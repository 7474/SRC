using Avalonia.Input;

namespace SRCSharpAvalonia
{
    /// <summary>
    /// Avalonia のキーを Windows の仮想キーコードにする（SRC の KeyState 関数は仮想キーコードで指定する）。
    /// https://learn.microsoft.com/windows/win32/inputdev/virtual-key-codes
    /// </summary>
    public static class VirtualKeys
    {
        public static int FromKey(Key key)
        {
            if (key >= Key.A && key <= Key.Z)
            {
                return 0x41 + (key - Key.A);
            }
            if (key >= Key.D0 && key <= Key.D9)
            {
                return 0x30 + (key - Key.D0);
            }
            if (key >= Key.NumPad0 && key <= Key.NumPad9)
            {
                return 0x60 + (key - Key.NumPad0);
            }
            if (key >= Key.F1 && key <= Key.F24)
            {
                return 0x70 + (key - Key.F1);
            }
            switch (key)
            {
                case Key.Back: return 0x08;
                case Key.Tab: return 0x09;
                case Key.Enter: return 0x0D;
                case Key.LeftShift:
                case Key.RightShift: return 0x10;
                case Key.LeftCtrl:
                case Key.RightCtrl: return 0x11;
                case Key.LeftAlt:
                case Key.RightAlt: return 0x12;
                case Key.Pause: return 0x13;
                case Key.Escape: return 0x1B;
                case Key.Space: return 0x20;
                case Key.PageUp: return 0x21;
                case Key.PageDown: return 0x22;
                case Key.End: return 0x23;
                case Key.Home: return 0x24;
                case Key.Left: return 0x25;
                case Key.Up: return 0x26;
                case Key.Right: return 0x27;
                case Key.Down: return 0x28;
                case Key.Insert: return 0x2D;
                case Key.Delete: return 0x2E;
                case Key.Multiply: return 0x6A;
                case Key.Add: return 0x6B;
                case Key.Subtract: return 0x6D;
                case Key.Decimal: return 0x6E;
                case Key.Divide: return 0x6F;
                default: return 0;
            }
        }
    }
}
