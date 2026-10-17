using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SRCCore.Logging
{
    /// <summary>
    /// 元の SRC から移したコードの「メッセージ＋引数の並び」の形のログを、メッセージテンプレートで出す。
    /// </summary>
    public static class LoggerExtensions
    {
        public static void Write(this ILogger logger, LogLevel level, Exception ex, string message, params string[] param)
        {
            try
            {
                if (!logger.IsEnabled(level)) { return; }
                if (param == null || param.Length == 0)
                {
                    logger.Log(level, ex, "{Message}", message ?? "");
                }
                else
                {
                    logger.Log(level, ex, "{Message} {Args}", message ?? "", string.Join(", ", param));
                }
            }
            catch
            {
                // ログの失敗で処理を止めない
            }
        }

        /// <summary>
        /// 呼び出し元のメソッド名をつけて出す。メソッド名を調べる処理は重いため、レベルが有効なときだけ行う。
        /// </summary>
        /// <param name="skipFrames">このメソッドから数えた呼び出し元までの深さ（直接呼ぶなら 1）</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void WriteWithCaller(this ILogger logger, LogLevel level, string message, string[] param, int skipFrames = 2)
        {
            try
            {
                if (!logger.IsEnabled(level)) { return; }
                var method = new StackFrame(skipFrames).GetMethod()?.Name ?? "";
                logger.Log(level, "{Method} {Message} {Args}", method, message ?? "", string.Join(", ", param ?? Array.Empty<string>()));
            }
            catch
            {
                // ログの失敗で処理を止めない
            }
        }

        /// <summary>呼び出し元のメソッド名をつけて Debug で出す</summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void DebugWithCaller(this ILogger logger, string message = "", params string[] param)
        {
            logger.WriteWithCaller(LogLevel.Debug, message, param);
        }

        /// <summary>呼び出し元のメソッド名をつけて Trace で出す</summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void TraceWithCaller(this ILogger logger, string message = "", params string[] param)
        {
            logger.WriteWithCaller(LogLevel.Trace, message, param);
        }
    }
}
