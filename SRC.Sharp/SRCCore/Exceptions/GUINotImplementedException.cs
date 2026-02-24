using System;

namespace SRCCore.Exceptions
{
    /// <summary>
    /// MockGUI でハンドラが注入されていないメソッドが呼び出された場合にスローされる例外。
    /// CmdData.Exec() はこの例外を捕捉して NextID を返します。
    /// </summary>
    public class GUINotImplementedException : Exception
    {
        public GUINotImplementedException() : base("GUI operation not implemented in this context.")
        {
        }

        public GUINotImplementedException(string methodName)
            : base($"GUI operation '{methodName}' not implemented in this context.")
        {
        }
    }
}
