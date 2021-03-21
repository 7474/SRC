// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace SRCCore.Commands
{
    // ユニット＆マップコマンドの実行を行うモジュール
    public partial class Command
    {
        private SRC SRC;
        private Events.Event Event => SRC.Event;
        private IGUI GUI => SRC.GUI;
        private IGUIStatus Status => SRC.GUIStatus;
        private Maps.Map Map => SRC.Map;
        private Expressions.Expression Expression => SRC.Expression;
        private Sound Sound => SRC.Sound;
        private COM COM => SRC.COM;

        public Command(SRC src)
        {
            SRC = src;
        }

        private void LogDebug(string message = "", params string[] param)
        {
            try
            {
                if (!SRC.Log.IsEnabled(LogLevel.Debug)) { return; }
                string method = new StackFrame(1).GetMethod().Name;
                SRC.Log.LogDebug(method
                    + $"({CommandState},{(WaitClickMode ? "W" : "-")},{(ViewMode ? "V" : "-")})"
                    + message
                    + " "
                    + string.Join(", ", param));
            }
            catch
            {
                // ignore
            }
        }
    }
}