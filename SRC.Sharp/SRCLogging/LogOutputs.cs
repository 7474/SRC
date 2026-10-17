using System;

namespace SRCLogging
{
    /// <summary>ファイルのほかに出す先</summary>
    [Flags]
    public enum LogOutputs
    {
        None = 0,
        /// <summary>デバッガの出力（System.Diagnostics.Debug）</summary>
        Debug = 1,
        /// <summary>標準エラー出力（CLI）。stdout はプロトコル・結果に使うため使わない</summary>
        StandardError = 2,
    }
}
