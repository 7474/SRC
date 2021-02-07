// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

namespace SRCCore.Expressions
{
    // 演算子の種類
    public enum OperatorType
    {
        PlusOp,
        MinusOp,
        MultOp,
        DivOp,
        IntDivOp,
        ExpoOp,
        ModOp,
        CatOp,
        EqOp,
        NotEqOp,
        LtOp,
        LtEqOp,
        GtOp,
        GtEqOp,
        NotOp,
        AndOp,
        OrOp,
        LikeOp
    }
}
