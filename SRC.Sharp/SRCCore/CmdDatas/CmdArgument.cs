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
        public string strArg { get; set; }

        // 引数の型
        public Expressions.ValueType argType { get; set; }

        public override string ToString()
        {
            return $"{strArg}({lngArg}L)({dblArg}d)";
        }
    }
}
