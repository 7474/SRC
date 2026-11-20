using System;

namespace SRCCore.Exceptions
{
    /// <summary>
    /// 元の SRC（VB5）の組み込み関数が起こす実行時エラーに当たる例外。
    /// 元の SRC の CallFunction（Expression.bas）はエラーを処理しないので、エラーはイベントのコマンドの Exec の
    /// ErrorHandler（CmdData.cls）まで戻り、「イベントデータが不正です」を出してイベントを止める。
    /// SRC# の CmdData.Exec も、この例外を受けて同じ文言を出す。
    /// </summary>
    public class VbRuntimeErrorException : Exception
    {
        /// <summary>VB のエラー番号（Err.Number）</summary>
        public int Number { get; }

        public VbRuntimeErrorException(int number, string message) : base(message)
        {
            Number = number;
        }

        /// <summary>
        /// VB のエラー 5（プロシージャの呼び出し、または引数が不正です。）。
        /// Mid$ の開始位置が 0 以下・Left$ の文字数が負など、組み込み関数の引数が範囲外のときに起きる。
        /// </summary>
        public static VbRuntimeErrorException InvalidProcedureCall(string procedure)
        {
            return new VbRuntimeErrorException(5, procedure + ": プロシージャの呼び出し、または引数が不正です。");
        }
    }
}
