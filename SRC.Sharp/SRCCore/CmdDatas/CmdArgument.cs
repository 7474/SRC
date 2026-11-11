namespace SRCCore.CmdDatas
{
    public class CmdArgument
    {
        public static readonly CmdArgument Empty = new CmdArgument
        {
            argType = Expressions.ValueType.UndefinedType,
            lngArg = 0,
            dblArg = 0d,
            strArg = ""
        };

        // 引数の値
        public int lngArg { get; set; }
        public double dblArg { get; set; }
        public string strArg { get; set; } = "";

        // 構文解析で分けたままの引数（引用符などを除く前）。null なら strArg と同じ
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Porting", "SRCP003", Justification = "SRC# で足した、引用符などを除く前の引数。null は strArg と同じであることを表す")]
        public string RawArg { get; set; }

        // 引数の型
        public Expressions.ValueType argType { get; set; }

        public override string ToString()
        {
            return string.Format(VB.SrcCulture.Japanese, "{0}({1}L)({2}d)", strArg, lngArg, dblArg);
        }
    }
}
